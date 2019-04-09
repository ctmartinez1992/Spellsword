using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;               //Required when using UI elements in scripts.

namespace Completed {
    public class Buttons : MonoBehaviour {
        public static Buttons instance = null;

        public GameObject playerGO;

        public GameObject buttonLookGO;
        public Button buttonLock;
        public Text buttonLockText;
        public GameObject buttonLookCancelGO;

        public GameObject buttonFireGO;
        public Button buttonFire;
        public Text buttonFireText;
        public GameObject buttonFireCancelGO;

        public GameObject buttonShieldGO;
        public Button buttonShield;
        public Text buttonShieldText;
        public GameObject buttonShieldOnGO;

        public GameObject buttonFlashGO;
        public Button buttonFlash;
        public Text buttonFlashText;
        public GameObject buttonFlashCancelGO;

        public GameObject buttonOuroborosGO;
        public Button buttonOuroboros;
        public Text buttonOuroborosText;

        public int buttonFireCD;
        public int buttonShieldCD;
        public int buttonFlashCD;
        public int buttonOuroborosCD;

        void Awake() {
            if(instance == null) {
                instance = this;
            }
        }

        void Start() {
            playerGO = GameObject.FindGameObjectWithTag("Player");

            buttonLookGO = GameObject.Find("Canvas/Skills/Look");
            buttonLookCancelGO = GameObject.Find("Canvas/Skills/LookCancel");
            buttonFireGO = GameObject.Find("Canvas/Skills/Fire");
            buttonFireCancelGO = GameObject.Find("Canvas/Skills/FireCancel");
            buttonShieldGO = GameObject.Find("Canvas/Skills/Shield");
            buttonShieldOnGO = GameObject.Find("Canvas/Skills/ShieldOn");
            buttonFlashGO = GameObject.Find("Canvas/Skills/Flash");
            buttonFlashCancelGO = GameObject.Find("Canvas/Skills/FlashCancel");
            buttonOuroborosGO = GameObject.Find("Canvas/Skills/Ouroboros");
        }

        public void OnClickButtonLookActivate() {
            buttonLookGO.SetActive(false);
            buttonLookCancelGO.SetActive(true);

            Player player = playerGO.GetComponent<Player>();
            player.lookSkillActivated = true;
            player.OnLookSkillActivated();
        }
        public void OnClickButtonFireActivate() {
            buttonFireGO.SetActive(false);
            buttonFireCancelGO.SetActive(true);

            Player player = playerGO.GetComponent<Player>();
            player.fireSkillActivated = true;
            player.OnFireSkillActivated();
        }
        public void OnClickButtonShieldActivate() {
            buttonShieldGO.SetActive(false);
            buttonShieldOnGO.SetActive(true);

            Player player = playerGO.GetComponent<Player>();
            player.shieldSkillActivated = true;
            player.OnShieldSkillActivated();
        }
        public void OnClickButtonFlashActivate() {
            buttonFlashGO.SetActive(false);
            buttonFlashCancelGO.SetActive(true);

            Player player = playerGO.GetComponent<Player>();
            player.flashSkillActivated = true;
            player.OnFlashSkillActivated();
        }
        public void OnClickButtonOuroborosActivate() {
            Player player = playerGO.GetComponent<Player>();
            player.ouroborosSkillActivated = true;
            player.OnOuroborosSkillActivated();
        }

        public void OnClickButtonLookCancel() {
            buttonLookGO.SetActive(true);
            buttonLookCancelGO.SetActive(false);

            Player player = playerGO.GetComponent<Player>();
            player.lookSkillActivated = false;
            player.OnLookSkillDeactivatedOrCasted();
        }
        public void OnClickButtonFireCancel() {
            buttonFireGO.SetActive(true);
            buttonFireCancelGO.SetActive(false);

            Player player = playerGO.GetComponent<Player>();
            player.fireSkillActivated = false;
            player.OnFireSkillDeactivatedOrCasted();
        }
        public void OnClickButtonShieldCancel() {
            buttonShieldGO.SetActive(true);
            buttonShieldOnGO.SetActive(false);

            Player player = playerGO.GetComponent<Player>();
            player.shieldSkillActivated = false;
            player.OnShieldSkillDeactivatedOrPopped();
        }
        public void OnClickButtonFlashCancel() {
            buttonFlashGO.SetActive(true);
            buttonFlashCancelGO.SetActive(false);

            Player player = playerGO.GetComponent<Player>();
            player.flashSkillActivated = false;
            player.OnFlashSkillDeactivatedOrCasted();
        }

        public void OnButtonFireEnterCooldown() {
            OnClickButtonFireCancel();

            buttonFire.interactable = false;
            buttonFireCD = 5;

            buttonFireText.text = buttonFireCD + "T";
        }
        public void OnButtonFireExitCooldown() {
            buttonFire.interactable = true;
            buttonFireText.text = "";
        }

        public void OnButtonShieldEnterCooldown() {
            OnClickButtonShieldCancel();

            buttonShield.interactable = false;
            buttonShieldCD = 10;

            buttonShieldText.text = buttonShieldCD + "T";
        }
        public void OnButtonShieldExitCooldown() {
            buttonShield.interactable = true;
            buttonShieldText.text = "";
        }

        public void OnButtonFlashEnterCooldown() {
            OnClickButtonFlashCancel();

            buttonFlash.interactable = false;
            buttonFlashCD = 5;

            buttonFlashText.text = buttonFlashCD + "T";
        }
        public void OnButtonFlashExitCooldown() {
            buttonFlash.interactable = true;
            buttonFlashText.text = "";
        }

        public void OnButtonOuroborosEnterCooldown() {
            buttonOuroboros.interactable = false;
            buttonOuroborosCD = 25;

            buttonFlashText.text = buttonFlashCD + "T";
        }
        public void OnButtonOuroborosExitCooldown() {
            buttonOuroboros.interactable = true;
            buttonOuroborosText.text = "";
        }

        public void Tick() {
            if(buttonFireCD > 0 && buttonFire.interactable == false) {
                buttonFireCD--;
                if(buttonFireCD == 0) {
                    OnButtonFireExitCooldown();
                }
                else {
                    buttonFireText.text = buttonFireCD + "T";
                }
            }
            if(buttonShieldCD > 0 && buttonShield.interactable == false) {
                buttonShieldCD--;
                if(buttonShieldCD == 0) {
                    OnButtonShieldExitCooldown();
                }
                else {
                    buttonShieldText.text = buttonShieldCD + "T";
                }
            }
            if(buttonFlashCD > 0 && buttonFlash.interactable == false) {
                buttonFlashCD--;
                if(buttonFlashCD == 0) {
                    OnButtonFlashExitCooldown();
                }
                else {
                    buttonFlashText.text = buttonFlashCD + "T";
                }
            }
            if(buttonOuroborosCD > 0 && buttonOuroboros.interactable == false) {
                buttonOuroborosCD--;
                if(buttonOuroborosCD == 0) {
                    OnButtonOuroborosExitCooldown();
                }
                else {
                    buttonOuroborosText.text = buttonOuroborosCD + "T";
                }
            }
        }
    }
}