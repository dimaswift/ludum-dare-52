using System;
using System.Collections.Generic;
using ECF.Behaviours.Behaviours;
using ECF.Domain;
using UnityEngine;

namespace ECF.Views
{
    public class CropView : MonoBehaviour, IToolUseResult
    {
        public Transform HoldPoint => phases[Crop.Phase].HoldPoint;

        private CropPhase? currentPhase;
        private readonly Dictionary<CropPhase, CropPhaseView> phases = new();

        private static readonly CropPhase[] AllPhases = (CropPhase[]) Enum.GetValues(typeof(CropPhase));
        public Crop Crop { get; private set; }
        private GardenBedBehaviour bed;
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
                phases.Add(cropPhase, obj.GetComponent<CropPhaseView>());
            }
        }

        public void SetUp(Crop crop)
        {
            Crop = crop;
            foreach (var o in phases)
            {
                o.Value.gameObject.SetActive(false);
            }
            OnPhaseChanged(crop.Phase);
            var view = phases[crop.Phase];
            view.SetContinuousProgress(crop.GrowProgress);
        }

        public void Subscribe(GardenBedBehaviour bed)
        {
            Unsubscribe();
            this.bed = bed;
            this.bed.Phase.Changed += OnPhaseChanged;
            this.bed.CurrentPhaseProgressNormalized.Changed += OnGrowthProgressChanged;
        }

        private void OnGrowthProgressChanged(float progress)
        {
            var view = phases[bed.Phase.Value];
            view.SetContinuousProgress(progress);
        }

        private void Unsubscribe()
        {
            if (bed == null)
            {
                return;
            }
            
            bed.Phase.Changed -= OnPhaseChanged;
            bed.CurrentPhaseProgressNormalized.Changed -= OnGrowthProgressChanged;
            bed = null;
        }
        
        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void OnPhaseChanged(CropPhase phase)
        {
            if (phase == currentPhase) return;
            if (currentPhase.HasValue)
            {
                if (phases.ContainsKey(currentPhase.Value))
                {
                    phases[currentPhase.Value].gameObject.SetActive(false);
                }
            }
            phases[phase].gameObject.SetActive(true);
            currentPhase = phase;
        }

    }
}