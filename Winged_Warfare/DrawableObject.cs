﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Winged_Warfare
{
    internal class DrawableObject : LevelObject
    {
        private Model _model;
        private readonly string _modelName;

        public DrawableObject(Vector3 position, Vector3 rotation, Vector3 scale, int line) : base(position,rotation, scale, line)
        {
            
        }

        public DrawableObject(Vector3 position, Vector3 rotation, Vector3 scale, String model) : base(position, rotation,
            scale)
        {
            if (!Game1.Models.TryGetValue(model, out _model))
            {
                Debug.WriteLine("Model not found");
            }
            _modelName = model;
        }

        public DrawableObject(Vector3 position, Vector3 rotation, Vector3 scale, String model, int line) : base(position, rotation,
            scale, line)
        {
            if (!Game1.Models.TryGetValue(model, out _model))
            {
                Debug.WriteLine("Model not found");
            }
            _modelName = model;
        }

         
        public override void Draw()
        {
            //Debug.WriteLine("DrawModel");
            UpdateWorldMatrix();
            if (Model != null)
            {
                int count = _model.Bones.Count;
                Matrix[] transforms = new Matrix[_model.Bones.Count];
                _model.CopyAbsoluteBoneTransformsTo(transforms);
                foreach (ModelMesh mesh in _model.Meshes)
                {
                    
                    for (var i = 0; false &&i < mesh.Effects.Count; i++)
                    {
                        var effect = mesh.Effects[i];
                        if (!(effect is IEffectMatrices effectMatrices))
                            throw new InvalidOperationException();
                        effectMatrices.World = transforms[mesh.ParentBone.Index] * WorldMatrix;
                        effectMatrices.View = Player.ViewMatrix;
                        effectMatrices.Projection = Player.ProjectionMatrix;
                        BasicEffect basicEffect = effect as BasicEffect;
                        basicEffect?.EnableDefaultLighting();
                        if (basicEffect != null)
                        {
                            basicEffect.Alpha = 1f;
                        }
                    }

                    foreach (var meshPart in mesh.MeshParts)
                    {
                        meshPart.Effect = Game1.AmbientEffect;

                        meshPart.Effect.Parameters["WorldMatrix"].SetValue(transforms[mesh.ParentBone.Index] * WorldMatrix);
                        meshPart.Effect.Parameters["ViewMatrix"].SetValue(Player.ViewMatrix);
                        meshPart.Effect.Parameters["ProjectionMatrix"].SetValue(Player.ProjectionMatrix);
                        meshPart.Effect.Parameters["AmbienceColor"].SetValue(Color.Red.ToVector4());
                        
                    }
                    mesh.Draw();
                }
            }
        }

        //Generates a new line for the level file, based on the current position, rotation and scale.
        public override string RegenerateLine()
        {
            NumberFormatInfo nfi = new NumberFormatInfo
            {
                NumberDecimalSeparator = "."
            };
            string line = "model," + Position.X.ToString(nfi) + "," + Position.Y.ToString(nfi) + "," + Position.Z.ToString(nfi) + "," + Rotation.X.ToString(nfi) + "," + Rotation.Y.ToString(nfi) + "," + Rotation.Z.ToString(nfi) + "," + Scale.X.ToString(nfi) + "," + Scale.Y.ToString(nfi) + "," + Scale.Z.ToString(nfi) + "," + _modelName;
            return line;
        }

        public Model Model
        {
            get => _model;
            set => _model = value;
        }

    }
}
