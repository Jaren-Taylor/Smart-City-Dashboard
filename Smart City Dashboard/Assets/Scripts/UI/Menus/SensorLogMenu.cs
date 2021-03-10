using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorLogMenu : MonoBehaviour
{
    [SerializeField]
    private ScrollablePopupMenu menu;

    private Dictionary<ISensor, SimpleCard> sensorMapping = new Dictionary<ISensor, SimpleCard>();

    public void AddSensor(ISensor sensor)
    {
        if (sensorMapping.ContainsKey(sensor)) throw new Exception("Sensor already found");
        var (statusMsg, statusEnum) = sensor.Status();
        var card = menu.AddNewItem(statusEnum.GetColor(), statusMsg);
        sensorMapping.Add(sensor, card);
    }

    public void RemoveSensor(ISensor sensor)
    {
        if(!sensorMapping.ContainsKey(sensor)) throw new Exception("Sensor does not exist in this menu");
        var card = sensorMapping[sensor];
        card.DestroyUIElement();
        sensorMapping.Remove(sensor);
    }

    public void UpdateSensorLog(ISensor sensor)
    {
        if (sensorMapping.TryGetValue(sensor, out var card))
        {
            var (statusMsg, statusEnum) = sensor.Status();
            UpdateCard(card, statusMsg, statusEnum.GetColor());            
        }
        else
        {
            AddSensor(sensor);
            UpdatePositionalStanding(card);
        }
    }

    private void UpdateCard(SimpleCard card, string message, Color color)
    {
        card.SetText(message);
        card.SetColor(color);
        UpdatePositionalStanding(card);
    }

    private void UpdatePositionalStanding(SimpleCard card)
    {
        return;
    }

}
