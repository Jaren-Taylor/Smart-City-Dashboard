using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeStream 
{
    private LinkedList<NodeController> buffer;
    private readonly TileGrid source;
    private LinkedList<Vector2Int> path;
    private NodeController ending;
    private NodeCollectionController.TargetUser userType;
    private bool isCorrupted;
    private Dictionary<LinkedListNode<NodeController>, Vector2Int> nodeDeregistrationMapping;

    public NodeStream(TileGrid dataSource, IEnumerable<Vector2Int> dataPath, Vector3 startingPosition, NodeController endingNode, NodeCollectionController.TargetUser targetUser)
    {
        source = dataSource;
        path = new LinkedList<Vector2Int>(dataPath);
        ending = endingNode;
        userType = targetUser;
        nodeDeregistrationMapping = new Dictionary<LinkedListNode<NodeController>, Vector2Int>();
        RegisterToTiles(path);
        InitializeBuffer(startingPosition);
    }

    /// <summary>
    /// Ensures that all tiles this stream is still registered to are deregistered before this object is destroyed
    /// </summary>
    ~NodeStream() 
    {
        DeregisterFromAllTiles();
    }

    #region Public Methods

    /// <summary>
    /// Tried to peek the next node. If it can, will return enum showing status of result. Only if result.IsSuccessful() is true will the position be valid
    /// </summary>
    public StreamResponse PeekNext(out Vector3 position)
    {
        var result = TryGetNextNode(out var node);
        if (result.IsSuccessful()) position = node.Position;
        else position = Vector3.zero;
        return result;
    }

    /// <summary>
    /// Tries to advance the node stream. Only advances if returned value is StreamResponse.Successful
    /// </summary>
    public StreamResponse MoveNext()
    {
        var result = TryGetNextNode(out _);
        if (result.IsSuccessful()) DropBufferHead();
        return result;
    }

    /// <summary>
    /// Gets the node currently sitting on the front of the stream.
    /// </summary>
    public Vector3 GetCurrent() => buffer.First.Value.Position;

    /// <summary>
    /// Checks if End of Stream
    /// </summary>
    public bool IsEndOfStream() => buffer.Count == 1 && !IsMoreTilesToRead();

    /// <summary>
    /// Returns true if tile along path has been destroyed
    /// </summary>
    public bool IsCorrupted() => isCorrupted;

    #endregion

    #region Private Methods

    private void InitializeBuffer(Vector3 startingPosition)
    {
        buffer = new LinkedList<NodeController>();

        if (source[path.First.Value] is Tile tile && !(tile.NodeCollection is null) && path.First is LinkedListNode<Vector2Int> first && path.First.Next is LinkedListNode<Vector2Int> second)
        {
            var exitingDirection = NodeCollectionController.GetDirectionFromDelta(second.Value, first.Value);
            AddIntoBuffer(tile.NodeCollection.GetExitDrivewayPath(tile.NodeCollection.GetClosestNodeTo(startingPosition), exitingDirection));
        }
        else
        {
            isCorrupted = true;
        }
    }

    /// <summary>
    /// Removes head of the buffer (advances current by one node)
    /// </summary>
    private void DropBufferHead()
    {
        if (buffer.Count == 1) throw new Exception("Cannot empty buffer completely");
        //Checks if this node is flagged as last node on a specific position
        if (nodeDeregistrationMapping.TryGetValue(buffer.First, out Vector2Int position))
        {
            //If it has been flagged, deregisters from specified position
            DeregisterFromTile(position);
            nodeDeregistrationMapping.Remove(buffer.First);
        }
        buffer.RemoveFirst();
    }

    /// <summary>
    /// Returns next node but does not advance buffer head at all.
    /// </summary>
    private StreamResponse TryGetNextNode(out NodeController controller)
    {
        controller = null;
        if (isCorrupted) return StreamResponse.Corrupted; //Checks if stream has been marked as corrupted
        else if (CanGetNextFromBuffer()) //Buffer already has next node readily available
        {
            controller = buffer.First.Next.Value;
            return StreamResponse.Successful;
        }
        else
        {
            //Next node not available in buffer, must try to access next tile in path
            return TryReadNextPathIntoBuffer(out controller);
        }
    }

    /// <summary>
    /// Attempts to read next path into buffer. Controller is only valid on StreamResponse.Successful. 
    /// </summary>
    private StreamResponse TryReadNextPathIntoBuffer(out NodeController controller)
    {
        controller = null;
        if (IsMoreTilesToRead())
        {
            Vector2Int currentTilePosition = path.First.Value; 
            Vector2Int nextTilePosition = path.First.Next.Value;

            //Gets the direction the path will be exiting current tile from
            NodeCollectionController.Direction exitingDirection = NodeCollectionController.GetDirectionFromDelta(nextTilePosition, currentTilePosition);

            //Calculates what position along the side of the tile to exit tile at
            int position = source[currentTilePosition].NodeCollection.GetPositionFrom(exitingDirection, buffer.First.Value);

            //Asks the next tile if an entity with this inbound direction and postion may enter
            bool canEnter = source[nextTilePosition].NodeCollection.CanEnterFromPosition(exitingDirection, position);

            if (canEnter) //Permission to enter has been granted, proceed into to get path in tile
            {
                controller = source[nextTilePosition].NodeCollection.GetInboundNodeFrom(exitingDirection, position);


                //Adds a flag to degister from current tile position the next time the buffer advances
                nodeDeregistrationMapping.Add(buffer.First, currentTilePosition);

                try
                {
                    //Appends the buffer with path starting a nextNode at that tile position
                    ReadNextPathIntoBuffer(exitingDirection, position, path.First.Next);

                }
                catch (IndexOutOfRangeException)
                {
                    return StreamResponse.Corrupted;
                }
                //Removes current tile position from path (will still be tracked by nodeDegregistrationMapping until it is deregistered)
                path.RemoveFirst();

                return StreamResponse.Successful;
            }
            else //If the node collection has refused enterance at this time, cannot get next node at this time.
            {
                return StreamResponse.Refused;
            }
        }
        else
        {
            return StreamResponse.EndOfStream;
        }
    }

    /// <summary>
    /// Adds the path out of the tilePosition provided starting from the provided entering direction and position
    /// </summary>
    private void ReadNextPathIntoBuffer(NodeCollectionController.Direction enteringDirection, int position, LinkedListNode<Vector2Int> tilePosition)
    {
        NodeCollectionController collection = source[tilePosition.Value].NodeCollection;
        if(tilePosition.Next is LinkedListNode<Vector2Int> nextTilePosition)
        {
            NodeCollectionController.Direction exitingDirection = NodeCollectionController.GetDirectionFromDelta(nextTilePosition.Value, tilePosition.Value);
            if (nextTilePosition.Next is null) // The nextTilePosition is the last tile 
            {
                //Pull into driveway logic
                AddIntoBuffer(GetDrivewayPathFromInbound(enteringDirection, position, exitingDirection, collection));
            }
            else //Not last one
            {
                //Traverse like normal
                AddIntoBuffer(GetNormalPathFromInbound(enteringDirection, position, exitingDirection, collection));
            }
        }
        else //Current tile position IS the last tile, can't calculate an exiting direction
        {
            AddIntoBuffer(GetFinalPath(enteringDirection, position, collection));
        }
    }

    #region Reading Paths Case Helpers

    /// <summary>
    /// Gets whatever path the NodeCollection has for entities entering from the provided entering direction and position
    /// </summary>
    private List<NodeController> GetFinalPath(NodeCollectionController.Direction enteringDirection, int position, NodeCollectionController collection)
    {
        return collection.GetFinalPath(enteringDirection, position, userType);
    }

    /// <summary>
    /// Gets a special path to direct the entity into a driveway. 
    /// </summary>
    private List<NodeController> GetDrivewayPathFromInbound(NodeCollectionController.Direction enteringDirection, int position, NodeCollectionController.Direction exitingDirection, NodeCollectionController collection)
    {
        return collection.GetEnterDrivewayPath(enteringDirection, position, exitingDirection, userType);
    }

    /// <summary>
    /// Gets the standard path by traversing an a direction starting from the provided node. Path returned contains provided node as starting position
    /// </summary>
    private List<NodeController> GetNormalPathFromNode(NodeController startingNode, NodeCollectionController.Direction exitingDirection)
    {
        List<NodeController> output = new List<NodeController>();
        NodeController node = startingNode;
        output.Add(node);
        switch (userType)
        {
            case NodeCollectionController.TargetUser.Pedestrians:
                while (node.GetNodeForPedestrianByDirection(exitingDirection) is NodeController controller)
                {
                    node = controller;
                    output.Add(node);
                }
                break;
            case NodeCollectionController.TargetUser.Vehicles:
                while (node.GetNodeForVehicleByDirection(exitingDirection) is NodeController controller)
                {
                    node = controller;
                    output.Add(node);
                }
                break;
            case NodeCollectionController.TargetUser.Both:
                throw new NotImplementedException();
        }
        return output;
    }

    /// <summary>
    /// Gets the standard path by traversing an a direction starting from the provided node. Path returned contains provided node as starting position
    /// </summary>
    private List<NodeController> GetNormalPathFromInbound(NodeCollectionController.Direction enteringDirection, int position, NodeCollectionController.Direction exitingDirection, NodeCollectionController collection)
    {
        return GetNormalPathFromNode(collection.GetInboundNodeFrom(enteringDirection, position), exitingDirection);
    }

    #endregion

    /// <summary>
    /// Adds provided node controllers into buffer
    /// </summary>
    private void AddIntoBuffer(IEnumerable<NodeController> values)
    {
        foreach (NodeController value in values) buffer.AddLast(value);
    }

    #region Status Checking Methods

    /// <summary>
    /// Checks if there are more tiles to read nodes from
    /// </summary>
    private bool IsMoreTilesToRead() => path.Count > 1;

    /// <summary>
    /// Checks if buffer currently has a next node after the first
    /// </summary>
    private bool CanGetNextFromBuffer() => buffer.Count > 1;

    #endregion

    #region Path Registration Methods

    /// <summary>
    /// Registers to provided positions
    /// </summary>
    /// <param name="positions"></param>
    private void RegisterToTiles(IEnumerable<Vector2Int> positions)
    {
        foreach(Vector2Int position in positions)
        {
            if (source[position] is Tile tile) tile.OnTileDestroyed += TileOnPathDestroyed;
            else isCorrupted = true; //If no tile is found at specified location, stream is marked as corrupted
        }
    }

    /// <summary>
    ///  Deregisters from position if the tile exists
    /// </summary>
    /// <param name="position"></param>
    private void DeregisterFromTile(Vector2Int position)
    {
        if(source[position] is Tile tile) tile.OnTileDestroyed -= TileOnPathDestroyed;
    }

    /// <summary>
    /// Deregisters from all remaining tiles this stream is registered to.
    /// </summary>
    private void DeregisterFromAllTiles()
    {
        //Deregisters from all positions still in path
        foreach (Vector2Int position in path)
        {
            DeregisterFromTile(position);
        }

        //Deregisters from all positions pending to be removed upon a specific node being removed
        foreach (Vector2Int otherPosition in nodeDeregistrationMapping.Values)
        {
            DeregisterFromTile(otherPosition);
        }
    }

    /// <summary>
    /// Callback function that tiles will call when they are destroyed.
    /// </summary>
    private void TileOnPathDestroyed(Tile tile)
    {
        isCorrupted = true; //Sets the stream into a corruption state
        DeregisterFromAllTiles(); //Only one thing to do: https://ih1.redbubble.net/image.519223350.4965/flat,750x,075,f-pad,750x1000,f8f8f8.u1.jpg
    }

    #endregion

    #endregion
}

/// <summary>
/// Used to describe the status of the stream.
/// </summary>
public enum StreamResponse
{
    Corrupted, //The stream should be dumped, the something in the data source was removed (Recalculate Path!)
    Refused, //A tile has refused access to it for some reason. Try again later
    EndOfStream, //The current node is the last on the path
    Successful //The requested operation was successful
}

/// <summary>
/// Adds extension to stream response to allow it to act as a boolean. Useless other than to make the code more visually appealing.
/// </summary>
public static class StreamResponseExtensions
{
    public static bool IsSuccessful(this StreamResponse value) => value == StreamResponse.Successful;
}
