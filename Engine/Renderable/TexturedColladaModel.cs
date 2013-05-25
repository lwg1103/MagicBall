using SlimDX;
using SlimDX.Direct3D11;
using Chwedziak.Diagnostics;
using System;

namespace MagicBall.Engine.Renderable
{
    class TexturedColladaModel : ColladaModel
    {
        Texture2D texture;
        ShaderResourceView resourceView;

        public TexturedColladaModel(string model, string geometry, Effect effect, string technique, string textureName)
            : base(model, geometry, effect, technique)
        {
            Device device = DeviceManager.Instance.Device;

            SamplerDescription a = new SamplerDescription();
            a.AddressU = TextureAddressMode.Wrap;
            a.AddressV = TextureAddressMode.Wrap;
            a.AddressW = TextureAddressMode.Wrap;

            a.Filter = Filter.MinPointMagMipLinear;

            SamplerState b = SamplerState.FromDescription(device, a);

            texture = Texture2D.FromFile(device, "../../textures/" + textureName);
            resourceView = new ShaderResourceView(device, texture);

            BeforeRender.Add((e, g, t) =>
            {
                Vector4 lightPos = Vector3.Transform(Vector3.Zero, RenderManager.Instance.GetRenderable("ball").GetTransformationMatrix());
                effect.GetVariableByName("gTrans").AsMatrix().SetMatrix(this.GetTransformationMatrix());
                effect.GetVariableByName("gLightPos").AsVector().Set(new Vector3(lightPos.X, lightPos.Y, lightPos.Z));
                effect.GetVariableByName("gLightDiffuse").AsVector().Set(new Vector4(0.2f, 0.2f, 0.2f, 1.0f));
                float lightStrength = (float)TexturedColladaModel.distort((double)CpuUsage.CurrentValue) / 100.0f;                
                effect.GetVariableByName("lightStrength").AsScalar().Set(lightStrength*2);
                device.ImmediateContext.PixelShader.SetShaderResource(resourceView, 0);
                effect.GetVariableByName("xTexture").AsResource().SetResource(resourceView);
                effect.GetVariableByName("TextureSampler").AsSampler().SetSamplerState(0, b);
                device.ImmediateContext.PixelShader.SetShaderResource(resourceView, 0);
                device.ImmediateContext.PixelShader.SetSampler(b, 0);
            });
        }

        public static double distort(double original)
        {
            return (original > 0.54)
                ? original + 0.54
                : original * Math.Pow(Math.Exp(1 - original), 5) / 10 / Math.E;
        }
    }
}
