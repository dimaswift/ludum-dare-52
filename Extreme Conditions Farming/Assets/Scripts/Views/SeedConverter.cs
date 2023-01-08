using ECF.Behaviours.Systems;

namespace ECF.Views
{
    public class SeedConverter : CropDeposit
    {
        
        
        
        protected override bool TryDeposit(CropView cropView, out int result)
        {
             var storage = Game.Instance.Simulation.GetSystem<ICropStorage>();
             return storage.ConvertToSeeds(cropView.Crop, out result);
        }
    }
}