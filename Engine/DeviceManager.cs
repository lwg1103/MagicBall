using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using SlimDX.Windows;
using System.Windows.Forms;

using Device = SlimDX.Direct3D11.Device;
using Resource = SlimDX.Direct3D11.Resource;

namespace MagicBall.Engine
{
    public class DeviceManager : Common.IDisposable
    {
        static private DeviceManager instance;

        static public DeviceManager Instance
        {
            get
            {
                if (instance == null) {
                    instance = new DeviceManager();
                }

                return instance;
            }
        }

        private RenderForm form;
        public RenderForm Form { get { return form; } }

        private Device device;
        public Device Device { get { return device; } }
        public DeviceContext Context { get { return device.ImmediateContext; } }

        public int ClientWidth { get { return Screen.PrimaryScreen.Bounds.Width; } }
        public int ClientHeight { get { return Screen.PrimaryScreen.Bounds.Height; } }

        private SwapChain swapChain;
        public SwapChain SwapChain { get { return swapChain; } }

        private RenderTargetView renderTarget;
        public RenderTargetView RenderTarget { get { return renderTarget; } }

        private DepthStencilView depthStencil;
        public DepthStencilView DepthStencil { get { return depthStencil; } }

        private int FullWidth;
        private int FullHeight;

        private DeviceManager()
        {
            form = null;
        }

        public void SetForm(RenderForm form)
        {
            if (this.form != null)
            {
                throw new System.InvalidOperationException("RenderForm instance already set");
            }

            this.form = form;
        }

        public void SetFullScreenDimensions(int width, int height)
        {
            FullWidth = width;
            FullHeight = height;
        }

        public void Initialize()
        {
            if (form == null)
            {
                throw new System.InvalidOperationException("RenderForm instance should be bound to DeviceManager using SetForm method");
            }

            InitializeDeviceSwapChain();
            InitializeBackBufferRenderTargets();
            InitializeContextStates();
        }

        public void Dispose()
        {
            swapChain.SetFullScreenState(false, null);

            Context.ClearState();
            
            renderTarget.Dispose();
            device.Dispose();
            swapChain.Dispose();
        }

        private void InitializeDeviceSwapChain()
        {
            var description = new SwapChainDescription
            {
                BufferCount = 1,
                Usage = Usage.RenderTargetOutput,
                OutputHandle = Form.Handle,
                IsWindowed = false,
                Flags = SwapChainFlags.None,
                SwapEffect = SwapEffect.Discard,
                ModeDescription = new ModeDescription(ClientWidth, ClientHeight, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                SampleDescription = new SampleDescription(2, 0)
            };

            var dev = device;
            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, description, out dev, out swapChain);
            device = dev;
        }

        private void InitializeBackBufferRenderTargets()
        {
            using (var resource = Resource.FromSwapChain<Texture2D>(swapChain, 0))
            {
                renderTarget = new RenderTargetView(device, resource);
            }

            Texture2DDescription depthBufferDesc = new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.D32_Float,
                Height = ClientHeight,
                Width = ClientWidth,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SampleDescription(2, 0),
                Usage = ResourceUsage.Default
            };

            var DepthBuffer = new Texture2D(device, depthBufferDesc);

            DepthStencilViewDescription dsViewDesc = new DepthStencilViewDescription
            {
                ArraySize = 0,
                Flags = 0,
                FirstArraySlice = 0,
                MipSlice = 0,
                Format = Format.D32_Float,
                Dimension = DepthStencilViewDimension.Texture2DMultisampled
            };

            depthStencil = new DepthStencilView(device, DepthBuffer, dsViewDesc);

            Context.OutputMerger.SetTargets(depthStencil, renderTarget);
        }

        private void InitializeContextStates()
        {

            Context.OutputMerger.DepthStencilState = States.Instance.depthEnabledStencilDisabled;
            Context.Rasterizer.State = States.Instance.cullNoneFillSolid;
            Context.OutputMerger.BlendState = States.Instance.blendEnabledSourceAlphaInverseSourceAlpha;

            Context.Rasterizer.SetViewports(new Viewport(0.0f, 0.0f, ClientWidth, ClientHeight));
        }
    }
}
