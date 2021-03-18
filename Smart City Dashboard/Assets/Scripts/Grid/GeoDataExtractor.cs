using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        //var (_, pointsOfInterest) = InterestPointMarking.SortIntoSets(foregroudMask);

        PixelType[][] pixelInfo = ExtractDataWithMask(forgroudMask, dataSource);

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

    private void FloodFillCheck()
    {

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
