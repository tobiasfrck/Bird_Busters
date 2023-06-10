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
        public static Vector2 Position = new Vector2(0, 0);
        public static MouseState CurMouseState;
        public static bool Control = false;
        public static bool Enablecontrol = false;


        //setzt die Mousposition in die mitte des Bildes bevor das Spiel beginnt;
        public static void Init()
        {
            Mouse.SetPosition(Game1.Width / 2, Game1.Height / 2);
        }

        public static void Update()
        {

            //Graphik Einstellungen automatisch Hinzufügen
            if (Keyboard.GetState().IsKeyDown(Keys.L) && Enablecontrol)
            {
                Control = !Control;
            }

            if (!Control)
            {
                CurMouseState = Mouse.GetState();
                PlayerMovement.SetX((CurMouseState.X - Game1.Width / 2f) / 1000f);
                PlayerMovement.SetY((CurMouseState.Y - Game1.Height / 2f) / 1000f);
                Mouse.SetPosition(Game1.Width / 2, Game1.Height / 2);
            }
        }
    }
}
