using SlimDX;
using SlimDX.Windows;
using System;
using MagicBall.Engine;
using MagicBall.Engine.Renderable;

namespace MagicBall
{
    static class Program
    {
        static void Main()
        {
            DeviceManager.Instance.SetForm(new RenderForm("MagicBall"));
            DeviceManager.Instance.SetFullScreenDimensions(1920, 1080);
            RenderManager.Instance.Initialize();

            EffectManager.Instance.Add("default", "effects/effects.fx");
            EffectManager.Instance.Add("tex", "effects/texEffects.fx");

            CreateWalls();

            CreateTable();

            CreateBall();

            MessagePump.Run(DeviceManager.Instance.Form, () => {
                RenderManager.Instance.Render();
            });

            RenderManager.Instance.Dispose();
        }

        static void CreateTable()
        {
            ColladaModel table = new ColladaModel(
                "models/capsule.dae",
                "Capsule001",
                EffectManager.Instance.Get("default"),
                "PointLightBlue"
            );

            RenderManager.Instance.AddRenderable("table", table);

            // Set gLight* constraints before model render
            /*
            table.BeforeRender.Add((effect, geomery, technique) => {
                // LightPos is the ball's centre current position
                Vector4 lightPos = Vector3.Transform(Vector3.Zero, RenderManager.Instance.GetRenderable("ball").GetTransformationMatrix());

                effect.GetVariableByName("gLightAtt").AsVector().Set(new Vector3(0f, 0.2f, 0f));
                effect.GetVariableByName("gLightRange").AsScalar().Set(100.0f);
                effect.GetVariableByName("gLightAmbient").AsVector().Set(new Vector4(0.3f, 0.3f, 0.3f, 1.0f));
                effect.GetVariableByName("gLightDiffuse").AsVector().Set(new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
                effect.GetVariableByName("gLightPos").AsVector().Set(new Vector3(lightPos.X, lightPos.Y, lightPos.Z));
            });
            */

            table.SetTransformationMatrix(
                Matrix.Multiply(Matrix.Multiply(Matrix.RotationX((float)Math.PI / 2), Matrix.Translation(0f, 10f, 0f)), Matrix.Scaling(2f, 2f, 2f))
            );
        }

        static void CreateBall()
        {
            RenderManager.Instance.AddRenderable(
                "ball",
                new BouncingColladaModel(
                    "models/ball.dae",
                    "Sphere001",
                    EffectManager.Instance.Get("default"),
                    "SolidRed",
                    50f,
                    0.03f
                )
            );

            Engine.RenderManager.Instance.GetRenderable("ball").SetTransformationMatrix(Matrix.Translation(0f, 35.2f, 0f));
        }

        static void CreateWalls()
        {
            RenderManager.Instance.AddRenderable("floor", CreateFloor());
            RenderManager.Instance.AddRenderable("ceiling", CreateCeiling());
            RenderManager.Instance.AddRenderable("wall-back", CreateWallBack());
            RenderManager.Instance.AddRenderable("wall-right", CreateWallRight());
        }

        static ColladaModel CreateWallBack()
        {
            ColladaModel wall = new ColladaModel(
                "models/plane2.dae",
                "Plane001",
                EffectManager.Instance.Get("default"),
                "SolidYellow"
            );

            wall.SetTransformationMatrix(Matrix.Scaling(2f, 1f, 1f) * Matrix.Translation(0f, 70f, 80f));

            return wall;
        }

        static ColladaModel CreateWallRight()
        {
            ColladaModel wall = new ColladaModel(
                "models/plane2.dae",
                "Plane001",
                EffectManager.Instance.Get("default"),
                "SolidYellow"
            );

            wall.SetTransformationMatrix(Matrix.Scaling(2f, 1f, 1f) * Matrix.RotationY((float)Math.PI / 2) * Matrix.Translation(80f, 70f, 0f));

            return wall;
        }

        static ColladaModel CreateCeiling()
        {
            ColladaModel ceiling = new ColladaModel(
                "models/plane2.dae",
                "Plane001",
                EffectManager.Instance.Get("default"),
                "SolidWhite"
            );

            ceiling.SetTransformationMatrix(Matrix.Scaling(2f, 2f, 2f) * Matrix.RotationX((float)Math.PI / 2) * Matrix.Translation(0f, 150f, 0f));

            return ceiling;
        }

        static ColladaModel CreateFloor()
        {
            ColladaModel floor = new TexturedColladaModel(
                "models/plane2.dae",
                "Plane001",
                EffectManager.Instance.Get("tex"),
                "SolidTexture",
                "snow.jpg"
            );

            floor.SetTransformationMatrix(Matrix.Scaling(5f, 5f, 5f) * Matrix.RotationX((float)Math.PI / 2) * Matrix.Translation(0f, -10f, 0f));

            return floor;
        }
    }
}
