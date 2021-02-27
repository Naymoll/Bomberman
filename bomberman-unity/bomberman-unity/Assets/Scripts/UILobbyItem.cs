using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UILobbyItem : MonoBehaviour
{
    [SerializeField]
    private TMP_Text text;

    public void SetText(string newText)
    {
        text.text = newText;
    }
}
