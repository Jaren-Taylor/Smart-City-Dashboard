using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorLogUIController : MonoBehaviour
{
    List<ISensor> sensors = new List<ISensor>();

    private ListeningStage stage = ListeningStage.Disabled;

    [SerializeField]
    private Menu Menu;

    [SerializeField]
    private GameObject pageReference;

    [SerializeField]
    private SensorLogMenu log;

    // Update is called once per frame
    void Update()
    {
        switch (stage)
        {
            case ListeningStage.Disabled:
                if (pageReference.activeInHierarchy && Menu.IsOpen()) stage = ListeningStage.FullRefresh;
                break;
            case ListeningStage.FullRefresh:
                FullRefreshLog();
                SetActiveListenMode(true);
                stage = ListeningStage.ChangeOnly;
                break;
            case ListeningStage.ChangeOnly:
                if(!pageReference.activeInHierarchy || !Menu.IsOpen())
                {
                    SetActiveListenMode(false);
                    stage = ListeningStage.Disabled;
                }
                break;
        } 
    }

    private void SetActiveListenMode(bool enable)
    {
        if (stage == ListeningStage.FullRefresh && enable)
        {
            foreach (var sensor in sensors) ListenToSensor(sensor);
            //Register to all sensor status change thingy
        }
        else if (stage == ListeningStage.ChangeOnly && !enable)
        {
            foreach (var sensor in sensors) StopListeningToSensor(sensor);
            //Deregister from all sensor status change thingy
        }
        else throw new Exception($"Can't set listening mode to {enable} while in {stage} state");
    }

    internal void RegisterSensor(ISensor sensor)
    {
        sensors.Add(sensor);
        log.TryAddSensor(sensor);
    }

    internal void DeregisterSensor(ISensor sensor)
    {
        sensors.Remove(sensor);
        log.TryRemoveSensor(sensor);
    }

    private void ListenToSensor(ISensor sensor) => sensor.StatusUpdated += UpdateSensorStatus;

    private void StopListeningToSensor(ISensor sensor) => sensor.StatusUpdated -= UpdateSensorStatus;

    private void FullRefreshLog()
    {
        foreach(ISensor sensor in sensors)
        {
            log.UpdateSensorLog(sensor);
        }
    }

    private void UpdateSensorStatus(ISensor sensor)
    {
        log.UpdateSensorLog(sensor);
    }

    private enum ListeningStage
    {
        Disabled,
        FullRefresh,
        ChangeOnly
    }
}
