using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedJoystick : Joystick
{
    [SerializeField] public RectTransform shootRegion;
    //[SerializeField] public Rex

    private void Awake()
    {
        shootRegion = GameObject.FindGameObjectWithTag("ShootUIRegion").GetComponent<RectTransform>();
    }
}