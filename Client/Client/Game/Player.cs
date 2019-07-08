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

        public Player() : base()
        {
            ReceiveSceneUpdates = true;
            ReceiveFixedUpdates = true;
        }

        protected void SetMaxInputs()
        {
            CurrentInput.SetMaxAxisVal(Config.AxisFunctions.Acceleration, 50);
            CurrentInput.SetMaxAxisVal(Config.AxisFunctions.Turning, 360);
            CurrentInput.SetMaxAxisVal(Config.AxisFunctions.SideSlide, 10);
            CurrentInput.SetMaxAxisVal(Config.AxisFunctions.Aiming, 180);
        }

        public virtual void Setup(string name)
        {
            Name = name;
   
            Ship = Node.GetComponent<Ships.ShipNode>();
            if (Ship == null)
                return;

            Team = Ship.TeamColor;

            PhysicsBody = Node.CreateComponent<RigidBody>();
            PhysicsBody.Mass = 10;
            PhysicsBody.Kinematic = false;
            PhysicsBody.SetAngularFactor(new Vector3(0, 1, 0));

            var shape = Node.CreateComponent<CollisionShape>();
            //shape.SetBox(new Vector3(1, 1.25f, 1), new Vector3(0, -0.25f, 0), Quaternion.Identity);
            shape.SetCapsule(1, 1.25f, new Vector3(0,-0.25f,0), Quaternion.Identity);

            Node.NodeCollision += HandleNodeCollision;

            AimX = Node.Rotation.YawAngle;

            SetMaxInputs();

        }

        protected override void OnUpdate(float timeStep)
        {
            base.OnUpdate(timeStep);
        }

        bool LastOnGround = false;
         
        protected override void OnFixedUpdate(PhysicsPreStepEventArgs e)
        {
             base.OnFixedUpdate(e);

            // Turning / horizontal aiming
            AimX += CurrentInput.AxisValues[Config.AxisFunctions.Turning];

            // Vertical aiming
            AimY += CurrentInput.AxisValues[Config.AxisFunctions.Aiming];

            Quaternion q = Quaternion.FromAxisAngle(Vector3.UnitY, AimX );
            PhysicsBody.SetRotation(q);

            if (OnGround)
            {
                if (!LastOnGround)
                    Hud.ChatPanel.AddChatText("Landed", -2);

                if (InAirTime > 0.5f)
                {
                    // landed and we were flying for more than a moment, so trigger an event
                    Landed?.Invoke(this, EventArgs.Empty);
                }
                if (InAirTime > 0)
                {
               //     PhysicsBody.SetLinearFactor(new Vector3(1, 0, 1));
                   PhysicsBody.SetLinearVelocity(new Vector3(PhysicsBody.LinearVelocity.X, 0, PhysicsBody.LinearVelocity.Z));

                    
                }
                InAirTime = 0;
                OnGroundTime += e.TimeStep;
            }
            else
            {
                OnGroundTime = 0;
                if (InAirTime == 0)
                {
                    PhysicsBody.SetLinearFactor(new Vector3(1, 1, 1));
                }
        
                InAirTime += e.TimeStep;
            }

            if (InAirTime < 0.3f && !IsSliding)
            {
                if (CurrentInput.HasLinearInput())
                {
                    Vector3 force = new Vector3(0, 0, 0);

                    float forwardFactor = CurrentInput.AxisValues[Config.AxisFunctions.Acceleration] / CurrentInput.GetMaxVal(Config.AxisFunctions.Acceleration);
                    
                    if (forwardFactor > 0)
                        forwardFactor *= ForwardMoveForce;
                    else
                        forwardFactor *= BackwardsMoveForce;

                    Hud.ChatPanel.AddChatText("Forward Factor " + forwardFactor.ToString(), -2);

                    float sideFactor = CurrentInput.AxisValues[Config.AxisFunctions.SideSlide] / CurrentInput.GetMaxVal(Config.AxisFunctions.SideSlide) * SidewaysMoveForce;

                    force += q * new Vector3(sideFactor, 0, forwardFactor);

                    PhysicsBody.ApplyImpulse(force);
                }
                else
                {
                     if (PhysicsBody.LinearVelocity.Length < 0.1)
                         PhysicsBody.SetLinearVelocity(new Vector3(0, PhysicsBody.LinearVelocity.Y, 0));
                }

                if (OnGround)
                    PhysicsBody.LinearDamping = DampingForce * 0.1f;
                else
                {
                    PhysicsBody.LinearDamping = 0;
                    Vector3 v = PhysicsBody.LinearVelocity;
                    v.Normalize();
                //    PhysicsBody.ApplyImpulse(new Vector3(-DampingForce * v.X, 0, -DampingForce * v.Z));
                }

                if (CurrentInput.ButtonValues[Config.ButtonFunctions.Jump])
                {
                    if (OkToJump && InAirTime < 0.1f)
                    {
                        PhysicsBody.SetLinearFactor(new Vector3(1, 1, 1));
                        PhysicsBody.ApplyImpulse(new Vector3(0,JumpForce,0));
                        InAirTime = 1.0f;

                        Jumped?.Invoke(this, EventArgs.Empty);
                        Hud.ChatPanel.AddChatText("Jummped", -2);
                        OkToJump = false;
                    }
                }
                else
                {
                    OkToJump = true;
                } 
            }
            else
            {
                // air movement
                if (InAirTime > 0.3f && !IsSliding)
                {
                    if (CurrentInput.HasLinearInput())
                    {
                        Vector3 force = new Vector3(0, 0, 0);

                        float sideFactor = CurrentInput.AxisValues[Config.AxisFunctions.SideSlide] / CurrentInput.GetMaxVal(Config.AxisFunctions.SideSlide) * SidewaysMoveForce;
                        float forwardFactor = CurrentInput.AxisValues[Config.AxisFunctions.Acceleration] / CurrentInput.GetMaxVal(Config.AxisFunctions.Acceleration);

                        force += q * new Vector3(sideFactor, 0, forwardFactor);
                        force.Normalize();
                        force *= AirMoveForce;
                        PhysicsBody.ApplyImpulse(force);
                    }
                }
            }
            LastOnGround = OnGround;
            ResetWorldCollision();
            CurrentInput.Clear();
        }

        private void HandleNodeCollision(NodeCollisionEventArgs eventData)
        {
            if (eventData.OtherBody.CollisionLayer == Game.Arena.WorldColisionLayer)
                WorldCollision(eventData);

            // TODO, change this to a base game class so we can use it for bullets
            Player otherPlayer = eventData.OtherBody.GetComponent<Player>();
            if (otherPlayer != null)
                PlayerCollision(eventData);
        }

        protected virtual void PlayerCollision(NodeCollisionEventArgs eventData)
        {

        }

        protected virtual void WorldCollision(NodeCollisionEventArgs eventData)
        {
            foreach (var contact in eventData.Contacts)
            {
                // contact point is below our center of mass
                if (contact.ContactPosition.Y <= Node.Position.Y)
                {
                    float l = contact.ContactNormal.Y;
                    if (l >= 0.75f)
                    {
                        OnGround = true;
                    }
                    else if (l > 0.1)
                    {
                        Hud.ChatPanel.AddChatText("Sliding", -2);
                        IsSliding = true;
                    }

                }
            }

            if (OnGround)
                IsSliding = false;
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
