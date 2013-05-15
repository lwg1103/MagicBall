using System;
using SlimDX;

using IDisposable = MagicBall.Engine.Common.IDisposable;

namespace MagicBall.Engine.Renderable
{
    public interface IRenderable : IDisposable
    {
        void Render();
        Matrix GetTransformationMatrix();
        void SetTransformationMatrix(Matrix matrix);
    }
}
