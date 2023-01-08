namespace ECF.Views
{
    public interface IToolTarget
    {
        bool CanUseTool(Tool tool);
        void OnHoverBegan(Tool tool);
        void OnHoverEnded();
        void UseTool(Tool tool);
    }
}