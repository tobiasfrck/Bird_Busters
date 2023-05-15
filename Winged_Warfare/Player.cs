using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Winged_Warfare
{
    //Does most of the stuff the player does. So:
    //Movement, has camera, has position, has bounding-box, has powerUps, has life.
    public class Player : Game
    {
        //Camera
        public static Vector3 CamTarget;
        public static Vector3 CamPosition;
        public static Matrix ProjectionMatrix; //3D into 2D
        public static Matrix ViewMatrix; //Location and Orientation of virtual Camera?
        public static Matrix WorldMatrix; //position in Space of objects
        //Camera end

        public Player()
        {
            
        }

        public static void Update()
        {
            ViewMatrix = Matrix.CreateLookAt(CamPosition, CamTarget, Vector3.Up);
        }

        // Camera related methods
        public static Vector3 GetCamPosition() => Player.CamPosition;
        public static Vector3 GetCamTarget() => Player.CamTarget;
        public static void SetCamTarget(Vector3 target) => Player.CamTarget = target;
        public static void SetCamPosition(Vector3 pos) => Player.CamPosition = pos;
        // Camera related methods end
    }
}
