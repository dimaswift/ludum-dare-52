using System;
using ECF.Domain;

namespace ECF.Behaviours.Systems
{
    public class CropTemplateBuilder
    {

        private readonly CropTemplate template = new()
        {
            PhaseStats = new()
            {
                SellPrices = new(),
                Durations = new(),
                WaterConsumption = new(),
                SeedConversionRate = new()
            }
        };

        private static readonly CropPhase[] Phases = Enum.GetValues(typeof(CropPhase)) as CropPhase[];
        
        internal CropTemplate Build()
        {
            foreach (CropPhase phase in Phases)
            {
                if (!template.PhaseStats.Durations.ContainsKey(phase))
                {
                    template.PhaseStats.Durations[phase] = 1;
                }

                if (!template.PhaseStats.SellPrices.ContainsKey(phase))
                {
                    template.PhaseStats.SellPrices[phase] = 1;
                }
                
                if (!template.PhaseStats.WaterConsumption.ContainsKey(phase))
                {
                    template.PhaseStats.WaterConsumption[phase] = 1;
                }
                
                if (!template.PhaseStats.SeedConversionRate.ContainsKey(phase))
                {
                    template.PhaseStats.SeedConversionRate[phase] = 1;
                }
            }
            
            return template;
        }
        
        public CropTemplateBuilder WithName(string name)
        {
            template.Name = name;
            return this;
        }
        
        
        public CropTemplateBuilder WithPhaseDuration(CropPhase phase, int ticks)
        {
            template.PhaseStats.Durations[phase] = ticks;
            return this;
        }

        public CropTemplateBuilder WithPhaseDuration(int ticks)
        {
            foreach (CropPhase phase in Phases)
            {
                template.PhaseStats.Durations[phase] = ticks;
            }
            return this;
        }
        
        public CropTemplateBuilder WithWaterConsumption(CropPhase phase, int consumption)
        {
            template.PhaseStats.WaterConsumption[phase] = consumption;
            return this;
        }

        public CropTemplateBuilder WithWaterConsumption(int consumption)
        {
            foreach (CropPhase phase in Phases)
            {
                template.PhaseStats.WaterConsumption[phase] = consumption;
            }
            return this;
        }
        
        public CropTemplateBuilder WithSeedConversionRate(CropPhase phase, int rate)
        {
            template.PhaseStats.SeedConversionRate[phase] = rate;
            return this;
        }

        public CropTemplateBuilder WithSeedConversionRate(int rate)
        {
            foreach (CropPhase phase in Phases)
            {
                template.PhaseStats.SeedConversionRate[phase] = rate;
            }         
            return this;
        }
        
        public CropTemplateBuilder WithSellPrice(CropPhase phase, int price)
        {
            template.PhaseStats.SellPrices[phase] = price;
            return this;
        }

        public CropTemplateBuilder WithSellPrice( int price)
        {
            foreach (CropPhase phase in Phases)
            {
                template.PhaseStats.SellPrices[phase] = price;
            }
            return this;
        }
    }
}