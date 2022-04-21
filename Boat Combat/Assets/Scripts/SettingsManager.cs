using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Com.BowenIvanov.BoatCombat
{
    public class SettingsManager : MonoBehaviour
    {
        PlayerManager playerManager;
        private void OnEnable()
        {
            if (PlayerManager.LocalPlayerInstance.TryGetComponent(out PlayerManager plm))
            {
                playerManager = plm;
                
            }
            else
            {
                Debug.LogError("No Player Controls Satus Found");
            }
        }

        public void setIsSliderControls(bool isSlider)
        {
            if (playerManager != null)
            {
                playerManager.setIsSliderControls(isSlider);
            }
            else
            {
                Debug.LogError("Player manager not found trying to set [Slider Controls]!");
            }
        }

        public void toggleDragCamera(Text text)
        {
            if (playerManager != null)
            {
                if(playerManager.toggleIsDragCamera() == true)
                {
                    text.text = "Drag Camera on";
                }
                else
                {
                    text.text = "Drag Camera off";

                }
            }
            else
            {
                Debug.LogError("Player manager not found trying to set [Drag Camera control]!");
            }
        }

        public void setSteeringSens(Slider slider)
        {
            if (playerManager != null)
            {
                playerManager.setHortAccelSensitivity(slider.value);
            }
            else
            {
                Debug.LogError("Player manager not found trying to set [setSteeringSens]!");
            }
        }

        public void setThrottleSens(Slider slider)
        {
            if (playerManager != null)
            {
                playerManager.setVertAccelSensitivity(slider.value);
            }
            else
            {
                Debug.LogError("Player manager not found trying to set [setThrottleSens]!");
            }
        }
    }
}
