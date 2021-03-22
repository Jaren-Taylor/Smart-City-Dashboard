using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuLoadMenu : MonoBehaviour
{
    [SerializeField]
    private SaveFileScrollPane scrollPane;


    public void OnDisable()
    {
        scrollPane.ClearRegion();
    }

    public void OnEnable()
    {
        scrollPane.PopulateRegion();
    }
}
