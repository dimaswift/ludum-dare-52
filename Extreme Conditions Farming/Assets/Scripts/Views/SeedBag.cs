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
        [SerializeField] private TextMeshPro seedCountText;

        private CropTemplate template;
        private IObservableValue<int> amount;

        public override void Init(ISimulation simulation)
        {
            base.Init(simulation);
            template = simulation.CropTemplateFactory.Get(templateId);
            amount = simulation.Inventory.Get(template.SeedId);
            amount.Changed += UpdateSeedCount;
            UpdateSeedCount(amount.Value);
        }

        protected override bool CanActivate()
        {
            return amount.Value > 0;
        }

        private void UpdateSeedCount(int amount)
        {
            seedCountText.text = amount.ToString();
        }
    }
}