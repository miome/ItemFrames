using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Objects;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using System.Xml.Serialization;
namespace ItemFrames.Framework
{
    public class ItemFrameFactory
    {
        public List<ItemFrame> itemFrames;
        public ItemFrameFactory()
        {
            this.itemFrames = new List<ItemFrame>();
        }
        public ItemFrame newItemFrame(int which, Vector2 tile, IMonitor monitor){
            ItemFrame itemFrame = new ItemFrame(which, tile, monitor);
            this.itemFrames.Add(itemFrame);
            return itemFrame;
        }
        public ItemFrame deleteItemFrame()

    }
}
