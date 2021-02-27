using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewManager : PersistantSceneObject<ViewManager>
{
    [SerializeField]
    private ViewBase startView;

    private void Start()
    {
        SwitchToView(startView.GetType());
    }

    public static void SwitchToView(Type viewType)
    {
        var instance = GetInstance();

        instance.DisableAllViews();

        var views = instance.gameObject.GetComponentsInChildren(viewType, true);

        Debug.Assert(views.Length == 1);

        ((ViewBase)(views[0])).Show(true);
    }

    private void DisableAllViews()
    {
        var views = gameObject.GetComponentsInChildren<ViewBase>();

        foreach(var view in views)
        {
            view.Show(false);
        }
    }
}
