using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorManager : MonoBehaviour
{
    public HashSet<CameraSensor> CameraSet = new HashSet<CameraSensor>();
    public static SensorManager Instance { get; private set; }

    private HeatMap heatMap;

    private void Awake()
    {
        ForceSingleInstance();
    }

    private void Start()
    {
        heatMap = new HeatMap(20, 20);
    }

    private void ForceSingleInstance()
    {
        if (Instance != null) Destroy(this);
        else Instance = this;
    }

    public void OnReceiveCameraData(List<CameraSensorData> sensorData)
    {
        Debug.Log($"Sensor Data Reveived! Sensor: {sensorData[0].SelfPostion} Size: {sensorData.Count}");
    }
}
