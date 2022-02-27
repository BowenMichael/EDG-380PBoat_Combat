using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BoyantObject : MonoBehaviour
{
    public Transform waterHeight;
    public List<Transform> boyantPoint = new List<Transform>();
    public Transform centerOfMass;
    public float boyantForce = 100f;
    public float submergedDepth = 2f;
    public float dampning = .5f;
    Rigidbody rb;
    public GameObject water;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 netBForce = Vector3.zero;
        int numOfForces = 0;
        foreach(Transform bPoint in boyantPoint)
        {
            if(bPoint.position.y > waterHeight.position.y)
            {
                continue;
            }
            //if (!Physics.Raycast(bPoint.position, -bPoint.up, out RaycastHit hit, 10f, 4)) return;

            float submergedValue = Mathf.Clamp(waterHeight.position.y- bPoint.position.y, 0.0f, 1.0f);
            Debug.Log("Submerged value: " + submergedValue);
            Vector3 localDampingForce = dampning * rb.mass * -bPoint.up;
            Vector3 bForce = localDampingForce + submergedValue * boyantForce * Vector3.up;
            netBForce += bForce;
            numOfForces++;
        }
        if (numOfForces > 0) ;
            rb.AddForceAtPosition(netBForce / numOfForces, centerOfMass.position);
        //if()
    }
}
