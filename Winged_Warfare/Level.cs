using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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
        private List<LevelObject> _levelObjects = new();

        //debug-mode-variables
        private static bool _debugMode = true;
        private static string DebugText = "Standard-Debug-Text";
        private static int _debugTool = 0; //0 = move, 1 = rotate, 2 = scale
        private int _selectedObject = 0;
        private KeyboardState _previousKeyboardState;

        public Player Player1;

        public Level()
        {
            Player1 = new Player();
            _previousKeyboardState = Keyboard.GetState();
        }

        public void UpdateObjects()
        {
            //Update all objects in list.
            foreach (LevelObject lvlObject in _levelObjects)
            {
                lvlObject.Update();
            }

            if (IsPressed(Keys.N))
            {
                Debug.WriteLine("Debug mode toggled");
                _debugMode = !_debugMode;
            }

            if (_debugMode)
            {
                DebugMode();
            }

            _previousKeyboardState = Keyboard.GetState();
        }

        public void DrawModels()
        {
            //Go through list of drawable objects in list and draw them.
            foreach (LevelObject lvlObject in _levelObjects)
            {
                lvlObject.Draw();
            }
        }
        public void LoadLevel(string levelPath)
        {
            //Read content of specified level file.
            //add objects to list.
            string[] lines;
            try
            {
                lines = File.ReadAllLines(levelPath);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            for (int i = 0; i < lines.Length; i++)
            {
                string[] attributes = lines[i].Split(",");
                int objectType = identifyObjectType(attributes[0]);
                switch (objectType)
                {
                    case 0:
                        //Create drawable object and add to list.
                        _levelObjects.Add(new DrawableObject(parseVector3(attributes, 1), parseVector3(attributes, 4), parseVector3(attributes, 7), attributes[10], i));
                        Debug.WriteLine("Created drawable object in line: " + i);
                        break;
                    case 1:
                        //Create spawnpoint and add to list.
                        break;
                    case 2:
                        //Create light and add to list.
                        break;
                    case 3:
                        //Create player origin and add to list.
                        break;
                    default:
                        Debug.WriteLine("Error in level file. Line: " + i);
                        break;

                }
            }
        }


        private int identifyObjectType(string objectType)
        {
            //Identify object type and return int.
            //-1 = error
            //0  = drawable object
            //1  = spawnpoint
            //2  = light
            //3  = player origin
            switch (objectType)
            {
                case "model":
                    return 0;
                case "spawn":
                    return 1;
                case "light":
                    return 2;
                case "origin":
                    return 3;
                default:
                    return -1;
            }
        }

        private Vector3 parseVector3(string[] vectorAttributes, int startIndex)
        {
            //Parse string to Vector3.
            CultureInfo culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            culture.NumberFormat.NumberDecimalSeparator = ".";
            return new Vector3(float.Parse(vectorAttributes[startIndex], NumberStyles.Any, culture), float.Parse(vectorAttributes[startIndex + 1], NumberStyles.Any, culture), float.Parse(vectorAttributes[startIndex + 2], NumberStyles.Any, culture));
        }

        private void DebugMode()
        {
            //Debug mode.
            if (IsPressed(Keys.Left))
            {
                Debug.WriteLine("Selected Object changed [left].");
                _selectedObject = (_selectedObject - 1) % _levelObjects.Count;
            }

            if (IsPressed(Keys.Right))
            {
                Debug.WriteLine("Selected Object changed [right].");
                _selectedObject = (_selectedObject + 1) % _levelObjects.Count;
            }

            if (IsPressed(Keys.M))
            {
                Debug.WriteLine("DebugTool changed.");
                _debugTool = (_debugTool + 1) % 3;
            }



            UpdateDebugText();
        }
        private void UpdateDebugText()
        {
            //Update debug text.
            String debugText = "";
            switch (_debugTool)
            {
                case 0:
                    debugText = "Move";
                    break;
                case 1:
                    debugText = "Rotate";
                    break;
                case 2:
                    debugText = "Scale";
                    break;
                default:
                    debugText = "Error";
                    break;
            }

            DebugText = "Selected Object: " + _selectedObject + "\n" + "Tool: " + debugText;
        }

        private bool IsPressed(Keys k)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyUp(k) && _previousKeyboardState.IsKeyDown(k))
            {
                return true;
            }
            return false;
        }

        public static string getDebugText()
        {
            return DebugText;
        }
        public static bool getDebugMode()
        {
            return _debugMode;
        }
    }
}
