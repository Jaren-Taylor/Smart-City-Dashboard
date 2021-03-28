using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class InputFieldMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI header;
    [SerializeField] private TextMeshProUGUI caption;
    [SerializeField] private TextMeshProUGUI placeholder;
    [SerializeField] private TextMeshProUGUI inputText;
    [SerializeField] private Button submitButton;
    [SerializeField] private Image background;

    public string Header { get => header.text; set => header.text = value; }
    public string Caption { get => caption.text; set => caption.text = value; }
    public string Placeholder { get => placeholder.text; set => placeholder.text = value; }
    public string InputText { get; private set; }

    public UnityAction<string> OnSubmit;

    private void Start()
    {
        submitButton.onClick.AddListener(() => OnSubmit?.Invoke(header.text));
    }

    public void Show(string header, string caption, string placeholder, UIBackgroundSprite spriteColor)
    {
        this.header.text = header;
        this.caption.text = caption;
        this.placeholder.text = placeholder;
        background.sprite = UIManager.BackgroundSprites[spriteColor];
        gameObject.SetActive(true);
    }
}
