using ECF.Domain;

namespace ECF.Behaviours.Systems
{
    public interface ICropTemplateFactory
    {
        CropTemplate GetOrCreate(string id, CropTemplateBuilder builder);
        CropTemplate Get(string id);
        CropTemplate CreateLinear(string id, string name, int growthRate, int waterConsumption,
            int seedConversionRate, int sellPrice);
    }
}