using SlimDX;
using SlimDX.Direct3D11;

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

            device.ImmediateContext.PixelShader.SetShaderResource(resourceView, 0);
            effect.GetVariableByName("xTexture").AsResource().SetResource(resourceView);
            effect.GetVariableByName("TextureSampler").AsSampler().SetSamplerState(0, b);
            device.ImmediateContext.PixelShader.SetShaderResource(resourceView, 0);
            device.ImmediateContext.PixelShader.SetSampler(b, 0);

            BeforeRender.Add((e, g, t) =>
            {

                device.ImmediateContext.PixelShader.SetShaderResource(resourceView, 0);
                effect.GetVariableByName("xTexture").AsResource().SetResource(resourceView);
                effect.GetVariableByName("TextureSampler").AsSampler().SetSamplerState(0, b);
                device.ImmediateContext.PixelShader.SetShaderResource(resourceView, 0);
                device.ImmediateContext.PixelShader.SetSampler(b, 0);
            });
        }
    }
}
