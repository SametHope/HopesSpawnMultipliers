using Microsoft.Xna.Framework;
using System;
using System.ComponentModel;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace HopesSpawnMultipliers
{
    public class HopesSpawnMultipliers : Mod
    {
        HopesSpawnMultipliers()
        {
            // I am not sure if this is actually relevant/beneficial but still...
            ContentAutoloadingEnabled = true;
        }
    }

    public class GlobalNPCRateModifier : GlobalNPC
    {
        // Allow instance fields
        public override bool InstancePerEntity => true;

        // For relative spawn rates and limits we need to store npcs' original values
        private int _originalSpawnRate = _UNSET;
        private int _originalMaxSpawns = _UNSET;

        // Const to use for unset values so we can store the original values and not override them by accident
        private const int _UNSET = -1;

        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            // Store original values if they are not already stored
            if (_originalSpawnRate == _UNSET) _originalSpawnRate = spawnRate;
            if (_originalMaxSpawns == _UNSET) _originalMaxSpawns = maxSpawns;

            // Notice we are diving the og value to get smaller values
            // This is because for some reason spawnRate works backwards and lower values mean there will be more spawns
            spawnRate = _originalSpawnRate / HopesSpawnMultipliersConfig.Instance.SpawnRateFactor ;

            // This line is self explanatory
            maxSpawns = _originalMaxSpawns * HopesSpawnMultipliersConfig.Instance.LimitRateFactor;
        }
    }

    public class HopesSpawnMultipliersConfig : ModConfig
    {
        // A static instance so we don't ModContent.GetInstance<>() each time we need it
        public static HopesSpawnMultipliersConfig Instance { get; private set; }

        // Actually set the instance, aka 'cache' it
        public override void OnLoaded() => Instance = this;

        // I don't trust setting this on load for some reason so I am going to override it here lol
        public override void OnChanged() => Instance = this;

        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Increment(1)]
        [Range(1, 1000)]
        [DefaultValue(1)]
        public int SpawnRateFactor { get; set; } = 1;

        [Increment(1)]
        [Range(1, 1000)]
        [DefaultValue(1)]
        public int LimitRateFactor { get; set; } = 1;
    }
}