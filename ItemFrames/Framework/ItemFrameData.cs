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
        [JsonIgnore]
        public int id;

        public string textureFile { get; set; }
        public string displayName { get; set; }
        public string description { get; set; }
        public Vector2 displayLocation { get; set; }
        public int price { get; set; } = 100;
        
    }
}
