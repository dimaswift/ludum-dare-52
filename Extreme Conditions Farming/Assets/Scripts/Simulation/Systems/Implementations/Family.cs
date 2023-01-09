using System;
using ECF.Domain;
using ECF.Domain.Common;

namespace ECF.Behaviours.Systems
{
    public class Family : IFamilySystem
    {
        public int DeadlyHungerLevel => 5000;
        public ObservableValue<int> Hunger { get; }
        public ObservableValue<int> MembersAmount { get; }
        public ObservableValue<int> PregnancyProgress { get; }
        
        private readonly FamilySystemData data;
        
        private const int PREGNANCY_DURATION = 30 * 24;

        public Family(FamilySystemData data)
        {
            this.data = data;
            Hunger = new ObservableValue<int>(data.Hunger);
            MembersAmount = new ObservableValue<int>(data.MembersAmount);
            PregnancyProgress = new ObservableValue<int>(data.PregnancyProgress);
        }
        
        public void OnInit(int time)
        {
            
        }

        public void OnDispose()
        {
            
        }

        public void OnTick(int time, int delta)
        {
            Hunger.Value += delta * MembersAmount.Value * 2;
            PregnancyProgress.Value += delta;
            if (PregnancyProgress.Value >= PREGNANCY_DURATION)
            {
                PregnancyProgress.Value = 0;
                MembersAmount.Value++;
            }
        }

        public bool KillFamilyMember()
        {
            if (MembersAmount.Value <= 1)
            {
                return false;
            }
            MembersAmount.Value--;
            return true;
        }


        public void SaveState()
        {
            data.Hunger = Hunger.Value;
            data.MembersAmount = MembersAmount.Value;
            data.PregnancyProgress = PregnancyProgress.Value;
        }

        public void Feed(int calories)
        {
            Hunger.Value = Math.Max(0, Hunger.Value - calories);
        }


    }
}