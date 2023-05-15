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
        public static Vector2 position = new Vector2(0, 0);
        public static MouseState mouse;
        public static int width = 800;
        public static int hight = 480;


        //setzt die Mousposition in die mitte des Bildes bevor das Spiel beginnt;
        public static void Init()
        {
            Mouse.SetPosition(width/2, hight/2);
        }

        public static void Update()
        { 
            //Graphik Einstellungen automatisch Hinzufügen
            mouse = Mouse.GetState();
            Player.setX((float)(mouse.X-width/2)/1000);
            Player.setY((float)(mouse.Y-hight/2)/1000);


            if (!Keyboard.GetState().IsKeyDown(Keys.L))
            Mouse.SetPosition(width/2, hight/2);
        }


    }


}
