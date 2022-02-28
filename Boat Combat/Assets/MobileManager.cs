using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileManager : MonoBehaviour
{
    [SerializeField] Slider throttle;
    [SerializeField] Slider steering;
    // Start is called before the first frame update
    void Start()
    {
#if !UNITY_ANDROID
        gameObject.SetActive(false);
#endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Slider getThrottle()
    {
        return throttle;
    }

    public Slider getSteering()
    {
        return steering;
    }
}
