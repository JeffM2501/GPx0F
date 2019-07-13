using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Urho;
using Urho.Physics;
using Urho.Resources;
using Urho.Audio;

using Game;

namespace Client.Game
{
    public class ClientPlayer : PlayerCore
    {
        public Ships.ShipNode Ship = null;

        public SoundSource3D ContactSoundOrigin = null;
        public SoundSource3D EngineSoundOrigin = null;


        public ClientPlayer() : base()
        {
            ReceiveFixedUpdates = true;
        }

        public virtual void Setup(string name)
        {
            Ship = Node.GetComponent<Ships.ShipNode>();
            if (Ship == null)
                return;

            base.Setup(name, Ship.TeamColor);
           
            ContactSoundOrigin = Node.CreateComponent<SoundSource3D>();
            ContactSoundOrigin.SetSoundType(SoundType.Effect.ToString());
            ContactSoundOrigin.NearDistance = 50;
            ContactSoundOrigin.FarDistance = 500;

            EngineSoundOrigin = Node.CreateComponent<SoundSource3D>();
            EngineSoundOrigin.SetSoundType(SoundType.Effect.ToString());
            EngineSoundOrigin.NearDistance = 50;
            EngineSoundOrigin.FarDistance = 500;

            Jumped += new EventHandler((s, e) => ContactSoundOrigin?.Play(Ship.JumpSound));
            Landed += new EventHandler((s, e) => ContactSoundOrigin?.Play(Ship.LandingSound));
            Spawned += new EventHandler((s, e) => ContactSoundOrigin?.Play(Ship.SpawnSound));

            StartBoosting += new EventHandler((s, e) => { EngineSoundOrigin.Gain = 0;  EngineSoundOrigin?.Play(Ship.BoostSound); });
            EndBoosting += new EventHandler((s, e) => EngineSoundOrigin?.Stop());
        }

      
        protected override void OnUpdate(float timeStep)
        {
            base.OnUpdate(timeStep);

            CurrentInput.ClearButtons();

            if (Boosting && EngineSoundOrigin.Playing && EngineSoundOrigin.Gain < MaxBoostGain)
            {
                EngineSoundOrigin.Gain += BoostGainRate * timeStep;
                if (EngineSoundOrigin.Gain > MaxBoostGain)
                    EngineSoundOrigin.Gain = MaxBoostGain;
            }
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

            float angleTurnDelta = CurrentInput.AxisValues[AxisFunctions.Turning];

            // Turning / horizontal aiming
            AimX += angleTurnDelta;

            // Vertical aiming
            AimY += CurrentInput.AxisValues[AxisFunctions.Aiming];

            float effectiveMoveFactor = 1;
            float effecitveMaxSpeed = MaxVel;
            if (Boosting)
            {
                effectiveMoveFactor = BoostForceMultiplyer;
                effecitveMaxSpeed = BoostMaxVel;
            }

            Quaternion q = Quaternion.FromAxisAngle(Vector3.UnitY, AimX );
            PhysicsBody.SetRotation(q);

            if (OnGround)
            {
                if (InAirTime > 0.5f)
                {
                    // landed and we were flying for more than a moment, so trigger an event
                    CallLanded();
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

                    float forwardFactor = CurrentInput.AxisValues[AxisFunctions.Acceleration] / CurrentInput.GetMaxVal(AxisFunctions.Acceleration);
                    
                    if (forwardFactor > 0)
                        forwardFactor *= ForwardMoveForce * effectiveMoveFactor;
                    else
                        forwardFactor *= BackwardsMoveForce;

                    float sideFactor = CurrentInput.AxisValues[AxisFunctions.SideSlide] / CurrentInput.GetMaxVal(AxisFunctions.SideSlide) * SidewaysMoveForce;

                    force += q * new Vector3(sideFactor, 0, forwardFactor);

                    if (PhysicsBody.LinearVelocity.LengthFast < effecitveMaxSpeed)
                        PhysicsBody.ApplyImpulse(force);
                }

                if (CurrentInput.ButtonValues[ButtonFunctions.Jump])
                {
                    if (OkToJump && InAirTime < 0.1f)
                    {
                        PhysicsBody.ApplyImpulse(new Vector3(0,JumpForce,0));
                        JumpTime = JumpForceLifeTime;
                        InAirTime = 0.5f;

                        CallJumped();
   
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

                        float sideFactor = CurrentInput.AxisValues[AxisFunctions.SideSlide] / CurrentInput.GetMaxVal(AxisFunctions.SideSlide) * SidewaysMoveForce;
                        float forwardFactor = CurrentInput.AxisValues[AxisFunctions.Acceleration] / CurrentInput.GetMaxVal(AxisFunctions.Acceleration);

                        force += q * new Vector3(sideFactor, 0, forwardFactor);
                        force.Normalize();
                        force *= AirMoveForce * effectiveMoveFactor;
                        if (PhysicsBody.LinearVelocity.LengthFast < effecitveMaxSpeed)
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
    }
}
