﻿
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
    //[XmlRoot(Namespace = "StardewValley")]
    //[XmlInclude(typeof(ItemFrames.Framework.ItemFrame))]
    public class ItemFrame : StardewValley.Objects.Furniture
    {
        public const int OBJECT = 1;
        public const int HAT = 2;
        public const int BIG_OBJECT = 3;
        public const int RING = 4;
        public const int FURNITURE = 5;

        //[XmlElement("displayItem")]
        public readonly NetRef<Item> displayItem = new NetRef<Item>();

        //[XmlElement("displayType")]
        public readonly NetInt displayType = new NetInt();

        private IMonitor monitor;

        public ItemFrame(int which, Vector2 tile, IMonitor monitor, Item dispItem=null) : base(which, tile)
        {
            this.monitor = monitor;
            if (dispItem != null)
            {
                this.displayItem = new NetRef<Item>(dispItem);
                this.setDisplayType();
            }
        }
        public ItemFrame() : base() 
        {
            
        }


        protected override void initNetFields()
        {
            base.initNetFields();
            base.NetFields.AddFields(this.displayItem, this.displayType);
        }


        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            Rectangle rectangle = this.sourceRect.Value;
            //this.monitor.Log($"drawF {x},{y} {rectangle.Width}, {rectangle.Height}");
            base.draw(spriteBatch, x, y, alpha);
            if (this.displayItem.Value != null)
            {
                float xOffset = (64 * rectangle.Width / 16 - 64) / 2;
                float yOffset = (64 * rectangle.Height / 16 - 64) / 2;
                float layerDepth = 1f - 1E-05f;
                if (x == -1)
                {
                    this.updateDrawPosition();
                    float itemx = this.drawPosition.X + xOffset;
                    float itemy = this.drawPosition.Y + yOffset;
                    //layerDepth = itemy / 10000f;
                    this.drawItem(this.displayItem, itemx, itemy, spriteBatch, layerDepth);

                }
                else
                {
                    float itemx = x * Game1.tileSize + xOffset;
                    float itemy = y * Game1.tileSize + yOffset;
                    layerDepth = itemy / 10000f;
                    this.drawItem(this.displayItem, itemx, itemy, spriteBatch, layerDepth);
                }
            }
        }

        public void drawItem(NetRef<Item> i, float x, float y, SpriteBatch spriteBatch, float layerDepth)
        {
            if (i.Value != null)
            {
                //this.monitor.Log($"draw [ {i.Get().Name}, {x}, {y} LD: {layerDepth}]");
                switch (this.displayType.Value)
                {
                    case 1:
                        i.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2(x + 2f, (float)(y  + 8 - 1))), 0.75f, 0.45f, layerDepth, false, Color.Black, false);
                        i.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2(x + 2f, (float)(y  + 4 - 1))), 0.75f, 1f, layerDepth+1E-05f, false, Color.White, false);
                        break;
                    case 3:
                        i.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2(x, (float)(y  + 4 - 1))), 0.75f, 1f, layerDepth, false, Color.White, false);
                        break;
                    case 2:
                        i.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2(x + 6f, (float)(y  + 16 - 1))), 0.75f, 0.45f, layerDepth, false, Color.Black, false);
                        i.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2(x + 6f, (float)(y  + 12 - 1))), 0.75f, 1f, layerDepth + 1E-05f, false, Color.White, false);
                        break;
                    case 4:
                        i.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2(x - 1f, (float)(y  + 8 - 1))), 0.75f, 0.45f, layerDepth, false, Color.Black, false);
                        i.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2(x - 1f, (float)(y  + 4 - 1))), 0.75f, 1f, layerDepth + 1E-05f, false, Color.White, false);
                        break;
                    case 5:
                        i.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2(x, (float)(y  + 8 - 1))), 0.75f, 0.45f, layerDepth, false, Color.Black, false);
                        i.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2(x, (float)(y  + 4 - 1))), 0.75f, 1f, layerDepth + 1E-05f, false, Color.White, false);
                        break;
                }
            }
        }
        public void setDisplayType(){
            this.displayType.Value = 1;
            if (this.displayItem.Value is Hat)
            {
                this.displayType.Value = 2;
            }
            else if (this.displayItem.Value is Ring)
            {
                this.displayType.Value = 4;
            }
            else if (this.displayItem.Value is Furniture)
            {
                this.displayType.Value = 5;
            }
            else if (this.displayItem.Value is StardewValley.Object)
            {
                this.displayType.Value = ((!(bool)(this.displayItem.Value as StardewValley.Object).bigCraftable) ? 1 : 3);
            }
        }

        public override bool clicked(Farmer who)
        {
            //return this.checkForAction(who, false);
            if (who.CurrentItem != null)
            {
                this.monitor.Log($"Click w/item detected. Set displayItem to {who.CurrentItem.Name}");
                this.displayItem.Set(who.CurrentItem.getOne());
                Game1.playSound("coin");
                this.setDisplayType();
                return true;
            }
            else
            {
                this.monitor.Log($"Click w/o item detected. Passing to base;");
                return base.clicked(who);
            }
        }
    }
}
