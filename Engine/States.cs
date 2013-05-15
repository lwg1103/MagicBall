using SlimDX.Direct3D11;

namespace MagicBall.Engine
{
    class States
    {
        public DepthStencilState depthEnabledStencilDisabled { get; private set; }
        public DepthStencilState depthDisabledStencilDisabled { get; private set; }
        public RasterizerState cullNoneFillSolid { get; private set; }
        public RasterizerState cullNoneFillWireframe { get; private set; }
        public RasterizerState cullBackFillSolid { get; private set; }
        public RasterizerState cullBackFillWireframe { get; private set; }
        public RasterizerState cullFrontFillSolid { get; private set; }
        public RasterizerState cullFrontFillWireframe { get; private set; }
        public BlendState blendDisabled { get; private set; }
        public BlendState blendEnabledSourceAlphaInverseSourceAlpha { get; private set; }
        public BlendState blendEnabledSourceAlphaDestinationAlpha { get; private set; }
        public BlendState blendEnabledOneOne { get; private set; }

        private static States instance;
        public static States Instance
        {
            get
            {
                if (instance == null) instance = new States();
                return instance;
            }
        }
        public States()
        {
            var device = DeviceManager.Instance.Device;
            DepthStencilStateDescription dsStateDesc = new DepthStencilStateDescription()
            {
                IsDepthEnabled = false,
                IsStencilEnabled = false,
                DepthWriteMask = DepthWriteMask.All,
                DepthComparison = Comparison.Less,
            };
            depthDisabledStencilDisabled = DepthStencilState.FromDescription(device, dsStateDesc);
            dsStateDesc.IsDepthEnabled = true;
            depthEnabledStencilDisabled = DepthStencilState.FromDescription(device, dsStateDesc);

            RasterizerStateDescription rasStateDesc = new RasterizerStateDescription()
            {
                CullMode = CullMode.None,
                DepthBias = 1,
                DepthBiasClamp = 10.0f,
                FillMode = FillMode.Solid,
                IsAntialiasedLineEnabled = false,
                IsDepthClipEnabled = false,
                IsFrontCounterclockwise = true,
                IsMultisampleEnabled = false,
                IsScissorEnabled = false
            };
            cullNoneFillSolid = RasterizerState.FromDescription(device, rasStateDesc);
            rasStateDesc.FillMode = FillMode.Wireframe;
            cullNoneFillWireframe = RasterizerState.FromDescription(device, rasStateDesc);
            rasStateDesc.CullMode = CullMode.Back;
            cullBackFillWireframe = RasterizerState.FromDescription(device, rasStateDesc);
            rasStateDesc.FillMode = FillMode.Solid;
            cullBackFillSolid = RasterizerState.FromDescription(device, rasStateDesc);
            rasStateDesc.CullMode = CullMode.Front;
            cullFrontFillSolid = RasterizerState.FromDescription(device, rasStateDesc);
            rasStateDesc.FillMode = FillMode.Wireframe;
            cullFrontFillWireframe = RasterizerState.FromDescription(device, rasStateDesc);

            BlendStateDescription blendStateDesc = new BlendStateDescription()
            {
                IndependentBlendEnable = false,
                AlphaToCoverageEnable = false,
            };

            blendStateDesc.RenderTargets[0] = new RenderTargetBlendDescription();
            blendStateDesc.RenderTargets[0].RenderTargetWriteMask = ColorWriteMaskFlags.All;
            blendDisabled = BlendState.FromDescription(device, blendStateDesc);
            blendStateDesc.RenderTargets[0].BlendEnable = true;
            blendStateDesc.RenderTargets[0].BlendOperation = BlendOperation.Add;
            blendStateDesc.RenderTargets[0].BlendOperationAlpha = BlendOperation.Add;
            blendStateDesc.RenderTargets[0].DestinationBlend = BlendOption.DestinationAlpha;
            blendStateDesc.RenderTargets[0].DestinationBlendAlpha = BlendOption.DestinationAlpha;
            blendStateDesc.RenderTargets[0].SourceBlend = BlendOption.SourceAlpha;
            blendStateDesc.RenderTargets[0].SourceBlendAlpha = BlendOption.SourceAlpha;
            blendEnabledSourceAlphaDestinationAlpha = BlendState.FromDescription(device, blendStateDesc);
            blendStateDesc.RenderTargets[0].DestinationBlend = BlendOption.InverseSourceAlpha;
            blendStateDesc.RenderTargets[0].DestinationBlendAlpha = BlendOption.InverseSourceAlpha;
            blendStateDesc.RenderTargets[0].SourceBlend = BlendOption.SourceAlpha;
            blendStateDesc.RenderTargets[0].SourceBlendAlpha = BlendOption.SourceAlpha;
            blendEnabledSourceAlphaInverseSourceAlpha = BlendState.FromDescription(device, blendStateDesc);
            blendStateDesc.RenderTargets[0].DestinationBlend = BlendOption.One;
            blendStateDesc.RenderTargets[0].DestinationBlendAlpha = BlendOption.One;
            blendStateDesc.RenderTargets[0].SourceBlend = BlendOption.One;
            blendStateDesc.RenderTargets[0].SourceBlendAlpha = BlendOption.One;
            blendEnabledOneOne = BlendState.FromDescription(device, blendStateDesc);
        }

    }
}
