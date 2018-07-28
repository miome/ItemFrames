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
        public static List<ItemFrameData> itemFrameData;
        public override void Entry(IModHelper helper)
        {
            instance = this;
            SaveEvents.AfterLoad += this.SaveEvents_AfterLoad;
            SaveEvents.BeforeSave += this.SaveEvents_BeforeSave;
            SaveEvents.AfterSave += this.SaveEvents_AfterSave;
            this.Config = this.Helper.ReadConfig<ModConfig>();
            ItemFrameMod.itemFrameData = new List<ItemFrameData>();
            ItemFrameData itemFrameData = new ItemFrameData();
            itemFrameData.displayName = "Wooden Frame";
            itemFrameData.height = 2;
            itemFrameData.width = 2;
            itemFrameData.textureFile = "data/Wooden_Frame.png";
            itemFrameData.displayLocation = new Vector2(8, 8);
            ItemFrameMod.itemFrameData.Add(itemFrameData);
            this.Helper.WriteJsonFile<List<ItemFrameData>>("data/frames.json", ItemFrameMod.itemFrameData);

        }
        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            ItemFrame frame = new ItemFrame(1602, new Vector2(0,0), this.Monitor);
            Game1.player.addItemToInventory(frame);
            Game1.addHUDMessage(new HUDMessage($"New ItemFrame added to Inventory"));
            this.Monitor.Log("New ItemFrame added to inventory", LogLevel.Trace);
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
            //this.Monitor.Log($"{asset.AssetName}");
            return asset.AssetNameEquals("Data/Furniture") || asset.AssetNameEquals("TileSheets/furniture");
        }
        public void Edit<T>(IAssetData asset)
        {
            this.Monitor.Log($"{asset.AssetName}");
            if (asset.AssetNameEquals("Data/Furniture"))
            {
                asset.AsDictionary<int, string>().Data.Add(2000, "'ItemFrame1'/painting/2 2/2 2/1/400");
            } else if (asset.AssetNameEquals("TileSheets/furniture")){
                var oldTex = asset.AsImage().Data;
                Texture2D newTex = new Texture2D(Game1.graphics.GraphicsDevice, oldTex.Width, Math.Max(oldTex.Height, 4096));
                asset.ReplaceWith(newTex);
                asset.AsImage().PatchImage(oldTex);
                Texture2D frameTexture = this.Helper.Content.Load<Texture2D>("data/Wooden_Frame.png", ContentSource.ModFolder);
                this.Monitor.Log($"{frameTexture.Height} {frameTexture.Width}");
                asset.AsImage().PatchImage(frameTexture, targetArea: furnitureRect(2000));
            }
        }
        private Rectangle furnitureRect(int index)
        {
            return new Rectangle((index % 32) * 16, (int)(index / 32) * 16, 32, 32);
        }
        public static ItemFrameData IFDataByName(string displayName){
            return null;
        }


    }
}
