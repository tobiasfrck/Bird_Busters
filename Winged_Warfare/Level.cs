using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace Winged_Warfare
{
    //Does everything concerning the current run and level
    //Loads level, spawns animals, has timer, checks end conditions, ends game.
    public class Level
    {
        //Has drawable object list.
        //Has spawnpoint list.
        //Has lights list.

        public Player player1;
        public Level()
        {
            player1 = new Player();
        }

        public void DrawModels()
        {
            //Go through list of drawable objects in list and draw them.
        }
        public void LoadLevel()
        {
            //Execute ReadFile.
            //Execute CreateObjects.
            //Maybe some more stuff like placing spawnpoints and placing the player origin.
        }
        public void ReadFile(string levelPath)
        {
            //Read content of specified level file.
            //Return 2D string array with objects and their properties.
        }
        public void CreateObjects()
        {
            //Take 2D string array and create each object and sort them into lists.
        }
    }
}
