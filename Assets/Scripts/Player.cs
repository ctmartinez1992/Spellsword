using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace Completed {
	public class Player : MovingObject {
		public float restartLevelDelay = 1f;		//Delay time in seconds to restart level.
        
		public AudioClip moveSound1;
		public AudioClip moveSound2;
        public AudioClip attackSound1;
        public AudioClip attackSound2;
		public AudioClip eatSound1;
		public AudioClip eatSound2;
		public AudioClip drinkSound1;
		public AudioClip drinkSound2;
		public AudioClip gameOverSound;

        public int hp;                              //The health points of the player.
		public int fp;                              //Used to store player food points total during level.

        private int moveCountForFP;                 //Every 10 moves, decrease the player's food points.

        public int turnCount;

        public GameObject[] hearts;
        public GameObject[] foods;

        public bool lookSkillActivated;
        public bool fireSkillActivated;
        public bool shieldSkillActivated;
        private GameObject shieldOnGO;
        public bool flashSkillActivated;
        public bool ouroborosSkillActivated;

        private bool clickedDown = false;
        public GameObject selectorValid;
        public GameObject selectorInvalid;

        public List<GameObject> skillSelectors = new List<GameObject>();

        private GameObject lookPanelGO;
        
        protected override void Start() {
            base.Start();

            hp = GameManager.instance.playerHP;
            fp = GameManager.instance.playerFP;
            moveCountForFP = GameManager.instance.playerMoveCountForFP;

            lookSkillActivated = false;
            fireSkillActivated = false;
            shieldSkillActivated = false;
            flashSkillActivated = false;
            ouroborosSkillActivated = false;

            hearts[0] = GameObject.Find("Canvas/HP/Heart1Empty");   hearts[1] = GameObject.Find("Canvas/HP/Heart1Full");
            hearts[2] = GameObject.Find("Canvas/HP/Heart2Empty");   hearts[3] = GameObject.Find("Canvas/HP/Heart2Full");
            hearts[4] = GameObject.Find("Canvas/HP/Heart3Empty");   hearts[5] = GameObject.Find("Canvas/HP/Heart3Full");
            hearts[6] = GameObject.Find("Canvas/HP/Heart4Empty");   hearts[7] = GameObject.Find("Canvas/HP/Heart4Full");
            hearts[8] = GameObject.Find("Canvas/HP/Heart5Empty");   hearts[9] = GameObject.Find("Canvas/HP/Heart5Full");
            hearts[10] = GameObject.Find("Canvas/HP/Heart6Empty");  hearts[11] = GameObject.Find("Canvas/HP/Heart6Full");
            hearts[12] = GameObject.Find("Canvas/HP/Heart7Empty");  hearts[13] = GameObject.Find("Canvas/HP/Heart7Full");

            foods[2] = GameObject.Find("Canvas/FP/Food1Full"); foods[1] = GameObject.Find("Canvas/FP/Food1Half"); foods[0] = GameObject.Find("Canvas/FP/Food1Empty");
            foods[5] = GameObject.Find("Canvas/FP/Food2Full"); foods[4] = GameObject.Find("Canvas/FP/Food2Half"); foods[3] = GameObject.Find("Canvas/FP/Food2Empty");
            foods[8] = GameObject.Find("Canvas/FP/Food3Full"); foods[7] = GameObject.Find("Canvas/FP/Food3Half"); foods[6] = GameObject.Find("Canvas/FP/Food3Empty");
            foods[11] = GameObject.Find("Canvas/FP/Food4Full"); foods[10] = GameObject.Find("Canvas/FP/Food4Half"); foods[9] = GameObject.Find("Canvas/FP/Food4Empty");
            foods[14] = GameObject.Find("Canvas/FP/Food5Full"); foods[13] = GameObject.Find("Canvas/FP/Food5Half"); foods[12] = GameObject.Find("Canvas/FP/Food5Empty");
            foods[17] = GameObject.Find("Canvas/FP/Food6Full"); foods[16] = GameObject.Find("Canvas/FP/Food6Half"); foods[15] = GameObject.Find("Canvas/FP/Food6Empty");
            foods[20] = GameObject.Find("Canvas/FP/Food7Full"); foods[19] = GameObject.Find("Canvas/FP/Food7Half"); foods[18] = GameObject.Find("Canvas/FP/Food7Empty");

            ResetHealthUI();
            ResetFoodUI();

            lookPanelGO = GameObject.Find("Canvas/LookPanel");
            lookPanelGO.SetActive(false);
		}

		private void OnDisable() {
			//When Player object is disabled, store the current local food total in the GameManager so it can be re-loaded in next level.
			GameManager.instance.playerFP = fp;
            GameManager.instance.playerHP = hp;
            GameManager.instance.playerMoveCountForFP = moveCountForFP;
		}

        private void ProcessInputNormal() {
#if UNITY_STANDALONE || UNITY_WEBPLAYER
            if(clickedDown && Input.GetMouseButtonUp(0)) {
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
            if(clickedDown && Input.touches[0].phase == TouchPhase.Ended) {
#endif
                clickedDown = false;
                selectorValid.SetActive(false);
                selectorInvalid.SetActive(false);
            }

            if(!GameManager.instance.playersTurn) {
                return;
            }

            int horizontal = 0;
            int vertical = 0;

#if UNITY_STANDALONE || UNITY_WEBPLAYER
            if(Input.GetMouseButtonDown(0)) {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
            if(Input.touchCount > 0) {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.touches[0].position);
                RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
#endif

                if(hit != false && hit.collider != null) {
                    clickedDown = true;

                    int distance = Mathf.FloorToInt(Vector3Int.Distance(Vector3Int.RoundToInt(this.transform.position), Vector3Int.RoundToInt(hit.collider.transform.position)));

                    if(distance < 2) {
                        selectorValid.SetActive(true);
                        selectorValid.transform.position = new Vector3(hit.collider.transform.position.x, hit.collider.transform.position.y, 0);

                        float x = hit.collider.transform.position.x - this.transform.position.x;
                        float y = hit.collider.transform.position.y - this.transform.position.y;

                        horizontal = ((x == 0) ? 0 : ((x > 0) ? 1 : -1));
                        vertical = ((y == 0) ? 0 : ((y > 0) ? 1 : -1));

                        //TODO: If the player moves diagonally, and there's a wall blocking the player in the horizontal or vertical direction.
                        //The player will be blocked from movement, but it will accept it as a valid movement. Fix that, it shouldn't.
                    }
                    else {
                        selectorInvalid.SetActive(true);
                        selectorInvalid.transform.position = new Vector3(hit.collider.transform.position.x, hit.collider.transform.position.y, 0);
                    }
                }
            }

            if(horizontal != 0 || vertical != 0) {
                AttemptAttack<Enemy>(horizontal, vertical, true);
                AttemptMove<Wall>(horizontal, vertical, false);
            }
        }

		private void Update() {
            if(!busy) {
                if(lookSkillActivated) {
#if UNITY_STANDALONE || UNITY_WEBPLAYER
                if(Input.GetMouseButtonDown(0)) {
                    Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
                    if(Input.touchCount > 0) {
                        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.touches[0].position);
#endif
                        bool selectedCorrectly = false;

                        foreach(GameObject selector in skillSelectors) {
                            if(Mathf.RoundToInt(selector.transform.position.x) == Mathf.RoundToInt(pos.x) && Mathf.RoundToInt(selector.transform.position.y) == Mathf.RoundToInt(pos.y)) {
                                selectedCorrectly = true;
                            }
                        }

                        if(selectedCorrectly) {
                            Vector2 pos2D = new Vector2(pos.x, pos.y);
                            RaycastHit2D hit = Physics2D.Raycast(pos2D, Vector2.zero);
                            if(hit.collider != null) {
                                GameObject lookPanelTitleGO = lookPanelGO.transform.GetChild(0).gameObject;
                                GameObject lookPanelImageGO = lookPanelGO.transform.GetChild(1).gameObject;
                                GameObject lookPanelTextGO = lookPanelGO.transform.GetChild(2).gameObject;

                                Text lookPanelTitle = lookPanelTitleGO.GetComponent<Text>();
                                Image lookPanelImage = lookPanelImageGO.GetComponent<Image>();
                                Text lookPanelText = lookPanelTextGO.GetComponent<Text>();

                                //TODO: When all sprites have been added to the spritesheet, go to the sprite editor and rename each of them to something that makes sense,
                                //and then use those to dynamically load the sprites and assign the chosen one to the image in the look window.

                                Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/spritesheet");

                                if(hit.collider.name == "Floor1(Clone)" || hit.collider.name == "Floor2(Clone)" || hit.collider.name == "Floor3(Clone)" || hit.collider.name == "Floor4(Clone)" ||
                                   hit.collider.name == "Floor5(Clone)" || hit.collider.name == "Floor6(Clone)" || hit.collider.name == "Floor7(Clone)" || hit.collider.name == "Floor8(Clone)") {
                                    lookPanelTitle.text = "Floor";
                                    lookPanelText.text = "The floor of the room. You may walk them, one by one.";
                                    lookPanelImage.sprite = sprites[8];
                                }
                                else if(hit.collider.name == "Wall1(Clone)" || hit.collider.name == "Wall2(Clone)" || hit.collider.name == "Wall3(Clone)" || hit.collider.name == "Wall4(Clone)" ||
                                        hit.collider.name == "Wall5(Clone)" || hit.collider.name == "Wall6(Clone)" || hit.collider.name == "Wall7(Clone)" || hit.collider.name == "Wall8(Clone)") {
                                    lookPanelTitle.text = "Wood Obstacle";
                                    lookPanelText.text = "These obstacles block your path. You may use them for protection or destroy them either by force or fire.";
                                    lookPanelImage.sprite = sprites[8];
                                }
                                else if(hit.collider.name == "OuterWall1(Clone)" || hit.collider.name == "OuterWall2(Clone)" || hit.collider.name == "OuterWall3(Clone)") {
                                    lookPanelTitle.text = "Stone Wall";
                                    lookPanelText.text = "Strong, sturdy and stoney. Nothing gets past them.";
                                    lookPanelImage.sprite = sprites[8];
                                }
                                else if(hit.collider.name == "Exit") {
                                    lookPanelTitle.text = "Portal";
                                    lookPanelText.text = "A portal that will take you to the next room. Every time you pass one, you become a little bit stronger.";
                                    lookPanelImage.sprite = sprites[8];
                                }
                                else if(hit.collider.name == "Player") {
                                    lookPanelTitle.text = "Player";
                                    lookPanelText.text = "This is you. Avoid death, it will kill you.";
                                    lookPanelImage.sprite = sprites[8];
                                }
                                else if(hit.collider.name == "EnemyMelee") {
                                    lookPanelTitle.text = "Orc Warrior";
                                    lookPanelText.text = "Fat, lazy and dumb. They only move sometimes, unless you get too close...";
                                    lookPanelImage.sprite = sprites[8];
                                }
                                else if(hit.collider.name == "EnemyArcher") {
                                    lookPanelTitle.text = "Goblin Archer";
                                    lookPanelText.text = "Likes to not use his sword and has intimacy issues.\nThey have a range of 3 tiles and fire arrows in the 4 directions of the rose of winds.";
                                    lookPanelImage.sprite = sprites[8];
                                }
                                else if(hit.collider.name == "EnemyFlyer") {
                                    lookPanelTitle.text = "Vampire Bat";
                                    lookPanelText.text = "Appears to be utterly confused and unaware of its surroundings.\nThey can fly and reach further, but their bites hurt little.";
                                    lookPanelImage.sprite = sprites[8];
                                }
                                else if(hit.collider.name == "Food1") {
                                    lookPanelTitle.text = "Fish";
                                    lookPanelText.text = "A staple of basic survival. How did a fish get in here...?";
                                    lookPanelImage.sprite = sprites[8];
                                }
                                else if(hit.collider.name == "Food2") {
                                    lookPanelTitle.text = "Water";
                                    lookPanelText.text = "A secret magical rare potion that stops your body from dying.\nIt was created many ears ago by the old deity as a gift to the world.\nThe gods made water and they saw that it was good.";
                                    lookPanelImage.sprite = sprites[8];
                                }
                                else if(hit.collider.name == "Potion1") {
                                    lookPanelTitle.text = "Potion";
                                    lookPanelText.text = "Strawberries and Elderberries mushed and mixed with sewage. Heals even the deepest psychological wounds.";
                                    lookPanelImage.sprite = sprites[8];
                                }

                                //TODO: For every tile/enemy/item/etc. Add an if and change the title, image and text to match the selected game object.

                                lookPanelGO.SetActive(true);
                            }

                            Buttons.instance.OnClickButtonLookCancel();
                        }
                    }
                }
                else if(fireSkillActivated) {
#if UNITY_STANDALONE || UNITY_WEBPLAYER
                if(Input.GetMouseButtonDown(0)) {
                    Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
                    if(Input.touchCount > 0) {
                        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.touches[0].position);
#endif
                        bool selectedCorrectly = false;

                        foreach(GameObject selector in skillSelectors) {
                            if(Mathf.RoundToInt(selector.transform.position.x) == Mathf.RoundToInt(pos.x) && Mathf.RoundToInt(selector.transform.position.y) == Mathf.RoundToInt(pos.y)) {
                                selectedCorrectly = true;
                            }
                        }

                        if(selectedCorrectly) {
                            GameObject fireball = Instantiate(Resources.Load<GameObject>("Prefabs/Fireball")) as GameObject;
                            fireball.transform.position = new Vector3(Mathf.RoundToInt(this.transform.position.x), Mathf.RoundToInt(this.transform.position.y), 0);

                            Fireball fireballScript = fireball.GetComponent<Fireball>();
                            fireballScript.target = new Vector3(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), 0);

                            Buttons.instance.OnButtonFireEnterCooldown();

                            Invoke("EndTurn", 0.2f);
                        }
                    }
                }
                else if(flashSkillActivated) {
#if UNITY_STANDALONE || UNITY_WEBPLAYER
                if(Input.GetMouseButtonDown(0)) {
                    Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
                    if(Input.touchCount > 0) {
                        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.touches[0].position);
#endif
                        bool selectedCorrectly = false;

                        foreach(GameObject selector in skillSelectors) {
                            if(Mathf.RoundToInt(selector.transform.position.x) == Mathf.RoundToInt(pos.x) && Mathf.RoundToInt(selector.transform.position.y) == Mathf.RoundToInt(pos.y)) {
                                selectedCorrectly = true;
                            }
                        }

                        if(selectedCorrectly) {
                            this.transform.position = new Vector3(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), 0);

                            Buttons.instance.OnButtonFlashEnterCooldown();

                            Invoke("EndTurn", 0.2f);
                        }
                    }
                }
                else {
                    ProcessInputNormal();
                }
            }
        }

        public void OnLookSkillActivated() {
            for(int x = -1; x <= BoardManager.instance.columns; ++x) {
                for(int y = -1; y <= BoardManager.instance.rows; ++y) {
                    GameObject selector = Instantiate(Resources.Load<GameObject>("Prefabs/SelectorValid")) as GameObject;
                    selector.transform.position = new Vector3(x, y, 0);
                    skillSelectors.Add(selector);
                }
            }
        }

        public void OnFireSkillActivated() {
            int playerX = Mathf.RoundToInt(this.transform.position.x);
            int playerY = Mathf.RoundToInt(this.transform.position.y);

            for(int x = -3; x <= 3; ++x) {
                for(int y = -3; y <= 3; ++y) {
                    if((x == y || x + y == 0) && (x != 0 && y != 0)) {
                        int xPos = playerX + x;
                        int yPos = playerY + y;
                        if((xPos >= 0 && xPos < BoardManager.instance.columns) && (yPos >= 0 && yPos < BoardManager.instance.rows)) {
                            GameObject selector = Instantiate(Resources.Load<GameObject>("Prefabs/SelectorValid")) as GameObject;
                            selector.transform.position = new Vector3(xPos, yPos, 0);
                            skillSelectors.Add(selector);
                        }
                    }
                }
            }
        }

        public void OnShieldSkillActivated() {
            shieldOnGO = Instantiate(Resources.Load<GameObject>("Prefabs/Shield")) as GameObject;
            shieldOnGO.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 0);
            shieldOnGO.transform.SetParent(this.transform);
            
            Invoke("EndTurn", 0.2f);
        }

        public void OnFlashSkillActivated() {
            int playerX = Mathf.RoundToInt(this.transform.position.x);
            int playerY = Mathf.RoundToInt(this.transform.position.y);

            for(int x = -2; x <= 2; ++x) {
                for(int y = -2; y <= 2; ++y) {
                    if((x == 2 || x == -2 || y == 2 || y == -2) && (x != 0 && y != 0)) {
                        int xPos = playerX + x;
                        int yPos = playerY + y;
                        if((xPos >= 0 && xPos < BoardManager.instance.columns) && (yPos >= 0 && yPos < BoardManager.instance.rows)) {
                            GameObject selector = Instantiate(Resources.Load<GameObject>("Prefabs/SelectorValid")) as GameObject;
                            selector.transform.position = new Vector3(xPos, yPos, 0);
                            skillSelectors.Add(selector);
                        }
                    }
                }
            }
        }

        public void OnOuroborosSkillActivated() {
            GameObject ouroborosEffect = Instantiate(Resources.Load<GameObject>("Prefabs/OuroborosEffect")) as GameObject;
            ouroborosEffect.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 0);

            GainFood();
            GainFood();
            GainFood();
            GainFood();
            LoseHealth(false);

            Buttons.instance.OnButtonOuroborosEnterCooldown();
            
            Invoke("EndTurn", 0.2f);
        }

        private void ClearSelectors() {
            foreach(GameObject selector in skillSelectors) {
                Destroy(selector);
            }

            skillSelectors.Clear();
        }

        public void OnLookSkillDeactivatedOrCasted() {
            ClearSelectors();
        }
        public void OnFireSkillDeactivatedOrCasted() {
            ClearSelectors();
        }
        public void OnShieldSkillDeactivatedOrPopped() {
            Destroy(shieldOnGO);
        }
        public void OnFlashSkillDeactivatedOrCasted() {
            ClearSelectors();
        }
        public void OnOuroborosSkillDeactivatedOrCasted() {
            //Do nothing...
        }

        private void EndTurn() {
            turnCount++;
            GameManager.instance.playersTurn = false;
        }

        //AttemptMove overrides the AttemptMove function in the base class MovingObject
        //AttemptMove takes a generic parameter T which for Player will be of the type Wall, it also takes integers for x and y direction to move in.
        protected override bool AttemptMove<T>(int xDir, int yDir, bool flyer) {
            bool didItMove = base.AttemptMove<T>(xDir, yDir, flyer);
            
			if(didItMove) {
                moveCountForFP++;
                if(moveCountForFP == 10) {
                    moveCountForFP = 0;
                    LoseFood();
                }

                SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
			}

			CheckIfGameOver();

            Invoke("EndTurn", 0.2f);

            return didItMove;
		}

		//It takes a generic parameter T which in the case of Player is a Wall which the player can attack and destroy.
		protected override void OnCantMove<T>(T component) {
            Wall hitWall = component as Wall;
            if(hitWall.breakable) {
                hitWall.TakeDamage(false, "");
            }
        }

        protected override bool AttemptAttack<T>(int xDir, int yDir, bool motion) {
            bool didItAttack = base.AttemptAttack<T>(xDir, yDir, motion);
            
            if(didItAttack) {
                moveCountForFP++;
                if(moveCountForFP == 10) {
                    moveCountForFP = 0;
                    LoseFood();
                }

                SoundManager.instance.RandomizeSfx(attackSound1, attackSound2);
            }

            CheckIfGameOver();

            Invoke("EndTurn", 0.2f);

            return didItAttack;
        }
        
        protected override void OnCanAttack<T>(T component) {
            Enemy hitEnemy = component as Enemy;
            hitEnemy.TakeDamage("blood");
        }

        //OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
        private void OnTriggerEnter2D(Collider2D other) {
			if(other.tag == "Exit") {
                if(GameManager.isThisTutorial) {
                    Invoke("Restart", restartLevelDelay);
                    enabled = false;
                }
                else if(GameManager.isThisQuest) {
                    Invoke("Restart", restartLevelDelay);
                    enabled = false;
                }
                else if(GameManager.isThisEndless) {
                    Invoke("Restart", restartLevelDelay);
                    enabled = false;
                }
			}
			else if(other.tag == "Food1") {
                GainFood();

                SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);

				other.gameObject.SetActive(false);
            }
            else if(other.tag == "Food2") {
                GainFood();
                GainFood();

                SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);

                other.gameObject.SetActive(false);
            }
            else if(other.tag == "Potion1") {
                GainHealth();

                SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);

                other.gameObject.SetActive(false);
            }
            else if(other.tag == "EmeraldSword") {
                LevelChanger.instance.FadeToVictory();
            }
        }

        public void LoseHealth(bool animateBlood) {
            if(shieldSkillActivated) {
                Buttons.instance.OnButtonShieldEnterCooldown();
            }
            else if(hp > 0) {
                hearts[(hp * 2) - 1].SetActive(false);
                hp--;

                if(animateBlood) {
                    GameObject blood = Instantiate(Resources.Load<GameObject>("Prefabs/Blood")) as GameObject;
                    blood.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 0);
                    blood.transform.SetParent(this.transform);
                }

                CheckIfGameOver();
            }
        }

        public void GainHealth() {
            if(hp < 7) {
                hp++;
                hearts[(hp * 2) - 1].SetActive(true);
            }
        }

        //Given the current hp, it sets the correct amount of hearts on the UI.
        public void ResetHealthUI() {
            for(int i = 6; i > hp - 1; i--) {
                hearts[i * 2 + 1].SetActive(false);
            }
        }
        
        //                                     0, 1, 2, 3, 4, 5, 6, 7,  8,  9,  10, 11, 12, 13, 14
        private int[] indexArrayLoseFoodUI = { 0, 1, 2, 4, 5, 7, 8, 10, 11, 13, 14, 16, 17, 19, 20 };

        public void LoseFood() {
            if(fp > 0) {
                foods[indexArrayLoseFoodUI[fp]].SetActive(false);
                fp--;
            }

            CheckIfGameOver();
        }

        //                                     0, 1, 2, 3, 4, 5, 6,  7,  8,  9,  10, 11, 12, 13
        private int[] indexArrayGainFoodUI = { 1, 2, 4, 5, 7, 8, 10, 11, 13, 14, 16, 17, 19, 20 };

        public void GainFood() {
            if(fp < 14) {
                foods[indexArrayGainFoodUI[fp]].SetActive(true);
                fp++;
                moveCountForFP = 0;
            }
        }

        //Given the current fp, it sets the correct amount of food on the UI.
        public void ResetFoodUI() {
            for(int i = 14; i > fp; i--) {
                foods[indexArrayLoseFoodUI[i]].SetActive(false);
            }
        }

        private void Restart() {
			//Load the last scene loaded, in this case Main, the only scene in the game. And we load it in "Single" mode so it replace the existing one and not load all the scene object in the current scene.
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
		}

        private void CheckIfGameOver() {
			if(fp <= 0 || hp <= 0) {
				SoundManager.instance.PlaySingle(gameOverSound);
				SoundManager.instance.musicSource.Stop();
				GameManager.instance.GameOver();
			}
		}
	}
}