using System;
using System.Collections.Generic;
using ECF.Domain;
using UnityEngine;
using UnityEngine.Serialization;

namespace ECF.Views
{
    [CreateAssetMenu(menuName = "ECF/CropConfig")]
    public class CropConfig : ScriptableObject
    {
        public CropView prefab;
        public string displayName = "Tomato";
        public Stat[] growthDuration;
        public Stat[] waterConsumption;
        public Stat[] seedConversionRate;
        public Stat[] sellPrice;
        public Stat[] nutrition;

        private static readonly Stat[] Linear =
        {
            new () { value = 10,  phase = CropPhase.Bud },
            new () { value = 10,  phase = CropPhase.Flower },
            new () { value = 10,  phase = CropPhase.Green },
            new () { value = 10,  phase = CropPhase.Rotten },
            new () { value = 10, phase = CropPhase.Overripe },
            new () { value = 10, phase = CropPhase.Unripe },
            new () { value = 10, phase = CropPhase.Sprout },
            new () { value = 10, phase = CropPhase.Seed },
            new () { value = 10, phase = CropPhase.Ripe },
        };


        private void OnEnable()
        {
            if (growthDuration.Length == 0)
            {
                growthDuration = Linear;
            }

            if (waterConsumption.Length == 0)
            {
                waterConsumption = Linear;
            }

            if (seedConversionRate.Length == 0)
            {
                seedConversionRate = Linear;
            }

            if (sellPrice.Length == 0)
            {
                sellPrice = Linear;
            }

            if (nutrition.Length == 0)
            {
                nutrition = Linear;
            }

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
        

        private static Dictionary<CropPhase, int> ToDictionary(Stat[] stats)
        {
            var dict = new Dictionary<CropPhase, int>();
            foreach (var stat in stats)
            {
                dict.Add(stat.phase, stat.value);
            }
            return dict;
        }

        public CropTemplate ToTemplate()
        {
            return new CropTemplate
            {
                Id = name,
                Name = displayName,
                PhaseStats = new PhaseStats()
                {
                    Durations = ToDictionary(growthDuration),
                    WaterConsumption = ToDictionary(waterConsumption),
                    SeedConversionRate = ToDictionary(seedConversionRate),
                    SellPrices = ToDictionary(sellPrice),
                    NutritionRate = ToDictionary(nutrition)
                }
            };
        }
        
        [Serializable]
        public class Stat
        {
            public CropPhase phase;
            public int value;
        }
    }
}