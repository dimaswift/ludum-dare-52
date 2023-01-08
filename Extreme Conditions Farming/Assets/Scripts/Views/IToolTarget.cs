using UnityEngine;

namespace ECF.Views
{
    public interface IToolTarget
    {
        void OnHoverBegan(Tool tool);
        void OnHoverEnded();
        IToolUseResult UseTool(Tool tool);
        float ToolHeight { get; }
        Vector3 Position { get; }
        
    }
}