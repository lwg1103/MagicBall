using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Xml;
using SlimDX;
using SlimDX.DXGI;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;
using System.IO;

namespace MagicBall.Engine
{

    public class CGeometry
    {
        public SlimDX.Direct3D11.Device device;
        public string ID, Name;
        private List<InputElement> VertexDeclarationList;
        public SlimDX.Direct3D11.Buffer Vertices;
        public int VertexCount;
        public int VertexSizeInBytes;
        public BoundingBox BBox;
        public BoundingSphere BSphere;
        public InputElement[] VertexDeclaration
        {
            get { return VertexDeclarationList.ToArray(); }
        }
        public DataStream VertexStream;


        private struct CInput
        {
            public int offset;
            public string semantic;
            public string source;

            public CInput(XmlNode N)
            {
                if (N.Name != "input") throw new Exception("Invalid node");
                offset = int.Parse(N.Attributes["offset"].Value);
                semantic = N.Attributes["semantic"].Value;
                switch (semantic)
                {
                    case "VERTEX": semantic = "POSITION"; break;
                    case "TEXTANGENT": semantic = "TANGENT"; break;
                    case "TEXBINORMAL": semantic = "BINORMAL"; break;
                }
                source = N.Attributes["source"].Value.Remove(0, 1);
            }

        }

        private class CSource
        {
            public string id;
            public List<Vector3> values;


            public CSource(XmlNode N)
            {
                values = new List<Vector3>();
                List<float> data = new List<float>();
                if (N.Name != "source") throw new Exception("Invalid node");
                foreach (XmlAttribute A in N.Attributes)
                    if (A.Name == "id") id = A.Value;
                foreach (XmlNode child in N.ChildNodes)
                    if (child.Name == "float_array")
                    {
                        string[] s = child.InnerText.Split(' ');
                        float f = 0;
                        foreach (string ss in s)
                        {
                            float.TryParse(ss.Replace('.', ',').Trim(), out f);
                            data.Add(f);
                        }
                    }
                for (int i = 0; i < data.Count; i += 3)
                    values.Add(new Vector3(data[i], data[i + 1], data[i + 2]));

            }
        }


        public CGeometry(SlimDX.Direct3D11.Device dev, XmlNode N)
        {
            device = dev;
            VertexDeclarationList = new List<InputElement>();

            ID = "";
            Name = "";
            if (N.Name != "geometry") throw new Exception("Invalid node");
            foreach (XmlAttribute A in N.Attributes)
                switch (A.Name)
                {
                    case "id": ID = A.Value; break;
                    case "name": Name = A.Value; break;
                }

            foreach (XmlNode child in N.ChildNodes)
                if (child.Name == "mesh") ParseMesh(child);
        }

        public CGeometry(SlimDX.Direct3D11.Device dev)
        {
            device = dev;
            VertexDeclarationList = new List<InputElement>();
            ID = "";
            Name = "";
        }


        private void ParseTriangles(XmlNode N, Dictionary<string, CSource> Sources)
        {

            if (N.Name != "triangles") throw new Exception("Invalid node");
            List<CInput> IL = new List<CInput>();
            int MaxOffset = 0;

            foreach (XmlNode child in N.ChildNodes)
                if (child.Name == "input")
                {
                    CInput CI = new CInput(child);
                    MaxOffset = (MaxOffset > CI.offset) ? MaxOffset : CI.offset;
                    IL.Add(CI);
                }


            int NoTriangles = int.Parse(N.Attributes["count"].Value);
            VertexCount = NoTriangles * 3;

            VertexSizeInBytes = IL.Count * 3 * 4;
            int totalsize = VertexCount * VertexSizeInBytes;



            string ss = N.InnerText;
            string[] s = ss.Split(' ');
            int[] ps = new int[s.Length];
            for (int i = 0; i < s.Length; i++) ps[i] = int.Parse(s[i].Replace('.', ','));
            int psize = VertexCount * (MaxOffset + 1);
            VertexStream = new DataStream(totalsize, true, true);
            int j = 0;
            for (int i = 0; i < VertexCount; i++)
            {

                for (int k = 0; k < IL.Count; k++)
                {
                    VertexStream.Write(Sources[IL[k].source].values[ps[j + IL[k].offset]]);
                }
                j += (MaxOffset + 1);
            }

            for (int i = 0; i < IL.Count; i++)
            {
                VertexDeclarationList.Add(new InputElement(IL[i].semantic, 0, Format.R32G32B32_Float, i * 12, 0));

            }
            BBox = BoundingBox.FromPoints(Sources[IL[0].source].values.ToArray());
            BSphere = BoundingSphere.FromPoints(Sources[IL[0].source].values.ToArray());
            BufferDescription D = new BufferDescription()
            {
                SizeInBytes = totalsize,
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.VertexBuffer,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            };
            
            VertexStream.Position = 0;
            Vertices = new SlimDX.Direct3D11.Buffer(device, VertexStream, D);

        }

        private void ParseMesh(XmlNode N)
        {
            Dictionary<string, CSource> Sources = new Dictionary<string, CSource>();
            if (N.Name != "mesh") throw new Exception("invalid node");
            foreach (XmlNode child in N.ChildNodes)
                switch (child.Name)
                {
                    case "source": CSource S = new CSource(child); Sources.Add(S.id, S); break;
                    case "vertices":
                        string key = child.FirstChild.Attributes["source"].Value.Remove(0, 1);
                        CSource src = Sources[key];
                        src.id = child.Attributes["id"].Value;
                        Sources.Remove(key);
                        Sources.Add(src.id, src);
                        break;
                    case "triangles": ParseTriangles(child, Sources); break;
                }

        }

        public void Render(EffectTechnique T)
        {
            InputLayout layout = new InputLayout(device, T.GetPassByIndex(0).Description.Signature, VertexDeclaration);
            
            device.ImmediateContext.InputAssembler.InputLayout = layout;
            device.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            
            device.ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(Vertices, VertexSizeInBytes, 0));
            for (int i = 0; i < T.Description.PassCount; i++)
            {
                T.GetPassByIndex(i).Apply(device.ImmediateContext);
                device.ImmediateContext.Draw(VertexCount, 0);
            }
        }

        public void Dispose()
        {
            VertexStream.Dispose();
        }
    }

    public struct CVertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector3 UV;
        public Vector3 Tangent;
        public Vector3 Binormal;
    }


    public class CDocument
    {
        SlimDX.Direct3D11.Device device;
        public XmlDocument doc;
        public XmlNode GeometryNode;
        public Dictionary<string, CGeometry> Geometries;

        public CDocument(SlimDX.Direct3D11.Device dev, string filename)
        {
            device = dev;
            doc = new XmlDocument();
            doc.Load(filename);
            GeometryNode = FindGeometry(doc);
            Geometries = new Dictionary<string, CGeometry>();
            ParseGeometries();
        }

        public CDocument(SlimDX.Direct3D11.Device dev, Stream s)
        {
            device = dev;
            doc = new XmlDocument();
            doc.Load(s);
            GeometryNode = FindGeometry(doc);
            Geometries = new Dictionary<string, CGeometry>();
            ParseGeometries();
        }



        private void ParseGeometries()
        {
            foreach (XmlNode N in GeometryNode.ChildNodes)
            {
                CGeometry G = new CGeometry(device, N);

                Geometries.Add(G.Name, G);

            }
        }
        public XmlNode FindGeometry(XmlNode N)
        {

            if (N.Name == "library_geometries")
                return N;
            foreach (XmlNode Child in N.ChildNodes)
            {
                XmlNode tmp = FindGeometry(Child);
                if (tmp != null) return tmp;

            }

            return null;

        }


    }

}