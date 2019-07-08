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
    public class Player : LogicComponent
    {
        public string Name = string.Empty;
        public Ships.TeamColors Team = Ships.TeamColors.Blue;
        public Ships.ShipNode Ship = null;

        public RigidBody PhysicsBody = null;

        protected bool OnGround = true;
        protected bool IsSliding = false;
        protected bool OkToJump = true;

        protected float AimX = 0;
        protected float AimY = 0;

        protected float InAirTime = 0;
        protected float OnGroundTime = 0;

        public event EventHandler Landed = null;
        public event EventHandler Jumped = null;

        protected float ForwardMoveForce = 25f;
        protected float BackwardsMoveForce = 10f;
        protected float SidewaysMoveForce = 5f;

        protected float AirMoveForce = 1.0f;
        protected float DampingForce = 5.0f;
        protected float JumpForce = 450.0f;

        public class FrameInput
        {
            public Dictionary<Config.AxisFunctions, float> AxisValues = new Dictionary<Config.AxisFunctions, float>();
            public Dictionary<Config.ButtonFunctions, bool> ButtonValues = new Dictionary<Config.ButtonFunctions, bool>();

            public Dictionary<Config.AxisFunctions, float> AxisMaxValues = new Dictionary<Config.AxisFunctions, float>();

            public FrameInput()
            {
                foreach (Config.AxisFunctions value in Enum.GetValues(typeof(Config.AxisFunctions)))
                    AxisValues.Add(value, 0);

                foreach (Config.ButtonFunctions value in Enum.GetValues(typeof(Config.ButtonFunctions)))
                    ButtonValues.Add(value, false);
            }

            public void SetMaxAxisVal(Config.AxisFunctions func, float value)
            {
                if (AxisMaxValues.ContainsKey(func))
                    AxisMaxValues[func] = value;
                else
                    AxisMaxValues.Add(func, value);
            }

            public float GetMaxVal(Config.AxisFunctions func)
            {
                if (AxisMaxValues.ContainsKey(func) && AxisMaxValues[func] != 0)
                    return AxisMaxValues[func];
                return 1;
            }

            public void Clear()
            {
                foreach (var item in ButtonValues.Keys.ToArray())
                    ButtonValues[item] = false;
                foreach (var item in AxisValues.Keys.ToArray())
                    AxisValues[item] = 0;
            }

            public bool HasLinearInput()
            {
                return AxisValues[Config.AxisFunctions.Acceleration] != 0 || AxisValues[Config.AxisFunctions.SideSlide] != 0;
            }
        }

        public FrameInput CurrentInput = new FrameInput();

        public Player(string name, Ships.TeamColors team, Node root, ResourceCache res)
        {
            Name = name;
            Ship = Ships.GetShipNode(res, root, team, "Mk3");
            Ship.Node.AddComponent(this);
            Team = team;

            PhysicsBody = Ship.Node.CreateComponent<RigidBody>();
            PhysicsBody.Mass = 10;
            PhysicsBody.Kinematic = false;

            ReceiveFixedUpdates = true;

            var shape = Ship.Node.CreateComponent<CollisionShape>();
            shape.SetCapsule(1, 1, Vector3.Zero, Quaternion.Identity);

            Node.NodeCollision += HandleNodeCollision;
        }

        protected override void OnFixedUpdate(PhysicsPreStepEventArgs e)
        {
            base.OnFixedUpdate(e);

            // Turning / horizontal aiming
            if (AimX != CurrentInput.AxisValues[Config.AxisFunctions.Turning])
                AimX = CurrentInput.AxisValues[Config.AxisFunctions.Turning];

            // Vertical aiming
            if (AimY != CurrentInput.AxisValues[Config.AxisFunctions.Aiming])
                AimY = CurrentInput.AxisValues[Config.AxisFunctions.Aiming];

            Quaternion q = Quaternion.FromAxisAngle(Vector3.UnitY, AimX );
            PhysicsBody.SetRotation(q);

            Vector3 vel = PhysicsBody.LinearVelocity;

            if (OnGround)
            {
                if (InAirTime > 0.5f)
                {
                    // landed and we were flying for more than a moment, so trigger an event
                    Landed?.Invoke(this, EventArgs.Empty);
                }
                InAirTime = 0;
                OnGroundTime += e.TimeStep;
            }
            else
            {
                OnGroundTime = 0;
                InAirTime += e.TimeStep;
            }

            if (InAirTime > 0.3f && !IsSliding)
            {
                if (CurrentInput.HasLinearInput())
                {
                    Vector3 force = new Vector3(0, 0, 0);

                    float forwardFactor = CurrentInput.AxisValues[Config.AxisFunctions.Acceleration] / CurrentInput.AxisMaxValues[Config.AxisFunctions.Acceleration];
                    if (forwardFactor > 0)
                        forwardFactor *= ForwardMoveForce;
                    else
                        forwardFactor *= BackwardsMoveForce;

                    float sideFactor = CurrentInput.AxisValues[Config.AxisFunctions.SideSlide] / CurrentInput.AxisMaxValues[Config.AxisFunctions.SideSlide] * SidewaysMoveForce;

                    force += q * new Vector3(sideFactor, 0, forwardFactor);

                    PhysicsBody.ApplyImpulse(force);
                }

                PhysicsBody.ApplyImpulse(new Vector3(-DampingForce * vel.X, 0, -DampingForce * vel.Z));

                if (CurrentInput.ButtonValues[Config.ButtonFunctions.Jump])
                {
                    if (OkToJump && InAirTime < 0.1f)
                    {
                        PhysicsBody.ApplyImpulse(new Vector3(0,JumpForce,0));
                        InAirTime = 1.0f;

                        Jumped?.Invoke(this, EventArgs.Empty);
                        OkToJump = false;
                    }
                }
                else
                    OkToJump = true;
            }
            else
            {
                // air movement
                if (InAirTime > 0.3f && !IsSliding)
                {
                    if (CurrentInput.HasLinearInput())
                    {
                        Vector3 force = new Vector3(0, 0, 0);

                        float sideFactor = CurrentInput.AxisValues[Config.AxisFunctions.SideSlide] / CurrentInput.AxisMaxValues[Config.AxisFunctions.SideSlide] * SidewaysMoveForce;
                        float forwardFactor = CurrentInput.AxisValues[Config.AxisFunctions.Acceleration] / CurrentInput.AxisMaxValues[Config.AxisFunctions.Acceleration];

                        force += q * new Vector3(sideFactor, 0, forwardFactor);
                        force.Normalize();
                        force *= AirMoveForce;
                        PhysicsBody.ApplyImpulse(force);
                    }
                }
            }

            ResetWorldCollision();
            CurrentInput.Clear();
        }

        private void HandleNodeCollision(NodeCollisionEventArgs obj)
        {
          //  obj.
        }

        protected void ResetWorldCollision()
        {
            if (PhysicsBody.Active)
            {
                OnGround = false;
                IsSliding = false;
            }
            else
            {
                // If body is not active, assume it rests on the ground
                OnGround = true;
                IsSliding = false;
            }
        }
    }
}
