using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        public DrawableObject(Vector3 position, Vector3 rotation, Vector3 scale, int line) : base(position,rotation, scale, line)
        {
            
        }

        public DrawableObject(Vector3 position, Vector3 rotation, Vector3 scale, String model, int line) : base(position, rotation,
            scale, line)
        {
            if (!Game1.Models.TryGetValue(model, out _model))
            {
                Debug.WriteLine("Model not found");
            }
        }

         
        public override void Draw()
        {
            //Debug.WriteLine("DrawModel");
            UpdateWorldMatrix();
            if (Model != null)
                _model.Draw(WorldMatrix, Player.ViewMatrix, Player.ProjectionMatrix);
        }

        public Model Model
        {
            get => _model;
            set => _model = value;
        }
    }
}
