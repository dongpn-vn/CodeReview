using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Dongpn.ObjectPool
{
    public abstract class SmallPoolItem : MonoBehaviour, IPoolable
    {

        public bool Avaiable
        {
            get;
            private set;
        }

        public virtual void ObjectActive()
        {
            Avaiable = false;
        }

        public virtual void ObjectDeactive()
        {
            Avaiable = true;
        }
    }
}

