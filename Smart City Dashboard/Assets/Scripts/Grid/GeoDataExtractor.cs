using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class GeoDataExtractor
{
    private readonly Texture2D dataSource;
    private readonly string name;

    private readonly Color maskColor = Color.black;
    private readonly Color replaceColor = Color.black;
    private readonly Color highlightColor = Color.blue;

    public GeoDataExtractor(Texture2D dataSource, string name)
    {
        this.dataSource = dataSource;
        this.name = name;
    }

    /// <summary>
    /// Extracts out the data from the image source
    /// </summary>
    public void Extract()
    {
        Color[] backgroundMaskColors = new[] { maskColor }; //Colors to be ignored
        bool[][] forgroudMask = TextureToBinary(dataSource, backgroundMaskColors); //Mask where every background index is false

        ZhangSuenThinning.ThinImage(forgroudMask); //Thins all paths down to minimally thin size
        //foregroud mask is now minimally small (with small issues)


        //Gets a hashset of all of the interesting points from the foreground mask
        var (_, pointsOfInterest) = InterestPointMarking.SortIntoSets(forgroudMask);

        PixelType[][] pixelInfo = ExtractDataWithMask(forgroudMask, dataSource);

        FloodFillCheck(pixelInfo, pointsOfInterest);

        TileGrid generatedGrid = CreateMap(forgroudMask, pixelInfo);

        StringBuilder strBuilder = new StringBuilder();
        strBuilder.Clear();
        strBuilder.Append(Application.dataPath);
        strBuilder.Append("/Saves/");
        strBuilder.Append(name);
        strBuilder.Append(".xml");
        SaveGameManager.SaveGame(strBuilder.ToString(), generatedGrid);  
    }

    private PixelType[][] ExtractDataWithMask(bool[][] image, Texture2D texture)
    {
        int width = texture.width;
        int height = texture.height;

        PixelType[][] output = new PixelType[height][];

        for (int row = 0; row < height; row++)
        {
            output[row] = new PixelType[height];
            for (int col = 0; col < width; col++)
            {
                if (image[row][col])
                {
                    Color color = texture.GetPixel(col, row);
                    if (color.r > 0) output[row][col] = PixelType.MainRoad;
                    else output[row][col] = PixelType.LocalRoad;
                }
                else
                {
                    output[row][col] = PixelType.None;
                }
            }
        }

        return output;
    }

    private TileGrid CreateMap(bool[][] image, PixelType[][] pixelInfo)
    {
        int height = image.Length;
        int width = image[0].Length;

        int densityOffsetter = 0;

        TileGrid grid = new TileGrid(width, height);

        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                PixelType location = pixelInfo[y][x];
                //When the pixel is blank, do nothing on this tile (for now)
                if (location == PixelType.None) continue;

                Vector2Int tilePos = new Vector2Int(x, y);
                grid[tilePos] = new RoadTile(true);
                if (location == PixelType.LocalRoad)
                {
                    TryPlacingHousesAround(tilePos, grid, pixelInfo, ref densityOffsetter);
                }
            }
        }

        return grid;
    }

    private void TryPlacingHousesAround(Vector2Int tilePos, TileGrid grid, PixelType[][] pixelInfo, ref int densityOffset)
    {
        foreach(Tile.Facing directions in Enum.GetValues(typeof(Tile.Facing)))
        {
            Vector2Int checkPos = tilePos + directions.ToVector2();
            if(!grid.Contains(checkPos) && grid.InBounds(checkPos.x, checkPos.y) && pixelInfo[checkPos.y][checkPos.x] == PixelType.None)
            {
                if(densityOffset == 0)
                {
                    grid[checkPos] = new BuildingTile(BuildingTile.StructureType.House, directions.Oppisite(), true);
                    densityOffset = UnityEngine.Random.Range(3, 6);
                }
                else
                {
                    densityOffset--;
                }
            }
        }
    }


    private PixelPathGraph FloodFillCheck(PixelType[][] pixelInfo, HashSet<Vector2Int> pointsOfInterest)
    {
        PixelPathGraph pathGraph = new PixelPathGraph();
        if (pointsOfInterest.Count == 0) return pathGraph;

        int height = pixelInfo.Length;
        int width = pixelInfo[0].Length;

        LinkedList<Vector2Int> unexploredPoI = new LinkedList<Vector2Int>(pointsOfInterest);

        List<Vector2Int> checkableDir = new List<Vector2Int>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(unexploredPoI.First.Value);
        pathGraph.AddNode(queue.Peek());

        while (queue.Count != 0) //Still has something to explore
        {
            var nodePos = queue.Dequeue();

            RepopulateCheckable(checkableDir);

            foreach(Tile.Facing direction in Enum.GetValues(typeof(Tile.Facing)))
            {
                Vector2Int checkPos = nodePos + direction.ToVector2();
                if (PixelExists(pixelInfo, checkPos, width, height) && pathGraph.IsNodeKnown(checkPos) is false)
                {
                    pathGraph.AddNode(checkPos);
                    pathGraph.ConnectNodes(checkPos, nodePos);
                    queue.Enqueue(checkPos);
                    
                    FoundDir(direction, checkableDir);
                }
                else NotFoundDir(direction, checkableDir);
            }

            foreach(Vector2Int remainingDirection in checkableDir)
            {
                Vector2Int checkPos = nodePos + remainingDirection;
                if (PixelExists(pixelInfo, checkPos, width, height) && pathGraph.IsNodeKnown(checkPos) is false)
                {
                    pathGraph.AddNode(checkPos);
                    pathGraph.ConnectNodes(checkPos, nodePos);
                    queue.Enqueue(checkPos);
                }
            }


            //The island from the last seed has been exhausted. Set next seed
            if(queue.Count == 0) 
            {
                while(unexploredPoI.Count > 0)
                {
                    if (pathGraph.IsNodeKnown(unexploredPoI.First.Value))
                    {
                        unexploredPoI.RemoveFirst();
                    }
                    else
                    {
                        queue.Enqueue(unexploredPoI.First.Value);
                        break;
                    }
                }
                //Try finding a new POI to look from
            }
        }

        return pathGraph;
    }

    private void RepopulateCheckable(List<Vector2Int> checkableDir)
    {
        checkableDir.Clear();
        checkableDir.Add(new Vector2Int(0, 1));
        checkableDir.Add(new Vector2Int(1, 1));
        checkableDir.Add(new Vector2Int(1, 0));
        checkableDir.Add(new Vector2Int(1, -1));
        checkableDir.Add(new Vector2Int(0, -1));
        checkableDir.Add(new Vector2Int(-1, -1));
        checkableDir.Add(new Vector2Int(-1, 0));
        checkableDir.Add(new Vector2Int(-1, 1));
    }

    private void FoundDir(Tile.Facing direction, List<Vector2Int> checkableDir)
    {
        Vector2Int vec = direction.ToVector2();
        if (direction.IsHorizontal()) checkableDir.RemoveAll(v => v.x == vec.x);
        else checkableDir.RemoveAll(v => v.y == vec.y);
    }

    private void NotFoundDir(Tile.Facing direction, List<Vector2Int> checkableDir)
    {
        checkableDir.Remove(direction.ToVector2());
    }

    private bool PixelExists(PixelType[][] pixelInfo, Vector2Int position, int width, int height)
    {
        if (position.x < 0 || position.x > width - 1 || position.y < 0 || position.y > height - 1) return false;
        return pixelInfo[position.y][position.x] != PixelType.None;
    }

    /// <summary>
    /// Creates a 2d boolean array and fills it with true for each pixel where the color does not match the mask
    /// </summary>
    private bool[][] TextureToBinary(Texture2D texture, Color[] maskedColors)
    {
        int width = texture.width;
        int height = texture.height;

        //Creates binary 2d array
        bool[][] binary = new bool[height][];
        for (int row = 0; row < height; row++) binary[row] = new bool[width];
        
        //Fills binary 2d array
        TextureToBinary(texture, maskedColors, binary);

        return binary;
    }

    /// <summary>
    /// Fills target array with true for each pixel where the color does not match the mask
    /// </summary>
    private void TextureToBinary(Texture2D texture, Color[] maskedColors, bool[][] target)
    {
        int width = texture.width;
        int height = texture.height;

        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                //Compares color on source with mask of colors to ignore
                target[row][col] = !MatchMask(texture.GetPixel(col, row), maskedColors);
            }
        }
    }

    /// <summary>
    /// Compares a color against a list of colors. True if match
    /// </summary>
    private bool MatchMask(Color test, Color[] mask)
    {
        foreach (Color m in mask)
        {
            if (test == m) return true;
        }
        return false;
    }

    private enum PixelType
    {
        None,
        MainRoad,
        LocalRoad
    }
}
