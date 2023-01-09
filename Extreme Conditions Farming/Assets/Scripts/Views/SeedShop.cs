using ECF.Domain;
using TMPro;
using UnityEngine;

namespace ECF.Views
{
    public class SeedShop : MonoBehaviour, IToolTarget
    {
        [SerializeField] private string cropId;
        public float ToolHeight => height;
        public Vector3 Position => dropPoint.position;

        [SerializeField] private TextMeshPro seedPrice;
        [SerializeField] private Transform dropPoint;
        [SerializeField] private AudioClip[] buySound;
        [SerializeField] private float height = 2;
        [SerializeField] private ParticleSystem buyEffect;
        [SerializeField] private GameObject hoverIndicator;

        private AudioSource source;

        protected virtual void Start()
        {
            source = GetComponent<AudioSource>();
            hoverIndicator.SetActive(false);
            seedPrice.text = Game.Instance.Settings.seedPrice.ToString();
        }

        public void OnHoverBegan(Tool tool)
        {
            hoverIndicator.SetActive(true);
        }

        public void OnHoverEnded()
        {
            hoverIndicator.SetActive(false);
        }
        
        public IToolUseResult UseTool(Tool tool)
        {
            if (tool is Hand hand)
            {
                if (hand.PickedUpResult != null)
                {
                    return null;
                }

                if (Game.Instance.Simulation.Inventory.Use(InventoryItems.Coins,
                        Game.Instance.Simulation.Config.SeedPrice))
                {
                    var template = Game.Instance.Simulation.CropTemplateFactory.Get(cropId);
                    Game.Instance.Simulation.Inventory.Add(template.SeedId, 1);
                    source.PlayOneShot(buySound.Random());
                    buyEffect.Emit(1);
                }
            }

            return null;
        }

    }
}