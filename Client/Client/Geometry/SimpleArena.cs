using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Urho;
using Urho.Resources;
using Urho.Physics;

using Client.Game;

namespace Client.Geometry
{
    public class SimpleArena : Game.Arena
    {
        public Light MainLight = null;
        public Node Ground = null;

        protected float GroundRepeat = 0.75f;
        protected float WallRepeat = 0.125f;

        public override bool Setup(ResourceCache resources, Scene world, float size)
        {
            return Setup(resources, world, size, 20, 50);
        }

        public bool Setup(ResourceCache resources, Scene world, float size, float wallHeight, float border)
        {
            if (!base.Setup(resources, world, size))
                return false;

            var skybox = world.CreateChild("Sky").CreateComponent<Skybox>();
            skybox.Model = resources.GetModel("Models/Box.mdl");
            skybox.Material = resources.GetMaterial("Materials/Skybox.xml");


            Ground = MakePlaneGround(size, "Materials/Ground.xml", GroundRepeat);
            MakeBorders(size - border, wallHeight, "Legacy/zone/Materials/OuterWall.xml", WallRepeat);

            var node = MakeBox("TestBox", new Vector3(10, 2.5f, 10), new Vector3(5, 5, 5), "Legacy/zone/Materials/BoxWall.xml", new Vector2(5 * WallRepeat, 5 * WallRepeat));

            var lightNode = world.CreateChild("DirectionalLight");
            lightNode.SetDirection(new Vector3(5, -5, -5f)); // The direction vector does not need to be normalized
            MainLight = lightNode.CreateComponent<Light>();
            MainLight.LightType = LightType.Directional;
            MainLight.CastShadows = true;
      //      MainLight.ShadowBias = new BiasParameters(0.00025f, 0.5f);
      //      MainLight.ShadowCascade = new CascadeParameters(10, 50, 200, size, .8f);
            MainLight.ShadowDistance = size * 3;
            MainLight.ShadowFadeDistance = size * 2;
            MainLight.ShadowResolution = 1.0f;
          //  MainLight.Brightness = 0.75f;
             MainLight.SpecularIntensity = 0.75f;

            return true;
        }
    }
}
