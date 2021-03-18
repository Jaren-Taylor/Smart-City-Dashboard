using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InterestPointMarking
{
    public static HashSet<Vector2Int> GetUninteresting(bool[][] image)
    {
        int height = image.Length;
        int width = image[0].Length;

        bool[] neighborSequenceData = new bool[9];

        HashSet<Vector2Int> markedPositions = new HashSet<Vector2Int>();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {

                //If pixel color is part of background mask, skip
                if (!image[y][x]) continue;

                //If the point is not interesting it is removed from the image
                if (!MatchesInterestPattern(image, y, x, neighborSequenceData, (y == 0 || x == 0 || y == height - 1 || x == width - 1)))
                {
                    markedPositions.Add(new Vector2Int(x, y));
                }
            }
        }

        return markedPositions;
    }

    public static void RemoveUnintersting(bool[][] image)
    {
        var markedPositions = GetUninteresting(image);

        foreach (Vector2Int marked in markedPositions)
        {
            image[marked.y][marked.x] = false; //y and x are backwards on purpose. Bool array is indexed [y][x]
        }
    }

    private static bool MatchesInterestPattern(bool[][] image, int row, int col, bool[] sequenceData, bool safeCheck)
    {
        ZhangSuenThinning.PopulateNeighborSequence(image, sequenceData, row, col, safeCheck);

        int numTrue = ZhangSuenThinning.NumberOfTrue(sequenceData, 1);
        int transitions = ZhangSuenThinning.TransitionsFromFalseToTrue(sequenceData, 1);

        return transitions > 2 || (transitions == 1 && numTrue <= 2) || numTrue > 4; 
    }
}
