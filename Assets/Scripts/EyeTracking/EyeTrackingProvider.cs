using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace KevinSkyba
{
    namespace EGHG
    {
        namespace EyeTracking
        {
            public abstract partial class EyeTrackingProvider : MonoBehaviour
            {
                protected Logger logger = new Logger(new LogHandler());

                /// <summary>
                /// Provides head gaze information as a world direction vector with origin <see cref="HeadTransform"/>.
                /// </summary>
                public abstract Quaternion HeadGaze { get; }

                /// <summary>
                /// Provides eye gaze information as a direction vector relative to <see cref="HeadGaze"/> with origin <see cref="HeadTransform"/>.
                /// </summary>
                public abstract Quaternion EyeGaze { get; }

                /// <summary>
                /// Provides eye gaze information as a world direction vector with origin <see cref="HeadTransform"/>.
                /// </summary>
                public Quaternion AbsoluteEyeGaze
                {
                    get
                    {
                        return HeadGaze * EyeGaze;
                    }
                }

                /// <summary>
                /// The transform where <see cref="HeadGaze"/> and <see cref="EyeGaze"/> originate from.
                /// </summary>
                public abstract Transform HeadTransform { get; }

                protected virtual void Start()
                {
                    if (recordingSettings.AutoStartRecord)
                    {
                        StartRecording();
                    }

                    if (streamingSettings.AutoStartStreaming)
                    {
                        StartStreaming();
                    }
                }

                protected virtual void OnDestroy()
                {
                    StopRecording();
                    StopStreaming();
                }


                protected virtual void FixedUpdate()
                {
                    var currentData = new RecordingData.RecordEntry()
                    {
                        EyeGaze = EyeGaze,
                        HeadGaze = HeadGaze,
                        HeadWorldPosition = HeadTransform.position
                    };

                    if (isRecording)
                    {
                        recordingData.RecordEntries.Add(currentData);
                    }

                    if (isStreaming)
                    {
                        StreamData(currentData);
                    }
                }
            }
        }
    }
}