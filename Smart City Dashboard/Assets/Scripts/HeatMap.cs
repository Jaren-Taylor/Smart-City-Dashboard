using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HeatMap 
{
    public readonly int Width;
    public readonly int Height;

    private const int DEGRADE_RATE = 32;
    private const int INCREASE_RATE = 64;

    private Texture2D texture;

    private readonly Color32[] emptyMap;

    private Dictionary<int, Color32> ColorMap = new Dictionary<int, Color32>()
    {
        {0, Color.green },
        {1, Color.yellow },
        {2, new Color(1, .6f, 0, 1) },
        {3, new Color(1, .3f, 0, 1) },
        {4, Color.red }
    };

    private readonly Dictionary<Vector2Int, int> trackedHeat;

    public HeatMap(int width, int height)
    {
        this.Width = width;
        this.Height = height;
        this.texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        this.texture.filterMode = FilterMode.Point;
        this.trackedHeat = new Dictionary<Vector2Int, int>();
        this.emptyMap = new Color32[Width * Height];
        FillEmptyMap();
    }


    public void Update(List<Vector2Int> positions)
    {
        ApplyData(positions);
        DegradeMap();
    }

    public void ClearMap()
    {
        trackedHeat.Clear();
    }

    public Texture2D CreatePNG()
    {
        texture.SetPixels32(emptyMap, 0);
        foreach (Vector2Int position in trackedHeat.Keys) 
            texture.SetPixel(position.x, position.y, GetColor(trackedHeat[position]));
        texture.Apply();
        return texture;
    }

    private void ApplyData(List<Vector2Int> positions)
    {
        foreach (Vector2Int position in positions)
        {
            if (InBounds(position))
            {
                ModifyHeat(position, INCREASE_RATE);
            }
        }
    }

    private void FillEmptyMap()
    {
        for (int i = 0; i < Width * Height; i++) { emptyMap[i] = ColorMap[0]; }
    }

    private void ModifyHeat(Vector2Int position, int delta)
    {
        if (trackedHeat.ContainsKey(position)) trackedHeat[position] += delta;
        else trackedHeat.Add(position, delta);
        ClampTrackedHeat(position);
    }

    private void ClampTrackedHeat(Vector2Int position)
    {
        var value = trackedHeat[position];
        if (value > 255) trackedHeat[position] = 255;
        else if (value <= 0) trackedHeat.Remove(position);
    }
    
    private void DegradeMap()
    {
        var keys = trackedHeat.Keys.ToArray();
        foreach (Vector2Int position in keys)
        {
            ModifyHeat(position, -DEGRADE_RATE);
        }
    }

    private bool InBounds(Vector2Int position) => position.x >= 0 && position.x < Width && position.y >= 0 && position.y < Height;
    private Color32 GetColor(int value) => ColorMap[Mathf.CeilToInt(value / 63.75f)]; 
}
