using System;
using UnityEngine;

namespace Dongpn.Factory {
    public abstract class FactoryPrefab<T> : MonoBehaviour where T : FactoryPrefab<T>
    {
        public static T GetInstance()
        {
            ResourceAttribute att = (ResourceAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(ResourceAttribute));

            if (att == null || string.IsNullOrEmpty(att.Path))
            {
                return new GameObject(typeof(T).Name).AddComponent<T>();
            }
            else
            {
                var prefabs = Resources.Load<GameObject>(att.Path);
                GameObject go = Instantiate(prefabs);
                T rt = go.GetComponent<T>();

                return rt == null ? go.AddComponent<T>() : rt;
            }
        }
    }
}

