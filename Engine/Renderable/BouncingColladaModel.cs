using SlimDX;
using SlimDX.Direct3D11;

namespace MagicBall.Engine.Renderable
{
    class BouncingColladaModel : TexturedColladaModel
    {
        float MaxBounce;
        float LimitBounce;
        float Bounce;
        float CurrentBounce;
        int Multipl;
        float BounceDiff;

        public BouncingColladaModel(string model, string geometry, Effect effect, string technique, string textureName, float maxBounce, float bounce)
            : base(model, geometry, effect, technique, textureName)
        {
            LimitBounce = maxBounce;
            MaxBounce = maxBounce;
            Bounce = bounce;
            CurrentBounce = 0.0f;
            Multipl = 1;
            BounceDiff= 0.0001f;

            BeforeRender.Add((e, g, t) => {
                if (CurrentBounce >= MaxBounce)
                {
                    Multipl = -1;
                }
                else if (CurrentBounce <= 0)
                {
                    Multipl = 1;
                }

                MaxBounce *= 1f - BounceDiff;

                if (MaxBounce <= LimitBounce / 10 || MaxBounce >= LimitBounce)
                {
                    BounceDiff *= -1;
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
