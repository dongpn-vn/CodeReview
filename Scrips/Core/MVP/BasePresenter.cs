using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePresenter<V> where V : IView
{
    private V view;
    public V View
    {
        get
        {
            return view;
        }
        set
        {
            view = value;
        }
    }

    public abstract void Init();
}
