using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerItem : MonoBehaviour
{
    [SerializeField]
    private TMP_Text text;

    [SerializeField]
    private Button btn_PlayerItem;

    [SerializeField]
    private Image img_Ready;

    public void SetText(string newText, bool ready)
    {
        img_Ready.gameObject.SetActive(ready);
        text.text = newText;
    }
}
