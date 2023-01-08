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
        }

        public void Place(Crop crop, ICropStorage cropStorage)
        {
            this.cropStorage = cropStorage;
            Crop = crop;
            var prefab = Game.Instance.ViewController.CropConfigs[crop.Id].prefab;
            var instance = Instantiate(prefab.gameObject, transform).GetComponent<CropView>();
            instance.transform.SetPositionAndRotation(cropPoint.position, cropPoint.rotation);
            phase = new(crop.Phase);
            instance.SetUp(crop, phase);
            cropView = instance;
        }

        public void Empty()
        {
            if (Crop == null)
            {
                return;
            }
            Destroy(cropView.gameObject);
            phase = null;
            cropView = null;
            Crop = null;
        }

        public bool CanUseTool(Tool tool)
        {
            if (tool.type != ToolType.Hand && tool.type != ToolType.SeedBag)
            {
                return false;
            }
            
            if (Crop == null)
            {
                return false;
            }
            
            return true;
        }

        public void OnHoverBegan(Tool tool)
        {
            hoverIndicator.SetActive(true);
        }

        public void OnHoverEnded()
        {
            hoverIndicator.SetActive(false);
        }

        public void UseTool(Tool tool)
        {
            if (cropView == null)
            {
                return;
            }

            if (tool.type == ToolType.Hand)
            {
                if (cropStorage.Sell(Crop, out var err))
                {
                    Empty();
                }
                return;
            }

            if (tool.type == ToolType.SeedBag)
            {
                if (cropStorage.ConvertToSeeds(Crop, out var err))
                {
                    Empty();
                }
                else
                {
                    Debug.LogError(err);
                }
            }
        }
    }
}