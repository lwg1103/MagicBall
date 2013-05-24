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
                    EffectManager.Instance.Get("tex"),
                    "GlowTexture",
                    "ball.jpg",
                    5f,
                    0.001f
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
            ColladaModel wall = new TexturedColladaModel(
                "models/plane2.dae",
                "Plane001",
                EffectManager.Instance.Get("tex"),
                "SolidTexture",
                "wood.bmp"
            );
            wall.SetTransformationMatrix(Matrix.Scaling(1f, 1f, 1f) * Matrix.Translation(0f, 70f, 80f));

            return wall;
        }

        static ColladaModel CreateWallRight()
        {
            ColladaModel wall = new TexturedColladaModel(
                "models/plane2.dae",
                "Plane001",
                EffectManager.Instance.Get("tex"),
                "SolidTexture",
                "wall.jpeg"
            );

            wall.SetTransformationMatrix(Matrix.Scaling(1f, 1f, 1f) * Matrix.RotationY((float)Math.PI / 2) * Matrix.Translation(80f, 70f, 0f));

            return wall;
        }

        static ColladaModel CreateCeiling()
        {
            ColladaModel ceiling = new TexturedColladaModel(
                "models/plane2.dae",
                "Plane001",
                EffectManager.Instance.Get("tex"),
                "SolidTexture",
                "ceiling.jpg"
            );

            ceiling.SetTransformationMatrix(Matrix.Scaling(1f, 1f, 1f) * Matrix.RotationX((float)Math.PI / 2) * Matrix.Translation(0f, 150f, 0f));

            return ceiling;
        }

        static ColladaModel CreateFloor()
        {
            ColladaModel floor = new TexturedColladaModel(
                "models/plane2.dae",
                "Plane001",
                EffectManager.Instance.Get("tex"),
                "SolidTexture",
                "floor.jpg"
            );

            floor.SetTransformationMatrix(Matrix.Scaling(1f, 1f, 1f) * Matrix.RotationX((float)Math.PI / 2) * Matrix.Translation(0f, -10f, 0f));

            return floor;
        }
    }
}
