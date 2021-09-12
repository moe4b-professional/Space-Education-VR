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
	public class AutoUIMaterial : MonoBehaviour
	{
		[SerializeField]
		Material target;

        void OnValidate()
        {
            Perform();
        }

        void Start()
        {
            Perform();
        }

        void Perform()
        {
            var graphics = GetComponentsInChildren<Graphic>(true);

            for (int i = 0; i < graphics.Length; i++)
                graphics[i].material = target;
        }
    }
}