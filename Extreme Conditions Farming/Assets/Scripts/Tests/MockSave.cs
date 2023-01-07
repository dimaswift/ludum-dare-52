using System;
using System.Collections.Generic;
using ECF.Simulation.Systems;

public class MockSave : ISaveSystem
{
    private readonly Dictionary<string, object> savedObjects = new();

    public void OnInit(int time)
    {

    }

    public void OnDispose()
    {

    }

    public void OnTick(int time, int delta)
    {

    }

    public void Save()
    {

    }

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
}