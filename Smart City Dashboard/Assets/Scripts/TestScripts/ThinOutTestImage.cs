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

        extract.Extract();

        Debug.Log(this.gameObject.name + ": Completed");


        /*
        var backgroundMask = new[] { maskColor };

        var forgroundImage = TextureToBinary(source.sprite.texture, backgroundMask);

        ZhangSuenThinning.ThinImage(forgroundImage);

        var thinnedTexture = CopyWithMask(source.sprite.texture, forgroundImage, replaceColor, false);

        target.sprite = Sprite.Create(
            thinnedTexture,
            new Rect(0, 0, thinnedTexture.width, thinnedTexture.height),
            new Vector2(0, 1));

        
        InterestPointMarking.RemoveUnintersting(forgroundImage);

        var highlightedTexture = CopyWithMask(thinnedTexture, forgroundImage, Color.white, true);

        interest.sprite = Sprite.Create(
            highlightedTexture,
            new Rect(0, 0, highlightedTexture.width, highlightedTexture.height),
            new Vector2(0, 1));*/
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
