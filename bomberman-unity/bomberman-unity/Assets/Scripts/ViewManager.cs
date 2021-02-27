using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewManager : MonoBehaviour
{
    private static ViewManager instance;

    private void Awake()
    {
        Debug.Assert(instance == null);
        instance = this;
    }

    public static void SwitchToView<T>() where T : ViewBase
    {
        instance.DisableAllViews();

        var views = instance.gameObject.GetComponentsInChildren<T>(true);

        Debug.Log(views);
        Debug.Assert(views.Length == 1);

        views[0].Show(true);
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
