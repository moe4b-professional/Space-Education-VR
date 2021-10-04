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
	[RequireComponent(typeof(Canvas))]
	public class AutoCanvasCamera : MonoBehaviour
	{
        void Awake()
        {
            var canvas = GetComponent<Canvas>();
            canvas.worldCamera = Camera.main;
        }
    }
}