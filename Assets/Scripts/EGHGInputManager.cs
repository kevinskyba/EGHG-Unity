using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using KevinSkyba.EGHG.EyeTracking;

namespace KevinSkyba
{
    namespace EGHG
    {
        /// <summary>
        /// Eye Gaze / Head Gaze Input Manager
        /// Manages objects and their interactions.
        /// </summary>
        public class EGHGInputManager : MonoBehaviour
        {
            /// <summary>
            /// Reference to the camera used.
            /// </summary>
            [SerializeField]
            private new Camera camera;

            /// <summary>
            /// Reference to an EyeTrackingProvider in the current scene to be platform independent.
            /// </summary>
            private EyeTrackingProvider eyeTrackingProvider;

            /// <summary>
            /// Currently eye-gaze-focused object
            /// </summary>
            private EGHGSelectable currentFocus;

            /// <summary>
            /// Timestamp of the last time the <see cref="currentFocus"/> was focused.
            /// </summary>
            private float lastFocusTime;

            /// <summary>
            /// ...
            /// </summary>
            [SerializeField]
            [Min(0)]
            [Tooltip("")]
            private float looseFocusTime = 0.25f;


            private void Start()
            {
                var eyeTrackingProviders = FindObjectsOfType<EyeTrackingProvider>();
                foreach (var etp in eyeTrackingProviders)
                {
                    if (etp.isActiveAndEnabled)
                        eyeTrackingProvider = etp;
                }

                if (eyeTrackingProvider == null)
                {
                    throw new Exception("Missing EyeTrackingProvider in scene.");
                }

                if (camera == null)
                {
                    camera = Camera.main;
                }
            }

            private void FixedUpdate()
            {
                Debug.DrawRay(eyeTrackingProvider.HeadTransform.position, eyeTrackingProvider.HeadGaze * Vector3.forward * 5, Color.blue);
                Debug.DrawRay(eyeTrackingProvider.HeadTransform.position, eyeTrackingProvider.AbsoluteEyeGaze * Vector3.forward * 5, Color.red);

                /**
                * Update current focus
                */
                EGHGSelectable selectable = null;

                // First try 3d objects on layer EGHG
                RaycastHit rayHit;
                if (Physics.Raycast(eyeTrackingProvider.HeadTransform.position, eyeTrackingProvider.EyeGaze.eulerAngles, out rayHit, 10.0f, ~LayerMask.NameToLayer("EGHG")))
                {
                    GameObject hitGameObject = rayHit.transform.gameObject;
                    selectable = hitGameObject.GetComponent<EGHGSelectable>();
                }

                // Then try UI elements based on the event system
                if (!selectable)
                {
                    PointerEventData pointerData = new PointerEventData(EventSystem.current);
                    pointerData.position = camera.WorldToScreenPoint(eyeTrackingProvider.EyeGaze.eulerAngles);

                    List<RaycastResult> results = new List<RaycastResult>();
                    EventSystem.current.RaycastAll(pointerData, results);
                    foreach (var result in results)
                    {
                        GameObject hitGameObject = result.gameObject;
                        var _selectable = hitGameObject.GetComponent<EGHGSelectable>();
                        if (_selectable)
                        {
                            selectable = _selectable;
                        }
                    }
                }


                if (selectable)
                {
                    if (selectable != null && currentFocus != selectable)
                    {
                        currentFocus?.EGHGEndFocus();
                        currentFocus = selectable;
                        currentFocus.EGHGStartFocus();
                    }
                }


                // Update lastFocusTime because something was focused
                lastFocusTime = Time.time;

                /**
                 * Handle unfocus by missing eye gaze
                 */
                if (lastFocusTime + looseFocusTime > Time.time)
                {
                    // Automatically unfocus
                    currentFocus?.EGHGEndFocus();
                    currentFocus = null;
                }
            }
        }
    }
}