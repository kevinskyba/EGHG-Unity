using System;
using UnityEngine;
using UnityEngine.UI;
using KevinSkyba.EGHG.EyeTracking;

namespace KevinSkyba
{
    namespace EGHG
    {
        namespace UI
        {
            public class CanvasVisualizationUI : MonoBehaviour
            {
                [SerializeField]
                private new Camera camera;

                [SerializeField]
                private Transform eyeGaze;

                [SerializeField]
                private Transform headGaze;

                [SerializeField]
                private Toggle visualizationToggle;

                /// <summary>
                /// Reference to an EyeTrackingProvider in the current scene to be platform independent.
                /// </summary>
                private EyeTrackingProvider eyeTrackingProvider;

                private void Awake()
                {
                    if (Application.platform == RuntimePlatform.Lumin)
                    {
                        gameObject.SetActive(false);
                    }
                }

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

                private void Update()
                {
                    Vector3 eyePosition = camera.WorldToScreenPoint(
                        eyeTrackingProvider.HeadTransform.position + (eyeTrackingProvider.AbsoluteEyeGaze * Vector3.forward) * 5);
                    eyeGaze.transform.position = eyePosition;

                    Vector3 headPosition = camera.WorldToScreenPoint(
                        eyeTrackingProvider.HeadTransform.position + (eyeTrackingProvider.HeadGaze * Vector3.forward) * 5);
                    headGaze.transform.position = headPosition;

                    if (visualizationToggle.isOn)
                    {
                        eyeGaze.gameObject.SetActive(true);
                        headGaze.gameObject.SetActive(true);
                    }
                    else
                    {
                        eyeGaze.gameObject.SetActive(false);
                        headGaze.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}