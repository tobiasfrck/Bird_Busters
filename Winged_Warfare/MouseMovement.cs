﻿using Microsoft.Xna.Framework;
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
        public static Vector2 position = new Vector2(0, 0);
        public static MouseState mouse;


        //setzt die Mousposition in die mitte des Bildes bevor das Spiel beginnt;
        public static void Init()
        {
            Mouse.SetPosition(Game1.Width/2, Game1.Height/2);
        }

        public static void Update()
        { 
            //Graphik Einstellungen automatisch Hinzufügen
            mouse = Mouse.GetState();
            PlayerMovement.SetX((float)(mouse.X- Game1.Width / 2)/1000);
            PlayerMovement.SetY((float)(mouse.Y- Game1.Height / 2)/1000);


            if (!Keyboard.GetState().IsKeyDown(Keys.L))
                Mouse.SetPosition(Game1.Width / 2, Game1.Height /2);
        }


    }


}
