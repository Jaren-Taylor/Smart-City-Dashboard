using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Range(minGridSize, maxGridSize)]
    public int gridSize;
    private const int minGridSize = 1;
    private const int maxGridSize = 100;

    private DigitalCursor cursor = null;

    private bool cursorEnabled = true;
    private bool clickRecieved = false;

    private GameObject ground = null;

    public LayerMask groundMask;

    private TileGrid grid;
    private Dictionary<Vector2Int, GameObject> activeStructures = new Dictionary<Vector2Int, GameObject>();

    public Material TileMaterial;
    public Material TransparentMaterial;

    public static GridManager Instance { get; private set; }

    private GridController GridSM;

    public bool CursorEnabled { get => cursorEnabled; set => SetCursor(value); }

    private void SetCursor(bool value)
    {
        if (value == cursorEnabled) return;
        if (value)
        {
            cursor = null;
            cursorEnabled = true;
        } 
        else
        {
            if (cursor != null)
            {
                RemoveTileIfTemporary(cursor.Position);
                cursorEnabled = false;
                cursor = null;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GridSM = new GridController(new PlaceRoadState());
        if (Instance != null) Destroy(this);
        Instance = this;
        grid = new TileGrid(gridSize, gridSize);
        ground = CreateGround();
    }

    private GameObject CreateGround()
    {
        var blueprint = new GameObject("Ground")
        {
            layer = LayerMask.NameToLayer("Grid")
        };  //Creates gameobject with name ground and assigns it to the grid layer


        var meshFilter = blueprint.AddComponent<MeshFilter>(); //Stores mesh data for objects
        var meshCollider = blueprint.AddComponent<MeshCollider>(); //Stores collider data for objects
        var meshRenderer = blueprint.AddComponent<MeshRenderer>(); //Stores material for rendering mesh with material
        meshRenderer.material = Resources.Load<Material>("Materials/Grass_Mat"); //Assigns material as grass

        var mesh = GenerateMesh(grid.Width, grid.Height); //Creates the mesh according to the specified grid size

        meshFilter.mesh = mesh; //Adds mesh to filter
        meshCollider.sharedMesh = mesh;  //Adds mesh to collider

        blueprint.transform.position = new Vector3(-.5f, 0f, -.5f); //Grid is offset by (0.5, 0.5) to account for the tiles being centered
        blueprint.transform.parent = this.transform;
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

    private int state = 0;

    public void StateNumberChangeHandler(int stateNum) => ChangeState(stateNum);

    private void ChangeState(int state)
    {
        if(this.state != state)
        {
            IGridControlState newState;
            this.state = state;
            switch (state)
            {
                case 0:
                    newState = new PlaceRoadState();
                    break;
                case 1:
                    newState = new PlaceStructureState(BuildingTile.StructureType.House);
                    break;
                case 2:
                    newState = new PlaceStructureState(BuildingTile.StructureType.TestStruct);
                    break;
                default:
                    newState = new PlaceRoadState();
                    this.state = 0;
                    break;
            }
            GridSM.SetState(newState, cursor);
        }
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
                GridSM.OnMouseDown(cursor);
            }
        }
        clickRecieved = false;
    }

    private void HandleCursorMovement()
    {
        DigitalCursor newCursor = new DigitalCursor();
        
        if(newCursor.Position != cursor?.Position || newCursor.SubDirection != cursor?.SubDirection)
        {
            GridSM.MoveCursor(cursor, newCursor);
            cursor = newCursor;
        }
    }

    

    public Tile GetTile(Vector2Int location) => grid[location];

    public void MakePermanent(Vector2Int point)
    {
        grid[point]?.MakePermanent();
        grid[point]?.SetTransparency(false);
    }

    public void CreateTemporaryTile<T>(Vector2Int point) where T : Tile, new() => AddTileToGrid(point, new T());

    private void CreatePermanentTile<T>(Vector2Int point) where T : Tile, new()
    {
        AddTileToGrid(point, default(T));
        MakePermanent(point);
    }

    public void AddTileToGrid(Vector2Int point, Tile tile)
    {
        if (grid[point] != null) //If there is a tile at this location already, try to remove
        {
            if (!RemoveTileIfTemporary(point)) return; //The tile could not be removed (is is permanent) so halting
        }
        //Tile location is ensured empty, can proceed to file location
        grid[point] = tile;
        if (grid[point].CreateManaged(point, grid.GetNeighbors(point))) ForceRemoveTileDirty(point);
        UpdateNeighbors(point);
    }

    private void RecalculateTile(Vector2Int point) 
    {
        if (grid[point]?.RecalculateManaged(grid.GetNeighbors(point)) ?? false) RemoveTile(point);
    }


    /// <summary>
    /// Removes tile from the grid based on if the tile if temporary. Updates Neighbors
    /// </summary>
    /// <param name="point">Point on grid to remove</param>
    /// <returns>Returns true if a tile was deleted</returns>
    public bool RemoveTileIfTemporary(Vector2Int point)
    {
        if (grid[point]?.IsPermanent ?? true) return false; //Returns false when either the tile is permanent or it is null
        else if (ForceRemoveTileDirty(point))
        {
            UpdateNeighbors(point);
            return true;
        }
        else return false;
    }

    public bool RemoveTile(Vector2Int point)
    {
        if (ForceRemoveTileDirty(point))
        {
            UpdateNeighbors(point);
            return true;
        }
        else return false;
    }


    /// <summary>
    /// Removes a tile, no matter its state
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    private bool ForceRemoveTileDirty(Vector2Int point)
    {
        grid[point]?.DeleteManaged();
        grid[point] = null;
        return true;
    }

    private void UpdateNeighbors(Vector2Int point)
    {
        foreach (var direction in Tile.Directions) RecalculateTile(point + direction);
    }
}
