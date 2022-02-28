using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KevinSkyba.EGHG.UI.Selectables
{
    public abstract class Selectable : MonoBehaviour
    {

        private void Awake()
        {
        }

        public void EGHGStartEyeFocus()
        {
            gameObject.SendMessage("EGHGOnStartEyeFocus", SendMessageOptions.DontRequireReceiver);
        }

        public void EGHGEndEyeFocus()
        {
            gameObject.SendMessage("EGHGOnEndEyeFocus", SendMessageOptions.DontRequireReceiver);
        }

        public void EGHGStartHeadFocus()
        {
            gameObject.SendMessage("EGHGOnStartHeadFocus", SendMessageOptions.DontRequireReceiver);
        }

        public void EGHGEndHeadFocus()
        {
            gameObject.SendMessage("EGHGOnEndHeadFocus", SendMessageOptions.DontRequireReceiver);
        }


        public void EGHGStartFocus()
        {
            gameObject.SendMessage("EGHGOnStartFocus", SendMessageOptions.DontRequireReceiver);
        }

        public void EGHGEndFocus()
        {
            gameObject.SendMessage("EGHGOnEndFocus", SendMessageOptions.DontRequireReceiver);
        }
    }
}
