using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Urho;
using Urho.Resources;

namespace Client
{
    public static class Ships
    {
        public class ShipNode : Component
        {
            public static float BobbleMag = 0.1f;
            public static float BoobleFreq = 0.5f;

            public StaticModel ModelNode = null;

            public float LifeTime = 0;

            public ShipNode() : base()
            {
                ReceiveSceneUpdates = true;
            }
  
            public enum TeamColors
            {
                Blue,
                Red,
                Green,
                Purple,
                White,
                Yellow,
                Black
            }
            public TeamColors TeamColor = TeamColors.Blue;

            protected override void OnUpdate(float timeStep)
            {
                LifeTime += timeStep;

                base.OnUpdate(timeStep);
                ModelNode.Node.Position = new Vector3(0, (BobbleMag * 2) + ((float)Math.Sin(LifeTime * BoobleFreq) * BobbleMag), 0);
            }
        }


        public static ShipNode GetShipNode(ResourceCache resources, Node root, ShipNode.TeamColors team, string shipType)
        {
            string path = "Models/Ships/" + shipType;
            Node coreNode = root.CreateChild("ship");
            ShipNode node = coreNode.CreateComponent<ShipNode>();
            node.TeamColor = team;

            node.ModelNode = coreNode.CreateChild("mesh").CreateComponent<StaticModel>();
            node.ModelNode.Node.Rotate(Quaternion.FromAxisAngle(Vector3.UnitY, -90));
            node.ModelNode.Model = resources.GetModel(path + "/model.mdl");
            node.ModelNode.CastShadows = true;
            node.ModelNode.Material = resources.GetMaterial(path +"/Materials/BaseMaterial.xml").Clone();
            node.ModelNode.Material.SetTexture(0, resources.GetTexture2D(path + "/Textures/" + team.ToString().ToLower() + ".png"));

            return node;
        }
    }
}
