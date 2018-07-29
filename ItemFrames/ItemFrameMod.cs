using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley.Objects;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley;
using ItemFrames.Framework;
using System.Linq;

namespace ItemFrames
{
    public class ItemFrameMod : Mod, IAssetEditor
    {
        private ModConfig Config;
        public static ItemFrameMod instance;
        public static List<ItemFrameData> itemFrameDatum;
        public int nextID = 2048;
        public override void Entry(IModHelper helper)
        {
            instance = this;
            SaveEvents.AfterLoad += this.SaveEvents_AfterLoad;
            SaveEvents.BeforeSave += this.SaveEvents_BeforeSave;
            SaveEvents.AfterSave += this.SaveEvents_AfterSave;
            MenuEvents.MenuChanged += this.MenuEvents_MenuChanged;
            this.Config = this.Helper.ReadConfig<ModConfig>();
            ItemFrameMod.itemFrameDatum = this.Helper.ReadJsonFile<List<ItemFrameData>>("data/frames.json");
            foreach(ItemFrameData ifd in itemFrameDatum){
                ifd.texture=this.Helper.Content.Load<Texture2D>(ifd.textureFile, ContentSource.ModFolder);
                ifd.id = nextID;
                nextID += (int)(ifd.texture.Width / 16);
                //TODO: Wrapping with items with height=32
            }
        }
        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            this.RestoreItemFrames();
        }
        private void SaveEvents_BeforeSave(object sender, EventArgs eventArgs){
            this.ConvertItemFrames();
        }
        private void SaveEvents_AfterSave(object sender, EventArgs eventArgs){
            this.RestoreItemFrames();
        }
        private void ConvertItemFrames()
        {
            foreach (GameLocation location in ItemFrameMod.GetLocations())
            {
                foreach (StardewValley.Object o in location.objects.Values)
                {
                    if(o is Chest chest){
                        for (int i = 0; i < chest.items.Count; i++)
                        {
                            if (chest.items[i] is ItemFrame frame)
                            {
                                chest.items[i] = frame.asFurniture();
                            }
                        }
                    }
                }
                if (location is StardewValley.Locations.DecoratableLocation decoLoc)
                {
                    for (int i = 0; i < decoLoc.furniture.Count; i++)
                    {
                        if (decoLoc.furniture[i] is ItemFrame frame)
                        {
                            decoLoc.furniture[i] = frame.asFurniture();
                        }
                    }
                }
            }
            for (int i = 0; i < Game1.player.items.Count; i++){
                if(Game1.player.items[i] is ItemFrame frame){
                    Game1.player.items[i] = frame.asFurniture();
                }
            } 
        }

        private void RestoreItemFrames(){
            foreach (GameLocation location in ItemFrameMod.GetLocations())
            {
                foreach (StardewValley.Object o in location.objects.Values){
                    if (o is Chest chest){
                        for (int i = 0; i<chest.items.Count; i++){
                            if(chest.items[i] is Furniture furniture2 && furniture2.Name=="ItemFrame"){
                                chest.items[i] = new ItemFrame(furniture2, this.Monitor);
                            }
                        }
                    }

                }
                if (location is StardewValley.Locations.DecoratableLocation decoLoc)
                {
                    for (int i = 0; i < decoLoc.furniture.Count; i++)
                    {
                        if(decoLoc.furniture[i].Name == "ItemFrame"){
                            decoLoc.furniture[i] = new ItemFrame(decoLoc.furniture[i],this.Monitor);
                        }
                    }
                }
            }
            for (int i = 0; i < Game1.player.items.Count; i++){
                if (Game1.player.items[i] is Furniture furniture && furniture.Name=="ItemFrame"){
                    Game1.player.items[i] = new ItemFrame(furniture, this.Monitor);
                }
            }
        }

        /// <summary>Get all game locations.</summary>
        public static IEnumerable<GameLocation> GetLocations()
        {
            return Game1.locations.Concat(
                    from location in Game1.locations.OfType<BuildableGameLocation>()
                    from building in location.buildings
                    where building.indoors.Value != null
                    select building.indoors.Value
                );
        }
        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Data/Furniture") || asset.AssetNameEquals("TileSheets/furniture");
        }
        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("Data/Furniture"))
            {
                foreach(ItemFrameData ifd in ItemFrameMod.itemFrameDatum){
                    string lw = $"{(int)(ifd.texture.Width / 16)} {(int)(ifd.texture.Height / 16)}";
                    string bb = $"{(int)(ifd.texture.Width / 16)} {Math.Min(2, (int)(ifd.texture.Height / 16))}";
                    this.Monitor.Log($"Adding '{ifd.displayName}'/painting/{lw}/{lw}/1/100", LogLevel.Trace);
                    asset.AsDictionary<int, string>().Data.Add(ifd.id, $"'{ifd.displayName}'/painting/{lw}/{bb}/1/100");
                }
            } else if (asset.AssetNameEquals("TileSheets/furniture")){
                var oldTex = asset.AsImage().Data;
                Texture2D newTex = new Texture2D(Game1.graphics.GraphicsDevice, oldTex.Width, Math.Max(oldTex.Height, 4096));
                asset.ReplaceWith(newTex);
                asset.AsImage().PatchImage(oldTex);
                foreach (ItemFrameData ifd in ItemFrameMod.itemFrameDatum){
                    asset.AsImage().PatchImage(ifd.texture, targetArea: furnitureRect(ifd.id, ifd.texture));
                }
            }
        }
        private Rectangle furnitureRect(int index, Texture2D texture)
        {
            return new Rectangle((index % 32) * 16, (int)(index / 32) * 16, texture.Width, texture.Height);
        }
        public static ItemFrameData IFDataByName(string displayName){
            ItemFrameMod.instance.Monitor.Log($"Looking up ItemFrameData for {displayName}", LogLevel.Trace);
            foreach (ItemFrameData itemFrameData in ItemFrameMod.itemFrameDatum){
                if($"'{itemFrameData.displayName}'" == displayName){
                    return itemFrameData;
                }
            }
            return null;
        }

        private void MenuEvents_MenuChanged(object sender, EventArgsClickableMenuChanged e)
        {
            if (Game1.activeClickableMenu is ShopMenu)
            {
                ShopMenu shop = (ShopMenu)Game1.activeClickableMenu;
                Dictionary<Item, int[]> items = Helper.Reflection.GetField<Dictionary<Item, int[]>>(shop, "itemPriceAndStock").GetValue();
                List<Item> selling = Helper.Reflection.GetField<List<Item>>(shop, "forSale").GetValue();
                int currency = Helper.Reflection.GetField<int>(shop, "currency").GetValue();
                bool isCatalogue = (currency == 0 && selling.Count > 0 && selling[0] is Furniture);
                if (isCatalogue || (shop.portraitPerson != null && shop.portraitPerson is NPC shopk && shopk.name == "Robin"))
                {
                    Dictionary<Item, int> newItemsToSell = new Dictionary<Item, int>();
                    foreach (ItemFrameData f in ItemFrameMod.itemFrameDatum)
                    {
                        Item item = (Item)new ItemFrame(f, new Vector2(0, 0), ItemFrameMod.instance.Monitor); 
                        items.Add(item, new int[] { isCatalogue ? 0 : f.price, int.MaxValue });
                        selling.Add(item);
                    }
                }
            }
        }
    }
}
