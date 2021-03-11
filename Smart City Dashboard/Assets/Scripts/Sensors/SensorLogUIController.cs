using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorLogUIController : MonoBehaviour
{
    List<ISensor> sensors = new List<ISensor>();

    private ListeningStage stage = ListeningStage.Disabled;

    [SerializeField]
    private TabButton tab;

    [SerializeField]
    private SensorLogMenu log;

    // Update is called once per frame
    void Update()
    {
        switch (stage)
        {
            case ListeningStage.Disabled:
                if (tab.IsTabEnabled) stage = ListeningStage.FullRefresh;
                break;
            case ListeningStage.FullRefresh:
                FullRefreshLog();
                SetActiveListenMode(true);
                stage = ListeningStage.ChangeOnly;
                break;
            case ListeningStage.ChangeOnly:
                if(!tab.IsTabEnabled)
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
        
        log.AddSensor(sensor);
    }

    internal void DeregisterSensor(ISensor sensor)
    {
        sensors.Remove(sensor);
        log.RemoveSensor(sensor);
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
