using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using UnityEngine;

public static class SaveGameManager 
{
    public static string FileName = "";

    public static string DefaultFileName = "Default";

    public static string LoadFromFile = "";

    public static readonly string SavesDirectory = Application.persistentDataPath + "/Saves";

    private static readonly StringBuilder strBuilder = new StringBuilder();

    private static readonly HashSet<char> InvalidCharacters = new HashSet<char>(System.IO.Path.GetInvalidFileNameChars());

    /// <summary>
    /// Loads the game from XML
    /// </summary>
    /// <param name="gameLocation">Searches this location for the XML file to load the game</param>
    /// <returns>Returns the loaded map from the file if found, otherwise returns null</returns>
    public static TileGrid ReadMapFromFile(string gameLocation)
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
    public static bool WriteMapToFile(string levelName, TileGrid saveData)
    {
        if (levelName == "") {
            strBuilder.Clear();
            strBuilder.Append(DefaultFileName);
            int count = GetSaveFileCount();
            if (count != 0) {
                strBuilder.Append(' ');
                strBuilder.Append(count.ToString());
            }
            levelName = strBuilder.ToString();
            FileName = levelName;
        }
        FileMode mode;
        string gameLocation = BuildSaveFilePath(levelName);
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

    public static bool WriteMapToFile(TileGrid saveData)
    {
        return WriteMapToFile(FileName, saveData);
    }

    internal static void RenameFile(string currentFilename, string enteredText)
    {
        if (FileNameInvalidOrTaken(enteredText, out _)) return;
        if (FileExists(currentFilename, out string currentPath))
        {
            string newPath = BuildSaveFilePath(enteredText);
            File.Move(currentPath, newPath);
        }
    }

    internal static void CopyFile(string currentFilename, string enteredText)
    {
        if (!FileNameInvalidOrTaken(enteredText, out _) && FileExists(currentFilename, out string currentPath))
        {
            File.Copy(currentPath, BuildSaveFilePath(enteredText));
        }
    }

    /// <summary>
    /// Enumerates the saves directory. Creates it if it doesn't exist
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<string> FetchSaveFiles() 
    {
        if (DirectoryExists())
        {
            try
            {
                return Directory.EnumerateFiles(SavesDirectory, "*.xml");
            }
            catch
            {
                Debug.LogError("Cannot load files from the directory:\n" + SavesDirectory);
                return Enumerable.Empty<string>();
            }
        }
        else
        {
            return Enumerable.Empty<string>();
        }
    }

    private static bool DirectoryExists()
    {
        if (!Directory.Exists(SavesDirectory))
        {
            try
            {
                Directory.CreateDirectory(SavesDirectory);
                return true;
            }
            catch
            {
                Debug.LogError("Failed to create directory at:\n" + SavesDirectory);
                return false;
            }
        }
        return true;
    }

    public static bool FileNameInvalidOrTaken(string fileName, out string response)
    {
        if(FileNameInvalid(fileName, out response))
        {
            return true;
        }
        else if (FileExists(fileName))
        {
            response = "That filename already exists";
            return true;
        }

        response = "";
        return false;
    }

    public static bool FileExists(string fileName) => FileExists(fileName, out _);
    public static bool FileExists(string fileName, out string filePath)
    {
        filePath = BuildSaveFilePath(fileName);
        return File.Exists(filePath);
    }

    public static bool FileNameInvalid(string fileName, out string response)
    {
        strBuilder.Clear();
        strBuilder.Append("The filename is not valid because ");

        if (string.IsNullOrEmpty(fileName))
        {
            strBuilder.Append("it is empty.");
            response = strBuilder.ToString();
            return true;
        }

        bool found = false;
        foreach (char c in fileName)
        {
            if (InvalidCharacters.Contains(c))
            {
                if (!found) strBuilder.Append("it contains the character(s): ( ");
                if (found) strBuilder.Append(", "); 
                found = true;
                strBuilder.Append(c);
            }
        }
        if (found)
        {
            strBuilder.Append(").");
            response = strBuilder.ToString();
            return true;
        }

        response = "";
        return false;
    }

    public static void LoadGame(string fileName)
    {
        SetFileName(fileName);
        GridManager.Instance.LoadGame();
    }

    public static void SetFileName(string fileName)
    {
        LoadFromFile = BuildSaveFilePath(fileName);
        FileName = fileName;
    }

    public static void DeleteSaveFile(string fileName)
    {
        File.Delete(@BuildSaveFilePath(fileName));
    }

    private static string BuildSaveFilePath(string fileName)
    {
        strBuilder.Clear();
        strBuilder.Append(SavesDirectory);
        strBuilder.Append("/");
        strBuilder.Append(fileName);
        strBuilder.Append(".xml");
        return strBuilder.ToString();
    }

    public static int GetSaveFileCount()
    {
        if (DirectoryExists())
        {
            try
            {
                if (FileName != "")
                {
                    return Directory.EnumerateFiles(SavesDirectory, FileName + "*.xml").Count();
                }
                else
                {
                    return Directory.EnumerateFiles(SavesDirectory, DefaultFileName + "*.xml").Count();
                }
            }
            catch
            {
                Debug.LogError("Failed to read directory at:\n" + SavesDirectory);
            }
        }
        return 0;
    }

    private static DataContractSerializer GetDataContract()
    {
        return new DataContractSerializer(typeof(TileGrid), new Type[] { typeof(RoadTile), typeof(BuildingTile), typeof(Tile) });
    }
  
}
