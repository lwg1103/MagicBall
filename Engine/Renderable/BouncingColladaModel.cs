using SlimDX;
using SlimDX.Direct3D11;

namespace MagicBall.Engine.Renderable
{
    class BouncingColladaModel : TexturedColladaModel
    {
        float MaxBounce;
        float Bounce;
        float CurrentBounce;
        int Multipl;

        public BouncingColladaModel(string model, string geometry, Effect effect, string technique, string textureName, float maxBounce, float bounce)
            : base(model, geometry, effect, technique, textureName)
        {
            MaxBounce = maxBounce;
            Bounce = bounce;
            CurrentBounce = 0.0f;
            Multipl = 1;

            BeforeRender.Add((e, g, t) =>
            {
                if (CurrentBounce >= MaxBounce)
                {
                    Multipl = -1;
                }
                else if (CurrentBounce <= 0)
                {
                    Multipl = 1;
                }

                CurrentBounce = CurrentBounce + Multipl * Bounce;
            });
        }

        public override Matrix GetTransformationMatrix()
        {
            return Matrix.Multiply(transformationMatrix, Matrix.Translation(0f, CurrentBounce, 0f));
        }
    }
}
