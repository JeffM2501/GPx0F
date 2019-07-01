using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Urho;
using Urho.Resources;

namespace Client.Geometry
{
    public class SimpleArena : Game.Arena
    {
        public Light MainLight = null;
        public Node Ground = null;

        protected float GroundRepeat = 1;
        protected float WallRepeat = 0.125f;

        public override bool Setup(ResourceCache resources, Scene world, float size)
        {
            return Setup(resources, world, size, 25, 50);
        }

        public bool Setup(ResourceCache resources, Scene world, float size, float wallHeight, float border)
        {
            if (!base.Setup(resources, world, size))
                return false;

            var skybox = world.CreateChild("Sky").CreateComponent<Skybox>();
            skybox.Model = resources.GetModel("Models/Box.mdl");
            skybox.Material = resources.GetMaterial("Materials/Skybox.xml");

//             var ring = world.CreateChild("sky2").CreateComponent<StaticModel>();
//             ring.Model = resources.GetModel("Models/MountainRing.mdl");
//             ring.Material = resources.GetMaterial("Materials/MountainRing.xml");

            //create scene node
          

            Ground = world.CreateChild("ground");
            Ground.Position = new Vector3(0.0f, 0.0f, 0.0f);
            Ground.Scale = new Vector3(size, size, size);

            var model = Ground.CreateComponent<StaticModel>();
            model.Model = resources.GetModel("Models/Plane.mdl");
            model.SetMaterial(resources.GetMaterial("Materials/Ground.xml").Clone());
            model.Material.SetUVTransform(Vector2.Zero, 0, size * GroundRepeat);
    //        model.CastShadows = true;


            Node node = world.CreateChild("eastWall");
            node.Position = new Vector3(size - border, 0, 0.0f);
            node.Scale = new Vector3(wallHeight, wallHeight, ((size - border) * 2));
            model = node.CreateComponent<StaticModel>();
            model.Model = resources.GetModel("Models/Box.mdl");

            model.CastShadows = true; model.SetMaterial(resources.GetMaterial("Legacy/zone/Materials/OuterWall.xml").Clone());
            model.Material.SetUVTransform(Vector2.Zero, 0, new Vector2(size * WallRepeat, wallHeight * WallRepeat));

            node = world.CreateChild("westWall");
            node.Position = new Vector3(-size + border, 0, 0.0f);
            node.Scale = new Vector3(wallHeight, wallHeight, ((size - border) * 2));
            model = node.CreateComponent<StaticModel>();
            model.Model = resources.GetModel("Models/Box.mdl");
            model.CastShadows = true;
            model.SetMaterial(resources.GetMaterial("Legacy/zone/Materials/OuterWall.xml").Clone());
            model.Material.SetUVTransform(Vector2.Zero, 0, new Vector2(size * WallRepeat, wallHeight * WallRepeat));

            node = world.CreateChild("northWall");
            node.Position = new Vector3(0, 0, size - border);
            node.Scale = new Vector3(((size - border) * 2), wallHeight, wallHeight);
            model = node.CreateComponent<StaticModel>();
            model.Model = resources.GetModel("Models/Box.mdl");
            model.CastShadows = true;
            model.SetMaterial(resources.GetMaterial("Legacy/zone/Materials/OuterWall.xml").Clone());
            model.Material.SetUVTransform(Vector2.Zero, 0, new Vector2(size * WallRepeat, wallHeight * WallRepeat));

            node = world.CreateChild("southWall");
            node.Position = new Vector3(0, 0, -size + border);
            node.Scale = new Vector3(((size - border) * 2), wallHeight, wallHeight);
            model = node.CreateComponent<StaticModel>();
            model.Model = resources.GetModel("Models/Box.mdl");
            model.CastShadows = true;
            model.SetMaterial(resources.GetMaterial("Legacy/zone/Materials/OuterWall.xml").Clone());
            model.Material.SetUVTransform(Vector2.Zero, 0, new Vector2(size * WallRepeat, wallHeight * WallRepeat));

            var lightNode = world.CreateChild("DirectionalLight");
            lightNode.SetDirection(new Vector3(5, -5, -5f)); // The direction vector does not need to be normalized
            MainLight = lightNode.CreateComponent<Light>();
            MainLight.LightType = LightType.Directional;
            MainLight.CastShadows = true;
            MainLight.ShadowBias = new BiasParameters(0.00025f, 0.5f);
            MainLight.ShadowCascade = new CascadeParameters(10, 50, 200, 0, .8f);
          //  MainLight.Brightness = 0.75f;
             MainLight.SpecularIntensity = 0.75f;

            return true;
        }
    }
}
