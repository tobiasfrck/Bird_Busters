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
    internal class Level
    {
        //Has drawable object list.
        //Has spawnpoint list.
        //Has lights list.
        private readonly List<LevelObject> _levelObjects = new();
        private readonly List<PathPoint> _startPoints = new();
        private readonly List<PathPoint> _tempPathPoints = new();
        private readonly List<BirdSpawnpoint> _spawnpoints = new();

        //debug-mode-variables
        private static bool _debugMode = true;
        private static string _debugText = "Standard-Debug-Text";
        private static int _debugTool = 0; //0 = move, 1 = rotate, 2 = scale
        private static int _debugToolResolution = 0; //0 = 0.1, 1 = 1, 2 = 10
        private const float DebugToolMinResolution = 0.01f;
        private int _selectedObject = 0; // represents the index of the selected object in the list
        private KeyboardState _previousKeyboardState;
        private string[] _levelContent;
        private string _levelPath = "";

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
            _levelContent = Array.Empty<string>();
            _levelPath = levelPath;
            try
            {
                _levelContent = File.ReadAllLines(levelPath);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            for (int i = 0; i < _levelContent.Length; i++)
            {
                LevelObject toAdd = LineToLevelObject(_levelContent[i], i);
                if (toAdd != null)
                {
                    _levelObjects.Add(toAdd);
                }
                else
                {
                    Debug.WriteLine("Error in level file. Line: " + i + " content: " + _levelContent[i]);
                }
            }

            //When loading is done, create graph for bird paths.
            CreateGraphForPaths();


            Debug.WriteLine("Level loaded with " + _levelObjects.Count + " objects.");
        }

        private LevelObject LineToLevelObject(string input, int lineNum)
        {
            string[] attributes = input.Split(",");
            int objectType = IdentifyObjectType(attributes[0]);
            LevelObject levelObject = null;
            switch (objectType)
            {
                case 0:
                    //Create drawable object and add to list.
                    levelObject = new DrawableObject(ParseVector3(attributes, 1), ParseVector3(attributes, 4),
                        ParseVector3(attributes, 7), attributes[10], lineNum);
                    Debug.WriteLine("Created drawable object in line: " + lineNum);
                    break;
                case 1:
                    //Create pathpoint and add to list.
                    PathPoint p = new PathPoint(ParseInt(attributes[1]),
                        new Vector2(ParseFloat(attributes[2]), ParseFloat(attributes[3])), lineNum);
                    for (int i = 4; i < attributes.Length; i++)
                    {
                        p.AddNextPointID(ParseInt(attributes[i]));
                    }
                    _tempPathPoints.Add(p);
                    Debug.WriteLine("Created pathpoint in line: " + lineNum);
                    levelObject = p;
                    break;
                case 2:
                    //Create BirdSpawnpoint and add to list.
                    //levelObject = new BirdSpawnpoint(ParseInt(sattributes[1]), ParseVector3(attributes, 2), lineNum);
                    BirdSpawnpoint s = new BirdSpawnpoint(ParseInt(attributes[1]), ParseFloat(attributes[2]), ParseFloat(attributes[3]), lineNum);
                    _spawnpoints.Add(s);
                    Debug.WriteLine("Created spawnpoint in line: " + lineNum);
                    levelObject = s;
                    break;
                case 3:
                    //Create light and add to list.
                    break;
                default:
                    Debug.WriteLine("Error in level file. Line: " + lineNum);
                    break;
            }
            return levelObject;
        }

        private int IdentifyObjectType(string objectType)
        {
            //Identify object type and return int.
            //-1 = error
            //0  = drawable object
            //1  = pathpoint
            //2  = spawnpoint
            //3  = light
            switch (objectType)
            {
                case "model":
                    return 0;
                case "pathpoint":
                    return 1;
                case "spawnpoint":
                    return 2;
                case "light":
                    return 3;
                default:
                    return -1;
            }
        }

        private static Vector3 ParseVector3(string[] vectorAttributes, int startIndex)
        {
            //Parse string to Vector3.
            CultureInfo culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            culture.NumberFormat.NumberDecimalSeparator = ".";
            return new Vector3(float.Parse(vectorAttributes[startIndex], NumberStyles.Any, culture), float.Parse(vectorAttributes[startIndex + 1], NumberStyles.Any, culture), float.Parse(vectorAttributes[startIndex + 2], NumberStyles.Any, culture));
        }

        private static int ParseInt(string intAttribute)
        {
            //Parse string to int.
            return int.Parse(intAttribute);
        }

        private static float ParseFloat(string floatAttribute)
        {
            //Parse string to float.
            CultureInfo culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            culture.NumberFormat.NumberDecimalSeparator = ".";
            return float.Parse(floatAttribute, NumberStyles.Any, culture);
        }

        //This is awful. :/
        private void CreateGraphForPaths()
        {
            Debug.WriteLine("Creating graph for paths.");
            foreach (PathPoint pathPoint in _tempPathPoints)
            {

                //Check if pathpoint is a spawnpoint. pathpoint is a spawnpoint if it has the same ID as the spawnpoint.
                BirdSpawnpoint sp = _spawnpoints.Find(X => X.GetSpawnpointID() == pathPoint.GetPointID());
                if (sp != null)
                {
                    Debug.WriteLine("BirdSpawnpoint found: " + sp.GetSpawnpointID());
                    pathPoint.SetSpawnpoint(sp);
                    sp?.SetPathPoint(pathPoint);
                    _startPoints.Add(pathPoint);
                }
                Debug.WriteLine("Pathpoint: " + pathPoint.GetPointID());

                //TODO: Change to findall.
                //add next points to pathpoint.
                foreach (int i in pathPoint.GetNextPointsID())
                {
                    Debug.WriteLine(_tempPathPoints.Find(X => X.GetPointID() == i).RegenerateLine());
                    pathPoint.AddNextPoint(_tempPathPoints.Find(X => X.GetPointID() == i));
                }
            }
        }

        [Conditional("DEBUG")]
        private void DebugMode()
        {
            //Debug mode.
            //Switch the selected object.
            if (IsPressed(Keys.Left))
            {
                Debug.WriteLine("Selected Object changed [left].");
                _selectedObject = Mod((_selectedObject - 1), _levelObjects.Count);
            }

            //Switch the selected object.
            if (IsPressed(Keys.Right))
            {
                Debug.WriteLine("Selected Object changed [right].");
                _selectedObject = Mod((_selectedObject + 1), _levelObjects.Count);
            }

            //Switch the debug tool.
            if (IsPressed(Keys.M))
            {
                Debug.WriteLine("DebugTool changed.");
                _debugTool = (_debugTool + 1) % 3;
            }

            //Lower the debug tool resolution.
            if (IsPressed(Keys.R))
            {
                Debug.WriteLine("DebugToolResolution changed.");
                _debugToolResolution = Mod(_debugToolResolution + 1, 4);
            }

            //Up the debug tool resolution.
            if (IsPressed(Keys.F))
            {
                Debug.WriteLine("DebugToolResolution changed.");
                _debugToolResolution = Mod(_debugToolResolution - 1, 4);
            }

            UpdateDebugText();

            if (_selectedObject > _levelObjects.Count)
            {
                return;
            }

            //-X
            if (IsPressed(Keys.J))
            {
                ModifyObjectTranslationRotationScale(-1, 0);
            }

            //+X
            if (IsPressed(Keys.U))
            {
                ModifyObjectTranslationRotationScale(1, 0);
            }

            //-Y
            if (IsPressed(Keys.K))
            {
                ModifyObjectTranslationRotationScale(-1, 1);
            }

            //+Y
            if (IsPressed(Keys.I))
            {
                ModifyObjectTranslationRotationScale(1, 1);
            }

            //-Z
            if (IsPressed(Keys.L))
            {
                ModifyObjectTranslationRotationScale(-1, 2);
            }

            //+Z
            if (IsPressed(Keys.O))
            {
                ModifyObjectTranslationRotationScale(1, 2);
            }

            // save changes to selected object to level file
            if (IsPressed(Keys.RightControl))
            {
                _levelContent[_selectedObject] = _levelObjects[_selectedObject].RegenerateLine();
                SaveLevel();
            }

            // reloads selected object with original values
            if (IsPressed(Keys.OemMinus))
            {
                LevelObject toReplace = LineToLevelObject(_levelContent[_selectedObject], _selectedObject);
                if (toReplace != null) { _levelObjects[_selectedObject] = toReplace; }
            }


        }

        //direction: -1 = decrease, 1 = increase
        //axis: 0 = X, 1 = Y, 2 = Z
        private void ModifyObjectTranslationRotationScale(int direction, int axis)
        {
            float change = (float)Math.Round(direction * DebugToolMinResolution * Math.Pow(10, _debugToolResolution), 3);

            Vector3 changeVector;

            switch (axis)
            {
                case 0:
                    changeVector = new Vector3(change, 0, 0);
                    break;
                case 1:
                    changeVector = new Vector3(0, change, 0);
                    break;
                case 2:
                    changeVector = new Vector3(0, 0, change);
                    break;
                default:
                    Debug.WriteLine("Error in axis.");
                    return;
            }

            switch (_debugTool)
            {
                case 0:
                    _levelObjects[_selectedObject].Move(changeVector);
                    break;
                case 1:
                    _levelObjects[_selectedObject].Rotate(changeVector);
                    break;
                case 2:
                    _levelObjects[_selectedObject].ScaleObject(changeVector);
                    break;
                default:
                    Debug.WriteLine("Error in debug tool.");
                    break;
            }
        }


        private void UpdateDebugText()
        {
            //Update debug text.
            string debugText = _debugTool switch
            {
                0 => "Move",
                1 => "Rotate",
                2 => "Scale",
                _ => "Error"
            };
            _debugText = "Selected Object: " + _selectedObject + "\n" +
                         "Tool: " + debugText + "\n" +
                         "Resolution: " + (float)Math.Round(DebugToolMinResolution * Math.Pow(10, _debugToolResolution), 3) + "\n" +
                         "Save: Right-CTRL, Recover: -";
        }

        private void SaveLevel()
        {
            //Save level.
            Debug.WriteLine("Saving level.");

            string levelContent = "";
            //Currently not a good solution, because it will delete non working level-objects.
            /*
            foreach (LevelObject levelObject in _levelObjects)
            {
                levelContent += levelObject.RegenerateLine() + "\n";
            }
            */
            foreach (string s in _levelContent)
            {
                levelContent += s + "\n";
            }

            File.WriteAllText(_levelPath, levelContent);
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

        public static string GetDebugText()
        {
            return _debugText;
        }

        public static bool GetDebugMode()
        {
            return _debugMode;
        }

        public List<PathPoint> GetStartPoints()
        {
            return _startPoints;
        }

        private static int Mod(int x, int m)
        {
            return (x % m + m) % m;
        }
    }
}
