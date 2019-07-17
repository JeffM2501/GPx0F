using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Urho;


namespace Client.Tutorials
{
    public class BasicTutorial : TutorialAPI.ITutorial
    {
        public string DisplayName => ClientResources.BasicTutorialName;

        public string DisplayText => ClientResources.BasicTutorialDescription;

        public bool UseSimpleArena => true;

        public int ArenaSize => 600;

        public bool Enabled => true;

        public int SortOrder => 1;

        protected enum TutorialStates
        {
            Startup,
            Movement,
            Jumping,
            Boosting,
        }
        protected TutorialStates TutState = TutorialStates.Startup;

        protected bool Jumped = false;
        protected bool Boosted = false;
        protected float[] DistanceMoved = new float[] { 0, 0, 0, 0 };
        protected float[] DistanceRotated = new float[] { 0, 0 };

        protected Vector3 LastPos = Vector3.Zero;
        protected Quaternion LastOrientation = Quaternion.Identity;


        protected delegate void TimerEventCallback(object data);

        private double CurrentTimer = -1;
        private object TimerData = null;
        private TimerEventCallback TimerEvent = null;


        protected bool SetTimer(double lenght, TimerEventCallback callback, object data = null)
        {
            if (CurrentTimer > 0)
                return false;

            CurrentTimer = lenght;
            TimerEvent = callback;
            TimerData = data;
            return true;
        }

        protected void CheckTimer(double deltaT)
        {
            if (CurrentTimer > 0)
            {
                CurrentTimer -= deltaT;
                if (CurrentTimer <= 0)
                {
                    TimerEvent?.Invoke(TimerData);
                    CurrentTimer = -1;
                    TimerEvent = null;
                    TimerData = null;
                }
            }
        }

        public void Cleanup()
        {
        }

        public void Init(string langauge)
        {
        }

        public void Startup()
        {
            TutorialAPI.SpawnTutorialPlayer(Vector3.Zero, Quaternion.Identity);

            LastPos = TutorialAPI.UserPlayer.Node.Position;
            LastOrientation = TutorialAPI.UserPlayer.Node.Rotation;

            TutorialAPI.AddTextMessage("Welcome to GPx0F");
            SetTimer(2, SetMoveTutorial);
        }

        protected void SetMoveTutorial(object data)
        {
            TutorialAPI.AddTextMessage("Use the W/A/S/D keys to move your tank\nUse the mouse or arrow keys to steer");
            TutState = TutorialStates.Movement;
        }

        protected void SetJumpTutorial()
        {
            TutorialAPI.AddTextMessage("Press tab to jump");
            TutState = TutorialStates.Jumping;
        }

        protected void SetBoostTutorial()
        {
            TutorialAPI.AddTextMessage("Press Shift to boost");
            TutState = TutorialStates.Boosting;

            TutorialAPI.UserPlayer.StartBoosting += UserPlayer_StartBoosting;
        }

        private void UserPlayer_StartBoosting(object sender, EventArgs e)
        {
            if (TutState != TutorialStates.Boosting)
                return;

            TutorialAPI.AddTextMessage("Boosting uses power");
            SetTimer(15, EndBoostTut);
        }

        protected void EndBoostTut(object data)
        {
            TutorialAPI.AddTextMessage(string.Empty);
        }

        public void Update(double timeStep)
        {
            CheckTimer(timeStep);

            Vector3 posDelta = TutorialAPI.UserPlayer.Node.Position - LastPos;
            Quaternion rotDelta = TutorialAPI.UserPlayer.Node.Rotation - LastOrientation;
            LastPos = TutorialAPI.UserPlayer.Node.Position;
            LastOrientation = TutorialAPI.UserPlayer.Node.Rotation;

            if (posDelta.X > 0.01f)
                DistanceMoved[0] += posDelta.X;
            else if (posDelta.X < -0.01f)
                DistanceMoved[1] += Math.Abs(posDelta.X);

            if (posDelta.Z > 0.01f)
                DistanceMoved[2] += posDelta.Z;
            else if (posDelta.Z < -0.01f)
                DistanceMoved[3] += Math.Abs(posDelta.Z);

            if (rotDelta.YawAngle > 0.01)
                DistanceRotated[0] += rotDelta.YawAngle;
            else if (rotDelta.YawAngle < -0.01)
                DistanceRotated[1] += Math.Abs(rotDelta.YawAngle);


            if (posDelta.Z > 0.1)
                Jumped = true;

            switch (TutState)
            {
                case TutorialStates.Movement:
                    if (DistanceMoved[0] > 1 && DistanceMoved[1] > 1 && DistanceMoved[2] > 1 && DistanceMoved[3] > 1 && DistanceRotated[0] > 1 && DistanceRotated[1] > 1)
                    {
                        SetJumpTutorial();
                    }
                    break;

                case TutorialStates.Jumping:
                    if (Jumped)
                    {
                        SetBoostTutorial();
                    }
                    break;
            }
        }
    }
}
