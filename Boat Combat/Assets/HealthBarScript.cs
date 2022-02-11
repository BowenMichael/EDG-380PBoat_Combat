using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarScript : MonoBehaviour
{
    private void LateUpdate()
    {
        //Vector3 pos = Camera.main.ScreenToWorldPoint(Camera.main.transform.position);
        transform.LookAt(Camera.main.transform.position);
        //transform.Rotate(0, 180, 0);
    }
}
