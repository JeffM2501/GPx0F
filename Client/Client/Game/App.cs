using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Urho;
using Urho.Resources;

namespace Client.Game
{
    public class App : Application
    {
        protected Scene World = null;
        protected Camera MainCamera = null;

        public App(ApplicationOptions options = null) : base(options) { }

        protected override void Start()
        {
            SetupScene();

            Renderer.SetViewport(0, new Viewport(Context, World, MainCamera, null));
            Input.KeyDown += Input_KeyDown;
        }


        private void Input_KeyDown(KeyDownEventArgs args)
        {
            if (args.Key == Key.Esc)
                Exit();
        }

        public void SetupScene()
        {
            World = new Scene();
            World.CreateComponent<Octree>();

            var skybox = World.CreateChild("Sky").CreateComponent<Skybox>();
            skybox.Model = ResourceCache.GetModel("Models/Box.mdl");
            skybox.Material = ResourceCache.GetMaterial("Materials/Skybox.xml");

            //create scene node
            Node node = World.CreateChild("Object");
            node.Position = new Vector3(0.0f, 0.0f, 0.0f);
            var model = node.CreateComponent<StaticModel>();
            model.Model = ResourceCache.GetModel("Models/Plane.mdl");
            model.s = ResourceCache.GetModel("Models/Plane.mdl");

            //             CustomGeometry _geometry = node.CreateComponent<CustomGeometry>();
            //             _geometry.BeginGeometry(0, PrimitiveType.TriangleList);
            // 
            //             float size = 100;
            //             // tri 1
            //             _geometry.DefineVertex(new Vector3(-size, 0, 0));
            //             _geometry.DefineTexCoord(new Vector2(0.0f, 0.0f));
            // 
            //             _geometry.DefineVertex(new Vector3(size, 0, 0));
            //             _geometry.DefineTexCoord(new Vector2(0.0f, 1.0f));
            // 
            //             _geometry.DefineVertex(new Vector3(size, 0, size));
            //             _geometry.DefineTexCoord(new Vector2(1.0f, 0.0f));
            // 
            //             // tri 2
            //             _geometry.DefineVertex(new Vector3(-size, 0, 0));
            //             _geometry.DefineTexCoord(new Vector2(0.0f, 1.0f));
            // 
            //             _geometry.DefineVertex(new Vector3(size, 0, size));
            //             _geometry.DefineTexCoord(new Vector2(1.0f, 1.0f));
            // 
            //             _geometry.DefineVertex(new Vector3(-size, 0, size));
            //             _geometry.DefineTexCoord(new Vector2(1.0f, 0.0f));
            // 
            //             _geometry.Commit();
            //     
            //             _geometry.SetMaterial(ResourceCache.GetMaterial("Materials/Ground.xml"));



            var lightNode = World.CreateChild("DirectionalLight");
            lightNode.SetDirection(new Vector3(5, 5, -5f)); // The direction vector does not need to be normalized
            var light = lightNode.CreateComponent<Light>();
            light.LightType = LightType.Point;
            light.Brightness = 2;
            light.FadeDistance = 100;

            // 
            //             var ground = World.CreateChild("ground");
            //             var groundModel = ground.CreateComponent<StaticModel>();
            // 
            //             var geo = new CustomGeometry();
            // 
            //             groundModel.ge
            // 
            //             geo.BeginGeometry()

            var cameraNode = World.CreateChild("camera");
            MainCamera = cameraNode.CreateComponent<Camera>();
            MainCamera.Node.Position = new Vector3(0, 1, 0);

        }
    }
}
