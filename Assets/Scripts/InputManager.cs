using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    enum KbInput
    {
        Left,
        Right,
        Up,
        Down,
        Run,
        Jump,
        Attack1,
        Attack2
    }
    class InputManager
    {
        KeyCode[] KbButtons;
        private InputManager()
        {
            KbButtons = new KeyCode[8];
            KbButtons[(int)KbInput.Left] = KeyCode.LeftArrow;
            KbButtons[(int)KbInput.Right] = KeyCode.RightArrow;
            KbButtons[(int)KbInput.Up] = KeyCode.UpArrow;
            KbButtons[(int)KbInput.Down] = KeyCode.DownArrow;
            KbButtons[(int)KbInput.Run] = KeyCode.LeftShift;
            KbButtons[(int)KbInput.Jump] = KeyCode.Z;
            KbButtons[(int)KbInput.Attack1] = KeyCode.X;
            KbButtons[(int)KbInput.Attack2] = KeyCode.C;
        }
    }
}
