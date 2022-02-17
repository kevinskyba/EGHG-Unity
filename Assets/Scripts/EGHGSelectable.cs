using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KevinSkyba
{
    namespace EGHG
    {
        public class EGHGSelectable : MonoBehaviour
        {
            private Button button;

            private void Awake()
            {
                button = GetComponent<Button>();
            }

            public void EGHGStartFocus()
            {
                Debug.Log("EGHGStartFocus");
                gameObject.SendMessage("EGHGOnStartFocus", SendMessageOptions.DontRequireReceiver);
                if (button)
                {
                    button.OnPointerEnter(null);
                    button.image.color = button.colors.highlightedColor;
                }
            }

            public void EGHGEndFocus()
            {
                Debug.Log("EGHGEndFocus");
                gameObject.SendMessage("EGHGOnEndFocus", SendMessageOptions.DontRequireReceiver);
                if (button)
                {
                    button.OnPointerExit(null);
                    button.image.color = button.colors.normalColor;
                }
            }
        }
    }
}