using UnityEngine;

namespace ECF.Views
{
    public interface IToolUseResult
    {
        Transform transform { get; }
        Transform HoldPoint { get; }
    }
}