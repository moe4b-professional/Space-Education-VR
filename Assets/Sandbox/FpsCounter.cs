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
	public class FpsCounter : MonoBehaviour
	{
		[SerializeField]
        Text label;

		float count;

		IEnumerator Start()
		{
			GUI.depth = 2;
			while (true)
			{
				if (Time.timeScale == 1)
				{
					yield return new WaitForSeconds(0.1f);
					count = (1 / Time.unscaledDeltaTime);
					label.text = "FPS :" + (Mathf.Round(count));
				}
				else
				{
					label.text = "Pause";
				}
				yield return new WaitForSeconds(0.5f);
			}
		}
	}
}