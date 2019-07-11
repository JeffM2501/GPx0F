using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Urho;
using Urho.Audio;
using Urho.Physics;
using Urho.Resources;

namespace Client.Game
{
    public class LocalPlayer : Player
    {
        protected Camera AttachedCamera = null;
        protected bool UseMouseLook = false;

        protected SoundSource UIBeeper = null;

        public LocalPlayer() : base()
        {
            UIBeeper = Node.CreateComponent<SoundSource>();
        }

        public void AttachCamera(Camera cam)
        {
            AttachedCamera = cam;
            Node.AddChild(cam.Node);
           // cam.Node.Rotation = Quaternion.FromAxisAngle(Vector3.UnitY, 90);
            cam.Node.Position = new Vector3(0, 1.5f, 0);
        }

        protected override void OnUpdate(float timeStep)
        {
            float maxLook = 15;

            base.OnUpdate(timeStep);
            if (UseMouseLook && AttachedCamera != null && (Math.Abs(AimY) > 1))
            {
                float newAngle = AttachedCamera.Node.Rotation.PitchAngle + (AimY / 60.0f);
                if (Math.Abs(newAngle) > maxLook)
                    newAngle = maxLook * Math.Sign(newAngle);

                AttachedCamera.Node.Rotation = (Quaternion.FromAxisAngle(Vector3.UnitX, newAngle));
            }
        }
    }
}
