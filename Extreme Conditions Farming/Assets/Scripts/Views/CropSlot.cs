using System;
using ECF.Behaviours.Systems;
using ECF.Domain;
using ECF.Domain.Common;
using UnityEngine;

namespace ECF.Views
{
    public class CropSlot : MonoBehaviour, IToolTarget
    {
        [SerializeField] private GameObject hoverIndicator;
        public float ToolHeight => 1;
        public Vector3 Position => transform.position;

        [SerializeField] private Transform cropPoint;
        public Crop Crop { get; private set; }

        private ObservableValue<CropPhase> phase = new(CropPhase.Overripe);

        private CropView cropView;
        private ICropStorage cropStorage;

        private void Start()
        {
            hoverIndicator.SetActive(false);
            Game.Instance.OnNewSimulationCreated += OnNewSimulationCreated;
        }

        private void OnNewSimulationCreated()
        {
            cropStorage = Game.Instance.Simulation.GetSystem<ICropStorage>();
            var crop = cropStorage.GetCropWithSlotNumber(transform.GetSiblingIndex());
            if (crop != null)
            {
                Place(crop);
            }
        }

        public void Place(Crop crop)
        {
            Crop = crop;
            cropStorage.Add(crop, transform.GetSiblingIndex());
            var prefab = Game.Instance.CropConfigs[crop.Id].prefab;
            var instance = Instantiate(prefab.gameObject, transform).GetComponent<CropView>();
            instance.transform.SetPositionAndRotation(cropPoint.position, cropPoint.rotation);
            instance.SetUp(crop);
            cropView = instance;
        }

        public void Empty()
        {
            if (Crop == null)
            {
                return;
            }
            cropStorage.Remove(Crop.SlotNumber, out _);
            phase = null;
            cropView = null;
            Crop = null;
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
                if (Crop != null)
                {
                    var crop = cropView;
                    Empty();
                    return crop;
                }
                if (hand.PickedUpResult != null)
                {
                    var crop = hand.PickedUpResult as CropView;
                    if (crop != null)
                    {
                        Place(crop.Crop);
                        hand.Release();
                    }
                }

                return null;
            }

            if (tool.type == ToolType.SeedBag)
            {
                if (cropStorage.ConvertToSeeds(Crop, out var err))
                {
                    var crop = cropView;
                    Empty();
                    return crop;
                }
            }

            return null;
        }
    }
}