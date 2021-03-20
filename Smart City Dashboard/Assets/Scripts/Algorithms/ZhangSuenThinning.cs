using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ZhangSuenThinning 
{
    /*  Image Origin is BOTTOM LEFT
     *  
     *  Pixels are denoted by this chart
     *   __________
     *  |P9  P2  P3|
     *  |P8  P1  P4|
     *  |P7  P6  P5|
     *   ----------
     * 
     * Stage 1:
     * All pixels are tested and are marked if they meet all following criteria:
     *      -The pixel (P1) is true and has eight neighbors
     *      -Has between [2,6] neighbors that are true
     *      -TransitionsFromFalseToTrue == 1
     *      -At least one of P2 and P4 and P6 is false
     *      -At least one of P4 and P6 and P8 is false
     * After collecting all pixels that satify ALL of the above requirements, they can be set to false
     * 
     * Stage 2:
     * All pixels are tested again and are marked if they meet all following criteria: (note, the first 3 criteria are the same, 4/5 are slightly altered)
     *      -The pixel (P1) is true and has eight neighbors
     *      -Has between [2,6] neighbors that are true
     *      -TransitionsFromFalseToTrue == 1
     *      -At least one of P2 and P4 and P8 is false
     *      -At least one of P2 and P6 and P8 is false
     * After collecting all pixels that satify ALL of the above requirements, they can be set to false
     * 
     * Repeat Stage 1 and Stage 2 again until no pixels are changed after the two steps
     */


    private static readonly int[] stage1step4 = { 1, 3, 5 };
    private static readonly int[] stage1step5 = { 3, 5, 7 };

    private static readonly int[] stage2step4 = { 1, 3, 7 };
    private static readonly int[] stage2step5 = { 1, 5, 7 };

    public static readonly int[][] SequenceRelations = { 
        new[] { 0, 0 }, //P1
        new[] { 0, 1 }, //P2
        new[] { 1, 1 }, //P3
        new[] { 1, 0 }, //P4
        new[] { 1,-1 }, //P5
        new[] { 0,-1 }, //P6
        new[] {-1,-1 }, //P7
        new[] {-1, 0 }, //P8
        new[] {-1, 1 }  //P9
    };

    /// <summary>
    /// Takes a list of booleans and counts the number of transitions from false to true. 
    /// Note, it also checks the transition from the last index to the first one.
    /// </summary>
    /// <param name="sequence"></param>
    /// <returns></returns>
    public static int TransitionsFromFalseToTrue(bool[] sequence, int startingIndex) //A
    {
        int output = 0;
        for(int i = startingIndex; i < sequence.Length - 1; i++)
        {
            if (!sequence[i] && sequence[i + 1]) 
                output++;
        }
        if (!sequence[sequence.Length - 1] && sequence[startingIndex]) output++;

        return output;
    }

    /// <summary>
    /// Counts the number of True's found in the sequence
    /// </summary>
    /// <param name="sequence"></param>
    /// <returns></returns>
    public static int NumberOfTrue(bool[] sequence, int startingIndex) //B
    {
        int output = 0;
        for(int i = startingIndex; i < sequence.Length; i++) if (sequence[i]) output++;
        return output;
    }

    /// <summary>
    /// Takes a neighbor sequence and checks if any of the indices that are in the mask are false. 
    /// </summary>
    /// <param name="sequence"></param>
    /// <param name="mask"></param>
    /// <returns></returns>
    private static bool AtLeastOneIsFalse(bool[] sequence, int[] mask)
    {
        foreach(int index in mask)
        {
            if (sequence[index] is false) return true;
        }
        return false;
    }

    public static void ThinImage(bool[][] image)
    {
        int maxIter = 100;
        int iter = 0;

        bool isStep1 = false; //Initialized as false to allow entry into loop to be correct
        bool hasChanged = false;

        bool[] checkingSequence = new bool[9];
        List<Vector2Int> markedPositions = new List<Vector2Int>();

        int height = image.Length;
        int width = image[0].Length;

        do
        {
            isStep1 = !isStep1; //Toggles is step 1 every loop to 'count' from step 1 to step 2
            if (isStep1)
            {
                hasChanged = false;
                iter++;
            }

            for (int row = 1; row < height - 1; row++)
            {
                for (int col = 1; col < width - 1; col++)
                {
                    if(row == 4 && col == 80)
                    {
                        int i = 420;
                    }

                    if (image[row][col] is false) continue; //First check (common to both stages)

                    PopulateNeighborSequence(image, checkingSequence, row, col); //Fills the checking sequence array the 3x3 values show in documents

                    int trueNeigh = NumberOfTrue(checkingSequence, 1); 
                    if (trueNeigh < 2 || trueNeigh > 6) continue; //Second check (common to both stages)

                    if (TransitionsFromFalseToTrue(checkingSequence, 1) != 1) continue; //Third check (common to both stages)

                    if (StepSpecificChecks(checkingSequence, isStep1) is false) continue; // Fourth and Fifth checks (unique to the current stage)

                    hasChanged = true;
                    markedPositions.Add(new Vector2Int(row, col));
                }
            }

            foreach(Vector2Int marked in markedPositions)
            {
                image[marked.x][marked.y] = false;
            }

            markedPositions.Clear();
        }
        while ((isStep1 || hasChanged) && (iter < maxIter));

        if (iter >= maxIter) Debug.LogError("Max iter reached");
    }

    public static void PopulateNeighborSequence(bool[][] image, bool[] sequence, int row, int col, bool safeCheck = false)
    {
        if (safeCheck)
        {
            int height = image.Length;
            int width = image[0].Length;

            for (int i = 0; i < 9; i++)
            {
                int checkRow = row + SequenceRelations[i][1];
                int checkCol = col + SequenceRelations[i][0];
                if (checkRow < 0 || checkRow >= height || checkCol < 0 || checkCol >= width)
                {
                    sequence[i] = false;
                    continue;
                }
                //Row is analogous to the y coord and Col is x coord
                sequence[i] = image[checkRow][checkCol];
            }
        }
        else
        {
            for (int i = 0; i < 9; i++)
            {
                //Row is analogous to the y coord and Col is x coord
                sequence[i] = image[row + SequenceRelations[i][1]][col + SequenceRelations[i][0]];
            }
        }
    }

    private static bool Stage1(bool[][] image)
    {
        bool hasChanged = false;


        return hasChanged;
    }


    private static bool StepSpecificChecks(bool[] sequence, bool isStage1)
    {
        if (isStage1) return Stage1SpecificChecks(sequence);
        else return Stage2SpecificChecks(sequence);
    }

    private static bool Stage1SpecificChecks(bool[] sequence)
    {
        if (AtLeastOneIsFalse(sequence, stage1step4) is false) return false;
        if (AtLeastOneIsFalse(sequence, stage1step5) is false) return false;
        return true;
    }

    private static bool Stage2SpecificChecks(bool[] sequence)
    {
        if (AtLeastOneIsFalse(sequence, stage2step4) is false) return false;
        if (AtLeastOneIsFalse(sequence, stage2step5) is false) return false;
        return true;
    }

}
