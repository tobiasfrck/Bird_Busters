using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Winged_Warfare
{
    public class MouseMovement
    {
        public static MouseState CurMouseState;
        public static bool Control = false;
        public static bool EnableControl = false;
        public static bool ControlButtonPressed = false;


        //setzt die Mousposition in die mitte des Bildes bevor das Spiel beginnt;
        public static void Init()
        {
            Mouse.SetPosition(Game1.Width / 2, Game1.Height / 2);
        }

        public static void Update()
        {


            if (Keyboard.GetState().IsKeyUp(Keys.L) && ControlButtonPressed)
                ControlButtonPressed = false;
            if (Keyboard.GetState().IsKeyDown(Keys.L) && !ControlButtonPressed)
            {
                Control = !Control;
                ControlButtonPressed = true;
                Mouse.SetPosition(Game1.Width / 2, Game1.Height / 2);
            }


            //Graphik Einstellungen automatisch Hinzufügen

            if (!Control)
            {
                CurMouseState = Mouse.GetState();
                PlayerMovement.SetX((float)(CurMouseState.X - Game1.Width / 2) / 1000);
                PlayerMovement.SetY((float)(CurMouseState.Y - Game1.Height / 2) / 1000);
                Mouse.SetPosition(Game1.Width / 2, Game1.Height / 2);
            }
        }
    }
}

