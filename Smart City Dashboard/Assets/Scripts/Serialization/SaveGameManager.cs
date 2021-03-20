using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using UnityEngine;

public static class SaveGameManager 
{
    public static string FileName = "";

    public static string LoadFromFile = "";
    /// <summary>
    /// Loads the game from XML
    /// </summary>
    /// <param name="gameLocation">Searches this location for the XML file to load the game</param>
    /// <returns>Returns the loaded map from the file if found, otherwise returns null</returns>
    public static TileGrid LoadGame(string gameLocation)
    {
        TileGrid grid = null;
        using (var fs = new FileStream(gameLocation, FileMode.Open))
        {
            DataContractSerializer dcs = GetDataContract();
            XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());

            grid = (TileGrid)dcs.ReadObject(reader);
        }

        return grid;
    }
    /// <summary>
    /// Saves the game to XML
    /// </summary>
    /// <param name="gameLocation"></param>
    /// <param name="saveData"></param>
    /// <returns></returns>
    public static bool SaveGame(string gameLocation, TileGrid saveData)
    {
        FileMode mode;
        if (File.Exists(gameLocation))
        {
             mode = FileMode.Truncate;
        }
        else
        {
            mode = FileMode.Create;
        }
        using (FileStream fs = new FileStream(gameLocation, mode))
        {
            using (XmlWriter xmlWriter = XmlWriter.Create(fs))
            {
                var dcs = GetDataContract();
                dcs.WriteObject(xmlWriter, saveData);
            }
        }

        return true;
    }
    private static DataContractSerializer GetDataContract()
    {
        return new DataContractSerializer(typeof(TileGrid), new Type[] { typeof(RoadTile), typeof(BuildingTile), typeof(Tile) });
    }
  
}
