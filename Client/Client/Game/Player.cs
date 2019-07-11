using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Urho;
using Urho.Physics;
using Urho.Resources;
using Urho.Audio;

namespace Client.Game
{
    public class Player : LogicComponent
    {
        public string Name = string.Empty;
        public Ships.TeamColors Team = Ships.TeamColors.Blue;
        public Ships.ShipNode Ship = null;

        public RigidBody PhysicsBody = null;
        public SoundSource3D SoundOrigin = null;
        public SoundSource SoundOrigin2D = null;

        public bool OnGround = true;
        public bool IsSliding = false;
        public bool OkToJump = true;

        public bool DidMove = false;

        protected float AimX = 0;
        protected float AimY = 0;

        protected float InAirTime = 0;
        protected float OnGroundTime = 0;
        protected float JumpTime = 0;

        public event EventHandler Spawned = null;
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
        protected float BoostMaxVel = 75;

        protected Vector3 DrivingFriction = new Vector3(20, 0 , 10);


        // core gameplay stats
        public float CurrentPower { get; protected set; } = 100;                 // Current Aux Power
        public float CurrentShields { get; protected set; } = 100;               // Current shield power (health)

        public float MaxPower { get; protected set; } = 100;                     // Max Aux Power
        public float MaxShields { get; protected set; } = 100;                   // Max shield power (health)

        public float ShieldRechargeRate { get; protected set; } = 10;            // shield points per second after not being damaged for a while
        public float PowerGenerationRate { get; protected set; } = 5;            // power points per second
        public float ShieldDamageRechargeDelay { get; protected set; } = 1;      // time after damage before shields recharge
        public float ShieldRechargePowerDrain { get; protected set; } = 5;       // power per second needed to charge shields
        public float BoostPowerDrain { get; protected set; } = 15;               // power per second while boosting
        public float BoostForceMultiplyer { get; protected set; } = 1.5f;        // force multiplier while boosting

        public float ShieldWarningLimit = 25;            // warning generated when shields go below this level

        // frame stats
        protected float ShieldRechargeStartDelay = 0;       // how much longer to wait until shields can start recharging.

        protected bool Boosting = false;
        protected bool LastFrameBoost = false;


        public event EventHandler StartBoosting = null;
        public event EventHandler EndBoosting = null;

        public event EventHandler PowerDepleted = null;
        public event EventHandler PowerRestored = null;

        public event EventHandler ShieldDepleted = null;
        public event EventHandler ShieldRestored = null;
        public event EventHandler ShieldRechargeStart = null;
        public event EventHandler ShieldDamaged = null;

        public event EventHandler StartShieldWarning = null;
        public event EventHandler EndShieldWarning = null;

        // sound assets
        Sound SpawnSound = null;
        Sound JumpSound = null;
        Sound LandingSound = null;

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

            public void ClearAxes()
            {
                foreach (var item in AxisValues.Keys.ToArray())
                    AxisValues[item] = 0;
            }

            public void ClearButtons()
            {
                foreach (var item in ButtonValues.Keys.ToArray())
                    ButtonValues[item] = false;
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
            shape.SetSphere(1.5f, new Vector3(0, 0.5f, 0), Quaternion.Identity);

            Node.NodeCollision += HandleNodeCollision;

            AimX = Node.Rotation.YawAngle;

            SoundOrigin = Node.CreateComponent<SoundSource3D>();
            SoundOrigin.SetSoundType(SoundType.Effect.ToString());
            SoundOrigin.NearDistance = 50;
            SoundOrigin.FarDistance = 500;

            SpawnSound = Ship.Resources.GetSound("Legacy/zone/pop.wav");
            JumpSound = Ship.Resources.GetSound("Legacy/zone/jump.wav");
            LandingSound = Ship.Resources.GetSound("Legacy/zone/land.wav");

            SoundOrigin2D = Node.CreateComponent<SoundSource>();
            SoundOrigin2D.SetSoundType(SoundType.Effect.ToString());

            Jumped += new EventHandler((s, e) => SoundOrigin?.Play(JumpSound));
            Landed += new EventHandler((s, e) => SoundOrigin?.Play(LandingSound));
            Spawned += new EventHandler((s, e) => SoundOrigin?.Play(SpawnSound));

            SetMaxInputs();
        }

        public void TakeDamage(float damage)
        {
            bool shieldBelowWarn = CurrentShields <= ShieldWarningLimit;

            CurrentShields -= damage;
            ShieldDamaged?.Invoke(this, EventArgs.Empty);

            if (!shieldBelowWarn && CurrentShields <= ShieldWarningLimit)
                StartShieldWarning?.Invoke(this, EventArgs.Empty);

            if (CurrentShields <= 0)
                ShieldDepleted?.Invoke(this, EventArgs.Empty);

            // start the clock for a recharge
            ShieldRechargeStartDelay = ShieldDamageRechargeDelay;
        }

        protected override void OnUpdate(float timeStep)
        {
            base.OnUpdate(timeStep);

            bool fullPower = CurrentPower >= MaxPower;
            bool emptyPower = CurrentPower <= 0;

            bool fullShields = CurrentShields >= MaxShields;
            bool shieldBelowWarn = CurrentShields <= ShieldWarningLimit;

            // add new power this frame
            CurrentPower += PowerGenerationRate * timeStep;

            LastFrameBoost = Boosting;
            Boosting = CurrentInput.ButtonValues[Config.ButtonFunctions.Boost];
            if (Boosting)
            {
                float boostPowerNeeded = BoostPowerDrain * timeStep;
                if (boostPowerNeeded > CurrentPower)
                    Boosting = false;
                else
                    CurrentPower -= boostPowerNeeded;
            }

            if (CurrentShields < MaxShields)
            {
                float powerNeededToCharge = ShieldRechargePowerDrain * timeStep;
                if (powerNeededToCharge > CurrentPower)
                {
                    // we can't charge, so don't
                    // but go ahead and tick the delay down if it won't go to zero, so that if they have power next frame, they will charge.
                    if (ShieldRechargeStartDelay > timeStep)
                        ShieldRechargeStartDelay -= timeStep;
                }
                else
                {
                    bool charge = true;
                    if (ShieldRechargeStartDelay > 0)
                    {
                        ShieldRechargeStartDelay -= timeStep;
                        if (ShieldRechargeStartDelay <= 0)
                            ShieldRechargeStart?.Invoke(this, EventArgs.Empty);
                        else
                            charge = false;
                    }

                    if (charge)
                    {
                        ShieldRechargeStartDelay = 0;
                        CurrentPower -= powerNeededToCharge;
                        CurrentShields += ShieldRechargeRate * timeStep;

                        if (CurrentShields > MaxShields)
                            CurrentPower = MaxShields;
                    }
                }
            }

            if (Boosting && !LastFrameBoost)
                StartBoosting?.Invoke(this, EventArgs.Empty);
            else if (!Boosting && LastFrameBoost)
                EndBoosting?.Invoke(this, EventArgs.Empty);

            if (CurrentPower > MaxPower)
                CurrentPower = MaxPower;

            if (shieldBelowWarn && CurrentShields > ShieldWarningLimit)
                EndShieldWarning?.Invoke(this, EventArgs.Empty);

            if (!fullShields && CurrentShields >= MaxShields)
                ShieldRestored?.Invoke(this, EventArgs.Empty);

            if (!fullPower && CurrentPower >= MaxPower)
                PowerRestored?.Invoke(this, EventArgs.Empty);
            else if (!emptyPower && CurrentPower <= 0)
                PowerDepleted?.Invoke(this, EventArgs.Empty);

            CurrentInput.ClearButtons();
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
            CurrentInput.ClearAxes();
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

        public void Spawn(Vector3 postion, Quaternion orientation)
        {
            Node.SetWorldPosition(postion);
            Node.SetWorldRotation(orientation);

            Spawned?.Invoke(this, EventArgs.Empty);
        }
    }
}
