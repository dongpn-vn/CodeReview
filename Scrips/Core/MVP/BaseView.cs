using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dongpn.Singleton;

public abstract class BaseView<V, P> : Singleton<V>, IView where V : BaseView<V,P>, IView where P : BasePresenter<V>, new()
{
    public static P Presenter;

    public override void Awake()
    {
        base.Awake();
        Presenter = new P();
        Presenter.View = (V)this;
        Presenter.Init();
    }

    protected override void OnDestroy()
    {
        Presenter.View = null;
        Presenter = null;
        base.OnDestroy();
    }
}
