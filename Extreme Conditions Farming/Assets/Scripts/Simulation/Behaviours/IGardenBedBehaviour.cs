using System.Collections.Generic;
using ECF.Domain;
using ECF.Domain.Common;

namespace ECF.Behaviours.Behaviours
{
    public interface IGardenBedBehaviour
    {
        ObservableValue<BedStatus> Status { get; }
        ObservableValue<int> ShapeLevel { get; }
        void AddAttribute(CropAttribute attribute);
        void RemoveAttribute(CropAttribute attribute);
        bool HasAttribute(CropAttribute attribute);
        bool Unlock();
    }
}