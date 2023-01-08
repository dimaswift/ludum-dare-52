using System;
using System.Collections;
using System.Collections.Generic;
using ECF.Behaviours.Behaviours;
using ECF.Domain;
using ECF.Domain.Common;
using UnityEngine;

namespace ECF.Views
{
    public class GardenBedView : MonoBehaviour, IToolTarget, IToolUseResult
    {
        public GardenBedBehaviour Behaviour => behaviour;
        public int ShapesCount => formStates.Length;
        
        
        [SerializeField] private GameObject selectedIndicator;
        [SerializeField] private GameObject hoverIndicator;
        [SerializeField] private GameObject lockedState;
        [SerializeField] private GameObject[] formStates;
        [SerializeField] private Transform[] fills;
        [SerializeField] private Transform fullEffect;
        [SerializeField] private AudioClip fullSound;

        public float ToolHeight
        {
            get
            {
                if (currentCrop != null)
                {
                    return currentCrop.height;
                }

                return 0;
            }
        }

        public Vector3 Position => transform.position;
        private AudioSource audioSource;
        
        private readonly HashSet<ToolType> supportedTools = new ()
        {
            ToolType.Hoe,
            ToolType.WateringCan,
        };

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
        
        public void SetFormState(int amount)
        {
            amount = Mathf.Clamp(amount, 0, formStates.Length - 1);
            for (int i = 0; i < formStates.Length; i++)
            {
                formStates[i].SetActive(amount == i);
            }
        }

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
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

            
            
            if (Mathf.Approximately(prevProgress, progress))
            {
                return;
            }
            
            prevProgress = progress;

            // if (Mathf.Approximately(progress, 1))
            // {
            //     StartCoroutine(PlayFullEffect());
            // }

            // Color color = Color.Lerp(Color.red, Color.green, progress);
            // color.a = 1f - Mathf.Clamp((Time.time - lastToolUseTime) / 2, 0, 1);
            // fillMaterial.SetColor(ColorProp, color);
            // fillMaterial.SetColor(EmissionColorProp, color);
        }

        IEnumerator PlayFullEffect()
        {
            audioSource.PlayOneShot(fullSound);
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * 3;
                fullEffect.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 2.0f, t);
                finishMaterial.SetColor(ColorProp, Color.Lerp(Color.green, new Color(0,1,0,0), t));
                finishMaterial.SetColor(EmissionColorProp, Color.Lerp(Color.green, new Color(0, 1, 0, 0), t));
                yield return null;
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
            behaviour.Water(2);
        }

        private void PlaceCrop(Crop crop)
        {
            var config = Game.Instance.ViewController.CropConfigs[crop.Id];
            var cropObject = Instantiate(config.prefab.gameObject, transform.position, Quaternion.identity)
                .GetComponent<CropView>();
            cropObject.transform.SetParent(transform);
            cropObject.SetUp(crop, behaviour.Phase);
            currentCrop = cropObject;
        }

        private void CollectCrop()
        {
            if (currentCrop != null)
            {
                Destroy(currentCrop.gameObject);
                currentCrop = null;
            }
        }

        public IToolUseResult UseTool(Tool tool)
        {
            switch (tool.type)
            {
                case ToolType.Hoe:
                    if (behaviour.ShapeLevel.Value < formStates.Length)
                    {
                        Plow();
                        tool.SetEffectMaterial(soilMaterial);
                        return this;
                    }
                    return null;
                case ToolType.WateringCan:
                    if (behaviour.WaterLevel.Value < 10)
                    {
                        Water();
                        return this;
                    }

                    return null;
                case ToolType.SeedBag:
                    var bag = tool as SeedBag;
                    var template = Game.Instance.Simulation.CropTemplateFactory.Get(bag.templateId);
                    if (behaviour.Plant(template, out var crop, out _))
                    {
                        PlaceCrop(crop);
                        return this;
                    }

                    return null;
                case ToolType.Shovel:
                    if (behaviour.TryHarvest(out var harvest))
                    {
                        CollectCrop();
                        return this;
                    }

                    return null;
            }

            return null;
        }


       
    }
}
