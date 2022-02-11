using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.BowenIvanov.BoatCombat
{
    public class SliderController : MonoBehaviour
    {
        [SerializeField] Gradient myTeam;
        [SerializeField] Gradient enemyTeam;

        int localTeam = 0;
        int otherTeam = 0;

        Slider healthBar;

        private void Start()
        {
            healthBar = GetComponent<Slider>();
            
        }

        public void setColors(float teamControlling, float evaluationInput)
        {
            if(localTeam == 0)
            {
                localTeam = PlayerManager.LocalPlayerInstance.GetComponent<PlayerManager>().getTeam();
                if (localTeam == -1)
                {
                    otherTeam = 1;
                }
                else
                {
                    otherTeam = -1;
                }
            }
            if (teamControlling == otherTeam)
            {
                healthBar.fillRect.GetComponent<Image>().color = enemyTeam.Evaluate(evaluationInput);
                //Debug.Log("other");
            }
            else if (teamControlling == localTeam)
            {
                healthBar.fillRect.GetComponent<Image>().color = myTeam.Evaluate(evaluationInput);
                //Debug.Log("local");
            }
        }
    }
}
