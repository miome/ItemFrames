
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
    public class ItemFrame:  StardewValley.Objects.Furniture
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
        }

    }
}
