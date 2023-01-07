using ECF.Domain;
using ECF.Simulation.Behaviours;

namespace ECF.Simulation.Systems
{
    public interface ICropSystem
    {
        CropBehaviour PlantCrop(CropTemplate template);
    }
}