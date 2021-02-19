using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GridManager : MonoBehaviour
{
    [Range(5, 100)]
    public int gridSize;

    private DigitalCursor cursor = null;

    private bool cursorEnabled = true; //When false, cursor will not be shown
    private bool clickRecieved = false; //When true, update function will pick this up and signal to its state controller

    private GameObject ground = null;

    public LayerMask groundMask; // The mask used to find the ground plane

    private int state = 0;
    private NodeController EntityLoc;

    public Material TileMaterial;
    public Material TransparentMaterial;

    public TileGrid Grid; // Data object that holds the information about all tiles
    public static GridManager Instance { get; private set; } //Singleton pattern

    private GridController GridSM; //Controls the state of build mode
    public GameObject NavPointPrefab;
    public bool CursorEnabled { get => cursorEnabled; set => SetCursor(value); }

    // Used as an event handler
    public void ToggleCursor() { SetCursor(!cursorEnabled); }

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

    void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        if (!TryLoadFile())
        {
            Grid = new TileGrid(gridSize, gridSize);
        }

        GridSM = new GridController(new PlaceRoadState());
        if (Instance != null) Destroy(this);
        Instance = this;
        
        ground = CreateGround();
        
        Grid.RefreshGrid();
    }

    private bool TryLoadFile()
    {
        if (SaveGameManager.LoadFromFile != "")
        {
            Grid = SaveGameManager.LoadGame(SaveGameManager.LoadFromFile);
            if (Grid == null) return false;
            SaveGameManager.LoadFromFile = "";
            gridSize = Grid.Width;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Creates the ground object and attaches it as a child
    /// </summary>
    /// <returns></returns>
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

        var mesh = GenerateMesh(Grid.Width, Grid.Height); //Creates the mesh according to the specified grid size

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

    /// <summary>
    /// When a place event occures this function handles it
    /// </summary>
    internal void PlaceHandler()
    {
        clickRecieved = true;
    }

    public void StateNumberChangeHandler(int stateNum) => ChangeState(stateNum);

    public NodeCollectionController GetCollectionAtTileLocation(Vector2Int position) => Grid[position]?.GetComponent<PathfindingTileInterface>().NodeCollection;

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
                    newState = new RemoveTileState();
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
        if (Input.GetKeyDown(KeyCode.P)) Debug.Log(Grid.ToString());

        if (Input.GetKeyDown(KeyCode.S))
        {
            GridSM.SuspendState(cursor);
            SaveGameManager.SaveGame("save.xml", Grid);
            GridSM.ResumeState(cursor);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            SaveGameManager.LoadFromFile = "save.xml";
            SceneManager.LoadScene(0);
            //SaveGameManager.LoadGame("save.xml");
        }


        if (Input.GetKeyDown(KeyCode.Q))
        {

            var roadTiles = Grid.GetRoadLocations();
            var buildingTiles = Grid.GetBuildingLocations();

            var tileLoc = roadTiles[0];
            var entity = VehicleEntity.Spawn(tileLoc, VehicleEntity.VehicleType.Bus);
            var biggest = -1f;
            Vector2Int farPos = Vector2Int.zero; 
            //Finding farthest road tile
            foreach(var pos in roadTiles)
            {
                var dist = (pos - tileLoc).magnitude;
                if (dist > biggest) {
                    biggest = dist;
                    farPos = pos;
                }
            }
            entity.TrySetDestination(farPos);

        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            GameObject path = new GameObject("Path Holder");

            List<Vector2Int> points = Pathfinding.GetListOfPositionsFromTo(new Vector2Int(1, 1), Grid.GetBuildingLocations()[1]);
            points.Reverse();

            List<GameObject> pointInstances = new List<GameObject>();

            if (points.Count > 0) pointInstances.Add(Instantiate(NavPointPrefab, new Vector3(points[0].x, 0f, points[0].y), Quaternion.identity, path.transform));
            for (int i = 1; i < points.Count; i++)
            {
                var navPt = Instantiate(NavPointPrefab, new Vector3(points[i].x, 0f, points[i].y), Quaternion.identity, path.transform);
                navPt.GetComponent<NavPoint>().Connections.Add(new NavPointConnection(pointInstances[i - 1].GetComponent<NavPoint>(), NavPointConnection.ConnectionType.Directed));
                pointInstances.Add(navPt);
            }
        }


        if (Input.GetKeyDown(KeyCode.C)) CursorEnabled = !CursorEnabled; //If C pressed, cursor is disabled
        if (CursorEnabled)
        {
            HandleCursorMovement(); //Updates state with cursor movement
            if (clickRecieved)
            {
                GridSM.OnMouseDown(cursor); //Sends down press event to state
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

    
    /// <summary>
    /// Gets tile from location
    /// </summary>
    /// <param name="location"></param>
    /// <returns></returns>
    public Tile GetTile(Vector2Int location) => Grid[location];


    /// <summary>
    /// Sets transparency of the tile at this location
    /// </summary>
    /// <param name="location"></param>
    /// <param name="value"></param>
    public void SetTransparency(Vector2Int location, bool value) => Grid[location]?.SetTransparency(value);

    /// <summary>
    /// Makes the tile permanent at location
    /// </summary>
    /// <param name="point"></param>
    public void MakePermanent(Vector2Int point)
    {
        Grid[point]?.MakePermanent();
        Grid[point]?.SetTransparency(false);
    }

    /// <summary>
    /// Creates tile with default value for type. Calls to AddTileToGrid.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="point"></param>
    public void CreateTemporaryTile<T>(Vector2Int point) where T : Tile, new() => AddTileToGrid(point, new T());


    /// <summary>
    /// Creates tile with default value for types. And then makes permanent.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="point"></param>
    private void CreatePermanentTile<T>(Vector2Int point) where T : Tile, new()
    {
        AddTileToGrid(point, default(T));
        MakePermanent(point);
    }

    /// <summary>
    /// Adds tile to grid at location. If it is occupied, trys to delete. Updates neighbors of change
    /// </summary>
    /// <param name="point"></param>
    /// <param name="tile"></param>
    public void AddTileToGrid(Vector2Int point, Tile tile)
    {
        if (Grid[point] != null) //If there is a tile at this location already, try to remove
        {
            if (!RemoveTileIfTemporary(point)) return; //The tile could not be removed (is is permanent) so halting
        }
        //Tile location is ensured empty, can proceed to file location
        Grid[point] = tile;
        if (Grid[point].CreateManaged(point, Grid.GetNeighbors(point))) ForceRemoveTileDirty(point);
        UpdateNeighbors(point);
    }

    /// <summary>
    /// Takes a tile that currently exists and ensures their model is correct for the current neighbor scenario.
    /// </summary>
    /// <param name="point"></param>
    private void RecalculateTile(Vector2Int point) 
    {
        if (Grid[point]?.RecalculateManaged(Grid.GetNeighbors(point)) ?? false) RemoveTile(point);
    }


    /// <summary>
    /// Removes tile from the grid based on if the tile if temporary. Updates Neighbors
    /// </summary>
    /// <param name="point">Point on grid to remove</param>
    /// <returns>Returns true if a tile was deleted</returns>
    public bool RemoveTileIfTemporary(Vector2Int point)
    {
        if (Grid[point]?.IsPermanent ?? true) return false; //Returns false when either the tile is permanent or it is null
        else if (ForceRemoveTileDirty(point))
        {
            UpdateNeighbors(point);
            return true;
        }
        else return false;
    }

    /// <summary>
    /// Removes tile and updates neighbors
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
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
    /// Removes a tile, no matter its state. Does not notify neighbors
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    private bool ForceRemoveTileDirty(Vector2Int point)
    {
        Grid[point]?.DeleteManaged();
        Grid[point] = null;
        return true;
    }


    /// <summary>
    /// Tell all neighbors of a point to update it's model
    /// </summary>
    /// <param name="point"></param>
    private void UpdateNeighbors(Vector2Int point)
    {
        foreach (var direction in Tile.Directions) RecalculateTile(point + direction);
    }
}
