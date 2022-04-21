using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SyncOpacityWithGameObject : MonoBehaviour
{
    [SerializeField]GameObject obj;
    Image img;
    Text text;
    [SerializeField]bool isImage;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        img = obj.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        Color color = text.color;
        if (isImage)
        {
            color = new Color(text.color.r, text.color.g, text.color.b, img.color.a);
        }
        text.color = color;
    }
}
