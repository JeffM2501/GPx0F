using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Urho;
using Urho.Gui;
using Urho.Resources;

namespace Client.Game.Hud
{
    public class FillBar : BorderImage
    {
        BorderImage Fill = null;

        public double MaxFillSpeed = 1;

        protected double DesiredFillValue = 0;
        protected double FillValue = 0;
        protected int Inset = 0;

        public double FillFactor { get; }

        [Preserve]
        public FillBar() : base()
        {
            Init();
        }
        [Preserve]
        public FillBar(IntPtr handle) : base(handle)
        {
            Init();
        }

        [Preserve]
        public FillBar(Context context) : base (context)
        {
            Init();
        }

        public void SetFillFactor(double value, bool now = false)
        {
            if (value == DesiredFillValue)
                return;

            DesiredFillValue = value;
            if (now)
                FillValue = value;

            SetFillBar();
        }

        public void SetInset(int value)
        {
            Inset = value;
            Fill.Position = new IntVector2(Inset, Inset);
            SetFillBar();
        }

        protected virtual void Init()
        {
            SetColor(Color.Black);

            ImageRect = new IntRect(48, 0, 64, 16);
            Border = new IntRect(4, 4, 4, 4);

            Fill = new BorderImage();
            AddChild(Fill);
            Fill.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
            Fill.ImageRect = new IntRect(48, 0, 64, 16);
            Fill.Border = new IntRect(4, 4, 4, 4);
            Fill.Position = new IntVector2(Inset, Inset);
            Fill.SetColor(Color.White);

            this.Resized += FillBar_Resized;
        }

        public void Setup(ResourceCache res)
        {
            SetFillBar();
        }

        public void SetFillColor(Color color)
        {
            Fill.SetColor(color);
        }

        private void FillBar_Resized(ResizedEventArgs obj)
        {
            SetFillBar();
        }

        protected void SetFillBar()
        {
            if (Fill == null)
                return;

            Fill.SetSize((int)(Width * FillValue) - (Inset * 2), Height - (Inset*2));
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            if (DesiredFillValue != FillValue)
            {
                double maxThisFrame = MaxFillSpeed * deltaTime;

                double incement = DesiredFillValue - FillValue;
                if (Math.Abs(incement) < 0.001)
                    FillValue = DesiredFillValue;
                else
                {
                    if (Math.Abs(incement) > maxThisFrame)
                        incement = maxThisFrame * Math.Sign(incement);

                    FillValue += incement;
                }
                SetFillBar();
            }
        }
    }
}
