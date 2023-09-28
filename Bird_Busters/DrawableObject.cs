using System;
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

namespace Bird_Busters
{
    internal class DrawableObject : LevelObject
    {
        private Model _model;
        private readonly string _modelName;

        public DrawableObject(Vector3 position, Vector3 rotation, Vector3 scale, int line) : base(position, rotation, scale, line)
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
                Vector3 DiffuseColor = new Vector3(0.5f, 0.5f, 0.5f);
                if (ModelName.Contains("Birb"))
                {
                    DiffuseColor = new Vector3(0.7f, 0.7f, 0.7f);
                }
                int count = _model.Bones.Count;
                Matrix[] transforms = new Matrix[_model.Bones.Count];
                _model.CopyAbsoluteBoneTransformsTo(transforms);
                foreach (ModelMesh mesh in _model.Meshes)
                {
                    foreach (Effect effect in mesh.Effects)
                    {
                        if (!(effect is IEffectMatrices effectMatrices))
                            throw new InvalidOperationException();
                        effectMatrices.World = transforms[mesh.ParentBone.Index] * WorldMatrix;
                        effectMatrices.View = Player.ViewMatrix;
                        effectMatrices.Projection = Player.ProjectionMatrix;
                        BasicEffect basicEffect = effect as BasicEffect;
                        basicEffect.LightingEnabled = true;
                        basicEffect.DirectionalLight0.Direction = new Vector3(0, -0.5f, -1f);
                        basicEffect.DirectionalLight0.DiffuseColor = DiffuseColor;
                        basicEffect.DirectionalLight0.SpecularColor = new Vector3(0.5f, 0.5f, 0.5f);
                        basicEffect.AmbientLightColor = new Vector3(0.42f, 0.4f, 0.45f);

                        if (basicEffect != null)
                        {
                            basicEffect.Alpha = 1f;
                        }
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

        public string ModelName
        {
            get => _modelName;
        }

        public Model Model
        {
            get => _model;
            set => _model = value;
        }

    }
}
