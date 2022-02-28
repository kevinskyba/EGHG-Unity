using System;
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
                /// <summary>
                /// Recording allows the device to save its <see cref="HeadGaze"/> and <see cref="EyeGaze"/> data
                /// to files.
                /// </summary>
                [Serializable]
                public class Recording
                {
                    public bool AutoStartRecord => autoStartRecord;
                    [SerializeField]
                    [Tooltip("Whether this provider should automatically start recording data")]
                    public bool autoStartRecord = false;
                }
                [SerializeField]
                [InspectorName("Recording")]
                private Recording recordingSettings;

                /// <summary>
                /// Whether the provider is currently recording.
                /// </summary>
                private bool isRecording;

                public bool IsRecording => isRecording;

                /// <summary>
                /// Container holding the <see cref="HeadGaze"/> and <see cref="EyeGaze"/> data for each frame.
                /// </summary>
                [Serializable]
                public class RecordingData
                {
                    [Serializable]
                    public struct RecordEntry
                    {
                        [SerializeField]
                        private Quaternion headGaze;
                        public Quaternion HeadGaze
                        {
                            get { return headGaze; }
                            set { headGaze = value; }
                        }

                        [SerializeField]
                        private Quaternion eyeGaze;
                        public Quaternion EyeGaze
                        {
                            get { return eyeGaze; }
                            set { eyeGaze = value; }
                        }

                        [SerializeField]
                        private Vector3 headWorldPosition;
                        public Vector3 HeadWorldPosition
                        {
                            get { return headWorldPosition; }
                            set { headWorldPosition = value; }
                        }
                    }
                    [SerializeField]
                    private List<RecordEntry> recordEntries = new List<RecordEntry>();
                    public List<RecordEntry> RecordEntries => recordEntries;

                    [SerializeField]
                    private double frameTime;
                    public double FrameTime 
                    {
                        get { return frameTime; }
                        set { frameTime = value; }
                    }
                }
                protected RecordingData recordingData;


                public void StartRecording()
                {
                    if (!isRecording)
                    {
                        logger.Log(typeof(EyeTrackingProvider).ToString(), "Start Recording");
                        isRecording = true;
                        recordingData = new RecordingData();
                        recordingData.FrameTime = Time.fixedDeltaTime;
                    }
                }

                public void StopRecording()
                {
                    if (isRecording)
                    {
                        logger.Log(typeof(EyeTrackingProvider).ToString(), "Stop Recording");
                        isRecording = false;
                        SaveRecording();
                    }
                }

                private void SaveRecording()
                {
                    // Find free file name
                    int suffix = 0;
                    string filePath = "";
                    do
                    {
                        filePath = Application.persistentDataPath + $"/recording-{suffix}.json";
                        suffix++;
                    } while (File.Exists(filePath) && suffix < 999);

                    // Save recording as json
                    StreamWriter file = File.CreateText(filePath);
                    string json = JsonUtility.ToJson(recordingData);
                    file.Write(json);
                    file.Close();

                    logger.Log(typeof(EyeTrackingProvider).ToString(), $"Saved Recording as {filePath}");
                }
            }
        }
    }
}