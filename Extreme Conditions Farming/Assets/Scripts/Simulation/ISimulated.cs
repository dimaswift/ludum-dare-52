using System;

namespace ECF.Behaviours
{
    public interface ISimulated
    {
        void OnInit(int time);
        void OnDispose();
        void OnTick(int time, int delta);
    }
}