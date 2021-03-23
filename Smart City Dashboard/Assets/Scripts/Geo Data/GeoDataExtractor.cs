using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public static class GeoDataExtractor
{
    private static readonly Color maskColor = Color.black;


    public static (TileGrid grid, Texture2D texture) ExtractTileMap(Texture2D dataSource)
    {
        var graph = ExtractPixelGraph(dataSource);
        var moneyAng = GetMoneyShot(graph);
        var rasterTexture = DrawToTextureAtAngle(graph, dataSource.width, dataSource.height, moneyAng);

        return (CreateMap(rasterTexture), rasterTexture);
    }



    /// <summary>
    /// Extracts out the data from the image source
    /// </summary>
    public static PixelPathGraph ExtractPixelGraph(Texture2D dataSource)
    {
        Color[] backgroundMaskColors = new[] { maskColor }; //Colors to be ignored
        bool[][] forgroudMask = TextureToBinary(dataSource, backgroundMaskColors); //Mask where every background index is false

        ZhangSuenThinning.ThinImage(forgroudMask); //Thins all paths down to minimally thin size
        //foregroud mask is now minimally small (with small issues)


        //Gets a hashset of all of the interesting points from the foreground mask
        var (_, pointsOfInterest) = InterestPointMarking.SortIntoSets(forgroudMask);

        PixelType[][] pixelInfo = ExtractDataWithMask(forgroudMask, dataSource);

        PixelPathGraph graph = FloodFillCheck(pixelInfo, pointsOfInterest);

        CollapseBetweenPoI(graph);

        CollapseSmallLoops(graph);

        return graph;
    }

    public static void WriteGraphToFile(string fileName, Texture2D mapTexture)
    {
        TileGrid generatedGrid = CreateMap(mapTexture);
        SaveGameManager.WriteMapToFile(fileName, generatedGrid);
    }

    private static PixelType[][] ExtractDataWithMask(bool[][] image, Texture2D texture)
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

    private static TileGrid CreateMap(Texture2D mapTexture)
    {
        int height = mapTexture.height;
        int width = mapTexture.width;

        int densityOffsetter = 0;

        TileGrid grid = new TileGrid(width, height);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color pixelColor = mapTexture.GetPixel(x,y);
                //When the pixel is blank, do nothing on this tile (for now)
                if (pixelColor == Color.black) continue;

                Vector2Int tilePos = new Vector2Int(x, y);
                grid[tilePos] = new RoadTile(true);
                if (pixelColor == Color.green)
                {
                    TryPlacingHousesAround(tilePos, grid, mapTexture, ref densityOffsetter);
                }
            }
        }

        return grid;
    }

    private static void TryPlacingHousesAround(Vector2Int tilePos, TileGrid grid, Texture2D mapTexture, ref int densityOffset)
    {
        foreach (Tile.Facing directions in Enum.GetValues(typeof(Tile.Facing)))
        {
            Vector2Int checkPos = tilePos + directions.ToVector2();
            if (!grid.Contains(checkPos) && grid.InBounds(checkPos.x, checkPos.y) && mapTexture.GetPixel(checkPos.x, checkPos.y) == Color.black)
            {
                if (densityOffset == 0)
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

    private static TileGrid CreateMap(bool[][] image, PixelType[][] pixelInfo)
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

    private static void TryPlacingHousesAround(Vector2Int tilePos, TileGrid grid, PixelType[][] pixelInfo, ref int densityOffset)
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

    private static PixelPathGraph FloodFillCheck(PixelType[][] pixelInfo, HashSet<Vector2Int> pointsOfInterest)
    {
        PixelPathGraph pathGraph = new PixelPathGraph();
        if (pointsOfInterest.Count == 0) return pathGraph;

        int height = pixelInfo.Length;
        int width = pixelInfo[0].Length;

        LinkedList<Vector2Int> unexploredPoI = new LinkedList<Vector2Int>(pointsOfInterest);

        List<Vector2Int> checkableDir = new List<Vector2Int>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(unexploredPoI.First.Value);
        pathGraph.AddNode(queue.Peek(), pixelInfo[queue.Peek().y][queue.Peek().x]);

        while (queue.Count != 0) //Still has something to explore
        {
            var nodePos = queue.Dequeue();

            RepopulateCheckable(checkableDir);

            foreach(Tile.Facing direction in Enum.GetValues(typeof(Tile.Facing)))
            {
                Vector2Int checkPos = nodePos + direction.ToVector2();
                if (PixelExists(pixelInfo, checkPos, width, height, out var pixel))
                {
                    if(!pathGraph.IsConnected(nodePos, checkPos))
                    {
                        if (pathGraph.DoesNodeExist(checkPos) is false)
                        {
                            pathGraph.AddNode(checkPos, pixel);
                            queue.Enqueue(checkPos);
                        }
                        pathGraph.ConnectNodes(checkPos, nodePos);
                        FoundDir(direction, checkableDir);
                    }
                    else FoundDir(direction, checkableDir);
                }
                else NotFoundDir(direction, checkableDir);
            }

            foreach(Vector2Int remainingDirection in checkableDir)
            {
                Vector2Int checkPos = nodePos + remainingDirection;
                if (PixelExists(pixelInfo, checkPos, width, height, out var pixel))
                {
                    if (!pathGraph.IsConnected(nodePos, checkPos))
                    {
                        if (pathGraph.DoesNodeExist(checkPos) is false)
                        {
                            pathGraph.AddNode(checkPos, pixel);
                            queue.Enqueue(checkPos);
                        }
                        pathGraph.ConnectNodes(checkPos, nodePos);
                    }
                }
            }


            //The island from the last seed has been exhausted. Set next seed
            if(queue.Count == 0) 
            {
                while(unexploredPoI.Count > 0)
                {
                    var position = unexploredPoI.First.Value;

                    if (pathGraph.DoesNodeExist(position))
                    {
                        unexploredPoI.RemoveFirst();
                    }
                    else
                    {
                        queue.Enqueue(position);
                        pathGraph.AddNode(position, pixelInfo[position.y][position.x]);
                        break;
                    }
                }
                //Try finding a new POI to look from
            }
        }

        return pathGraph;
    }


    private static void CollapseSmallLoops(PixelPathGraph graph)
    {
        var changes = false;
        do
        {
            LinkedList<Vector2Int> unexploredPoI = new LinkedList<Vector2Int>(graph.GetPointsOfInterest());
            graph.ClearVisited();
            changes = false;

            LinkedList<(Vector2Int from, Vector2Int to)> collapseFromTo = new LinkedList<(Vector2Int from, Vector2Int to)>();

            Dictionary<Vector2Int, Vector2Int> collapseMapping = new Dictionary<Vector2Int, Vector2Int>();

            var exploringNode = unexploredPoI.First;
            while(exploringNode != null)
            {
                if (graph.DoesNodeExist(exploringNode.Value))
                {
                    graph.MarkVisited(exploringNode.Value);

                    var connCopy = new HashSet<Vector2Int> (graph.GetConnections(exploringNode.Value));

                    foreach (var con in connCopy)
                    {
                        if (!graph.IsNodeOfInterest(exploringNode.Value)) break;
                        if (graph.IsConnected(exploringNode.Value, con) 
                            && graph.IsNodeOfInterest(con) 
                            && Vector2.Distance(exploringNode.Value, con) <= 1)
                        {
                            //graph.CollapseConnection(con, exploringNode.Value);
                            collapseFromTo.AddLast((con, exploringNode.Value));
                            changes = true;
                        }
                    }
                }

               exploringNode = exploringNode.Next;
            }

            foreach(var node in collapseFromTo)
            {
                var (from, to) = node;
                if(graph.DoesNodeExist(from) && graph.DoesNodeExist(to) && graph.IsConnected(from, to))
                {
                    graph.CollapseConnection(from, to);
                }
            }

            collapseFromTo.Clear();
            collapseMapping.Clear();
        }
        while (changes);
    }

    private static void CollapseBetweenPoI(PixelPathGraph graph)
    {
        LinkedList<Vector2Int> unexploredPoI = new LinkedList<Vector2Int>(graph.GetPointsOfInterest());

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(unexploredPoI.First.Value);
        
        graph.ClearVisited();
        graph.MarkVisited(unexploredPoI.First.Value);

        int counter = 0;

        while (queue.Count != 0 && counter < 100000)
        {
            var nodePos = queue.Dequeue();
            counter++;

            if (graph.TryGetConnections(nodePos, out var connections))
            {
                if(connections.Count == 2)
                {

                    var nodes = new List<Vector2Int>(connections);
                    bool isCollinear = nodePos.IsCollinearWith(nodes[0], nodes[1]);

                    if (isCollinear)
                    {
                        graph.CollapseConnection(nodePos, nodes[0]);
                    }

                    foreach(var connectedNode in connections)
                    {
                        if (!graph.IsVisited(connectedNode))
                        {
                            graph.MarkVisited(connectedNode);
                            queue.Enqueue(connectedNode);
                        } 
                    }
                }
                else
                {
                    foreach (var connectedNode in connections)
                    {
                        if (!graph.IsVisited(connectedNode))
                        {
                            graph.MarkVisited(connectedNode);
                            queue.Enqueue(connectedNode);
                        }
                    }
                }
            }

            if(queue.Count == 0)
            {
                while (unexploredPoI.Count > 0)
                {
                    var position = unexploredPoI.First.Value;

                    if (graph.IsVisited(position))
                    {
                        unexploredPoI.RemoveFirst();
                    }
                    else
                    {
                        graph.MarkVisited(position);
                        queue.Enqueue(position);
                        break;
                    }
                }
            }
        }
        if (counter == 100000)
        {
            throw new Exception("Failed to get remove collinear points under timer");
        }

        graph.ClearVisited();

        foreach(var centerPoint in graph.GetPointsOfInterest())
        {
            var connCopy = graph.GetConnections(centerPoint).ToList();
            foreach (var connectedPoint in connCopy)
            {
                if(graph.DoesNodeExist(connectedPoint)) CleanConnectionUntilNextPoI(centerPoint, connectedPoint, graph);
            }
        }
    }

    private static void CleanConnectionUntilNextPoI(Vector2Int currentPoI, Vector2Int connectedPoint, PixelPathGraph graph)
    {
        LinkedList<Vector2Int> markersUntilNextPoI = new LinkedList<Vector2Int>();
        markersUntilNextPoI.AddFirst(currentPoI);
        markersUntilNextPoI.AddLast(connectedPoint);
        while (!graph.IsNodeOfInterest(markersUntilNextPoI.Last.Value))
        {
            markersUntilNextPoI.AddLast(GetNextPointOn2WayConnection(markersUntilNextPoI.Last.Previous.Value, markersUntilNextPoI.Last.Value, graph));
        }

        //Markers list now contains all points up to and including the next poi

        var checkNode = markersUntilNextPoI.First.Next;

        LinkedList<Vector2Int> collapseCheck = new LinkedList<Vector2Int>();

        //float runningDistanceTotal = 0;

        LinkedList<Vector2Int> absorbedPoints = new LinkedList<Vector2Int>();

        while (!graph.IsNodeOfInterest(checkNode.Value) && checkNode.Next != null)
        {
            float distanceToLine = DistanceToLine(checkNode.Value, checkNode.Previous.Value, checkNode.Next.Value);

            float averageOfDistances = 0;
            if(absorbedPoints.Count != 0)
            {
                foreach(var node in absorbedPoints)
                {
                    averageOfDistances += DistanceToLine(node, checkNode.Previous.Value, checkNode.Next.Value);
                }

                averageOfDistances /= absorbedPoints.Count;
            }
            //runningDistanceTotal += distanceToLine;
            //if (distanceToLine.IsBetween(-1, 1))
            if(distanceToLine < 1 && (absorbedPoints.Count == 0 || averageOfDistances < 1))
            {
                //Remove point
                graph.CollapseConnection(checkNode.Value, checkNode.Next.Value);
                checkNode = checkNode.Next;
                absorbedPoints.AddLast(checkNode.Previous.Value);
                markersUntilNextPoI.Remove(checkNode.Previous);
            }
            else
            {
                checkNode = checkNode.Next;
                absorbedPoints.Clear();
                //runningDistanceTotal = 0f;
            }
        }


    }

    private static float DistanceToLine(Vector2 point, Vector2 lineP1, Vector2 lineP2)
    {
        return point.DistanceToLine(lineP1, lineP2);
    }

    private static float SignedDistanceToLine(Vector2 point, Vector2 lineP1, Vector2 lineP2)
    {
        return point.SignedDistanceToLine(lineP1, lineP2);
    }

    private static Vector2Int GetNextPointOn2WayConnection(Vector2Int previousPoint, Vector2Int currentPoint, PixelPathGraph graph)
    {
        var connection = graph.GetConnections(currentPoint).ToList();
        return (connection[0] == previousPoint) ? connection[1] : connection[0];
    }

    public static float GetMoneyShot(PixelPathGraph graph)
    {
        List<(int sqrLength, float angle)> angleSqrLength = new List<(int sqrLength, float angle)>();

        var allLines = graph.GetAllLines();

        foreach (var (p1, p2) in allLines)
        {
            var (leftmostPT, rightmostPT) = (p1.x < p2.x) ? (p1, p2) : (p2, p1);

            var delta = rightmostPT - leftmostPT;

            var vertAngle = Vector2.SignedAngle(delta, Vector3.right);
            float correctedAngle;

            if (vertAngle.IsBetween(-45, 45)) correctedAngle = vertAngle;
            else if (vertAngle > 0) correctedAngle = (90 - vertAngle);
            else correctedAngle = (90 + vertAngle);

            angleSqrLength.Add(((rightmostPT - leftmostPT).sqrMagnitude, correctedAngle));
        }

        float minVal = -45;
        float maxVal = 46;
        int bucketCount = 9;
        List<List<float>> Buckets = new List<List<float>>();
        List<int> BucketWeight = new List<int>();

        for(int i = 0; i < bucketCount; i++)
        {
            Buckets.Add(new List<float>());
            BucketWeight.Add(0);
        }

        float bucketSize = (maxVal - minVal) / bucketCount;

        foreach(var (sqrLen, angle) in angleSqrLength)
        {
            int bucketNum = Mathf.FloorToInt((angle - minVal) / bucketSize);
            Buckets[bucketNum].Add(angle);
            BucketWeight[bucketNum] += sqrLen;
        }

        int max = 0;
        for(int i = 0; i < bucketCount; i++)
        {
            if (BucketWeight[i] > BucketWeight[max]) max = i;
        }

        float total = 0f; 
        foreach(float ang in Buckets[max])
        {
            total += ang;
        }

        return total / Buckets[max].Count;
    }

    public static  Texture2D DrawToTexture(PixelPathGraph graph, int width, int height)
    {
        Texture2D newTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        newTexture.filterMode = FilterMode.Point;

        graph.ClearVisited();
        for (int x = 0; x < width; x++) for (int y = 0; y < height; y++) newTexture.SetPixel(x, y, Color.black);

        foreach(var nodePos in graph.GetAllPoints())
        {
            graph.TryGetConnections(nodePos, out var connections);
            graph.TryGetPixelType(nodePos, out var colorData);
            foreach (var connectedNode in connections)
            {
                if (graph.IsVisited(connectedNode)) continue;

                graph.TryGetPixelType(connectedNode, out PixelType type);

                DrawLineToTexture(
                    newTexture,
                    nodePos,
                    connectedNode,
                    (colorData == PixelType.LocalRoad || graph.GetPixelType(connectedNode) == PixelType.LocalRoad)
                        ? Color.green
                        : Color.red,
                    true);
            }

            newTexture.SetPixel(nodePos.x, nodePos.y, (colorData == PixelType.LocalRoad) ? Color.green : Color.red);
            graph.MarkVisited(nodePos);
        }

        newTexture.Apply();

        return newTexture;
    }

    public static Texture2D DrawToTextureAtAngle(PixelPathGraph graph, int width, int height, float angleInDeg)
    {
        Texture2D newTexture = new Texture2D(width * 2, height * 2, TextureFormat.RGBA32, false);
        newTexture.filterMode = FilterMode.Point;

        int xOffset = width / 2;
        int yOffset = height / 2;

        Vector2 center = new Vector2(xOffset, yOffset);

        graph.ClearVisited();
        for (int x = 0; x < width * 2; x++) for (int y = 0; y < height * 2; y++) newTexture.SetPixel(x, y, Color.black);

        foreach (var nodePos in graph.GetAllPoints())
        {
            graph.TryGetConnections(nodePos, out var connections);
            graph.TryGetPixelType(nodePos, out var colorData);

            var rotatedNodePt = nodePos.RotateAround(angleInDeg, center);

            foreach (var connectedNode in connections)
            {
                if (graph.IsVisited(connectedNode)) continue;

                graph.TryGetPixelType(connectedNode, out PixelType type);

                DrawLineToTexture(
                    newTexture,
                    rotatedNodePt,
                    connectedNode.RotateAround(angleInDeg, center),
                    (colorData == PixelType.LocalRoad || graph.GetPixelType(connectedNode) == PixelType.LocalRoad)
                        ? Color.green
                        : Color.red,
                    true,
                    xOffset,
                    yOffset);
            }

            newTexture.SetPixel(rotatedNodePt.x + xOffset, rotatedNodePt.y + yOffset, (colorData == PixelType.LocalRoad) ? Color.green : Color.red);
            graph.MarkVisited(nodePos);
        }

        newTexture.Apply();

        Texture2D punchedTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        punchedTexture.filterMode = FilterMode.Point;

        punchedTexture.SetPixels(newTexture.GetPixels(xOffset, yOffset, width, height));
        punchedTexture.Apply();

        return punchedTexture;
    }

    private static void DrawLineToTexture(Texture2D texture, Vector2Int pos1, Vector2Int pos2, Color color, bool excludeEnds = false, int xOffset = 0, int yOffset = 0)
    {
        int dx = pos2.x - pos1.x;
        int dy = pos2.y - pos1.y;

        int nx = Mathf.Abs(dx);
        int ny = Mathf.Abs(dy);

        int sign_x = dx > 0 ? 1 : -1;
        int sign_y = dy > 0 ? 1 : -1;


        int ix = 0;
        int iy = 0;

        Vector2Int drawingPos = pos1;

        if (excludeEnds is false) texture.SetPixel(drawingPos.x + xOffset, drawingPos.y + yOffset, color);
        while (ix < nx || iy < ny)
        {
            if ((1 + 2 * ix) * ny < (1 + 2 * iy) * nx)
            {
                drawingPos.x += sign_x;
                ix++;
            }
            else
            {
                drawingPos.y += sign_y;
                iy++;
            }

            if (excludeEnds is false || drawingPos != pos2) texture.SetPixel(drawingPos.x + xOffset, drawingPos.y + xOffset, color);
        }
    }

    private static bool Are2AxisAligned(Vector2Int position1, Vector2Int position2)
    {
        var delta = position2 - position1;
        return (delta.x == 0 || delta.y == 0);
    }

    private static bool AreDiagonallyAlligned(Vector2Int position1, Vector2Int position2)
    {
        var delta = position2 - position1;
        return Mathf.Abs(delta.x) == Mathf.Abs(delta.y);
    }

    private static void RepopulateCheckable(List<Vector2Int> checkableDir)
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

    private static void FoundDir(Tile.Facing direction, List<Vector2Int> checkableDir)
    {
        Vector2Int vec = direction.ToVector2();
        if (direction.IsHorizontal()) checkableDir.RemoveAll(v => v.x == vec.x);
        else checkableDir.RemoveAll(v => v.y == vec.y);
    }

    private static void NotFoundDir(Tile.Facing direction, List<Vector2Int> checkableDir)
    {
        checkableDir.Remove(direction.ToVector2());
    }

    private static bool PixelExists(PixelType[][] pixelInfo, Vector2Int position, int width, int height, out PixelType pixel)
    {
        pixel = PixelType.None;
        if (position.x < 0 || position.x > width - 1 || position.y < 0 || position.y > height - 1) return false;
        pixel = pixelInfo[position.y][position.x];
        return pixel != PixelType.None;
    }

    /// <summary>
    /// Creates a 2d boolean array and fills it with true for each pixel where the color does not match the mask
    /// </summary>
    private static bool[][] TextureToBinary(Texture2D texture, Color[] maskedColors)
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
    private static void TextureToBinary(Texture2D texture, Color[] maskedColors, bool[][] target)
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
    private static bool MatchMask(Color test, Color[] mask)
    {
        foreach (Color m in mask)
        {
            if (test == m) return true;
        }
        return false;
    }

    public enum PixelType
    {
        None,
        MainRoad,
        LocalRoad
    }
}
