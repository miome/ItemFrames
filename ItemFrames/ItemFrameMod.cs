using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley.Objects;
using StardewValley.Locations;
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
            //ItemFrame frame = new ItemFrame(1602, new Vector2(0,0), this.Monitor);
            //Game1.player.addItemToInventory(frame);
            //Game1.addHUDMessage(new HUDMessage($"New ItemFrame added to Inventory"));
            //this.Monitor.Log("New ItemFrame added to inventory", LogLevel.Trace);
            this.RestoreItemFrames();
            Game1.player.setInventory(new List<Item>());
            foreach(ItemFrameData ifd in ItemFrameMod.itemFrameDatum){
                if (!Game1.player.hasItemInInventoryNamed(ifd.displayName))
                {
                    Game1.player.addItemToInventory(new ItemFrame(ifd.id, new Vector2(0, 0), this.Monitor));
                }
            }
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
            //this.Monitor.Log($"{asset.AssetName}");
            return asset.AssetNameEquals("Data/Furniture") || asset.AssetNameEquals("TileSheets/furniture");
        }
        public void Edit<T>(IAssetData asset)
        {
            this.Monitor.Log($"{asset.AssetName}");
            if (asset.AssetNameEquals("Data/Furniture"))
            {
                //asset.AsDictionary<int, string>().Data.Add(2000, "'Wooden Frame'/painting/2 2/2 2/1/400");
                foreach(ItemFrameData ifd in ItemFrameMod.itemFrameDatum){
                    string lw = $"{(int)(ifd.texture.Width / 16)} {(int)(ifd.texture.Height / 16)}";
                    string bb = $"{(int)(ifd.texture.Width / 16)} {Math.Min(2, (int)(ifd.texture.Height / 16))}";
                    this.Monitor.Log($"'{ifd.displayName}'/painting/{lw}/{lw}/1/100", LogLevel.Trace);
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


    }
}
