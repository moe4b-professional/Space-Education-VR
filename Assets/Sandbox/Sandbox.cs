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

using Google.XR.Cardboard;
using UnityEngine.XR.Management;
using UnityEngine.InputSystem;

namespace Default
{
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
	public class Sandbox : MonoBehaviour
	{
		public static bool IsInVR
        {
			get
            {
				return XRGeneralSettings.Instance.Manager.isInitializationComplete;
			}
        }

		void Start()
		{
			QualitySettings.vSyncCount = 0;
			//Application.targetFrameRate = 60;
			Time.fixedDeltaTime = 1f / 60;

			Screen.sleepTimeout = SleepTimeout.NeverSleep;
			Screen.brightness = 1.0f;

			switch (Application.platform)
			{
				case RuntimePlatform.Android:
				case RuntimePlatform.IPhonePlayer:
					InitializeVR();
					break;
			}
		}

		void InitializeVR()
		{
			StartCoroutine(Coroutine());
			IEnumerator Coroutine()
			{
				var xrManager = XRGeneralSettings.Instance.Manager;

				if (xrManager.isInitializationComplete == false)
					yield return xrManager.InitializeLoader();

				xrManager.StartSubsystems();

				Api.Recenter();
			}
		}

		void Update()
		{
			if (IsInVR)
			{
				Api.UpdateScreenParams();
			}
		}
	}
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
}