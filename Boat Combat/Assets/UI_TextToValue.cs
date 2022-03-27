using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Com.BowenIvanov.BoatCombat
{
    [RequireComponent(typeof(Text))]
    public class UI_TextToValue : MonoBehaviour
    {
        Text text;
        string original;
        // Start is called before the first frame update
        void Start()
        {
            text = GetComponent<Text>();
            original = text.text;
        }

        public void setTextToValue(Slider slider)
        {
            text.text = original + slider.value;
        }
    }
}
