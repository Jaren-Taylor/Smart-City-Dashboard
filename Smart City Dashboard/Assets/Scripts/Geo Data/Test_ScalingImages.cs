using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test_ScalingImages : MonoBehaviour
{
    public Image source;
    public Image trimmed;
    public Image rescaled;

    // Start is called before the first frame update
    void Start()
    {
        Texture2D texture = source.sprite.texture;
        Vector2Int trimmedDem = new Vector2Int(texture.width - 40, texture.height - 40);

        //Texture2D trimmedTex = texture.

        //trimmed.sprite = Sprite.Create(texture, new Rect(0, 0, trimmedDem.x, trimmedDem.y), Vector2.zero);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
