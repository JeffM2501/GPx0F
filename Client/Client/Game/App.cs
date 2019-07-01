using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Urho;
using Urho.Resources;
using Urho.Actions;

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

            var ring = World.CreateChild("sky2").CreateComponent<StaticModel>();
            ring.Model = ResourceCache.GetModel("Models/MountainRing.mdl");
            ring.Material = ResourceCache.GetMaterial("Materials/MountainRing.xml");

            //create scene node
            float size = 500;
            float repeatPerUnit = 0.5f;

            Node node = World.CreateChild("Object");
            node.Position = new Vector3(0.0f, 0.0f, 0.0f);
            node.Scale = new Vector3(size, size, size);
;           var model = node.CreateComponent<StaticModel>();
            model.Model = ResourceCache.GetModel("Models/Plane.mdl");
            
            model.SetMaterial(ResourceCache.GetMaterial("Materials/Ground.xml"));
            model.Material.SetUVTransform(Vector2.Zero, 0, size * repeatPerUnit);

            var lightNode = World.CreateChild("DirectionalLight");
            lightNode.SetDirection(new Vector3(5, -5, -5f)); // The direction vector does not need to be normalized
            var light = lightNode.CreateComponent<Light>();
            light.LightType = LightType.Directional;
            light.Brightness = 1;


            var cameraNode = World.CreateChild("camera");
            MainCamera = cameraNode.CreateComponent<Camera>();
            MainCamera.Node.Position = new Vector3(0, 1, 0);
            var zone = cameraNode.CreateComponent<Zone>();
            zone.SetBoundingBox(new BoundingBox(MainCamera.Frustum));
            zone.AmbientColor = new Color(0.25f, 0.25f, 0.25f, 1);

            cameraNode.RunActionsAsync(new RotateAroundBy(100, Vector3.Zero, 0 , 360, 0, TransformSpace.Local));
            cameraNode.RunActionsAsync(new MoveBy(100,Vector3.UnitZ * 100));
        }
    }
}
