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
            public class RecordingUI : MonoBehaviour
            {
                [SerializeField]
                private Toggle recordingToggle;

                /// <summary>
                /// Reference to an EyeTrackingProvider in the current scene to be platform independent.
                /// </summary>
                private EyeTrackingProvider eyeTrackingProvider;


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


                    if (!eyeTrackingProvider.IsRecording && recordingToggle.isOn)
                    {
                        recordingToggle.isOn = false;
                    }
                    else if (eyeTrackingProvider.IsRecording && !recordingToggle.isOn)
                    {
                        recordingToggle.isOn = true;
                    }
                }

                private void Update()
                {
                    if (!recordingToggle.isOn && eyeTrackingProvider.IsRecording)
                    {
                        eyeTrackingProvider.StopRecording();
                    }
                    else if (recordingToggle.isOn && !eyeTrackingProvider.IsRecording)
                    {
                        eyeTrackingProvider.StartRecording();
                    }

                    if (!eyeTrackingProvider.IsRecording && recordingToggle.isOn)
                    {
                        recordingToggle.isOn = false;
                    }
                    else if (eyeTrackingProvider.IsRecording && !recordingToggle.isOn)
                    {
                        recordingToggle.isOn = true;
                    }
                }
            }
        }
    }
}