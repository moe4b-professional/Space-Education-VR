using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Default
{
	public class CassettePlayer : MonoBehaviour
	{
		[SerializeField]
		private ClipsProperty clips;
		public ClipsProperty Clips => clips;
		[Serializable]
		public class ClipsProperty
		{
			[SerializeField]
			private AudioClip insert;
			public AudioClip Insert => insert;
			
			[SerializeField]
			private AudioClip button;
			public AudioClip Button => button;
			
			[SerializeField]
			private AudioClip eject;
			public AudioClip Eject => eject;
			
			[SerializeField]
			private AudioClip noise;
			public AudioClip Noise => noise;
		}

		[SerializeField]
		private SourcesProperty sources;
		public SourcesProperty Sources => sources;
		[Serializable]
		public class SourcesProperty
		{
			[SerializeField]
			private AudioSource clip;
			public AudioSource Clip => clip;
			
			[SerializeField]
			private AudioSource noise;
			public AudioSource Noise => noise;

			internal void Stop()
			{
				noise.Stop();
				clip.Stop();
			}
		}

		public AudioClip Current { get; private set; }

		public Coroutine Play(AudioClip clip)
		{
			Stop();

			Current = clip;

			return StartCoroutine(Procedure());
			IEnumerator Procedure()
			{
				yield return PlayClip(Clips.Insert, timeOffset: 0.2f);
				yield return PlayClip(Clips.Button, timeOffset: 0.2f);
				
				BeginNoise();
				yield return PlayClip(clip);
				EndNoise();
				
				yield return PlayClip(Clips.Eject);

				Current = null;
			}
		}

		public void Stop()
        {
			StopAllCoroutines();
			sources.Stop();
			Current = null;
		}

		private Coroutine PlayClip(AudioClip clip, float timeOffset = 0f)
		{
			return StartCoroutine(Procedure(clip, timeOffset));
			IEnumerator Procedure(AudioClip clip, float timeOffset)
			{
				var stamp = Time.time + clip.length - timeOffset;

				sources.Clip.PlayOneShot(clip);

				while (true)
				{
					yield return new WaitForEndOfFrame();
					
					if(Time.time >= stamp) break;
				}
			}
		}

		private void BeginNoise()
		{
			var source = sources.Noise;

			source.clip = Clips.Noise;
			source.loop = true;
			source.Play();
		}
		private void EndNoise()
		{
			var source = sources.Noise;

			source.Stop();
		}
	}
}