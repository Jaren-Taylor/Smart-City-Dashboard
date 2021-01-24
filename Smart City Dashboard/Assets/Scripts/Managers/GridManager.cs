using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject road0Way;
    public GameObject road2Way;
    public GameObject road3Way;
    public GameObject road4Way;
    public GameObject roadEndCap;
    public GameObject roadCorner;


    [Range(minGridSize, maxGridSize)]
    public int gridSize;
    private const int minGridSize = 1;
    private const int maxGridSize = 100;

    public Vector2Int? Cursor = null;

    private bool cursorEnabled = true;
    private bool clickRecieved = false;

    public LayerMask groundMask;

    private GameObject ground = null;

    private TileGrid grid;
    private Dictionary<Vector2Int, GameObject> activeStructures = new Dictionary<Vector2Int, GameObject>();

    public bool CursorEnabled { get => cursorEnabled; set => SetCursor(value); }

    private void SetCursor(bool value)
    {
        if (value == cursorEnabled) return;
        if (value)
        {
            Cursor = null;
            cursorEnabled = true;
        } 
        else
        {
            if (Cursor != null)
            {
                RemoveTileIfTemporary(Cursor.Value);
                cursorEnabled = false;
                Cursor = null;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        grid = new TileGrid(gridSize, gridSize);
        ground = CreateGround();
    }

    private GameObject CreateGround()
    {
        var blueprint = new GameObject("Ground")
        {
            layer = LayerMask.NameToLayer("Grid"),
        };  //Creates gameobject with name ground and assigns it to the grid layer


        var meshFilter = blueprint.AddComponent<MeshFilter>(); //Stores mesh data for objects
        var meshCollider = blueprint.AddComponent<MeshCollider>(); //Stores collider data for objects
        var meshRenderer = blueprint.AddComponent<MeshRenderer>(); //Stores material for rendering mesh with material
        meshRenderer.material = Resources.Load<Material>("Materials/Grass_Mat"); //Assigns material as grass

        var mesh = GenerateMesh(grid.Width, grid.Height); //Creates the mesh according to the specified grid size

        meshFilter.mesh = mesh; //Adds mesh to filter
        meshCollider.sharedMesh = mesh;  //Adds mesh to collider

        blueprint.transform.position = new Vector3(-.5f, 0f, -.5f); //Grid is offset by (0.5, 0.5) to account for the tiles being centered

        return blueprint;
    }

    /* This is how the mesh is drawn
     *    1 ---------------2
     *    |              _/|
     *    |     T1    _/   |
     *  Height    _ /      |
     *    |   _ /    T2    |
     *    | /              |
     *    0 ----Width----- 3
     * Unity uses a clockwise winding order to determine triangle fronts
     */
    private Mesh GenerateMesh(int width, int height)
    {
        Mesh mesh = new Mesh
        {
            vertices = new Vector3[] {
            Vector3.zero,                   //Point 0
            new Vector3(0, 0, height),       //Point 1
            new Vector3(width, 0, height),  //Point 2
            new Vector3(width, 0, 0)       //Point 3
        },
            uv = new Vector2[] {
            new Vector2(0,0),  //P0
            new Vector2(0,1),  //P1
            new Vector2(1,1),  //P2
            new Vector2(1,0)   //P3
        },

            //                    Triangle 1    Triangle 2
            triangles = new int[] { 0, 1, 2, 0, 2, 3 }
        };

        mesh.RecalculateBounds();
        mesh.RecalculateNormals(); //Calculates the normals according to the provided mesh

        return mesh;
    }

    internal void PlaceHandler()
    {
        clickRecieved = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) CursorEnabled = !CursorEnabled;
        if (CursorEnabled)
        {
            HandleCursorMovement();
            if (clickRecieved)
            {
                if (Cursor.HasValue) MakePermanant(Cursor.Value);
            }
        }
        clickRecieved = false;
    }

    private void HandleCursorMovement()
    {
        Vector2Int? newCursor = GetMouseLocationOnGrid();
        if(newCursor != Cursor)
        {
            if (Cursor != null) RemoveTileIfTemporary(Cursor.Value); //If the cursor was valid, removes temp tile at cursor
            if (newCursor != null)
            {
                TryCreateTemporaryRoad(newCursor.Value); //If new position is on grid, creates temp tile at cursor
            }
            Cursor = newCursor;
        }
    }

    private void MakePermanant(Vector2Int point) => GetStructure(point)?.MakePermanent();

    private Structure GetStructure(Vector2Int point)
    {
        if (activeStructures.ContainsKey(point)) return activeStructures[point].GetComponent<Structure>();
        else return null;
    }


    private void TryCreateTemporaryRoad(Vector2Int point)
    {
        CreateTile(point, TileType.Road,  true, false);
        UpdateNeighbors(point);
    }

    private void RemoveTileIfTemporary(Vector2Int point)
    {
        if(RemoveTile(point,false)) UpdateNeighbors(point);
    }

    private Vector2Int? GetMouseLocationOnGrid()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundMask))
        {
            Vector3Int rayPosition = Vector3Int.RoundToInt(hit.point);
            return new Vector2Int(rayPosition.x, rayPosition.z);
        }
        return null;
    }

    /// <summary>
    /// Creates tile at location, 
    /// </summary>
    /// <param name="point">Point on grid</param>
    /// <param name="newType">Type of tile</param>
    /// <param name="markTemporary">If true, will mark tile as temporary</param>
    /// <param name="overrideLocation">If true, will replace tile at location</param>
    public void CreateTile(Vector2Int point, TileType newType, bool markTemporary, bool replace)
    {
        TileType current = grid[point];
        if (current == TileType.OffGrid) return;
        if (replace) ReplaceTile(point, newType, markTemporary); //Override mode, so will replace tile regardless of what is there
        else
        {
            if (current == TileType.Empty) ReplaceTile(point, newType, markTemporary); //Tile is empty, so create tile here
            else return; //Tile will not be replaced because replace mode is off
        }
    }

    private void ReplaceTile(Vector2Int point, TileType newType, bool markTemporary)
    {
        TileType current = grid[point];
        if (current == TileType.OffGrid) return;
        if (current != TileType.Empty)
        {
            RemoveTile(point, true);
        }

        grid[point] = newType;
        switch (newType)
        {
            case TileType.Empty:
                break;
            case TileType.Road:
                activeStructures[point] = WhatRoadTileAmI(point);
                if (!markTemporary) MakePermanant(point);
                break;
            case TileType.Structure:
                activeStructures[point] = WhatBuildingTileAmI(point);
                if (!markTemporary) MakePermanant(point);
                break;
            default:
                throw new NotImplementedException("No case had been added for creating new object");
        }
    }

    private GameObject WhatBuildingTileAmI(Vector2Int point)
    {
        throw new NotImplementedException("This is a stub, add logic to position buildings when spawned here");
        //Return a instantiated game object here with a Building Tile component
    }

    private void RecalculateTile(Vector2Int point)
    {
        TileType type = grid[point];
        if (type == TileType.Empty || type == TileType.OffGrid) return;
        ReplaceTile(point, type, !GetStructure(point).isPermanent);
    }


    /// <summary>
    /// Removes tile from the grid based on if the tile if temporary.
    /// </summary>
    /// <param name="point">Point on grid to remove</param>
    /// <param name="forceRemove">If true, will delete the tile regardless</param>
    public bool RemoveTile(Vector2Int point, bool forceRemove)
    {
        switch (grid[point])
        {
            case TileType.Empty:
            case TileType.OffGrid:
                break;
                //throw new System.Exception($"No tile to remove at point: {point}");
            
                //throw new System.IndexOutOfRangeException($"Point [{point}] outside of grid bounds");
            case TileType.Road:
            case TileType.Structure:
                if (activeStructures.ContainsKey(point))
                {
                    if(forceRemove || !GetStructure(point).isPermanent)
                    { 
                        grid[point] = TileType.Empty;
                        Destroy(activeStructures[point]);
                        activeStructures.Remove(point);
                        return true;
                    }
                }
                break;
        }
        return false;
    }

    public void UpdateNeighbors(Vector2Int point)
    {
        Vector2Int left = new Vector2Int(point.x - 1, point.y);
        Vector2Int right = new Vector2Int(point.x + 1, point.y);
        Vector2Int top = new Vector2Int(point.x, point.y - 1);
        Vector2Int bottom = new Vector2Int(point.x, point.y + 1);

        RecalculateTile(left);
        RecalculateTile(right);
        RecalculateTile(top);
        RecalculateTile(bottom);
    }

    
    public GameObject WhatRoadTileAmI(Vector2Int point)
    {
        NeighborInfo neighbors = grid.GetNeighbors(point.x, point.y);
        int count = 0;
        bool left = false, right = false, top = false, bottom = false;

        if (neighbors.left == TileType.Road)   { count++; left   = true; }
        if (neighbors.right == TileType.Road)  { count++; right  = true; }
        if (neighbors.top == TileType.Road)    { count++; top    = true; }
        if (neighbors.bottom == TileType.Road) { count++; bottom = true; }

        // all this switch statement does it handle which way the tile should be rotated
        GameObject prefab = null;
        Quaternion rotation = Quaternion.identity;
        switch (count) {
            // 0 way
            case 0:
                prefab = road0Way;
                break;
            // end cap
            case 1:
                prefab = roadEndCap;
                if (top) {
                    rotation = Quaternion.Euler(0, 180, 0);
                }
                else if (right) {
                    rotation = Quaternion.Euler(0, 90, 0);
                }
                else if (left) {
                    rotation = Quaternion.Euler(0, -90, 0);
                }
                break;
            // 2 way or corner
            case 2:
                if (right && top || right && bottom || left && top || left && bottom) {
                    prefab = roadCorner;
                    if (right && top) {
                        rotation = Quaternion.Euler(0, 180, 0);
                    } else if (right && bottom) {
                        rotation = Quaternion.Euler(0, 90, 0);
                    } else if (left && top) {
                        rotation = Quaternion.Euler(0, -90, 0);
                    }
                }
                else { 
                    prefab = road2Way;
                    if (right) rotation = Quaternion.Euler(0, 90, 0);
                }
                break;
            // 3 way
            case 3:
                prefab = road3Way;
                if (!bottom) {
                    rotation = Quaternion.Euler(0, 180, 0);
                } else if (!right) {
                    rotation = Quaternion.Euler(0, -90, 0);
                } else if (!left) {
                    rotation = Quaternion.Euler(0, 90, 0);
                }
                break;
            // 4 way
            case 4:
                prefab = road4Way;
                break;
        }
        /*
        if (onlyNeighbors && orientation == TileOrientation.center) {
            return null;
        } else {
            return Instantiate(prefab, tileGrid[x, y].transform.position, rotation);
        }*/

        return Instantiate(prefab, new Vector3(point.x, 0f, point.y), rotation, transform);
    }
}
