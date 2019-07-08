using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Urho;
using Urho.Physics;
using Urho.Resources;

namespace Client.Game
{
    public class Arena
    {
        public static uint WorldColisionLayer = 2;

        protected Scene World = null;
        protected ResourceCache ResCache = null;

        public Urho.Rect Bounds { get; protected set; } = new Rect();

        public virtual bool Setup(ResourceCache resources, Scene world, float size)
        {
            ResCache = resources;
            World = world;
            return true; // does nothing, that always works
        }

        public virtual Vector3 GetSpawn()
        {
            return new Vector3(0, 2, 0);
        }

        protected Node SetCollidable(Node node, Vector3 minBBox, Vector3 maxBBox, Quaternion rot)
        {
            var phys = node.CreateComponent<RigidBody>();
            phys.Kinematic = true;
            phys.CollisionLayer = WorldColisionLayer;

            var collision = node.CreateComponent<CollisionShape>();
            collision.SetBox(minBBox, maxBBox, rot);

            return node;
        }

        protected Node SetCollidable(Node node)
        {
            var sm = node.GetComponent<StaticModel>();
            var phys = node.CreateComponent<RigidBody>();
            phys.Kinematic = true;
            phys.CollisionLayer = WorldColisionLayer;

            var collision = node.CreateComponent<CollisionShape>();
            collision.SetTriangleMesh(sm.Model, 0, Vector3.One, Vector3.Zero, Quaternion.Identity);

            return node;
        }

        protected Node MakeBox(string name, Vector3 pos, Vector3 scale, string materialName, Vector2 uvRepeat)
        {
            var node = World.CreateChild(name);
            node.Position = pos;
            node.Scale = scale;
            var model = node.CreateComponent<StaticModel>();
            model.Model = ResCache.GetModel("Models/Box.mdl");
            model.CastShadows = true;
            model.SetMaterial(ResCache.GetMaterial(materialName).Clone());
            model.Material.SetUVTransform(Vector2.Zero, 0, uvRepeat);

            SetCollidable(node);
//             var phys = node.CreateComponent<RigidBody>();
//             phys.Kinematic = true;
//             phys.CollisionLayer = WorldColisionLayer;
// 
//             var collision = node.CreateComponent<CollisionShape>();
//             collision.SetBox(scale * -1, scale, Quaternion.Identity);

            return node;
        }
    }
}
