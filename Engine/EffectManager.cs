using System;
using System.Collections.Generic;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.D3DCompiler;

namespace MagicBall.Engine
{
    public class EffectManager
    {
        static private EffectManager instance;

        static public EffectManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EffectManager();
                }

                return instance;
            }
        }

        private Dictionary<string, Effect> effects;

        public EffectManager()
        {
            effects = new Dictionary<string, Effect>();
        }

        public void Add(string name, string file)
        {
            using (var bc = ShaderBytecode.CompileFromFile(file, "bidon", "fx_5_0", ShaderFlags.OptimizationLevel3, EffectFlags.None))
            {
                effects.Add(name, new Effect(DeviceManager.Instance.Device, bc));
            }
        }

        public Effect Get(string effect)
        {
            return effects[effect];
        }

        public void Render()
        {
            foreach (KeyValuePair<string, Effect> effect in effects)
            {
                effect.Value.GetVariableByName("gWorld").AsMatrix().SetMatrix(Matrix.Identity);
                effect.Value.GetVariableByName("gView").AsMatrix().SetMatrix(GetViewMatrix());
                effect.Value.GetVariableByName("gProj").AsMatrix().SetMatrix(GetProjectionMatrix());
            }
        }

        private Matrix GetViewMatrix()
        {
            return RenderManager.Instance.Camera.CameraView;
        }

        private Matrix GetProjectionMatrix()
        {
            return Matrix.PerspectiveFovLH(
                (float)Math.PI / 4,
                (float)DeviceManager.Instance.ClientWidth / (float)DeviceManager.Instance.ClientHeight,
                0.1f,
                1000f
            );
        }
    }
}
