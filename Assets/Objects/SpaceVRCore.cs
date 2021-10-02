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
using UnityEngine.XR.Management;
using Google.XR.Cardboard;

namespace Default
{
	public class SpaceVRCore : MonoBehaviour
	{
		[SerializeField]
		FPSData targetFPS;
		[Serializable]
		public class FPSData
        {
			[SerializeField]
            int rendering = 60;
            public int Rendering => rendering;

			[SerializeField]
            int physics = 60;
            public int Physics => physics;
        }

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
			Application.targetFrameRate = targetFPS.Rendering;
			Time.fixedDeltaTime = 1f / targetFPS.Physics;

			Screen.sleepTimeout = SleepTimeout.NeverSleep;
			Screen.brightness = 1.0f;

			switch (Application.platform)
			{
				case RuntimePlatform.Android:
				case RuntimePlatform.IPhonePlayer:
					InitializeVR();
					break;
			}

			//Disable Renderers Shadows & Motion Vectors
			{
				var renderers = FindObjectsOfType<Renderer>();

				foreach (var renderer in renderers)
				{
					renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
					renderer.receiveShadows = false;
					renderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
				}
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
}