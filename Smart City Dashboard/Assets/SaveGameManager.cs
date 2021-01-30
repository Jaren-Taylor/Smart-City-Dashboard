using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using UnityEngine;

public static class SaveGameManager 
{
    public static string LoadFromFile = "";

    public static TileGrid LoadGame(string gameLocation)
    {
        TileGrid grid = null;
        using (var fs = new FileStream(gameLocation, FileMode.Open))
        {
            DataContractSerializer dcs = new DataContractSerializer(typeof(TileGrid), new Type[] { typeof(RoadTile), typeof(BuildingTile), typeof(Tile) });
            XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());

            grid = (TileGrid)dcs.ReadObject(reader);
        }

        return grid;
    }

    public static bool SaveGame(string gameLocation, TileGrid saveData)
    {
        var serializer = new DataContractSerializer(typeof(TileGrid), new Type[] { typeof(RoadTile), typeof(BuildingTile), typeof(Tile) } );
        using (FileStream fs = new FileStream(gameLocation, FileMode.Truncate))
        {
            using (XmlWriter xmlWriter = XmlWriter.Create(fs))
            {
                serializer.WriteObject(xmlWriter, saveData);
            }
        }

        return true;
    }
}
