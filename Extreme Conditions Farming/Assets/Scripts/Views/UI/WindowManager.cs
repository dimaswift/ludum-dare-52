using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ECF.Views.UI
{
    public class WindowManager : MonoBehaviour
    {
        private readonly Dictionary<Type, Window> windows = new ();

        public async void Show<T>() where T : Window
        {
            if (!windows.TryGetValue(typeof(T), out var win))
            {
                var path = $"Windows/{typeof(T).Name}";
                var prefab = Resources.Load<GameObject>(path);
                if (prefab == null)
                {
                    Debug.LogError($"Window prefab not found, should be in Resources/{path}");
                    return;
                }
                win = Instantiate(prefab).GetComponent<Window>();
                win.transform.SetParent(transform);
                win.gameObject.SetActive(false);
                win.Init();
                var rect = win.GetComponent<RectTransform>();
                rect.anchorMax = new Vector2(1,1);
                rect.anchorMin = new Vector2();
                rect.anchoredPosition = new Vector2();
                rect.rect.Set(0, 0, 0, 0);
                windows.Add(typeof(T), win);
            }

            if (win == null)
            {
                return;
            }

            await win.Show();
        }
    }
}