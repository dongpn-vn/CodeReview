using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Dongpn.Singleton
{

    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static bool isApplicationQuiting = false;

        private static T instance;

        public static T Instance
        {
            get
            {
                if(isApplicationQuiting)
                {
                    return null;
                }

                if (instance == null)
                {
                    instance = new GameObject(typeof(T).Name).AddComponent<T>();
                }
                return instance;
            }

            protected set => instance = value;
        }

        public static bool InstanceExists { get { return instance != null; } }

        public virtual void Awake()
        {
            if (instance != null && instance.GetInstanceID() != this.GetInstanceID())
            {
                Destroy(gameObject);
            }
            else
            {
                instance = (T)this;
            }
        }


        protected virtual void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

        private void OnApplicationQuit()
        {
            isApplicationQuiting = true;
        }
    }
}