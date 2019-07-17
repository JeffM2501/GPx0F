using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Urho;
using Urho.Physics;

namespace Game
{
    public class PlayerCore : LogicComponent
    {
        public string Name = string.Empty;
        public Ships.TeamColors Team = Ships.TeamColors.Blue;

        public RigidBody PhysicsBody = null;
        protected float MaxBoostGain = 1;
        protected float BoostGainRate = 4;

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

        protected Vector3 DrivingFriction = new Vector3(20, 0, 10);


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


        public FrameInput CurrentInput = new FrameInput();

        public PlayerCore() : base()
        {
            ReceiveSceneUpdates = true;

            CurrentInput.SetMaxAxisVal(AxisFunctions.Turning, 720);
            CurrentInput.SetMaxAxisVal(AxisFunctions.Aiming, 180);

        }

        protected void CallSpawned ()
        {
            Spawned?.Invoke(this, EventArgs.Empty);
        }
        protected void CallLanded ()
        {
            Landed?.Invoke(this, EventArgs.Empty);
        }
        protected void CallJumped ()
        {
            Jumped?.Invoke(this, EventArgs.Empty);
        }
        protected void CallStartBoosting ()
        {
            StartBoosting?.Invoke(this, EventArgs.Empty);
        }
        protected void CallEndBoosting ()
        {
            EndBoosting?.Invoke(this, EventArgs.Empty);
        }
        protected void CallPowerDepleted ()
        {
            Spawned?.Invoke(this, EventArgs.Empty);
        }
        protected void CallPowerRestored ()
        {
            PowerRestored?.Invoke(this, EventArgs.Empty);
        }
        protected void CallShieldDepleted ()
        {
            ShieldDepleted?.Invoke(this, EventArgs.Empty);
        }
        protected void CallShieldRestored ()
        {
            ShieldRestored?.Invoke(this, EventArgs.Empty);
        }
        protected void CallShieldRechargeStart ()
        {
            ShieldRechargeStart?.Invoke(this, EventArgs.Empty);
        }
        protected void CallShieldDamaged ()
        {
            ShieldDamaged?.Invoke(this, EventArgs.Empty);
        }
        protected void CallStartShieldWarning ()
        {
            StartShieldWarning?.Invoke(this, EventArgs.Empty);
        }
        protected void CallEndShieldWarning ()
        {
            EndShieldWarning?.Invoke(this, EventArgs.Empty);
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

        public virtual void Setup(string name, Ships.TeamColors color)
        {
            Name = name;

  
            Team = color;

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
        }

        protected virtual void HandleNodeCollision(NodeCollisionEventArgs eventData)
        {
            if (eventData.OtherBody.CollisionLayer == Game.Maps.Arena.WorldColisionLayer)
                WorldCollision(eventData);

            // TODO, change this to a base game class so we can use it for bullets
            PlayerCore otherPlayer = eventData.OtherBody.GetComponent<PlayerCore>();
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

        protected virtual void ResetWorldCollision()
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

        public virtual void Spawn(Vector3 postion, Quaternion orientation)
        {
            Node.SetWorldPosition(postion);
            Node.SetWorldRotation(orientation);

            Spawned?.Invoke(this, EventArgs.Empty);
        }
    }
}
