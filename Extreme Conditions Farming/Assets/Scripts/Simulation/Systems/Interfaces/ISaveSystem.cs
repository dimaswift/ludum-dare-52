using System;

namespace ECF.Simulation.Systems
{
    public interface ISaveSystem : ISystem
    {
        void Save<T>(T value, string key = null);
        T Load<T>(Func<T> defaultValueHandler);
        T Load<T>(string key, Func<T> defaultValueHandler);
    }
}