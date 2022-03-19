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
        public enum Mode
        {
            EGHG,
            EyeGaze
        };

        [Header("General")]
        [SerializeField]
        private Mode mode;

        /// <summary>
        /// Reference to the camera used.
        /// </summary>
        [SerializeField]
        private new Camera camera;

        [SerializeField]
        private SpriteMask eyeGazeMask;

        private AudioSource audioSource;

        /// <summary>
        /// Reference to an EyeTrackingProvider in the current scene to be platform independent.
        /// </summary>
        private EyeTrackingProvider eyeTrackingProvider;

        /// <summary>
        /// Currently eye-gaze-focused object
        /// </summary>
        private GameObject currentEyeFocus;

        /// <summary>
        /// Currently head-gaze-focused object
        /// </summary>
        private GameObject currentHeadFocus;

        /// <summary>
        /// Currently eghg-focused object
        /// </summary>
        private GameObject currentFocus;

        /// <summary>
        /// Timestamp of the last time the <see cref="currentEyeFocus"/> was focused.
        /// </summary>
        private float lastEyeFocusTime;

        /// <summary>
        /// Timestamp of the last time the <see cref="currentHeadFocus"/> was focused.
        /// </summary>
        private float lastHeadFocusTime;

        /// <summary>
        /// Timestamp of the last time the <see cref="currentFocus"/> was focused.
        /// </summary>
        private float lastFocusTime;

        /// <summary>
        /// Timestamp of the last time the <see cref="currentFocus"/> was focused.
        /// </summary>
        private float firstFocusTime;

        /// <summary>
        /// 
        /// </summary>
        [Header("EGHG")]
        [SerializeField]
        [Min(0)]
        [Tooltip("")]
        private float looseFocusTime = 0.5f;

        /// <summary>
        /// Timestamp of the last time selection was triggered.
        /// </summary>
        private float lastSelectionTime;

        private float requiredFocusTime = 0.5f;

        /// <summary>
        /// ...
        /// </summary>
        [SerializeField]
        [Min(0)]
        [Tooltip("")]
        private float selectCooldownTime = 0.75f;

        [SerializeField]
        private bool enablePandasConnector = false;

        [SerializeField]
        private PandasConnector.Settings connectorSettings;

        private EGHG eghg;
        private PandasConnector pandasConnector;

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

            audioSource = GetComponent<AudioSource>();

            eghg = new EGHG(200, new EGHG.EGHGParameters()
            {
                movingAverageWindow = 6,

                fixationLookback = 15,
                fixationRange = 3,

                selectionFixation = 0.7f,
                selectionTolerance = 125,
                selectionTargetAcceleration = 450,
                selectionMaxAcceleration = 1200,
                selectionLookback = 15,
                selectionQuantile = 0.5f,

                selectionDistance = 5
            });

            if (enablePandasConnector)
            {
                pandasConnector = new PandasConnector(connectorSettings, new Dictionary<string, object>()
                {
                    { "selectionFixation", eghg.Parameters.selectionFixation },
                    { "selectionTolerance", eghg.Parameters.selectionTolerance },
                    { "movingAverageWindow", eghg.Parameters.movingAverageWindow },
                    { "fixationLookback", eghg.Parameters.fixationLookback },
                    { "selectionTargetAcceleration", eghg.Parameters.selectionTargetAcceleration }
                });
                pandasConnector.RegisterColumn(FixationDataProvider.FIXATION_KEY, PandasConnector.ColumnType.Float64);
                pandasConnector.RegisterColumn(SelectionDataProvider.SELECTION_KEY, PandasConnector.ColumnType.Bool);
                pandasConnector.RegisterColumn(SelectionDataProvider.X_ACCELERATION_MEANS_KEY, PandasConnector.ColumnType.Float64);
                pandasConnector.RegisterColumn(AccelerationDataProvider.ACCELERATION_EYE_HEAD_KEY, PandasConnector.ColumnType.Vector2);
            }
        }

        private void LateUpdate()
        {
            if (mode == Mode.EGHG)
                FixedUpdateEGHG();
            else
                FixedUpdateEyeGaze();
        }

        private void FixedUpdateEGHG()
        {
            Debug.DrawRay(eyeTrackingProvider.HeadTransform.position, eyeTrackingProvider.HeadGaze * Vector3.forward * 5, Color.blue);
            Debug.DrawRay(eyeTrackingProvider.HeadTransform.position, eyeTrackingProvider.AbsoluteEyeGaze * Vector3.forward * 5, Color.red);

            eghg.AddDataSet(eyeTrackingProvider.HeadGaze, eyeTrackingProvider.EyeGaze, eyeTrackingProvider.HeadTransform.position);

            eyeGazeMask.transform.position = eyeTrackingProvider.HeadTransform.position + eyeTrackingProvider.AbsoluteEyeGaze * Vector3.forward * 0.5f;

            if (enablePandasConnector)
            {
                pandasConnector.WriteColumnValue(FixationDataProvider.FIXATION_KEY, eghg.DataSets[0].GetData<float>(FixationDataProvider.FIXATION_KEY));
                pandasConnector.WriteColumnValue(SelectionDataProvider.SELECTION_KEY, eghg.DataSets[0].GetData<bool>(SelectionDataProvider.SELECTION_KEY));
                pandasConnector.WriteColumnValue(SelectionDataProvider.X_ACCELERATION_MEANS_KEY, eghg.DataSets[0].GetData<float>(SelectionDataProvider.X_ACCELERATION_MEANS_KEY));
                pandasConnector.WriteColumnValue(AccelerationDataProvider.ACCELERATION_EYE_HEAD_KEY, eghg.DataSets[0].GetData<Vector2>(AccelerationDataProvider.ACCELERATION_EYE_HEAD_KEY));
                pandasConnector.Send();
            }

            /**
             * Focus
             */

            UpdateEyeFocus();
            UpdateHeadFocus();

            // Handle overall focus
            if (currentEyeFocus != null && currentHeadFocus != null && currentHeadFocus == currentEyeFocus)
            {
                if (currentHeadFocus != currentFocus)
                {
                    currentFocus?.SendMessage("EGHGOnEndFocus", SendMessageOptions.DontRequireReceiver);
                    currentFocus = currentEyeFocus;
                    currentFocus.SendMessage("EGHGOnStartFocus", SendMessageOptions.DontRequireReceiver);
                    firstFocusTime = Time.time;

                    eghg.Reset();
                }

                lastFocusTime = Time.time;
            }

            if (lastFocusTime + looseFocusTime < Time.time)
            {
                // Update lastFocusTime because something was focused
                lastHeadFocusTime = Time.time;
                currentFocus?.SendMessage("EGHGOnEndFocus", SendMessageOptions.DontRequireReceiver);
                currentFocus = null;
            }

            /**
             * Selection
             */

            // Check EGHG selection
            if (currentFocus != null && firstFocusTime + requiredFocusTime < Time.time && eghg.DataSets[0].GetData<bool>(SelectionDataProvider.SELECTION_KEY))
            {
                // Check if enough time has passed since last time
                if (lastSelectionTime + selectCooldownTime < Time.time)
                {
                    currentFocus.SendMessage("EGHGOnSelection", SendMessageOptions.DontRequireReceiver);
                    lastSelectionTime = Time.time;
                    firstFocusTime = Time.time;

                    eghg.Reset();
                    audioSource.Play();

                    StartCoroutine(ResetEGHG());
                }
            }
        }

        private IEnumerator ResetEGHG()
        {
            yield return new WaitForSeconds(selectCooldownTime);
            eghg.Reset();
        }

        private void UpdateEyeFocus()
        {
            /**
            * Update current focus
            */
            GameObject selectable = null;

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
                }
            }

            if (currentEyeFocus != null && selectable == currentEyeFocus)
            {
                // Update lastFocusTime because something was focused
                lastEyeFocusTime = Time.time;
            }

            /**
             * Handle unfocus by focus time
             */
            if (lastEyeFocusTime + looseFocusTime < Time.time)
            {
                // Automatically unfocus
                currentEyeFocus?.SendMessage("EGHGOnEndEyeFocus", SendMessageOptions.DontRequireReceiver);
                currentEyeFocus = null;
            }
        }

        private void UpdateHeadFocus()
        {
            /**
            * Update current focus
            */
            GameObject selectable = null;

            // First try 3d objects on layer EGHG
            RaycastHit rayHit;
            if (Physics.Raycast(eyeTrackingProvider.HeadTransform.position, eyeTrackingProvider.HeadGaze * Vector3.forward, out rayHit, 10.0f, ~LayerMask.NameToLayer("EGHG")))
            {
                GameObject hitGameObject = rayHit.transform.gameObject;
                selectable = hitGameObject;
            }

            // Then try UI elements based on the event system
            if (!selectable)
            {
                PointerEventData pointerData = new PointerEventData(EventSystem.current);
                pointerData.position = camera.WorldToScreenPoint(eyeTrackingProvider.HeadTransform.position + eyeTrackingProvider.HeadGaze * Vector3.forward);

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
                if (selectable != null && currentHeadFocus != selectable)
                {
                    currentHeadFocus?.SendMessage("EGHGOnEndHeadFocus", SendMessageOptions.DontRequireReceiver);
                    currentHeadFocus = selectable;
                    currentHeadFocus.SendMessage("EGHGOnStartHeadFocus", SendMessageOptions.DontRequireReceiver);
                }
            }

            if (currentHeadFocus != null && selectable == currentHeadFocus)
            {
                // Update lastFocusTime because something was focused
                lastHeadFocusTime = Time.time;
            }

            /**
             * Handle unfocus by focus time
             */
            if (lastHeadFocusTime + looseFocusTime < Time.time)
            {
                // Automatically unfocus
                currentHeadFocus?.SendMessage("EGHGOnEndHeadFocus", SendMessageOptions.DontRequireReceiver);
                currentHeadFocus = null;
            }
        }
    }
}
