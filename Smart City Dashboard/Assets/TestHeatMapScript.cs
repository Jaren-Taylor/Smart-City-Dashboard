using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestHeatMapScript : MonoBehaviour
{

    private float current = 0f;
    private float target = 1f;
    private int width = 10, height = 10;
    private Image img;
    private HeatMap testMap;

    // Start is called before the first frame update
    void Start()
    {
        testMap = new HeatMap(width, height);
        img = GetComponent<Image>();
        UpdateSpriteFromMap();
    }

    private void Update()
    {
        current += Time.deltaTime;
        if(current > target)
        {
            current -= target;
            UpdateSpriteFromMap();
        }
    }
    private void UpdateSpriteFromMap() => img.sprite = Sprite.Create(testMap.CreatePNG(), new Rect(0, 0, width, height), Vector2.zero);
}
