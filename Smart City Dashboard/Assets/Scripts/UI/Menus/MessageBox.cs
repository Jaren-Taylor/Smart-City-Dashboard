using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageBox : Menu
{
    public TextMeshProUGUI Title;
    public TextMeshProUGUI Body;

    public void SetValues(string title, string body)
    {
        Title.text = title;
        Body.text = body;
    }
}
