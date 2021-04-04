using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class SaveFileScrollPane : MonoBehaviour
{
    [SerializeField]
    private GameObject CardArea;
    [SerializeField]
    private TextMeshProUGUI textArea;
    [HideInInspector]
    public DictionaryCard SelectedCard;
    private List<DictionaryCard> cards = new List<DictionaryCard>();

    public string Filename { get => textArea.text; set => textArea.text = value; }

    public bool TryRenameSelection(string newName, out string errorResponse)
    {
        string currentText = textArea.text;
        if (!string.IsNullOrEmpty(currentText))
        {
            if (SaveGameManager.FileNameInvalidOrTaken(newName, out errorResponse)) return false;
            else
            {
                RenameCardWithHeader(textArea.text, newName);
                SaveGameManager.RenameFile(textArea.text, newName);
                textArea.SetText(newName);
                //TODO: Change the date modified to right now
                return true;
            }
        }
        else
        {
            errorResponse = "No selection to rename";
            return false;
        }
    }

    public void RenameCardWithHeader(string currentText, string newText)
    {
        foreach(var card in cards)
        {
            if(card.Header == currentText) card.Header = newText;
        }
    }

    public void PopulateRegion()
    {
        FetchFiles();
    }

    public void ClearRegion()
    {
        ClearCards();
    }

    private void ClearCards()
    {
        foreach (DictionaryCard card in cards)
        {
            Destroy(card.gameObject);
        }
        cards.Clear();
    }

    private void FetchFiles()
    {
        DictionaryCard card;
        foreach (string file in SaveGameManager.FetchSaveFiles())
        {
            card = DictionaryCard.Spawn(CardArea.transform, UIBackgroundSprite.Blue, System.IO.Path.GetFileNameWithoutExtension(file));
            card.AddItem("Created", File.GetCreationTime(file).ToString());
            card.AddItem("Last Modified", File.GetLastWriteTime(file).ToString());
            card.OnClick.AddListener(CardClicked);
            card.OnRemoved.AddListener(CardDeleted);
            cards.Add(card);
        }
    }

    private void CardDeleted(UIClickable card)
    {
        DictionaryCard detailedCard = (DictionaryCard)card;
        SaveGameManager.DeleteSaveFile(detailedCard.Header);
        cards.Remove(detailedCard);
    }

    private void CardClicked(UIClickable card)
    {
        SelectedCard = (DictionaryCard)card;
        string fileName = SelectedCard.Header;
        textArea.SetText(fileName);
    }
}
