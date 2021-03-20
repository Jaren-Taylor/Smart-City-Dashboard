using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadMenu : Menu
{
    [SerializeField]
    private GameObject CardArea;
    private List<DictionaryCard> cards = new List<DictionaryCard>();

    public override void Open()
    {
        FetchFiles();
        base.Open();
    }

    public override void Close()
    {
        ClearCards();
        base.Close();
    }

    public void ClearCards()
    {
        foreach (DictionaryCard card in cards)
        {
            Destroy(card.gameObject);
        }
        cards.Clear();
    }

    public void FetchFiles()
    {
        DictionaryCard card;
        foreach (string file in SaveGameManager.FetchSaveFiles())
        {   
            card = DictionaryCard.Spawn(CardArea.transform, UIBackgroundSprite.Blue, System.IO.Path.GetFileNameWithoutExtension(file));
            card.AddItem("Created", File.GetCreationTime(file).ToString());
            card.AddItem("Last Modified", File.GetLastWriteTime(file).ToString());
            card.OnClick.AddListener(LoadGame);
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

    private void LoadGame(UIClickable card)
    {
        SaveGameManager.LoadGame(((DictionaryCard)card).Header);
    }

    
}
