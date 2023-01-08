using ECF.Behaviours.Systems;

namespace ECF.Views
{
    public class Shop : CropDeposit
    {
        protected override bool TryDeposit(CropView cropView, out int result)
        {
            var storage = Game.Instance.Simulation.GetSystem<ICropStorage>();
            return storage.Sell(cropView.Crop, out result);
        }
    }
}