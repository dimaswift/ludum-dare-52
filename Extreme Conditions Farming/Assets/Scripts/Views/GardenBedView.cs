using System;
using System.Collections;
using System.Collections.Generic;
using ECF.Behaviours.Behaviours;
using ECF.Domain;
using ECF.Domain.Common;
using UnityEngine;

namespace ECF.Views
{
    public class GardenBedView : MonoBehaviour, IToolTarget
    {
        [SerializeField] private GameObject selectedIndicator;
        [SerializeField] private GameObject hoverIndicator;
        [SerializeField] private GameObject lockedState;
        [SerializeField] private GameObject[] formStates;
        private AudioSource source;
        
        private readonly HashSet<ToolType> supportedTools = new ()
        {
            ToolType.Hoe,
            ToolType.WateringCan,
        };

        private GardenBedBehaviour behaviour;

        private readonly ObservableValue<bool> selected = new(false);
        
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
            source = GetComponent<AudioSource>();
            selected.Changed += s => selectedIndicator.SetActive(s);
            selectedIndicator.SetActive(false);
            hoverIndicator.SetActive(false);
        }

        public void Init(GardenBedBehaviour behaviour)
        {
            this.behaviour = behaviour;
            hoverIndicator.SetActive(false);
            SetFormState(behaviour.Data.ShapeLevel);
            behaviour.ShapeLevel.Changed += SetFormState;
            behaviour.Status.Changed += StatusOnChanged;
            StatusOnChanged(behaviour.Status.Value);
        }

        private void StatusOnChanged(BedStatus status)
        {
            lockedState.SetActive(behaviour.Status.Value == BedStatus.Locked);
        }


        [Serializable]
        public class FormState
        {
            public int threshold;
            public GameObject model;
        }

        public bool CanUseTool(Tool tool)
        {
            if (behaviour.Status.Value == BedStatus.Locked)
            {
                return false;
            }

            switch (tool.type)
            {
                case ToolType.Hoe:
                    return behaviour.Status.Value == BedStatus.Empty && behaviour.ShapeLevel.Value < formStates.Length - 1;
                case ToolType.WateringCan:
                    if (behaviour.WaterLevel.Value >= 10)
                        return false;
                    return behaviour.Status.Value == BedStatus.Planted || behaviour.Status.Value == BedStatus.Planted;
            }
            
            return supportedTools.Contains(tool.type);
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
            behaviour.WaterLevel.Value++;
        }

        public void UseTool(Tool tool)
        {
            switch (tool.type)
            {
                case ToolType.Hoe:
                    if (behaviour.ShapeLevel.Value < formStates.Length)
                    {
                        Plow();
                        return;
                    }
                    return;
                case ToolType.WateringCan:
                    if (behaviour.WaterLevel.Value < 10)
                    {
                        Water();
                        return;
                    }
                    break;
            }
        }
    }
}
