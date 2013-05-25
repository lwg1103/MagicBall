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
            CreateTableTop();
            CreateTableLegs();
            
        }

        static void CreateTableLegs()
        {
            CreateTableLeg(25.0f, 50.0f);
            CreateTableLeg(-25.0f, 50.0f); 
            CreateTableLeg(25.0f, -50.0f);
            CreateTableLeg(-25.0f, -50.0f);
        }

        static void CreateTableLeg(float x, float y)
        {
            RenderManager.Instance.AddRenderable("table-leg-front-" + x + "-" + y, CreateTableLegElement(x, y - 3.0f,y > 0));
            RenderManager.Instance.AddRenderable("table-leg-back-" + x + "-" + y, CreateTableLegElement(x, y + 3.0f, y > 0));
            RenderManager.Instance.AddRenderable("table-leg-right-" + x + "-" + y, CreateTableLegElementRotated(x + 3.0f, y, x > 0));
            RenderManager.Instance.AddRenderable("table-leg-left-" + x + "-" + y, CreateTableLegElementRotated(x - 3.0f, y, x > 0));
        }

        static ColladaModel CreateTableLegElement(float x, float y, bool inverted)
        {
            ColladaModel element = new TexturedColladaModel(
                "models/plane2.dae",
                "Plane001",
                EffectManager.Instance.Get("tex"),
                "SolidTexture",
                "table.jpeg"
            );
            Matrix rotation = (inverted) ? Matrix.RotationX((float)Math.PI) : Matrix.RotationX(0);
            element.SetTransformationMatrix(Matrix.Scaling(0.04f, 0.18f, 0.04f) * rotation * Matrix.Translation(x, 5.0f, y));

            return element;
        }

        static ColladaModel CreateTableLegElementRotated(float x, float y, bool inverted)
        {
            ColladaModel element = new TexturedColladaModel(
                "models/plane2.dae",
                "Plane001",
                EffectManager.Instance.Get("tex"),
                "SolidTexture",
                "table.jpeg"
            );

            Matrix rotation = (inverted) ? Matrix.RotationY(3*(float)Math.PI / 2) : Matrix.RotationY((float)Math.PI / 2);
            element.SetTransformationMatrix(Matrix.Scaling(0.04f, 0.18f, 0.04f) * rotation * Matrix.Translation(x, 5.0f, y));

            return element;
        }

        static void CreateTableTop()
        {
            RenderManager.Instance.AddRenderable("table-top-up", CreateTableTopUp());
            RenderManager.Instance.AddRenderable("table-top-down", CreateTableTopDown());
            RenderManager.Instance.AddRenderable("table-top-back", CreateTableTopBack());
            RenderManager.Instance.AddRenderable("table-top-front", CreateTableTopFront());
            RenderManager.Instance.AddRenderable("table-top-right", CreateTableTopRight());
            RenderManager.Instance.AddRenderable("table-top-left", CreateTableTopLeft());
        }

        static ColladaModel CreateTableTopUp()
        {
            ColladaModel table = new TexturedColladaModel(
                "models/plane2.dae",
                "Plane001",
                EffectManager.Instance.Get("tex"),
                "SolidTexture",
                "table.jpeg"
            );

            table.SetTransformationMatrix(Matrix.Scaling(0.5f, 0.75f, 0.5f) * Matrix.RotationX(3*(float)Math.PI / 2) * Matrix.Translation(0f, 20f, 0f));

            return table;
        }

        static ColladaModel CreateTableTopDown()
        {
            ColladaModel table = new TexturedColladaModel(
                "models/plane2.dae",
                "Plane001",
                EffectManager.Instance.Get("tex"),
                "SolidTexture",
                "table.jpeg"
            );

            table.SetTransformationMatrix(Matrix.Scaling(0.5f, 0.75f, 0.5f) * Matrix.RotationX(3*(float)Math.PI / 2) * Matrix.Translation(0f, 18f, 0f));

            return table;
        }

        static ColladaModel CreateTableTopBack()
        {
            ColladaModel table = new TexturedColladaModel(
                "models/plane2.dae",
                "Plane001",
                EffectManager.Instance.Get("tex"),
                "SolidTexture",
                "table.jpeg"
            );

            table.SetTransformationMatrix(Matrix.Scaling(0.5f, 0.02f, 0.5f) * Matrix.RotationY((float)Math.PI) * Matrix.Translation(0f, 19f, 60f));

            return table;
        }

        static ColladaModel CreateTableTopFront()
        {
            ColladaModel table = new TexturedColladaModel(
                "models/plane2.dae",
                "Plane001",
                EffectManager.Instance.Get("tex"),
                "SolidTexture",
                "table.jpeg"
            );

            table.SetTransformationMatrix(Matrix.Scaling(0.5f, 0.02f, 0.5f) * Matrix.Translation(0f, 19f, -60f));

            return table;
        }

        static ColladaModel CreateTableTopRight()
        {
            ColladaModel table = new TexturedColladaModel(
                "models/plane2.dae",
                "Plane001",
                EffectManager.Instance.Get("tex"),
                "SolidTexture",
                "table.jpeg"
            );

            table.SetTransformationMatrix(Matrix.Scaling(0.75f, 0.02f, 0.5f) * Matrix.RotationY(3*(float)Math.PI / 2) * Matrix.Translation(40f, 19f, 0f));

            return table;
        }

        static ColladaModel CreateTableTopLeft()
        {
            ColladaModel table = new TexturedColladaModel(
                "models/plane2.dae",
                "Plane001",
                EffectManager.Instance.Get("tex"),
                "SolidTexture",
                "table.jpeg"
            );

            table.SetTransformationMatrix(Matrix.Scaling(0.75f, 0.02f, 0.5f) * Matrix.RotationY((float)Math.PI / 2) * Matrix.Translation(-40f, 19f, 0f));

            return table;
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
            wall.SetTransformationMatrix(Matrix.Scaling(1f, 1f, 1f) * Matrix.RotationY((float)Math.PI) * Matrix.Translation(0f, 70f, 80f));

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

            wall.SetTransformationMatrix(Matrix.Scaling(1f, 1f, 1f) * Matrix.RotationY(3*(float)Math.PI / 2) * Matrix.Translation(80f, 70f, 0f));

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

            floor.SetTransformationMatrix(Matrix.Scaling(1f, 1f, 1f) * Matrix.RotationX(3*(float)Math.PI / 2) * Matrix.Translation(0f, -10f, 0f));

            return floor;
        }
    }
}
