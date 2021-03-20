using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using System.Net;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.UI;
using NUnit.Framework;

public class GoogleMapsTestQuery : MonoBehaviour
{
    static readonly HttpClient Http = new HttpClient();
    public Texture2D texture;
    public Image picture;
    private string apikey;
    private bool KeyCreated = false; 

    private void Update()
    {
        //crude quickfix
        if(KeyCreated) StartCoroutine(GetTexture(CreateQuery(apikey)));
    }

    /// <summary>
    /// Given a NxN size, an integer based zoom, a location, and an apikey it will return a Google Maps Static Map in the form of a texture
    /// </summary>
    /// <param name="size"></param>
    /// <param name="zoom"></param>
    /// <param name="center"></param>
    /// <returns></returns>
    static string CreateQuery(string size, string zoom, string center, string apikey)
    {

        center = center.Replace(" ", "+").Replace(",", "+");
        return $"https://maps.googleapis.com/maps/api/staticmap?size={size}&zoom={zoom}&center={center}&style=feature:landscape|color:0x000000&style=element:labels|invert_lightness:true&style=feature:road|color:0x880000&style=feature:road.local|geometry|color:0x008800&style=feature:road|weight:1&style=feature:road|element:labels|visibility:off&style=feature:poi|element:labels|visibility:off&style=feature:poi|geometry|color:0x880000&style=feature:transit|visibility:off&style=feature:water|visibility:off&style=feature:administrative|visibility:off&key={apikey}";
    }

    /// <summary>
    /// returns a basic static map of saint clair shores for testing purposes
    /// </summary>
    /// <returns></returns>
    static string CreateQuery(string apikey)
    {
        Debug.Log($"https://maps.googleapis.com/maps/api/staticmap?size=460x460&zoom=16&center=Saint+Clair+Shores+Michigan&style=feature:landscape|color:0x000000&style=element:labels|invert_lightness:true&style=feature:road|color:0x880000&style=feature:road.local|geometry|color:0x008800&style=feature:road|weight:1&style=feature:road|element:labels|visibility:off&style=feature:poi|element:labels|visibility:off&style=feature:poi|geometry|color:0x880000&style=feature:transit|visibility:off&style=feature:water|visibility:off&style=feature:administrative|visibility:off&key={apikey}"
    );
        return $"https://maps.googleapis.com/maps/api/staticmap?size=460x460&zoom=16&center=Saint+Clair+Shores+Michigan&style=feature:landscape|color:0x000000&style=element:labels|invert_lightness:true&style=feature:road|color:0x880000&style=feature:road.local|geometry|color:0x008800&style=feature:road|weight:1&style=feature:road|element:labels|visibility:off&style=feature:poi|element:labels|visibility:off&style=feature:poi|geometry|color:0x880000&style=feature:transit|visibility:off&style=feature:water|visibility:off&style=feature:administrative|visibility:off&key={apikey}";
    }


    public IEnumerator GetTexture(string uri)
    {
        KeyCreated = false;
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(uri);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
           texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
           picture.sprite = Sprite.Create(texture, new Rect(0,0,texture.width, texture.height) , Vector2.zero);
        }
    }

    public void SetApiKey(string input)
    {
        apikey = input;
        Debug.Log(apikey);
        KeyCreated = true;
    }

}
