using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Default
{
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
	public class LookAtCamera : MonoBehaviour
    {
        [SerializeField]
        private Vector3 offset = Vector3.zero;

        private Camera camera => Camera.main;

        void LateUpdate()
        {
            if(camera)
            {
                var target = camera.transform;

                var direction = (transform.position - target.position);

                var up = Vector3.Scale(target.up, Vector3.up);
                var rotation = Quaternion.LookRotation(direction, up);

                transform.rotation = rotation;
            }
        }
    }
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
}