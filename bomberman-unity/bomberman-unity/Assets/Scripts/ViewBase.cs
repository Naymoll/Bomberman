using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ViewBase : MonoBehaviour
{
    public virtual void Show(bool show)
    {
        gameObject.SetActive(show);
    }
}
