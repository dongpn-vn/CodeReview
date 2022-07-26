using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dongpn.ObjectPool
{
    public abstract class ObjectPool<T> : MonoBehaviour where T : LargePoolItem
    {
        [SerializeField]
        private GameObject prefab;

        private Dictionary<int, T> objectsDic;

        private int currentIndex;

        protected virtual void Start()
        {
            objectsDic = new Dictionary<int, T>();
        }

        public T RequestObject()
        {
            if(currentIndex < objectsDic.Count)
            {
                currentIndex++;
                return objectsDic[currentIndex-1];
            }
            else
            {
                T rt = Instantiate(prefab).AddComponent<T>();
                objectsDic.Add(currentIndex, rt);
                //Debug.Log("Request Index: "+currentIndex);
                rt.Index = currentIndex;
                currentIndex++;
                return rt;
            }
        }

        private void SwapObjects(int indexOne, int indexTwo)
        {
            T temp = objectsDic[indexOne];
            objectsDic[indexOne] = objectsDic[indexTwo];
            objectsDic[indexOne].Index = indexOne;

            objectsDic[indexTwo] = temp;
            objectsDic[indexTwo].Index = indexTwo;
        }

        public void ObjectRecycle(int index)
        {
            currentIndex--;
            SwapObjects(index, currentIndex);
            //Debug.Log("Recycle Index:"+currentIndex);
        }
        
    }
}

