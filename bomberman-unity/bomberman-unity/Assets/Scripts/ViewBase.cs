using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ViewBase : MonoBehaviour
{
    public virtual void Show(bool show)
    {
        gameObject.SetActive(show);
        if (show)
        {
            OnShow?.Invoke();
        }
        else
        {
            OnHide?.Invoke();
        }
    }

    public event Action OnShow;

    public event Action OnHide;
}
