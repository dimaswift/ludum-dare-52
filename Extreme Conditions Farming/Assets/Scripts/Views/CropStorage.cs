using System;
using System.Collections.Generic;
using ECF.Behaviours.Systems;
using ECF.Domain;
using UnityEngine;

namespace ECF.Views
{
    public class CropStorage : MonoBehaviour
    {
        private CropSlot[] slots;

        private ICropStorage cropStorage;
        
        private void Start()
        {
            slots = GetComponentsInChildren<CropSlot>();
            Game.Instance.OnNewSimulationCreated += OnNewSimulationCreated;
        }

        private void OnNewSimulationCreated()
        {
            foreach (CropSlot crop in slots)
            {
                crop.Empty();
            }
            cropStorage = Game.Instance.Simulation.GetSystem<ICropStorage>();
            foreach (Crop crop in cropStorage.GetCrops())
            {
                PlaceCrop(crop);
            }
            cropStorage.OnCropAdded += PlaceCrop;
        }

        private CropSlot GetEmptySlot()
        {
            foreach (CropSlot slot in slots)
            {
                if (slot.Crop == null)
                {
                    return slot;
                }
            }
            return null;
        }
        
        private void PlaceCrop(Crop crop)
        {
            var slot = GetEmptySlot();
            if (slot == null) return;
            slot.Place(crop, cropStorage);
        }
    }
}