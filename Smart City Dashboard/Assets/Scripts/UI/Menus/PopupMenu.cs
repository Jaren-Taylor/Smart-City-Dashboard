using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class PopupMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject popupCenter;

    [SerializeField]
    private TextMeshProUGUI title;
    [SerializeField]
    private TextMeshProUGUI message;

    public void SetValues(string title, string message)
    {
        this.title.SetText(title);
        this.message.SetText(message);
    }
}
