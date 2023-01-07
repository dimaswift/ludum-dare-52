using System;
using ECF.Behaviours.Systems;
using Newtonsoft.Json;
using UnityEngine;

namespace ECF.Behaviours.Services
{
    public class PlayerPrefsStorage : IStorageService
    {
        public void Save<T>(T value, string key = null)
        {
            var json = JsonConvert.SerializeObject(value);
            if (key == null)
            {
                key = typeof(T).Name;
            }
            PlayerPrefs.SetString(key, json);
            PlayerPrefs.Save();
        }

        public T Load<T>(Func<T> defaultValueHandler)
        {
            return Load(typeof(T).Name, defaultValueHandler);
        }

        public T Load<T>(string key, Func<T> defaultValueHandler)
        {
            if (Exists<T>(key))
            {
                var data = JsonConvert.DeserializeObject<T>(PlayerPrefs.GetString(key));
                if (data == null)
                {
                    data = defaultValueHandler();
                }
                return data;
            }
            return defaultValueHandler();
        }

        public bool Exists<T>(string key = null)
        {
            if (key == null)
            {
                key = typeof(T).Name;
            }
            return PlayerPrefs.HasKey(key);
        }

        public void Delete<T>()
        {
            PlayerPrefs.DeleteKey(typeof(T).Name);
            PlayerPrefs.Save();
        }

        public void DeleteAll()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }
    }
}