using System;
using System.Collections.Generic;
using ECF.Domain;
using ECF.Domain.Common;
using UnityEngine;

namespace ECF.Views
{
    public class CropView : MonoBehaviour, IToolUseResult
    {
        public float height;
        
        private ObservableValue<CropPhase> phase;
        private CropPhase? prevPhase;

        private readonly Dictionary<CropPhase, GameObject> phases = new();

        private static readonly CropPhase[] AllPhases = (CropPhase[]) Enum.GetValues(typeof(CropPhase));
        public Crop Crop { get; private set; }

        private void Awake()
        {
            foreach (CropPhase cropPhase in AllPhases)
            {
                var obj = transform.Find(cropPhase.ToString()).gameObject;
                if (obj == null)
                {
                    Debug.LogError($"Crop phase on {name} " + cropPhase + " not found");
                    continue;
                }
                phases.Add(cropPhase, obj);
            }
        }

        public void SetUp(Crop crop, ObservableValue<CropPhase> phase)
        {
            Crop = crop;
            this.phase = phase;
            phase.Changed += OnPhaseChanged;
            foreach (var o in phases)
            {
                o.Value.SetActive(false);
            }
            OnPhaseChanged(phase.Value);
        }

        private void OnDestroy()
        {
            if (phase != null)
            {
                phase.Changed -= OnPhaseChanged;
            }
        }

        private void OnPhaseChanged(CropPhase phase)
        {
            if (phase == prevPhase) return;
            if (prevPhase.HasValue)
            {
                if (phases.ContainsKey(prevPhase.Value))
                {
                    phases[prevPhase.Value].SetActive(false);
                }
            }
            phases[phase].SetActive(true);
            prevPhase = phase;
        }
    }
}