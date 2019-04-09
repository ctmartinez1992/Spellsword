using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Completed {
    public class LevelChanger : MonoBehaviour {
        public static LevelChanger instance = null;

        public Animator animator;
        public GameObject fader;

        public int nextScene;

        void Awake() {
            if(instance == null) {
                instance = this;
            }
        }

        public void FadeToMainMenu() {
            nextScene = 0;
            fader.SetActive(true);
            animator.SetTrigger("FadeOut");
        }

        public void FadeToTutorial() {
            GameManager.instance.callLevel = 20;
            nextScene = 1;
            fader.SetActive(true);
            animator.SetTrigger("FadeOut");
        }

        public void FadeToQuest() {
            GameManager.instance.callLevel = 0;
            nextScene = 2;
            fader.SetActive(true);
            animator.SetTrigger("FadeOut");
        }

        public void FadeToEndless() {
            GameManager.instance.callLevel = 0;
            nextScene = 3;
            fader.SetActive(true);
            animator.SetTrigger("FadeOut");
        }

        public void FadeToOptions() {
            nextScene = 4;
            fader.SetActive(true);
            animator.SetTrigger("FadeOut");
        }

        public void FadeToCredits() {
            nextScene = 5;
            fader.SetActive(true);
            animator.SetTrigger("FadeOut");
        }

        public void FadeToVictory() {
            nextScene = 6;
            fader.SetActive(true);
            animator.SetTrigger("FadeOut");
        }

        public void FadeToGameOver() {
            nextScene = 7;
            fader.SetActive(true);
            animator.SetTrigger("FadeOut");
        }

        public void OnFadeInComplete() {
            fader.SetActive(false);
        }

        public void OnFadeOutComplete() {
            SceneManager.LoadScene(nextScene);
        }
    }
}