using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KevinSkyba.EGHG
{
    public class StayInFront : MonoBehaviour
    {
        [SerializeField]
        private float distance;

        [SerializeField]
        private GameObject reference;

        private void LateUpdate()
        {
            if (reference == null) return;

            transform.position = reference.transform.position + reference.transform.forward * distance;
        }
    }
}
