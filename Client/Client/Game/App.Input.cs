﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Game
{
    public partial class App
    {
        protected bool InputInGameMode = false;

        private void SetInputMode(bool game)
        {
            InputInGameMode = game;

            if (InputInGameMode)
                Input.SetMouseMode(Urho.MouseMode.Wrap);
            else
                Input.SetMouseMode(Urho.MouseMode.Absolute);

            Input.SetMouseVisible(!InputInGameMode);
        }

        

        public Player.FrameInput ThisFrameInput = new Player.FrameInput();


        private bool IsAngleFunction(Config.AxisFunctions func)
        {
            return func == Config.AxisFunctions.Aiming || func == Config.AxisFunctions.Turning;
        }

        private static bool useMouse = true;
        protected void UpdateFrameInput(float deltaT)
        {
            if (ThisFrameInput == null)
                return;

    //        ThisFrameInput.Clear();

            if (InputInGameMode)
            {
                // process mouse
                float mouseXValue = Input.MouseMove.X * Config.Current.MouseXSensitivity;
                float mouseYValue = Input.MouseMove.Y * Config.Current.MouseYSensitivity;
                float mouseZValue = Input.MouseMoveWheel * Config.Current.MouseZSensitivity;

                if (useMouse)
                {
                    if (Config.Current.MouseXAxisFunciton != Config.AxisFunctions.None)
                        ThisFrameInput.AxisValues[Config.Current.MouseXAxisFunciton] = mouseXValue;
                    if (Config.Current.MouseYAxisFunciton != Config.AxisFunctions.None)
                        ThisFrameInput.AxisValues[Config.Current.MouseYAxisFunciton] = mouseYValue;
                    if (Config.Current.MouseZAxisFunciton != Config.AxisFunctions.None)
                        ThisFrameInput.AxisValues[Config.Current.MouseZAxisFunciton] = mouseZValue;

                    // validate that everything is within input ranges

                    foreach (Config.AxisFunctions func in Enum.GetValues(typeof(Config.AxisFunctions)))
                    {
                        if (Math.Abs(ThisFrameInput.AxisValues[func]) > (ThisFrameInput.GetMaxVal(func) * deltaT))
                            ThisFrameInput.AxisValues[func] = ThisFrameInput.GetMaxVal(func) * Math.Sign(ThisFrameInput.AxisValues[func]) * deltaT;
                    }
                }

                foreach (var button in Config.Current.MouseButtonFunctions)
                {
                    bool down = Input.GetMouseButtonDown(button.Button);
                    if (down)
                        ThisFrameInput.ButtonValues[button.Function] = true;
                }
            }
            // check for keyboard axes
            foreach (var keyAxis in Config.Current.KeyboardAxisFunctions)
            {
                float val = GetGetKeysetAxisValue(keyAxis.Keys);
                if (val != 0)
                {
                    ThisFrameInput.AxisValues[keyAxis.Function] = val * ThisFrameInput.GetMaxVal(keyAxis.Function);
                    if (IsAngleFunction(keyAxis.Function))
                        ThisFrameInput.AxisValues[keyAxis.Function] *= deltaT;
                }
                    
            }

            foreach (var button in Config.Current.KeyboardButtonFunctions)
            {
                bool down = Input.GetKeyDown(button.ButtonKey);
                if (down)
                    ThisFrameInput.ButtonValues[button.Function] = true;
            }

            // get all active joysticks TODO: Cache this by index to make lookups faster
            Dictionary<string, Urho.JoystickState> joyStates = new Dictionary<string, Urho.JoystickState>();
            for (uint i = 0; i < Input.NumJoysticks; i++)
            {
                Urho.JoystickState state;
                if (Input.TryGetJoystickState(i, out state))
                {
                    joyStates.Add(state.Name.ToString(), state);
                }
            }

            // check each mapped axis
            foreach (var stickAxis in Config.Current.JoystickAxisFunctions)
            {
                if (joyStates.ContainsKey(stickAxis.DeviceName))
                {
                    float val = joyStates[stickAxis.DeviceName].GetAxisPosition(stickAxis.ControlIndex);
                    if (Math.Abs(val) > 0.001)
                        ThisFrameInput.AxisValues[stickAxis.Function] = val * ThisFrameInput.GetMaxVal(stickAxis.Function);

                    if (IsAngleFunction(stickAxis.Function))
                        ThisFrameInput.AxisValues[stickAxis.Function] *= deltaT;
                }
            }

            // button events
            foreach (var button in Config.Current.JoystickButtonFunctions)
            {
                if (joyStates.ContainsKey(button.DeviceName))
                {
                    bool down = false;
                    if (button.IsHat)
                        down = joyStates[button.DeviceName].GetHatPosition(button.ControlIndex) == button.ControlFactor;
                    else
                        down = joyStates[button.DeviceName].GetButtonDown(button.ControlIndex) >= button.ControlFactor;

                    if (down)
                        ThisFrameInput.ButtonValues[button.Function] = true;
                }
            }
        }

        float GetGetKeysetAxisValue(Config.AxisKeyset keyset)
        {
            bool pos = Input.GetKeyDown(keyset.PositiveKey);
            bool neg = Input.GetKeyDown(keyset.NegativeKey);

            if (pos && neg || !pos && !neg)
                return 0;

            float val = Input.GetKeyDown(keyset.HalfSpeedKey) ? 0.5f : 1;

            return val * (neg ? -1 : 1);
        }
    }
}
