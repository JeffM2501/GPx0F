using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Urho;
using Urho.Physics;
using Urho.Resources;

namespace Game.Maps
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

        public void AddStaticPhysics(Node node)
        {
            var phys = node.CreateComponent<RigidBody>();
            phys.Kinematic = true;
            phys.CollisionLayer = WorldColisionLayer;
            phys.SetLinearFactor(Vector3.Zero);
            phys.SetAngularFactor(Vector3.Zero);
            phys.UseGravity = false;
            phys.Mass = 100000000;
        }

        public Node SetCollidable(Node node, Vector3 size, Vector3 origin, Quaternion rot)
        {
            AddStaticPhysics(node);

            var collision = node.CreateComponent<CollisionShape>();
            collision.SetBox(size, origin, rot);

            return node;
        }

        public Node SetCollidable(Node node)
        {
            var sm = node.GetComponent<StaticModel>();
            AddStaticPhysics(node);

            var collision = node.CreateComponent<CollisionShape>();
            collision.SetTriangleMesh(sm.Model, 0, Vector3.One, Vector3.Zero, Quaternion.Identity);

            return node;
        }

        public Node MakeBox(string name, Vector3 pos, Vector3 scale, string materialName, Vector2 uvRepeat)
        {
            var node = World.CreateChild(name);
            node.Position = pos;
            node.Scale = scale;
            var model = node.CreateComponent<StaticModel>();
            model.Model = ResCache.GetModel("Models/Box.mdl");
            model.CastShadows = true;
            model.SetMaterial(ResCache.GetMaterial(materialName).Clone());
            model.Material.SetUVTransform(Vector2.Zero, 0, uvRepeat);

            SetCollidable(node, Vector3.One,Vector3.Zero,Quaternion.Identity);

            return node;
        }

        public void MakeBorders (float size, float wallHeight, string materialName, float uvRepeat)
        {
            Vector3 box = new Vector3(wallHeight, wallHeight, wallHeight);

            var node = MakeBox("eastWall", new Vector3(size, wallHeight / 2, 0.0f), new Vector3(wallHeight, wallHeight, ((size) * 2)), materialName, new Vector2(size * uvRepeat, wallHeight * uvRepeat));
            node.GetComponent<CollisionShape>().SetBox(new Vector3(1, 20 ,1), new Vector3(0, 9 ,0),Quaternion.Identity);

            node = MakeBox("westWall", new Vector3(-size, wallHeight / 2, 0.0f), new Vector3(wallHeight, wallHeight, ((size) * 2)), materialName, new Vector2(size * uvRepeat, wallHeight * uvRepeat));
            node.GetComponent<CollisionShape>().SetBox(new Vector3(1, 20, 1), new Vector3(0, 9, 0), Quaternion.Identity);

            node = MakeBox("northWall", new Vector3(0, wallHeight / 2, size ), new Vector3(((size ) * 2), wallHeight, wallHeight), materialName, new Vector2(size * uvRepeat, wallHeight * uvRepeat));
            node.GetComponent<CollisionShape>().SetBox(new Vector3(1, 20, 1), new Vector3(0, 9, 0), Quaternion.Identity);

            node = MakeBox("southWall", new Vector3(0, wallHeight / 2, -size), new Vector3(((size) * 2), wallHeight, wallHeight), materialName, new Vector2(size * uvRepeat, wallHeight * uvRepeat));
            node.GetComponent<CollisionShape>().SetBox(new Vector3(1, 20, 1), new Vector3(0, 9, 0), Quaternion.Identity);

        }

        public Node MakePlaneGround( float size, string material, float repeat)
        {
            Node ground = World.CreateChild("ground");
            ground.Position = new Vector3(0.0f, 0.0f, 0.0f);
            ground.Scale = new Vector3(size * 4, 1, size * 4);

            var model = ground.CreateComponent<StaticModel>();
            model.Model = ResCache.GetModel("Models/Plane.mdl");
            model.SetMaterial(ResCache.GetMaterial(material).Clone());
            model.Material.SetUVTransform(Vector2.Zero, 0, size * repeat);
            AddStaticPhysics(ground);
            var collision = ground.CreateComponent<CollisionShape>();
            collision.SetStaticPlane(Vector3.Zero, Quaternion.Identity);
            ground.GetComponent<RigidBody>().Friction = 1;

            return ground;
        }

    }
}
