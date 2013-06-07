using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using SlimDX.Windows;

using Device = SlimDX.Direct3D11.Device;
using Resource = SlimDX.Direct3D11.Resource;

namespace MagicBall.Engine
{
    class RenderManager
    {
        static private RenderManager instance;

        static public RenderManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RenderManager();
                }

                return instance;
            }
        }

        private Camera camera;
        public Camera Camera { get { return camera; } }

        private Dictionary<string, Renderable.IRenderable> renderables;
        private float phi;
        private float theta;
        private float radius;
        private float mouseX = -1;
        private float mouseY = -1;

        public RenderManager()
        {
            renderables = new Dictionary<string, Renderable.IRenderable>();
            camera = new Camera();

            phi = 0.9f;
            theta = 0.2f;
            radius = 300f;

            camera.Target = new Vector3(0, 5f, 0);

            camera.SetPositionSpherical(phi, theta, radius);
        }

        public void AddRenderable(string name, Renderable.IRenderable renderable)
        {
            renderables.Add(name, renderable);
        }

        public Renderable.IRenderable GetRenderable(string name)
        {
            return renderables[name];
        }

        public void Initialize()
        {
            DeviceManager.Instance.Initialize();

            DeviceManager.Instance.Form.MouseClick += EndAnimation;
            DeviceManager.Instance.Form.MouseMove += MouseMoved;
            DeviceManager.Instance.Form.KeyDown += EndAnimation;
        }

        void MouseMoved(object sender, MouseEventArgs e)
        {
            if (mouseX == -1 && mouseY == -1)
            {
                mouseX = e.X;
                mouseY = e.Y;
            }
            else
            {
                if (Math.Abs(mouseX - e.X) + Math.Abs(mouseY - e.Y) > 20)
                {
                    EndAnimation(sender, e);
                }
            }
        }

        void EndAnimation(object sender, EventArgs e)
        {
            DeviceManager.Instance.Form.Close();
        }

        public void Render()
        {
            DeviceManager.Instance.Context.ClearRenderTargetView(DeviceManager.Instance.RenderTarget, new Color4(0f, 0f, 0f));
            DeviceManager.Instance.Context.ClearDepthStencilView(DeviceManager.Instance.DepthStencil, DepthStencilClearFlags.Depth, 1, 0);

            Engine.EffectManager.Instance.Render();
            
            foreach (KeyValuePair<string, Renderable.IRenderable> renderable in renderables)
            {
                renderable.Value.Render();
            }

            DeviceManager.Instance.SwapChain.Present(0, PresentFlags.None);
        }

        public void Dispose()
        {
            foreach (KeyValuePair<string, Renderable.IRenderable> renderable in renderables)
            {
                renderable.Value.Dispose();
            }

            DeviceManager.Instance.Dispose();
        }
    }
}
