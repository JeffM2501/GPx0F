using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{

    public enum AxisFunctions
    {
        None,
        Turning,
        Acceleration,
        Aiming,
        SideSlide,
    }

    public enum ButtonFunctions
    {
        None,
        Jump,
        PrimaryFire,
        SecondaryFire,
        Drop,
        Accessory,
        Spawn,
        StartChat,
        SendChat,
        Menu,
        MenuBack,
        MenuAccept,
        Boost,
        ToggleReverseThrottle,
        HoldReverseThrottle,
    }

    public class FrameInput : EventArgs
    {
        public Dictionary<AxisFunctions, float> AxisValues = new Dictionary<AxisFunctions, float>();
        public Dictionary<ButtonFunctions, bool> ButtonValues = new Dictionary<ButtonFunctions, bool>();

        public Dictionary<AxisFunctions, float> AxisMaxValues = new Dictionary<AxisFunctions, float>();

        public FrameInput()
        {
            foreach (AxisFunctions value in Enum.GetValues(typeof(AxisFunctions)))
                AxisValues.Add(value, 0);

            foreach (ButtonFunctions value in Enum.GetValues(typeof(ButtonFunctions)))
                ButtonValues.Add(value, false);
        }

        public void SetMaxAxisVal(AxisFunctions func, float value)
        {
            if (AxisMaxValues.ContainsKey(func))
                AxisMaxValues[func] = value;
            else
                AxisMaxValues.Add(func, value);
        }

        public float GetMaxVal(AxisFunctions func)
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
            return AxisValues[AxisFunctions.Acceleration] != 0 || AxisValues[AxisFunctions.SideSlide] != 0;
        }

        public event EventHandler<FrameInput> GetUserInput = null;
        public virtual void GetInput(object sender)
        {
            GetUserInput?.Invoke(sender, this);
        }
    }
}
