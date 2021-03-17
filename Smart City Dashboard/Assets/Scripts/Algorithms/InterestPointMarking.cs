using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InterestPointMarking
{
    public static void RemoveUnintersting(bool[][] image)
    {
        int height = image.Length;
        int width = image[0].Length;

        bool[] neighborSequenceData = new bool[9];

        bool any = false;

        List<Vector2Int> markedPositions = new List<Vector2Int>();

        for (int row = 1; row < height - 1; row++)
        {
            for (int col = 1; col < width - 1; col++)
            {
                //If pixel color is part of background mask, skip
                if (!image[row][col]) continue;

                //If the point is not interesting it is removed from the image
                if (!MatchesInterestPattern(image, row, col, neighborSequenceData))
                {
                    markedPositions.Add(new Vector2Int(row, col));
                }
            }
        }

        foreach (Vector2Int marked in markedPositions)
        {
            image[marked.x][marked.y] = false;
        }
    }

    private static bool MatchesInterestPattern(bool[][] image, int row, int col, bool[] sequenceData)
    {
        ZhangSuenThinning.PopulateNeighborSequence(image, sequenceData, row, col);

        int transitions = ZhangSuenThinning.TransitionsFromFalseToTrue(sequenceData, 1);
        int numTrue = ZhangSuenThinning.NumberOfTrue(sequenceData, 1);

        return transitions > 2 || (transitions == 1 && numTrue == 1); 
    }
}
