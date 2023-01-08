using ECF.Behaviours;
using ECF.Domain;
using ECF.Domain.Common;
using TMPro;
using UnityEngine;

namespace ECF.Views
{
    public class SeedBag : Tool
    {
        public string templateId;
        
        [SerializeField] private int maxSeeds = 10;
        [SerializeField] private TextMeshPro seedCountText;
        [SerializeField] private Transform seedsModel;
        [SerializeField] private Transform topSeedPosition;
        [SerializeField] private Transform bottomSeedPosition;
        
        private CropTemplate template;
        public IObservableValue<int> Amount { get; private set; }

        public override void Init(ISimulation simulation)
        {
            base.Init(simulation);
            template = simulation.CropTemplateFactory.Get(templateId);
            Amount = simulation.Inventory.Get(template.SeedId);
            Amount.Changed += UpdateSeedCount;
            UpdateSeedCount(Amount.Value);
        }

        public override bool CanActivate(IToolTarget target)
        {
            if (target is GardenBedView bed)
            {
                return Amount.Value > 0 
                       && bed.Behaviour.Status.Value == BedStatus.Empty 
                       && bed.Behaviour.ShapeLevel.Value >= bed.ShapesCount - 1;
            }

            return false;
        }

        private void SetSeedPosition(int seedCount)
        {
            var posNorm = Mathf.Clamp01((float) seedCount / maxSeeds);
            seedsModel.localPosition = Vector3.Lerp(bottomSeedPosition.localPosition, topSeedPosition.localPosition, posNorm);
        }
        
        private void UpdateSeedCount(int amount)
        {
            seedCountText.text = amount.ToString();
            SetSeedPosition(amount);
        }
    }
}