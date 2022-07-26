using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dongpn.Singleton
{
    public class AllSceneSingleton<T> : Singleton<T> where T : Singleton<T>
    {
        public override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }
    }
}

