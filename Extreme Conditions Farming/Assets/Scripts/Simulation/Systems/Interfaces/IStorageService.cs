using System;

namespace ECF.Behaviours.Systems
{
    public interface IStorageService
    {
        void Save<T>(T value, string key = null);
        T Load<T>(Func<T> defaultValueHandler);
        T Load<T>(string key, Func<T> defaultValueHandler);
        bool Exists<T>(string key = null);
        void Delete<T>();
    }
}