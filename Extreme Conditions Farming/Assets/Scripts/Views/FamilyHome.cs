using ECF.Behaviours;
using ECF.Behaviours.Systems;
using TMPro;
using UnityEngine;

namespace ECF.Views
{
    public class FamilyHome : CropDeposit
    {
        [SerializeField] private TextMeshPro membersText;
        [SerializeField] private Transform hungerBar;

        private IFamilySystem familySystem;
        private Material hungerBarMaterial;
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
        

        protected override void Start()
        {
            base.Start();
            Game.Instance.OnNewSimulationCreated += OnNewSimulationCreated;
            hungerBarMaterial = hungerBar.GetComponentInChildren<Renderer>().material;
        }

        private void OnNewSimulationCreated()
        {
            familySystem = Game.Instance.Simulation.GetSystem<IFamilySystem>();
            familySystem.MembersAmount.Changed += OnMembersAmountChanged;
            OnMembersAmountChanged(familySystem.MembersAmount.Value);
            familySystem.Hunger.Changed += OnHungerChanged;
            OnHungerChanged(familySystem.Hunger.Value);
        }

        private void OnHungerChanged(int amount)
        {
            var n = Mathf.Clamp01(amount / (float)familySystem.DeadlyHungerLevel);
            hungerBarMaterial.SetColor(BaseColor, Color.Lerp(Color.green, Color.red, n));
            hungerBarMaterial.SetColor(EmissionColor, Color.Lerp(Color.green, Color.red, n));
            hungerBar.localScale = new Vector3(n, 1, 1);
        }

        private void OnMembersAmountChanged(int amount)
        {
            membersText.text = $"MEMBERS {amount}";
        }
        protected override bool TryDeposit(CropView cropView, out int result)
        {
            var storage = Game.Instance.Simulation.GetSystem<ICropStorage>();
            return storage.FeedFamily(cropView.Crop, out result);
        }
        
        
    }
}