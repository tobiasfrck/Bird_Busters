using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class TextureItem
{
    public Texture2D Texture { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }

    public TextureItem(Texture2D texture, Vector2 position, Vector2 size)
    {
        Texture = texture;
        Position = position;
        Size = size;
    }
}
