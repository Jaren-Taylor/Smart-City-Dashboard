using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadMenu : Menu
{
    [SerializeField]
    private GameObject CardArea;
    private List<DetailedCard> cards = new List<DetailedCard>();

    private StringBuilder strBuilder = new StringBuilder();

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
        foreach (DetailedCard card in cards)
        {
            Destroy(card.gameObject);
        }
        cards.Clear();
    }

    public void FetchFiles()
    {
        DetailedCard card;
        foreach (string file in Directory.EnumerateFiles(Application.dataPath+"/Saves", "*.xml"))
        {   
            card = DetailedCard.Spawn(CardArea.transform, UIBackgroundSprite.Blue, System.IO.Path.GetFileNameWithoutExtension(file));
            card.AddItem("Last Saved", File.GetLastWriteTime(file).ToString());
            card.OnClick += LoadGame;
            card.OnRemoveClicked.AddListener(CardDeleted);
            cards.Add(card);
        }
    }

    private void CardDeleted(UIElement card)
    {
        DetailedCard detailedCard = (DetailedCard)card;
        DeleteSaveFile(detailedCard.GetHeader());
        cards.Remove(detailedCard);
    }

    private void LoadGame(UIElement card)
    {
        DetailedCard detailedCard = (DetailedCard)card;
        strBuilder.Clear();
        strBuilder.Append(Application.dataPath);
        strBuilder.Append("/Saves/");
        strBuilder.Append(detailedCard.GetHeader());
        strBuilder.Append(".xml");
        SaveGameManager.LoadFromFile = strBuilder.ToString();
        GridManager.Instance.LoadGame();
    }
    
    private void DeleteSaveFile(string name)
    {
        strBuilder.Clear();
        strBuilder.Append(Application.dataPath);
        strBuilder.Append("/Saves/");
        strBuilder.Append(name);
        strBuilder.Append(".xml");
        File.Delete(@strBuilder.ToString());
        strBuilder.Append(".meta");
        File.Delete(@strBuilder.ToString());
    }
}
