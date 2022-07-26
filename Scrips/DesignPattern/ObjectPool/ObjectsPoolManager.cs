using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dongpn.Singleton;
using System.Threading.Tasks;
using System;

namespace Dongpn.ObjectPool
{
    public class ObjectsPoolManager : Singleton<ObjectsPoolManager>
    {

        static Dictionary<string, List<SmallPoolItem>> poolDic = new Dictionary<string, List<SmallPoolItem>>();

        private static List<SmallPoolItem> GetList(string typeOf) 
        {
            if (poolDic.ContainsKey(typeOf))
            {
                return poolDic[typeOf];
            }
            else
            {
                List<SmallPoolItem> list = new List<SmallPoolItem>();
                poolDic[typeOf] = list;
                return list;
            }
        }

        public T RequestObject<T>() where T : SmallPoolItem
        {
            Debug.Log("Request Objects");
            List<SmallPoolItem> list = GetList(typeof(T).Name);

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Avaiable)
                {
                    return (T) list[i];
                }
            }

            T next_object = GetNewInstance<T>();
            list.Add(next_object);
            return next_object;
        }

        private T GetNewInstance<T>() where T : SmallPoolItem
        {
            ResourceAttribute att = (ResourceAttribute) Attribute.GetCustomAttribute(typeof(T), typeof(ResourceAttribute));

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

        protected override void OnDestroy()
        {
            foreach (string key in poolDic.Keys) {
                poolDic.Remove(key);
            }
            poolDic = null;
            base.OnDestroy();
        }

    }

}
