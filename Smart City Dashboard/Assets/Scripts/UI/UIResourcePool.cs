using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UIResourcePool
{
    private static readonly Dictionary<UIBackgroundSprite, Sprite> BackgroundSprites = new Dictionary<UIBackgroundSprite, Sprite>();

    public static Sprite GetBackgroundSprite(UIBackgroundSprite backgroundSprite)
    {
        if(BackgroundSprites.TryGetValue(backgroundSprite, out var output))
        {
            return output;
        }
        else
        {
            BackgroundSprites.Add(backgroundSprite, Resources.Load<Sprite>(backgroundSprite.GetAddress()));
            return BackgroundSprites[backgroundSprite];
        }
    }
}
