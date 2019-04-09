using UnityEngine;
using System.Collections;

namespace Completed {	
	public class Loader : MonoBehaviour {
		public GameObject gameManager;
		public GameObject soundManager;

		void Awake() {
            Camera.main.orthographicSize = Screen.height / 24 / 2;
            
            if(GameManager.instance == null) {
				Instantiate(gameManager);
			}
			if(SoundManager.instance == null) {
				Instantiate(soundManager);
			}
		}
	}
}