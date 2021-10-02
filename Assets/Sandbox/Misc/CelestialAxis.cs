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
	public class CelestialAxis : MonoBehaviour
	{
		[SerializeField]
		Transform target;

		[SerializeField]
		internal float distance = 20;

		void OnValidate()
		{
			target.localPosition = Vector3.forward * distance;
		}
	}
}