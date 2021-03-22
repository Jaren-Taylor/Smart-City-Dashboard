using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class APIKey
{
    private static readonly string keyPath = Application.persistentDataPath + "/APIKey.txt";
    private static string storedKey = "";

    public static bool HasKey() => TryGetKey(out storedKey);

    public static void ClearKeyCache() => storedKey = "";

    public static bool TrySaveAPIKey(string key)
    {
        storedKey = key;
        try
        {
            using StreamWriter stream = new StreamWriter(File.Open(keyPath, FileMode.Create));
            stream.WriteLine(key);
            return true;
        }
        catch (IOException)
        {
            return false;
        }
    }

    public static bool TryGetKey(out string key)
    {
        if (string.IsNullOrEmpty(storedKey))
        {
            if(TryLoadingKey(out key))
            {
                storedKey = key;
                return true;
            }
            else return false;
        }
        else
        {
            key = storedKey;
            return true;
        }
    }

    public static bool IsKeyValid(string key)
    {
        if (string.IsNullOrEmpty(key)) return false;
        else return true;
    }

    public static bool IsAPIKeyValid() => IsKeyValid(storedKey);

    private static bool TryLoadingKey(out string key)
    {
        if (File.Exists(keyPath))
        {
            try
            {
                key = System.IO.File.ReadAllText(keyPath);
            }
            catch (IOException)
            {
                key = "";
                return false;
            }
            return true;
        }
        else
        {
            key = "";
            return false;
        }
    }


}
