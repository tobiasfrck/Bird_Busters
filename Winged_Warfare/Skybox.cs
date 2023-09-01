using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Winged_Warfare
{
    internal class Skybox : LevelObject
    {
        private Model _model;
        private Effect _skyBoxEffect;
        private TextureCube _skyBoxTexture;
        private float _size = 150f;


        public Skybox(Vector3 position, Vector3 rotation, Vector3 scale) : base(position, rotation, scale)
        {
            _skyBoxEffect = Game1.Instance.Content.Load<Effect>("Shaders/skybox");
            _skyBoxTexture = Game1.Instance.Content.Load<TextureCube>("Shaders/skybox_cubemap");
            if (!Game1.Models.TryGetValue("Shaders/skyBoxCube", out _model))
            {
                Console.WriteLine("[SkyBox]: Model not found");
            }
        }

        public override void Draw()
        {
            UpdateWorldMatrix();
            if (Model != null)
            {
                foreach (var pass in _skyBoxEffect.CurrentTechnique.Passes)
                {
                    int count = _model.Bones.Count;
                    Matrix[] transforms = new Matrix[_model.Bones.Count];
                    _model.CopyAbsoluteBoneTransformsTo(transforms);
                    foreach (ModelMesh mesh in _model.Meshes)
                    {

                        foreach (var meshPart in mesh.MeshParts)
                        {
                            meshPart.Effect = _skyBoxEffect;

                            meshPart.Effect.Parameters["World"].SetValue(
                                Matrix.CreateScale(_size) * Matrix.CreateTranslation(Player.GetCamPosition()));
                            meshPart.Effect.Parameters["View"].SetValue(Matrix.CreateRotationY(MathHelper.ToRadians(-35))*Player.ViewMatrix);
                            meshPart.Effect.Parameters["Projection"].SetValue(Player.ProjectionMatrix);
                            meshPart.Effect.Parameters["SkyBoxTexture"].SetValue(_skyBoxTexture);
                            meshPart.Effect.Parameters["CameraPosition"].SetValue(Player.GetCamPosition());
                        }
                        mesh.Draw();
                    }
                }
            }
        }

        public override void Update()
        {
        }

        public Model Model
        {
            get => _model;
            set => _model = value;
        }
    }
}
