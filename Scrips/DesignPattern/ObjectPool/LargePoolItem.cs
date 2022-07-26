using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dongpn.ObjectPool
{
    public abstract class LargePoolItem : MonoBehaviour, IPoolable
    {
        public int Index
        {
            get;
            set;
        }

        public virtual void ObjectActive()
        {

        }

        public virtual void ObjectDeactive()
        {

        }

    }

}
