using UnityEngine;
using System.Collections;

namespace Completed {
	public class SoundManager : MonoBehaviour {
		public AudioSource efxSource;
		public AudioSource musicSource;

		public static SoundManager instance = null;	

		public float lowPitchRange = .95f;				//The lowest a sound effect will be randomly pitched.
		public float highPitchRange = 1.05f;			//The highest a sound effect will be randomly pitched.

		void Awake() {
			if(instance == null) {
				instance = this;
			}
			else if(instance != this) {
				Destroy(gameObject);
			}
			else {
				DontDestroyOnLoad(gameObject);
			}
		}

		public void PlaySingle(AudioClip clip) {
			efxSource.clip = clip;
			efxSource.Play();
		}

		public void RandomizeSfx(params AudioClip[] clips) {
			int randomIndex = Random.Range(0, clips.Length);
			float randomPitch = Random.Range(lowPitchRange, highPitchRange);
			efxSource.pitch = randomPitch;
			efxSource.clip = clips[randomIndex];

			efxSource.Play();
		}
	}
}