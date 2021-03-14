using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorLogMenu : MonoBehaviour
{
    [SerializeField]
    private ScrollablePopupMenu menu;
    [SerializeField]
    private SensorInfoMenu sensorInfoMenu;

    private ISensor targetedSensor;

    private Dictionary<ISensor, SimpleCard> sensorMapping = new Dictionary<ISensor, SimpleCard>();
    private Dictionary<UIElement, ISensor> cardMapping = new Dictionary<UIElement, ISensor>();

    public void AddSensor(ISensor sensor)
    {
        if (sensorMapping.ContainsKey(sensor)) throw new Exception("Sensor already found");
        var (statusMsg, statusEnum) = sensor.Status();
        var card = menu.AddNewItem(statusEnum.GetColor(), statusMsg);
        sensorMapping.Add(sensor, card);
        cardMapping.Add(card, sensor);
        card.OnClick += CardClicked;
    }

    private void CardClicked(UIElement card)
    {
        if(cardMapping.TryGetValue(card, out ISensor sensor))
        {
            sensorInfoMenu.DisableUserInput();
            var tilePosition = sensor.GetTilePosition();
            CameraManager.Instance.OnReachedTarget += ReachedSensor;
            CameraManager.Instance.OnReachedTarget += sensorInfoMenu.SetVisible;
            targetedSensor = sensor;
            CameraManager.Instance.TrackPosition(tilePosition.ToGridVector3(), Config.minSize, true);
        }
    }

    private void ReachedSensor(Vector3 position)
    {
        if(targetedSensor != null && targetedSensor.GetTilePosition() == position.ToGridInt())
        {
        }
        else
        {
            Debug.Log("Something went wrong in the process");
        }
    }

    public void RemoveSensor(ISensor sensor)
    {
        if(!sensorMapping.ContainsKey(sensor)) throw new Exception("Sensor does not exist in this menu");
        var card = sensorMapping[sensor];
        if (card != null) card.DestroyUIElement();
        sensorMapping.Remove(sensor);
        cardMapping.Remove(card);
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

    private void UpdateCard(SimpleCard card, string message, UIBackgroundSprite sprite)
    {
        card.SetText(message);
        card.SetBackgroundSprite(sprite);
        UpdatePositionalStanding(card);
    }

    private void UpdatePositionalStanding(SimpleCard card)
    {
        return;
    }
}
