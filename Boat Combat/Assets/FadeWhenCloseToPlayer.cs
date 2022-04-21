using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.BowenIvanov.BoatCombat
{
    public class FadeWhenCloseToPlayer : MonoBehaviour
    {
        [SerializeField] float distanceToStartFade = 5;
        [SerializeField] float minDistanceFade = 1;
        [SerializeField] float distance = 0;
        Image img;
        // Start is called before the first frame update
        void Start()
        {
            img = GetComponent<Image>();
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}