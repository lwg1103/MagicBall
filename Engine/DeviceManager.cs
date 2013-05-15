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

        public int ClientWidth { get { return Form.ClientSize.Width; } }
        public int ClientHeight { get { return Form.ClientSize.Height; } }

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

            InitializeDeviceAndSwapChain();
            InitializeRenderTarget();
            InitializeContextStates();
        }

        public void Dispose()
        {
            Context.ClearState();
            
            renderTarget.Dispose();
            device.Dispose();
            swapChain.Dispose();
        }

        private void InitializeDeviceAndSwapChain()
        {
            var scd = new SwapChainDescription()
            {
                BufferCount = 1,
                Usage = Usage.RenderTargetOutput,
                OutputHandle = Form.Handle,
                IsWindowed = true,
                ModeDescription = new ModeDescription(0, 0, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                SampleDescription = new SampleDescription(1, 0),
                Flags = SwapChainFlags.AllowModeSwitch,
                SwapEffect = SwapEffect.Discard
            };

            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, scd, out device, out swapChain);

            using (var factory = SwapChain.GetParent<Factory>())
            {
                factory.SetWindowAssociation(form.Handle, WindowAssociationFlags.IgnoreAltEnter);
            }

            form.KeyDown += (o, e) =>
            {
                if (e.Alt && e.KeyCode == Keys.Enter)
                {
                    swapChain.IsFullScreen = !swapChain.IsFullScreen;
                }
            };
        }

        private void InitializeRenderTarget()
        {
            using (var resource = Resource.FromSwapChain<Texture2D>(SwapChain, 0))
            {
                renderTarget = new RenderTargetView(device, resource);
            }

            Format depthFormat = Format.D24_UNorm_S8_UInt;

            Texture2DDescription depthBufferDesc = new Texture2DDescription()
            {
                ArraySize = 1,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = depthFormat,
                Height = ClientHeight,
                Width = ClientWidth,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default
            };

            var DepthBuffer = new Texture2D(device, depthBufferDesc);

            DepthStencilViewDescription dsViewDesc = new DepthStencilViewDescription()
            {
                ArraySize = 0,
                Format = depthFormat,
                Dimension = DepthStencilViewDimension.Texture2D,
                MipSlice = 0,
                Flags = 0,
                FirstArraySlice = 0
            };

            depthStencil = new DepthStencilView(device, DepthBuffer, dsViewDesc);

            Context.OutputMerger.SetTargets(depthStencil, renderTarget);
        }

        private void InitializeContextStates()
        {

            Context.OutputMerger.DepthStencilState = States.Instance.depthEnabledStencilDisabled;
            Context.Rasterizer.State = States.Instance.cullNoneFillSolid;
            Context.OutputMerger.BlendState = States.Instance.blendDisabled;

            Context.Rasterizer.SetViewports(new Viewport(0.0f, 0.0f, ClientWidth, ClientHeight));
        }
    }
}
