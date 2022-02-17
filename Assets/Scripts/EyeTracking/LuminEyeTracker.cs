using UnityEngine;
#if PLATFORM_LUMIN
using UnityEngine.XR.MagicLeap;
#endif

namespace KevinSkyba
{
    namespace EGHG
    {
        namespace EyeTracking
        {
            public class LuminEyeTracker : EyeTrackingProvider
            {
                [SerializeField]
                private GameObject head;

                private Quaternion lastGazeDirection;
                private Quaternion lastHeadPose;

                public override Quaternion HeadGaze => lastHeadPose;

                public override Quaternion EyeGaze => lastGazeDirection;

                public override Transform HeadTransform => head.transform;

                /// <summary>
                /// Used to save the state of toggling recording.
                /// </summary>
                bool recordingToggle = false;

                private void Awake()
                {
#if PLATFORM_LUMIN
                    MLEyes.Start();
                    MLInput.Start();
#endif
                }

                protected override void Start()
                {
                    base.Start();
                    // Camera might not be available at Awake(), so do this at Start()
                    if (head == null)
                    {
                        head = Camera.main.gameObject;
                    }
                }


                private void OnDisable()
                {
#if PLATFORM_LUMIN
                    MLEyes.Stop();
                    MLInput.Stop();
#endif
                }

                private void Update()
                {
#if PLATFORM_LUMIN
                    if (MLEyes.IsStarted)
                    {
                        // Get eye and head gaze
                        lastHeadPose = Quaternion.LookRotation(head.transform.forward, Vector3.up).normalized;
                        lastGazeDirection =
                            Quaternion.LookRotation((MLEyes.FixationPoint - head.transform.position).normalized, Vector3.up).normalized
                            *
                            Quaternion.Inverse(Quaternion.LookRotation(head.transform.forward, Vector3.up).normalized);
                    }
                    
                    if (MLInput.GetController(MLInput.Hand.Left).IsBumperDown 
                        && MLInput.GetController(MLInput.Hand.Left).TriggerValue > 0.9f && !recordingToggle)
                    {
                        recordingToggle = true;
                        if (IsRecording)
                        {
                            StopRecording();
                        } else
                        {
                            StartRecording();
                        }
                    } else if (!MLInput.GetController(MLInput.Hand.Left).IsBumperDown && recordingToggle)
                    {
                        recordingToggle = false;
                    }
#endif
                }
            }
        }
    }
}