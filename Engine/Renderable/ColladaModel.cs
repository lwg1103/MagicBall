using SlimDX;
using SlimDX.DXGI;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;
using System.Collections.Generic;

namespace MagicBall.Engine.Renderable
{
    public delegate void RenderDelegate(Effect effect, CGeometry geometry, string technique);

    public class ColladaModel : IRenderable
    {
        protected Matrix transformationMatrix;
        
        CGeometry geometry;
        Effect effect;
        string technique;
        
        public List<RenderDelegate> BeforeRender;

        public ColladaModel(string model, string geometry, Effect effect, string technique)
        {
            CDocument modelDoc = new CDocument(DeviceManager.Instance.Device, model);

            this.geometry = modelDoc.Geometries[geometry];
            this.effect = effect;
            this.technique = technique;
            this.transformationMatrix = Matrix.Identity;

            this.BeforeRender = new List<RenderDelegate>();
        }

        public virtual void Render()
        {
            foreach (RenderDelegate del in BeforeRender)
            {
                del(effect, geometry, technique);
            }

            effect.GetVariableByName("gTrans").AsMatrix().SetMatrix(GetTransformationMatrix());
            effect.GetVariableByName("gOpacity").AsScalar().Set(1.0f);

            geometry.Render(effect.GetTechniqueByName(technique));
        }

        public virtual Matrix GetTransformationMatrix()
        {
            return transformationMatrix;
        }

        public void SetTransformationMatrix(Matrix matrix)
        {
            transformationMatrix = matrix;
        }

        public void Dispose()
        {
            geometry.Dispose();
        }
    }
}
