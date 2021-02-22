using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorManager : MonoBehaviour
{
    public Action<HeatMap> OnHeatMapUpdated;


    private float totalTime = 0f;
    private float callDelay;
    protected Rigidbody ownerRigidBody = null;
    public int TargetCallsPerSecond
    {
        get => (int)(1 / callDelay);
        set => callDelay = (value > 0) ? (1f / value) : 1f;
    }

    public HashSet<CameraSensor> CameraSet = new HashSet<CameraSensor>();
    public static SensorManager Instance { get; private set; }

    private HeatMap heatMap;

    private List<Vector2Int> TrackedPoints = new List<Vector2Int>();

    private void Awake()
    {
        ForceSingleInstance();
    }

    private void Start()
    {
        TargetCallsPerSecond = 4;
        int mapSize = GridManager.Instance.gridSize;
        heatMap = new HeatMap(mapSize, mapSize);
    }

    private void Update()
    {
        totalTime += Time.deltaTime;
        if(totalTime > callDelay)
        {
            UpdateManagedServices();
            totalTime -= callDelay;
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            var cursor = new DigitalCursor();
            if (cursor.OnGrid)
            {
                TryCreateCameraAt(cursor.Position);
            }
        }
    }

    private void UpdateManagedServices()
    {
        UpdateHeatMap();
    }

    private void UpdateHeatMap()
    {
        heatMap.Update(TrackedPoints);
        if(TrackedPoints.Count > 0)
        {
            TrackedPoints.Clear();
        }
        OnHeatMapUpdated?.Invoke(heatMap);
    }

    private void ForceSingleInstance()
    {
        if (Instance != null) Destroy(this);
        else Instance = this;
    }

    public void OnReceiveCameraData(List<CameraSensorData> sensorData)
    {
        foreach (var data in sensorData) TrackedPoints.Add(ToMapBounds(data.Position));

        //Debug.Log($"Sensor Data Reveived! Sensor: {sensorData[0].SelfPostion} Size: {sensorData.Count}");
    }

    private static Vector2Int ToMapBounds(Vector3 position)
    {
        return Vector2Int.RoundToInt(new Vector2(position.x, position.z));
    }

    public static bool TryCreateCameraAt(Vector2Int tilePosition)
    {
        return TryCreateSensorAt<CameraSensor>(tilePosition);
    }

    public static bool RemoveSensorsAt(Vector2Int tilePosition)
    {
        if (GridManager.GetTile(tilePosition) is Tile tile)
        {
            tile.RemoveComponent<CameraManager>();
        }
        return true;
    }

    private static bool TryCreateSensorAt<T>(Vector2Int tilePosition) where T : Component
    {
        //Makes sure the tile exists and doesn't have a T type sensor on the tile already
        if(GridManager.GetTile(tilePosition) is Tile tile && !tile.TryGetComponent<T>(out _))
        {
            tile.AddComponent<T>();
            return true;
        }
        return false;
    }
}
