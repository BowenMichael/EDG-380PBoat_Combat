using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileManager : MonoBehaviour
{
    [SerializeField] Slider throttle;
    [SerializeField] Slider steering;
    [SerializeField] RectTransform shootRegion;
    [SerializeField] bool backLeavesApplication = false;
    // Start is called before the first frame update
    void Start()
    {
#if !(UNITY_ANDROID || UNITY_IOS)
        gameObject.SetActive(false);
#endif
        Input.backButtonLeavesApp = backLeavesApplication;
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
    
    public RectTransform getShoot()
    {
        return shootRegion;
    }


}
