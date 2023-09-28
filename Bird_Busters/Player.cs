using System;
using System.Data;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bird_Busters
{
    //Does most of the stuff the player does. So:
    //Movement, has camera, has position, has bounding-box, has powerUps, has life.
    public class Player
    {
        //Camera
        public static Vector3 CamTarget;
        public static Vector3 CamPosition;
        public static Matrix ProjectionMatrix; //3D into 2D
        public static Matrix ViewMatrix; //Location and Orientation of virtual Camera?
        //Camera end

        public Player()
        {
            
        }

        public static void Update()
        {
            ViewMatrix = FPSCamera.View;
            CamTarget = FPSCamera.direction;
            CamPosition = FPSCamera.position;
            ProjectionMatrix = FPSCamera.Projection;
        }

        // Camera related methods
        public static Vector3 GetCamPosition() => Player.CamPosition;
        public static Vector3 GetCamTarget() => Player.CamTarget;
        public static void SetCamTarget(Vector3 target) => Player.CamTarget = target;
        public static void SetCamPosition(Vector3 pos) => Player.CamPosition = pos;
        // Camera related methods end

    }
}
  