using TMPro;
using UnityEngine;

public class Header : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI tmpText;

    public TextMeshProUGUI TMPText
    {
        get { return tmpText; }
        set { tmpText = value; }
    }
}
