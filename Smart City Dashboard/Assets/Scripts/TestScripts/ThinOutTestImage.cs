using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThinOutTestImage : MonoBehaviour
{
    public Image source;
    public Image target;
    public Image interest;

    private readonly Color maskColor = Color.black;
    private readonly Color replaceColor = Color.black;
    private readonly Color highlightColor = Color.blue;

    // Start is called before the first frame update
    void Start()
    {

        var extract = new GeoDataExtractor(source.sprite.texture, this.gameObject.name);

        var graph = extract.Extract();

        var nodeTexture = DrawGraphToTexture(graph, source.sprite.texture.width, source.sprite.texture.height, true);

        var moneyAng = extract.GetMoneyShot(graph);


        var rasterTexture = extract.DrawToTextureAtAngle(graph, nodeTexture.width, nodeTexture.height, moneyAng);

        Debug.Log(this.gameObject.name + " : ang = " + moneyAng);

        var backgroundMask = new[] { maskColor };

        var forgroundImage = TextureToBinary(source.sprite.texture, backgroundMask);

        ZhangSuenThinning.ThinImage(forgroundImage);

        var thinnedTexture = CopyWithMask(source.sprite.texture, forgroundImage, replaceColor, false);

        source.sprite = Sprite.Create(
            thinnedTexture,
            new Rect(0, 0, thinnedTexture.width, thinnedTexture.height),
            new Vector2(0, 1));

        target.sprite = Sprite.Create(
            rasterTexture,
            new Rect(0, 0, rasterTexture.width, rasterTexture.height),
            new Vector2(0, 1));

        interest.sprite = Sprite.Create(
           nodeTexture,
           new Rect(0, 0, nodeTexture.width, nodeTexture.height),
           new Vector2(0, 1));


        extract.WriteGraphToFile(this.gameObject.name, rasterTexture);
    }


    private Texture2D DrawGraphConnectionsToTexture(PixelPathGraph graph, int width, int height)
    {
        Texture2D newTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        newTexture.filterMode = FilterMode.Point;

        foreach(var nodePosition in graph.GetPointsOfInterest())
        {
            graph.TryGetPixelType(nodePosition, out var currentPixel);
            graph.TryGetConnections(nodePosition, out var connections);
            foreach (var connection in connections)
            {
                
            }
        }

        newTexture.Apply();

        return newTexture;
    }


    private Texture2D DrawGraphToTexture(PixelPathGraph graph, int width, int height, bool highlightMode = true)
    {
        Texture2D newTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        newTexture.filterMode = FilterMode.Point;

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                var pos = new Vector2Int(x, y);
                if (graph.TryGetPixelType(pos, out GeoDataExtractor.PixelType type) && type != GeoDataExtractor.PixelType.None)
                {
                    if (highlightMode)
                    {
                        if(!graph.IsNodeOfInterest(pos))
                        {
                            newTexture.SetPixel(x, y, (type == GeoDataExtractor.PixelType.LocalRoad) ? Color.green : Color.red);
                            continue;
                        }
                        else newTexture.SetPixel(x, y, Color.white);
                    }
                    else newTexture.SetPixel(x, y, (type == GeoDataExtractor.PixelType.LocalRoad) ? Color.green : Color.red);
                }
                else newTexture.SetPixel(x, y, Color.black);
            }
        }

        newTexture.Apply();

        return newTexture;
    }

    private Texture2D CopyTexture(Texture2D texture)
    {
        Texture2D newTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
        newTexture.filterMode = FilterMode.Point;

        newTexture.SetPixels32(texture.GetPixels32());

        return newTexture;
    }

    private Texture2D CopyWithMask(Texture2D texture, bool[][] mask, Color maskColor, bool invert)
    {
        Texture2D newTexture = CopyTexture(texture);

        int width = texture.width;
        int height = texture.height;

        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                if (mask[row][col] == !invert)
                {
                    var color = texture.GetPixel(col, row);
                    if (color.r != 0) color.r = 1;
                    if (color.g != 0) color.g = 1;
                    if (color.b != 0) color.b = 1;
                    newTexture.SetPixel(col, row, color);
                }
                else
                {
                    newTexture.SetPixel(col, row, maskColor);
                }
            }
        }

        newTexture.Apply();

        return newTexture;
    }

    private bool[][] TextureToBinary(Texture2D texture, Color[] maskedColors)
    {
        int width = texture.width;
        int height = texture.height;

        bool[][] binary = new bool[height][];

        for(int row = 0; row < height; row++)
        {
            binary[row] = new bool[width];
        }

        TextureToBinary(texture, maskedColors, binary);

        return binary;
    }

    private void TextureToBinary(Texture2D texture, Color[] maskedColors, bool[][] target)
    {
        int width = texture.width;
        int height = texture.height;

        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                target[row][col] = !MatchMask(texture.GetPixel(col, row), maskedColors);
            }
        }
    }

    private bool MatchMask(Color test, Color[] mask)
    {
        foreach (Color m in mask)
        {
            if (test == m) return true;
        }
        return false;
    }
}
