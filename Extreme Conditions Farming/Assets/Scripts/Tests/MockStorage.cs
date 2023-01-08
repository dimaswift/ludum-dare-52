using System;
using System.Collections.Generic;
using ECF.Behaviours.Systems;

public class MockStorage : IStorageService
{
    private readonly Dictionary<string, object> savedObjects = new();
    
    public void Save<T>(T value, string key = null)
    {
        if (key == null)
            key = typeof(T).Name;
        savedObjects[key] = value;
    }

    public T Load<T>(Func<T> defaultValueHandler)
    {
        return Load(typeof(T).Name, defaultValueHandler);
    }

    public T Load<T>(string key, Func<T> defaultValueHandler)
    {
        if (savedObjects.TryGetValue(key, out var value))
        {
            return (T)value;
        }

        savedObjects[key] = defaultValueHandler();
        return (T)savedObjects[key];
    }

    public bool Exists<T>(string key = null)
    {
        if (key == null)
            key = typeof(T).Name;
        return savedObjects.ContainsKey(key);
    }

    public void Delete<T>()
    {
        savedObjects.Remove(typeof(T).Name);
    }
}