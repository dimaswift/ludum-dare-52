namespace ECF.Domain.Game
{
    public class PlayerSave
    {
        public SimulationState SimulationState { get; set; }
    }

    public enum GamePhase
    {
        MainMenu,
        Paused,
        Playing,
        GameOver
    }
}