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
        Vector3 offset = Vector3.zero;

        Camera camera;

        void Awake()
        {
            camera = Camera.main;
        }

        void Update()
        {
            var direction = (transform.position - camera.transform.position);

            var rotation = Quaternion.LookRotation(direction, Vector3.up);

            transform.rotation = rotation;
        }
    }
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
}