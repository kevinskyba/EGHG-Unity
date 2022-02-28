using System;
using System.Collections.Generic;
using UnityEngine;

namespace KevinSkyba
{
    namespace EGHG
    {
        namespace EyeTracking
        {
            public class ReplayEyeTracker : EyeTrackingProvider
            {
                [SerializeField]
                private new Camera camera;


                private Quaternion lastGazePoint;
                private Quaternion lastHeadPose;


                public override Quaternion HeadGaze => lastHeadPose;

                public override Quaternion EyeGaze => lastGazePoint;

                public override Transform HeadTransform => camera.transform;

                [SerializeField]
                private TextAsset recordingJSON;

                [SerializeField]
                private uint from;

                [SerializeField]
                private uint to;

                [SerializeField]
                private bool playing;

                [SerializeField]
                private uint currentReplayIndex;
                private bool previousPlaying;

                protected override void Start()
                {
                    base.Start();
                    if (camera == null)
                    {
                        camera = Camera.main;
                    }
                }

                private void FixedUpdate()
                {
                    base.FixedUpdate();

                    if (recordingJSON == null) return;

                    if (playing && !previousPlaying)
                    {
                        // Changed to playing
                        currentReplayIndex = from;
                        recordingData = JsonUtility.FromJson<RecordingData>(recordingJSON.text);
                    }

                    if (playing)
                    {
                        if (recordingData.RecordEntries.Count > currentReplayIndex)
                        {
                            var entry = recordingData.RecordEntries[(int)currentReplayIndex];
                            lastHeadPose = entry.HeadGaze;
                            lastGazePoint = entry.EyeGaze;
                            camera.transform.position = entry.HeadWorldPosition;
                            camera.transform.rotation = entry.HeadGaze;
                        }

                        currentReplayIndex++;
                        if (currentReplayIndex > to)
                        {
                            currentReplayIndex = from;
                        }
                    }

                    previousPlaying = playing;
                }

                private void Update()
                {
                    
                }
            }
        }
    }
}