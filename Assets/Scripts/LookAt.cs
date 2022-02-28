using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KevinSkyba.EGHG
{
    public class LookAt : MonoBehaviour
    {
        [SerializeField]
        private GameObject reference;

        private void LateUpdate()
        {
            if (reference == null) return;

            transform.LookAt(reference.transform);
        }
    }
}
