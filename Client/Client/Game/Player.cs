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

        public bool OnGround = true;
        public bool IsSliding = false;
        public bool OkToJump = true;

        public bool DidMove = false;

        protected float AimX = 0;
        protected float AimY = 0;

        protected float InAirTime = 0;
        protected float OnGroundTime = 0;
        protected float JumpTime = 0;

        public event EventHandler Landed = null;
        public event EventHandler Jumped = null;

        protected float ForwardMoveForce = 8;
        protected float BackwardsMoveForce = 4;
        protected float SidewaysMoveForce = 1f;

        protected float AirMoveForce = 0.7f;
        protected float DampingForce = 7.0f;
        protected float JumpForce = 1.0f;
        protected float JumpForceLifeTime = 0.125f;

        protected float MaxTilt = 5;
        protected float MaxTiltSpeed = 20;
        protected float MaxSpeedForTilt = 75;

        protected float MaxVel = 50;

        protected Vector3 DrivingFriction = new Vector3(20, 0 , 10);

        public class FrameInput : EventArgs
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

            public event EventHandler<FrameInput> GetUserInput = null;
            public virtual void GetInput(object sender)
            {
                GetUserInput?.Invoke(sender, this);
            }
        }

        public FrameInput CurrentInput = new FrameInput();

        public Player() : base()
        {
            ReceiveSceneUpdates = true;
            ReceiveFixedUpdates = true;
        }

        public Color GetIconColor()
        {
            switch (Team)
            {
                case Ships.TeamColors.Black:
                    return Color.Black;
                case Ships.TeamColors.Blue:
                    return Color.Blue;
                case Ships.TeamColors.Green:
                    return Color.Green;
                case Ships.TeamColors.Purple:
                    return Color.Red + Color.Blue;
                case Ships.TeamColors.Red:
                    return Color.Red;
                case Ships.TeamColors.White:
                    return Color.White;
                case Ships.TeamColors.Yellow:
                    return Color.Yellow;
            }

            return Color.Gray;
        }

        protected void SetMaxInputs()
        {
            CurrentInput.SetMaxAxisVal(Config.AxisFunctions.Acceleration, 50);
            CurrentInput.SetMaxAxisVal(Config.AxisFunctions.Turning, 720);
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
            PhysicsBody.Mass = 1;
            PhysicsBody.Kinematic = false;
            PhysicsBody.SetAngularFactor(new Vector3(0, 1, 0));

            PhysicsBody.RollingFriction = 100f;
            PhysicsBody.Friction = 10f;
            PhysicsBody.AngularDamping = 1;
            PhysicsBody.LinearDamping = 0.25f;

       //     PhysicsBody.SetAnisotropicFriction(DrivingFriction);

            var shape = Node.CreateComponent<CollisionShape>();
            shape.Margin = 0.1f;
           // shape.SetCapsule(1.5f, 2, Vector3.Zero, Quaternion.Identity);
            //shape.SetBox(new Vector3(1, 1.25f, 1), new Vector3(0, -0.25f, 0), Quaternion.Identity);
            shape.SetSphere(1.5f, new Vector3(0,0.5f,0), Quaternion.Identity);

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
            DoPhysicsUpdate(e.TimeStep);
        }

        protected virtual void DoPhysicsUpdate(float timeStep)
        { 
            CurrentInput.GetInput(this);

            float angleTurnDelta = CurrentInput.AxisValues[Config.AxisFunctions.Turning];

            // Turning / horizontal aiming
            AimX += angleTurnDelta;

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
                InAirTime = 0;
                OnGroundTime += timeStep;
            }
            else
            {
                OnGroundTime = 0;      
                InAirTime += timeStep;
            }

            if (JumpTime > 0 && !IsSliding)
            {
                PhysicsBody.ApplyImpulse(new Vector3(0, JumpForce, 0));
                JumpTime -= timeStep;
            }

            DidMove = false;


            if (InAirTime < 0.3f && !IsSliding)
            {
                if (CurrentInput.HasLinearInput())
                {
                    DidMove = true;

                    Vector3 force = new Vector3(0, 0, 0);

                    float forwardFactor = CurrentInput.AxisValues[Config.AxisFunctions.Acceleration] / CurrentInput.GetMaxVal(Config.AxisFunctions.Acceleration);
                    
                    if (forwardFactor > 0)
                        forwardFactor *= ForwardMoveForce;
                    else
                        forwardFactor *= BackwardsMoveForce;

                    float sideFactor = CurrentInput.AxisValues[Config.AxisFunctions.SideSlide] / CurrentInput.GetMaxVal(Config.AxisFunctions.SideSlide) * SidewaysMoveForce;

                    force += q * new Vector3(sideFactor, 0, forwardFactor);

                    if (PhysicsBody.LinearVelocity.LengthFast < MaxVel)
                        PhysicsBody.ApplyImpulse(force);
                }

                if (CurrentInput.ButtonValues[Config.ButtonFunctions.Jump])
                {
                    if (OkToJump && InAirTime < 0.1f)
                    {
                        PhysicsBody.ApplyImpulse(new Vector3(0,JumpForce,0));
                        JumpTime = JumpForceLifeTime;
                        InAirTime = 0.5f;

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
                        if (PhysicsBody.LinearVelocity.LengthFast < MaxVel)
                            PhysicsBody.ApplyImpulse(force);
                    }
                }
            }

            if (Ship != null)
            {
                float angleTiltRatio = 10;
                float maxRotSpeed = MaxTiltSpeed * timeStep;
                float resetSpeed = maxRotSpeed * 0.1f;
                float maxTilt = MaxTilt * (PhysicsBody.LinearVelocity.Length / MaxSpeedForTilt);

                float desiredDelta = angleTurnDelta * angleTiltRatio * -1;
                if (Math.Abs(desiredDelta) > maxRotSpeed)
                    desiredDelta = maxRotSpeed * Math.Sign(desiredDelta);

                Ship.SkidTilt += desiredDelta;
                if (Math.Abs(Ship.SkidTilt) > maxTilt)
                    Ship.SkidTilt = maxTilt * Math.Sign(Ship.SkidTilt);
                else if (desiredDelta == 0)
                {
                    if (Math.Abs(Ship.SkidTilt) <= resetSpeed)
                        Ship.SkidTilt = 0;
                    else
                        Ship.SkidTilt -= Math.Sign(Ship.SkidTilt) * resetSpeed;
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
             //   if (!(contact.ContactPosition.Y > Node.Position.Y))
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
                    else
                    {
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
