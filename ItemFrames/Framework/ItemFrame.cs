
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
    public class ItemFrame : StardewValley.Objects.Furniture
    {
        public const int OBJECT = 1;
        public const int HAT = 2;
        public const int BIG_OBJECT = 3;
        public const int RING = 4;
        public const int FURNITURE = 5;

        [XmlElement("displayItem")]
        public readonly NetRef<Item> displayItem = new NetRef<Item>();

        [XmlElement("displayType")]
        public readonly NetInt displayType = new NetInt();

        private IMonitor monitor;

        public ItemFrame(int which, Vector2 tile, IMonitor monitor) : base(which, tile)
        {
            this.monitor = monitor;
        }

        protected override void initNetFields()
        {
            base.initNetFields();
            base.NetFields.AddFields(this.displayItem, this.displayType);
        }


        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            this.monitor.Log($"drawF {x},{y}");
            base.draw(spriteBatch, x, y, alpha);
            if (this.displayItem.Value != null)
            {
                if (x == -1)
                {
                    this.updateDrawPosition();
                    this.drawItem(this.displayItem, this.drawPosition.X, this.drawPosition.Y, spriteBatch);

                }
                else
                {
                    this.drawItem(this.displayItem, x*Game1.tileSize, y*Game1.tileSize, spriteBatch);
                }
            }
            /*
            Rectangle rectangle;
            if (x == -1) {
                this.updateDrawPosition();
                Texture2D texture = Furniture.furnitureTexture;
                Vector2 position = Game1.GlobalToLocal(Game1.viewport, this.drawPosition);
                Rectangle? sourceRectangle = this.sourceRect;
                Color color = Color.White * alpha;
                Vector2 zero = Vector2.Zero;
                int effects = ((bool)base.flipped) ? 1 : 0;
                float layerDepth;
                rectangle = base.boundingBox.Value;
                layerDepth = (float)(rectangle.Bottom - 8) / 10000f;
                spriteBatch.Draw(texture, position, sourceRectangle, color, 0f, zero, 4f, (SpriteEffects)effects, layerDepth);
                this.drawItem(this.displayItem, position.X, position.Y, spriteBatch);
            }
            else
            {
                Texture2D texture2 = Furniture.furnitureTexture;
                Vector2 position2 = Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * 64), (float)(y * 64 - (this.sourceRect.Height * 4 - base.boundingBox.Height))));
                Rectangle? sourceRectangle2 = this.sourceRect;
                Color color2 = Color.White * alpha;
                Vector2 zero2 = Vector2.Zero;
                int effects2 = ((bool)base.flipped) ? 1 : 0;
                float layerDepth2;
                rectangle = base.boundingBox.Value;
                layerDepth2 = (float)(rectangle.Bottom - 8) / 10000f;
                spriteBatch.Draw(texture2, position2, sourceRectangle2, color2, 0f, zero2, 4f, (SpriteEffects)effects2, layerDepth2);
                //this.drawItem(this.displayItem, position2.X, position2.Y, spriteBatch);
                this.drawItem(this.displayItem, x*64, y*64, spriteBatch);
            }
            if (base.heldObject.Value != null)
            {
                if (base.heldObject.Value is Furniture)
                {
                    (base.heldObject.Value as Furniture).drawAtNonTileSpot(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(base.boundingBox.Center.X - 32), (float)(base.boundingBox.Center.Y - (base.heldObject.Value as Furniture).sourceRect.Height * 4 - (((bool)this.drawHeldObjectLow) ? (-16) : 16)))), (float)(base.boundingBox.Bottom - 7) / 10000f, alpha);
                }
                else
                {
                    Texture2D shadowTexture = Game1.shadowTexture;
                    Vector2 position3 = Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(base.boundingBox.Center.X - 32), (float)(base.boundingBox.Center.Y - (((bool)this.drawHeldObjectLow) ? 32 : 85)))) + new Vector2(32f, 53f);
                    Rectangle? sourceRectangle3 = Game1.shadowTexture.Bounds;
                    Color color3 = Color.White * alpha;
                    rectangle = Game1.shadowTexture.Bounds;
                    float x2 = (float)rectangle.Center.X;
                    rectangle = Game1.shadowTexture.Bounds;
                    spriteBatch.Draw(shadowTexture, position3, sourceRectangle3, color3, 0f, new Vector2(x2, (float)rectangle.Center.Y), 4f, SpriteEffects.None, (float)base.boundingBox.Bottom / 10000f);
                    spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(base.boundingBox.Center.X - 32), (float)(base.boundingBox.Center.Y - (((bool)this.drawHeldObjectLow) ? 32 : 85)))), GameLocation.getSourceRectForObject(base.heldObject.Value.ParentSheetIndex), Color.White * alpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, (float)(base.boundingBox.Bottom + 1) / 10000f);
                }
            }
            /*if (this.displayItem.Value != null)
            {
                this.drawItem(this.displayItem, x, y, spriteBatch);
                //this.displayItem.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * 64) + 2f, (float)(y * 64 - 64 + 21 + 8 - 1))), 0.75f, 0.45f, Math.Max(0f, (float)((y + 1) * 64 - 24) / 10000f) + (float)x * 1E-05f + 1E-05f, false, Color.Black, false);
                //this.displayItem.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * 64) + 2f, (float)(y * 64 - 64 + 21 + 4 - 1))), 0.75f, 1f, 0f, false, Color.White, false);
            }*/
        }

        public void drawItem(NetRef<Item> i, float x, float y, SpriteBatch spriteBatch)
        {
            if (i.Value != null)
            {
                y += 64;
                this.monitor.Log($"draw [ {i.Get().Name}, {x}, {y} ]");
                switch (this.displayType.Value)
                {
                    case 1:
                        i.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2(x + 2f, (float)(y - 64 + 21 + 8 - 1))), 0.75f, 0.45f, Math.Max(0f, (float)(y - 24 + 64) / 10000f) + (float)x * 1E-05f + 1E-05f, false, Color.Black, false);
                        i.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2(x + 2f, (float)(y - 64 + 21 + 4 - 1))), 0.75f, 1f, Math.Max(0f, (float)(y + 64 - 24) / 10000f) + (float)x * 1E-05f + 2E-05f, false, Color.White, false);
                        break;
                    case 3:
                        i.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2(x, (float)(y - 64 + 21 + 4 - 1))), 0.75f, 1f, Math.Max(0f, (float)((y + 1) * 64 - 24) / 10000f) + (float)x * 1E-05f + 1E-05f, false, Color.White, false);
                        break;
                    case 2:
                        i.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2(x + 6f, (float)(y - 64 + 21 + 16 - 1))), 0.75f, 0.45f, Math.Max(0f, (float)(y + 64 - 24) / 10000f) + (float)x * 1E-05f + 1E-05f, false, Color.Black, false);
                        i.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2(x + 6f, (float)(y - 64 + 21 + 12 - 1))), 0.75f, 1f, Math.Max(0f, (float)(y + 64 - 24) / 10000f) + (float)x * 1E-05f + 2E-05f, false, Color.White, false);
                        break;
                    case 4:
                        i.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2(x - 1f, (float)(y - 64 + 21 + 8 - 1))), 0.75f, 0.45f, Math.Max(0f, (float)(y + 64 - 24) / 10000f) + (float)x * 1E-05f + 1E-05f, false, Color.Black, false);
                        i.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2(x - 1f, (float)(y - 64 + 21 + 4 - 1))), 0.75f, 1f, Math.Max(0f, (float)(y +  64 - 24) / 10000f) + (float)x * 1E-05f + 2E-05f, false, Color.White, false);
                        break;
                    case 5:
                        i.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2(x, (float)(y  - 64 + 21 + 8 - 1))), 0.75f, 0.45f, Math.Max(0f, (float)(y + 64 - 24) / 10000f) + (float)x * 1E-05f + 1E-05f, false, Color.Black, false);
                        i.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2(x, (float)(y  - 64 + 21 + 4 - 1))), 0.75f, 1f, Math.Max(0f, (float)(y + 64 - 24) / 10000f) + (float)x * 1E-05f + 2E-05f, false, Color.White, false);
                        break;
                }
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
                return true;
            }
            else
            {
                this.monitor.Log($"Click w/o item detected. Passing to base;");
                return base.clicked(who);
            }
        }



        /*
        public const int OBJECT = 1;
        public const int HAT = 2;
        public const int BIG_OBJECT = 3;
        public const int RING = 4;
        public const int FURNITURE = 5;

        [XmlElement("displayItem")]
        public readonly NetRef<Item> displayItem = new NetRef<Item>();

        [XmlElement("displayType")]
        public readonly NetInt displayType = new NetInt();

        protected override void initNetFields()
        {
            base.initNetFields();
            base.NetFields.AddFields(this.displayItem, this.displayType);
        }

        public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
        {
            if (justCheckingForActivity)
            {
                return who.CurrentItem != null;
            }
            Item currentItem = who.CurrentItem;
            if (currentItem != null)
            {
                this.displayItem.Value = currentItem.getOne();
                Game1.playSound("coin");
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
                return true;
            }
            return false;
        }
        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            base.draw(spriteBatch, x, y, alpha);
            if (this.displayItem.Value != null)
            {
                switch (this.displayType.Value)
                {
                    case 1:
                        this.displayItem.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * 64) + 2f, (float)(y * 64 - 64 + 21 + 8 - 1))), 0.75f, 0.45f, Math.Max(0f, (float)((y + 1) * 64 - 24) / 10000f) + (float)x * 1E-05f + 1E-05f, false, Color.Black, false);
                        this.displayItem.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * 64) + 2f, (float)(y * 64 - 64 + 21 + 4 - 1))), 0.75f, 1f, Math.Max(0f, (float)((y + 1) * 64 - 24) / 10000f) + (float)x * 1E-05f + 2E-05f, false, Color.White, false);
                        break;
                    case 3:
                        this.displayItem.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * 64), (float)(y * 64 - 64 + 21 + 4 - 1))), 0.75f, 1f, Math.Max(0f, (float)((y + 1) * 64 - 24) / 10000f) + (float)x * 1E-05f + 1E-05f, false, Color.White, false);
                        break;
                    case 2:
                        this.displayItem.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * 64) + 6f, (float)(y * 64 - 64 + 21 + 16 - 1))), 0.75f, 0.45f, Math.Max(0f, (float)((y + 1) * 64 - 24) / 10000f) + (float)x * 1E-05f + 1E-05f, false, Color.Black, false);
                        this.displayItem.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * 64) + 6f, (float)(y * 64 - 64 + 21 + 12 - 1))), 0.75f, 1f, Math.Max(0f, (float)((y + 1) * 64 - 24) / 10000f) + (float)x * 1E-05f + 2E-05f, false, Color.White, false);
                        break;
                    case 4:
                        this.displayItem.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * 64) - 1f, (float)(y * 64 - 64 + 21 + 8 - 1))), 0.75f, 0.45f, Math.Max(0f, (float)((y + 1) * 64 - 24) / 10000f) + (float)x * 1E-05f + 1E-05f, false, Color.Black, false);
                        this.displayItem.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * 64) - 1f, (float)(y * 64 - 64 + 21 + 4 - 1))), 0.75f, 1f, Math.Max(0f, (float)((y + 1) * 64 - 24) / 10000f) + (float)x * 1E-05f + 2E-05f, false, Color.White, false);
                        break;
                    case 5:
                        this.displayItem.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * 64), (float)(y * 64 - 64 + 21 + 8 - 1))), 0.75f, 0.45f, Math.Max(0f, (float)((y + 1) * 64 - 24) / 10000f) + (float)x * 1E-05f + 1E-05f, false, Color.Black, false);
                        this.displayItem.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * 64), (float)(y * 64 - 64 + 21 + 4 - 1))), 0.75f, 1f, Math.Max(0f, (float)((y + 1) * 64 - 24) / 10000f) + (float)x * 1E-05f + 2E-05f, false, Color.White, false);
                        break;
                }
            }
        } */

    }
}
