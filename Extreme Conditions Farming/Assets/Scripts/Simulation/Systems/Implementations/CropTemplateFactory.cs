using System.Collections.Generic;
using ECF.Domain;

namespace ECF.Behaviours.Systems
{
    public class CropTemplateFactory : ICropTemplateFactory
    {
        private readonly Dictionary<string, CropTemplate> templates = new();

        public CropTemplate GetOrCreate(string id, CropTemplateBuilder builder)
        {
            if (templates.TryGetValue(id, out var t))
            {
                return t;
            }
            var template = builder.Build();
            template.Id = id;
            template.SeedId = $"{id}_seed";
            template.HarvestId = $"{id}_harvest";
            templates.Add(id, template);
            return template;
        }

        public CropTemplate Get(string id)
        {
            if (templates.TryGetValue(id, out CropTemplate t))
            {
                return t;
            }
            return null;
        }

        public CropTemplate CreateLinear(string id, string name, int growthDuration, int waterConsumption,
            int seedConversionRate, int sellPrice, int nutrition)
        {
            if (templates.TryGetValue(id, out var t))
            {
                return t;
            }
            
            return GetOrCreate(id, new CropTemplateBuilder()
                .WithName(name)
                .WithPhaseDuration(growthDuration)
                .WithWaterConsumption(waterConsumption)
                .WithSeedConversionRate(seedConversionRate)
                .WithNutrition(nutrition)
                .WithSellPrice(sellPrice));
        }
    }
}