using ECF.Domain.Common;

namespace ECF.Behaviours
{
    public interface IFamilySystem : ISystem
    {
        void Feed(int calories);
        int DeadlyHungerLevel { get; }
        bool KillFamilyMember();
        ObservableValue<int> Hunger { get; }
        ObservableValue<int> MembersAmount { get; }
        ObservableValue<int> PregnancyProgress { get; }
    }
}