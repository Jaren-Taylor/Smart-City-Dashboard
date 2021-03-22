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
    private TextMeshProUGUI TextArea;

    private List<DictionaryCard> cards = new List<DictionaryCard>();

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
        Debug.Log("Safe file deleted!");
        //SaveGameManager.DeleteSaveFile(detailedCard.Header);
        cards.Remove(detailedCard);
    }

    private void CardClicked(UIClickable card)
    {
        DictionaryCard detailedCard = (DictionaryCard)card;
        string fileName = detailedCard.Header;
        TextArea.SetText(fileName);
    }
}
