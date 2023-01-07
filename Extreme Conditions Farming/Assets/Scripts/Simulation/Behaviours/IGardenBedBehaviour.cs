using System.Collections.Generic;
using ECF.Domain;
using ECF.Domain.Common;

namespace ECF.Simulation.Behaviours
{
    public interface IGardenBedBehaviour
    {
        IObservableValue<BedStatus> Status { get; }
        void AddAttribute(CropAttribute attribute);
        void RemoveAttribute(CropAttribute attribute);
        bool HasAttribute(CropAttribute attribute);
        bool Unlock();
    }
}