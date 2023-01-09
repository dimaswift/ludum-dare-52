using ECF.Domain;
using UnityEngine;
using UnityEngine.Serialization;

namespace ECF.Views
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "ECF/GameSettings")]
    public class GameSettings : ScriptableObject
    {
        public int gardenBedCount = 28;
        public float bedUnlockPriceMultiplier = 1.3f;
        public int baseUnlockPrice = 10;
        public int unlockedBedsAmount = 6;
        public int startCoins = 100; 
        public int waterCapacity = 10;
        public int maxWaterLevel = 10;
        public int seedPrice = 5;
        public int wateringCanWaterAmount = 5;

        public SimulationConfig GetSimulationConfig()
        {
            return new SimulationConfig()
            {
                GardenBedCount = gardenBedCount,
                BedUnlockPriceMultiplier = bedUnlockPriceMultiplier,
                BaseUnlockPrice = baseUnlockPrice,
                UnlockedBedsAmount = unlockedBedsAmount,
                StartCoins = startCoins,
                GardenBedWaterCapacity = waterCapacity,
                MaxWaterLevel = maxWaterLevel,
                SeedPrice = seedPrice,
                WateringCanWaterAmount = wateringCanWaterAmount
            };
        }
    }
}