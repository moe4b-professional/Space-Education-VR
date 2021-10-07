using System;
using System.Collections;
using UnityEngine;

using UnityEngine.XR.Management;
using Google.XR.Cardboard;

namespace Default
{
	[DefaultExecutionOrder(DefaultExecutionOrder)]
	public class SpaceVRCore : MonoBehaviour
	{
		public const int DefaultExecutionOrder = -200;
		                                                                                                                
		public static SpaceVRCore Instance { get; private set; }
		
		[SerializeField]
		private FPSData targetFPS;
		[Serializable]
		public class FPSData
		{
			[SerializeField]
			private int rendering = 60;
			public int Rendering => rendering;

			[SerializeField]
			private int physics = 60;
			public int Physics => physics;
		}
		
		[SerializeField]
		private CassettePlayer cassettePlayer;
		public CassettePlayer CassettePlayer => cassettePlayer;
		
		public static bool IsInVR => XRGeneralSettings.Instance.Manager.isInitializationComplete;

        private void Awake()
        {
			Instance = this;
		}

        private void Start()
		{
			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = targetFPS.Rendering;
			Time.fixedDeltaTime = 1f / targetFPS.Physics;

			Screen.sleepTimeout = SleepTimeout.NeverSleep;
			Screen.brightness = 1.0f;
			
			//Disable Renderers Shadows & Motion Vectors
			{
				var renderers = FindObjectsOfType<Renderer>();

				for (var i = 0; i < renderers.Length; i++)
				{
					renderers[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
					renderers[i].receiveShadows = false;
					renderers[i].motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
				}
			}
			
			if(Application.isMobilePlatform)
				InitializeVR();
		}

        private void InitializeVR()
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

        private void Update()
		{
			if (IsInVR)
			{
				Api.UpdateScreenParams();
			}
		}
	}
}