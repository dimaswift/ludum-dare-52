using UnityEngine;
using UnityEngine.Serialization;

namespace ECF.Views
{
    [CreateAssetMenu(menuName = "ECF/CropConfig")]
    public class CropConfig : ScriptableObject
    {
        public CropView prefab;
        public string displayName;
        public int growthDuration = 10;
        public int waterConsumption = 10;
        public int seedConversionRate = 3;
        public int sellPrice = 100;
        
    }
}