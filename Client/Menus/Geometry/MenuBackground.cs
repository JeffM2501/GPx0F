using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urho;
using Urho.Actions;
using Urho.Resources;

using Game;
using Game.Maps;

namespace Client.Menus.Geometry
{
    public class MenuBackground : SimpleArena
    {
        protected Ships.ShipNode MenuShip = null;

        public override bool Setup(ResourceCache resources, Scene world, float size)
        {
            if (!base.Setup(resources, world, size))
                return false;

            Random rng = new Random();
            int t = rng.Next(0, (int)Ships.TeamColors.Black);

            MenuShip = Ships.GetShipNode(resources, world, (Ships.TeamColors)t, "Mk3");
            MenuShip.Node.Position = new Vector3(-2, 0, 0);
            MenuShip.Node.Rotate(Quaternion.FromAxisAngle(Vector3.UnitY, rng.Next(160, 200) * -1.0f));
            //    KeepSpinning();


            float aspect = 5.0f / 20.0f;
            var node = world.CreateChild("block");
            node.Position = new Vector3(15,2f,10);

            float boxSize = 20;

            node.Scale = new Vector3(boxSize, boxSize * aspect, boxSize);
            node.Rotate(Quaternion.FromAxisAngle(Vector3.UnitY,-45));
            var model = node.CreateComponent<StaticModel>();
            model.Model = resources.GetModel("Models/Box.mdl");
            model.CastShadows = true;
            model.SetMaterial(resources.GetMaterial("Legacy/zone/Materials/BoxWall.xml").Clone());

            float repeatsPerUnit = 0.25f;

            model.Material.SetUVTransform(Vector2.Zero, 0, new Vector2(boxSize * repeatsPerUnit, boxSize * repeatsPerUnit * aspect));

            MainLight.Node.SetDirection(new Vector3(-0.75f, -1, -0.25f));
            return true;
        }

        protected void KeepSpinning()
        {
            MenuShip.Node.RunActionsAsync(new RotateAroundBy(60, Vector3.Zero, 0, 720, 0, TransformSpace.Local)).ContinueWith((x) => KeepSpinning());

        }
       
    }
}
