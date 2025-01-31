using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chapter3.Utils
{
    internal class InputManager
    {
        private KeyboardState _oldState, _newState;

        public enum Controls
        {
            Left,
            Right,
            SoftDrop,
            HardDrop,
            RotateCW,
            RotateCCW
        }

        Dictionary<Controls, Keys> Controlscheme;

        public InputManager()
        {
            //initialize these states.
            _newState = Keyboard.GetState();
            _oldState = _newState;

            //define the controlscheme for our game:
            Controlscheme = new Dictionary<Controls, Keys>();

            Controlscheme.Add(Controls.Left, Keys.Left);
            Controlscheme.Add(Controls.Right, Keys.Right);
            Controlscheme.Add(Controls.SoftDrop, Keys.Down);
            Controlscheme.Add(Controls.HardDrop, Keys.Up);
            Controlscheme.Add(Controls.RotateCW, Keys.LeftControl);
            Controlscheme.Add(Controls.RotateCCW, Keys.LeftShift);

        }

        public void Update()
        {
            _oldState = _newState;
            _newState = Keyboard.GetState();
        }

        public bool IsPressed(Controls key)
        {
            if (!Controlscheme.ContainsKey(key))
                throw new Exception("Control key does not exist.");

            Keys k = Controlscheme[key];
            return _oldState.IsKeyUp(k) && _newState.IsKeyDown(k);
        }

        public bool IsDown(Controls key)
        {
            if (!Controlscheme.ContainsKey(key))
                throw new Exception("Control key does not exist.");

            Keys k = Controlscheme[key];
            return _newState.IsKeyDown(k);
        }

        public bool IsReleased(Controls key)
        {
            if (!Controlscheme.ContainsKey(key))
                throw new Exception("Control key does not exist.");

            Keys k = Controlscheme[key];
            return _oldState.IsKeyDown(k) && _newState.IsKeyUp(k);
        }
    }
}
