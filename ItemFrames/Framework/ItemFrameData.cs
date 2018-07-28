using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System.Collections.Generic;
namespace ItemFrames.Framework
{
    public class ItemFrameData
    {
        [JsonIgnore]
        public Texture2D texture;

        public string textureFile { get; set; }
        public string displayName { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public Vector2 displayLocation { get; set; }
        
    }
}
