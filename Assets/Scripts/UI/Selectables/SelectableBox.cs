
using KevinSkyba.EGHG.EyeTracking;
using System;
using UnityEngine;

namespace KevinSkyba.EGHG.UI.Selectables
{
    public class SelectableBox : Selectable
    {
        /// <summary>
        /// Reference to an EyeTrackingProvider in the current scene to be platform independent.
        /// </summary>
        private EyeTrackingProvider eyeTrackingProvider;

        [SerializeField]
        private Material selectedMaterial;
        private Material _selectedMaterial;
        private Color originalSelectedMaterialColor;

        [SerializeField]
        private Material unselectedMaterial;
        private Material _unselectedMaterial;
        private Color originalUnselectedMaterialColor;

        private MeshRenderer meshRenderer;

        private bool selected = false;

        private Vector3 velocity = Vector3.zero;
        private float smoothingTime = 0.5f;

        private void Start()
        {
            meshRenderer = GetComponent<MeshRenderer>();

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

            _selectedMaterial = Instantiate(selectedMaterial);
            _unselectedMaterial = Instantiate(unselectedMaterial);

            originalSelectedMaterialColor = selectedMaterial.color;
            originalUnselectedMaterialColor = unselectedMaterial.color;
        }

        public void EGHGOnStartFocus()
        {
            _selectedMaterial.color = originalSelectedMaterialColor * Color.gray;
            _unselectedMaterial.color = originalUnselectedMaterialColor * Color.gray;
        }

        public void EGHGOnEndFocus()
        {
            _selectedMaterial.color = originalSelectedMaterialColor;
            _unselectedMaterial.color = originalUnselectedMaterialColor;
        }

        public void EGHGOnSelection()
        {
            selected = !selected;

            if (selected)
            {
                meshRenderer.material = _selectedMaterial;
            }
            else
            {
                meshRenderer.material = _unselectedMaterial;
            }
        }

        private void Update()
        {
            if (selected)
            {
                // Target is 1m in front
                Vector3 target = eyeTrackingProvider.HeadTransform.position + eyeTrackingProvider.HeadTransform.forward * 1;
                transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, smoothingTime);
            }
        }
    }
}