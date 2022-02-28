using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using KevinSkyba.EGHG.EyeTracking;
using KevinSkyba.EGHG.Data;
using KevinSkyba.Pandas;

namespace KevinSkyba.EGHG
{
    /// <summary>
    /// Eye Gaze / Head Gaze Input Manager
    /// Manages objects and their interactions.
    /// </summary>
    public partial class InputManager : MonoBehaviour
    {

        [Header("Eye Gaze")]
        [SerializeField]
        private float eyeGazeSelectionTime;

        [SerializeField]
        private float eyeGazeSelectionCooldownTime;

        private float currentEyeGazeSelectionTime;
        private float lastEyeGazeSelectionTime;


        private void FixedUpdateEyeGaze()
        {
            Debug.DrawRay(eyeTrackingProvider.HeadTransform.position, eyeTrackingProvider.HeadGaze * Vector3.forward * 5, Color.blue);
            Debug.DrawRay(eyeTrackingProvider.HeadTransform.position, eyeTrackingProvider.AbsoluteEyeGaze * Vector3.forward * 5, Color.red);

            eyeGazeMask.transform.position = eyeTrackingProvider.HeadTransform.position + eyeTrackingProvider.AbsoluteEyeGaze * Vector3.forward * 0.5f;

            /**
            * Update current focus
            */
            GameObject selectable = null;

            float delta = Time.time - lastEyeGazeSelectionTime;
            if (delta < eyeGazeSelectionCooldownTime)
            {
                return;
            }

            // First try 3d objects on layer EGHG
            RaycastHit rayHit;
            if (Physics.Raycast(eyeTrackingProvider.HeadTransform.position, eyeTrackingProvider.AbsoluteEyeGaze * Vector3.forward, out rayHit, 10.0f, ~LayerMask.NameToLayer("EGHG")))
            {
                GameObject hitGameObject = rayHit.transform.gameObject;
                selectable = hitGameObject;
            }

            // Then try UI elements based on the event system
            if (!selectable)
            {
                PointerEventData pointerData = new PointerEventData(EventSystem.current);
                pointerData.position = camera.WorldToScreenPoint(eyeTrackingProvider.HeadTransform.position + eyeTrackingProvider.AbsoluteEyeGaze * Vector3.forward);

                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, results);
                foreach (var result in results)
                {
                    GameObject hitGameObject = result.gameObject;
                    var _selectable = hitGameObject;
                    if (_selectable && _selectable.layer == LayerMask.NameToLayer("EGHG"))
                    {
                        selectable = _selectable;
                    }
                }
            }


            if (selectable)
            {
                if (selectable != null && currentEyeFocus != selectable)
                {
                    currentEyeFocus?.SendMessage("EGHGOnEndEyeFocus", SendMessageOptions.DontRequireReceiver);
                    currentEyeFocus = selectable;
                    currentEyeFocus.SendMessage("EGHGOnStartEyeFocus", SendMessageOptions.DontRequireReceiver);
                    currentEyeGazeSelectionTime = Time.time;
                }
            }
            else
            {
                currentEyeFocus?.SendMessage("EGHGOnEndEyeFocus", SendMessageOptions.DontRequireReceiver);
                currentEyeFocus = null;
            }

            if (currentEyeFocus != null && selectable == currentEyeFocus)
            {
                delta = Time.time - currentEyeGazeSelectionTime;
                float deltaPct = delta / eyeGazeSelectionTime;

                eyeGazeMask.alphaCutoff = delta;

                if (deltaPct >= 1)
                {
                    currentEyeFocus?.SendMessage("EGHGOnSelection", SendMessageOptions.DontRequireReceiver);
                    audioSource.Play();
                    currentEyeFocus = null;
                    lastEyeGazeSelectionTime = Time.time;
                    eyeGazeMask.alphaCutoff = 1.0f;
                }
            }
        }
    }
}
