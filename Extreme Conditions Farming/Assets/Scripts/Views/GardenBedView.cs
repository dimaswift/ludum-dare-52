using System;
using ECF.Behaviours;
using ECF.Behaviours.Behaviours;
using ECF.Behaviours.Systems;
using ECF.Domain;
using ECF.Domain.Common;
using TMPro;
using UnityEngine;

namespace ECF.Views
{
    public class GardenBedView : MonoBehaviour, IToolTarget, IToolUseResult
    {
        public Transform HoldPoint
        {
            get
            {
                if (currentCrop != null)
                {
                    return currentCrop.HoldPoint;
                }

                return transform;
            }
        }
        public GardenBedBehaviour Behaviour => behaviour;
        public int ShapesCount => formStates.Length;
        
        [SerializeField] private TextMeshPro unlockPriceText;
        [SerializeField] private GameObject selectedIndicator;
        [SerializeField] private GameObject hoverIndicator;
        [SerializeField] private GameObject lockedState;
        [SerializeField] private GameObject[] formStates;
        [SerializeField] private Transform[] fills;
        [SerializeField] private Transform fullEffect;

        public float ToolHeight
        {
            get
            {
                if (currentCrop != null)
                {
                    return 2;
                }

                return 0;
            }
        }

        public Vector3 Position => transform.position;

        private GardenBedBehaviour behaviour;

        private readonly ObservableValue<bool> selected = new(false);
        private Material fillMaterial;
        private Material finishMaterial;
        private Material soilMaterial;
        private float prevProgress;
        private static readonly int ColorProp = Shader.PropertyToID("_BaseColor");
        private static readonly int EmissionColorProp = Shader.PropertyToID("_EmissionColor");

        private float currentWaterLevelNormalized;
        private float targetWaterLevelNormalized;
        private CropView currentCrop;
        
        private void SetFormState(int amount)
        {
            amount = Mathf.Clamp(amount, 0, formStates.Length - 1);
            for (int i = 0; i < formStates.Length; i++)
            {
                formStates[i].SetActive(behaviour.Status.Value != BedStatus.Locked && amount == i);
            }
        }

        private void Awake()
        {
            selected.Changed += s => selectedIndicator.SetActive(s);
            selectedIndicator.SetActive(false);
            hoverIndicator.SetActive(false);
            fillMaterial = fullEffect.GetComponentInChildren<MeshRenderer>().material;
            soilMaterial = Instantiate(formStates[0].GetComponentInChildren<MeshRenderer>().sharedMaterial);
            foreach (GameObject state in formStates)
            {
                state.GetComponentInChildren<MeshRenderer>().sharedMaterial = soilMaterial;
            }
            foreach (Transform fill in fills)
            {
                fill.GetComponentInChildren<MeshRenderer>().sharedMaterial = fillMaterial;
                fill.transform.localScale = Vector3.zero;
            }
            finishMaterial = Instantiate(fillMaterial);
            foreach (MeshRenderer meshRenderer in fullEffect.GetComponentsInChildren<MeshRenderer>())
            {
                meshRenderer.sharedMaterial = finishMaterial;
            }
            fillMaterial.SetColor(ColorProp, Color.clear);
            fillMaterial.SetColor(EmissionColorProp, Color.clear);
            finishMaterial.SetColor(ColorProp, Color.clear);
            finishMaterial.SetColor(EmissionColorProp, Color.clear);
        }

        public void Init(GardenBedBehaviour behaviour)
        {
            this.behaviour = behaviour;
            hoverIndicator.SetActive(false);
            SetFormState(behaviour.Data.ShapeLevel);
            behaviour.ShapeLevel.Changed += SetFormState;
            behaviour.Status.Changed += StatusOnChanged;
            this.behaviour.WaterLevel.Changed += WaterLevelOnChanged;
            StatusOnChanged(behaviour.Status.Value);
            WaterLevelOnChanged(this.behaviour.WaterLevel.Value);
            unlockPriceText.text = behaviour.Data.UnlockPrice.ToString();
            currentWaterLevelNormalized = targetWaterLevelNormalized;
            if (this.behaviour.Data.Crop != null)
            {
                PlaceCrop(this.behaviour.Data.Crop);
            }
        }

        private void WaterLevelOnChanged(int water)
        {
            UpdateSoil();
        }

        private void StatusOnChanged(BedStatus status)
        {
            lockedState.SetActive(behaviour.Status.Value == BedStatus.Locked);
            SetFormState(behaviour.Data.ShapeLevel);
        }

        private void UpdateSoil()
        {
            targetWaterLevelNormalized = (float) behaviour.WaterLevel.Value / behaviour.MaxWaterLevel;
        }
        
        private void Update()
        {
            currentWaterLevelNormalized = Mathf.Lerp(currentWaterLevelNormalized, targetWaterLevelNormalized, Time.deltaTime * 2);
            soilMaterial.SetColor(ColorProp, Color.Lerp(Color.white, Color.gray, currentWaterLevelNormalized));
            
            float progress = 0;
           
            if (Game.Instance.Tools.Current != null)
            {
                switch (Game.Instance.Tools.Current.type)
                {
                    case ToolType.Hoe:
                        progress = behaviour.ShapeLevel.Value / ((float)ShapesCount - 1);
                        
                        break;
                    case ToolType.WateringCan:
                        progress = (float) behaviour.WaterLevel.Value / behaviour.MaxWaterLevel;
   
                        break;
                }

                foreach (Transform fill in fills)
                {
                    fill.transform.localScale =
                        Vector3.Lerp(fill.transform.localScale, new Vector3(1, 1, progress), Time.deltaTime * 15);
                }
            }
        }
        

        [Serializable]
        public class FormState
        {
            public int threshold;
            public GameObject model;
        }
        
        public void OnHoverBegan(Tool tool)
        {
            hoverIndicator.SetActive(true);
        }

        public void OnHoverEnded()
        {
            hoverIndicator.SetActive(false);
        }

        private void Plow()
        {
            behaviour.ImproveShape();
        }

        private void Water()
        {
            behaviour.Water(Game.Instance.Settings.wateringCanWaterAmount);
        }

        private void PlaceCrop(Crop crop)
        {
            var config = Game.Instance.CropConfigs[crop.Id];
            var cropObject = Instantiate(config.prefab.gameObject, transform.position, Quaternion.identity)
                .GetComponent<CropView>();
            cropObject.transform.SetParent(transform);
            cropObject.SetUp(crop);
            cropObject.Subscribe(behaviour);
            currentCrop = cropObject;
        }
        

        private IToolUseResult UseHand(Hand hand)
        {
            if (behaviour.Status.Value == BedStatus.Locked)
            {
                if (behaviour.Unlock())
                {
                    AudioSource.PlayClipAtPoint(Game.Instance.Sounds.coins.Random(), transform.position);
                }

                return null;
            }

            if (hand.PickedUpResult is CropView holdingCrop)
            {
                if (holdingCrop.Crop.Phase.CanPlant())
                {
                    behaviour.PlaceCrop(holdingCrop.Crop);
                    PlaceCrop(holdingCrop.Crop);
                    hand.Release();
                    return null;
                }

                return null;
            }

            if (behaviour.TryHarvest(out var collected))
            {
                var c = currentCrop;
                c.SetUp(collected);
                currentCrop = null;
                return c;
            }

            return null;
        }

        public IToolUseResult UseTool(Tool tool)
        {
            if (tool is Hand hand)
            {
                return UseHand(hand);
            }

            if (tool is Hoe hoe)
            {
                return UseHoe(hoe);
            }
            
            if (tool is WateringCan wateringCan)
            {
                return UseWateringCan(wateringCan);
            }
            
            if (tool is SeedBag seed)
            {
                return UseSeedBag(seed);
            }
            
            return null;
        }

        private IToolUseResult UseSeedBag(SeedBag bag)
        {
            var template = Game.Instance.Simulation.CropTemplateFactory.Get(bag.templateId);
            if (behaviour.Plant(template, out var crop, out _))
            {
                PlaceCrop(crop);
                return this;
            }
            return null;
        }

        private IToolUseResult UseWateringCan(WateringCan wateringCan)
        {
            if (behaviour.WaterLevel.Value < behaviour.MaxWaterLevel)
            {
                Water();
                return this;
            }

            return null;
        }

        private IToolUseResult UseHoe(Hoe hoe)
        {
            if (behaviour.ShapeLevel.Value < formStates.Length)
            {
                Plow();
                hoe.SetEffectMaterial(soilMaterial);
                return this;
            }

            return null;
        }
    }
}
