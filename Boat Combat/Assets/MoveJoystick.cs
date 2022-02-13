using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveJoystick : MonoBehaviour
{
    RectTransform parent;
    RectTransform rt;
    // Start is called before the first frame update
    void Start()
    {
        rt = GetComponent<RectTransform>();
        parent = rt.parent.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.touchCount > 0)
        //{
        //    Touch t = Input.GetTouch(0);
        //    rt.position = new Vector2(Mathf.Clamp(t.position.x, parent.rect.min.x, parent.rect.max.x), 
        //                                Mathf.Clamp(t.position.y, parent.rect.min.y, parent.rect.max.y));
        //}
    }
}
