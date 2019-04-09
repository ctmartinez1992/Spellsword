using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Completed {
	using System.Collections.Generic;		//Allows us to use Lists. 
	using UnityEngine.UI;					//Allows us to use UI.

	public class GameManager : MonoBehaviour {
		public float levelStartDelay = 2f;						//Time to wait before starting level, in seconds.
		public float turnDelay = 0.25f;                         //Delay between each Player turn.

        public int playerFP = 14;                               //Starting value for Player food points.
        public int playerHP = 7;                                //Starting value for Player health points.
        public int playerMoveCountForFP = 0;                    //The current move count that controls the player's food.
        public int extraEnemies = 0;						    //Enemies that have went through the stairs, pass on to the next level.

        public static GameManager instance = null;				//Static instance of GameManager which allows it to be accessed by any other script.

		public bool playersTurn = true;		                    //Boolean to check if it's players turn, hidden in inspector but public.

		private Text levelText;									//Text to display current level number.
		private GameObject levelImage;							//Image to block out level as levels are being set up, background for levelText.

		private BoardManager boardScript;						//Store a reference to our BoardManager which will set up the level.
        public int callLevel = 0;                               //If 0 then endless mode, if 1 to 20 then quest levels, if 21 to 30 then tutorial levels.
		public int level = 1;									//Current level number, expressed in game as "Level 1". Only used in endless mode.

        public static bool isThisTutorial;
        public static bool isThisQuest;
        public static bool isThisEndless;

		private List<Enemy> enemies;							//List of all Enemy units, used to issue them move commands.
		private bool enemiesMoving;								//Boolean to check if enemies are moving.

		private bool doingSetup = true;							//Boolean to check if we're setting up board, prevent Player from moving during setup.

		void Awake() {
			if(instance == null) {
				instance = this;
			}
			else if(instance != this) {
				Destroy(gameObject);
			}

			DontDestroyOnLoad(gameObject);

			enemies = new List<Enemy>();
			boardScript = GetComponent<BoardManager>();
		}

        //This is called only once, and the parameter tell it to be called only after the scene was loaded.
        //Otherwise, our Scene Load callback would be called the very first load, and we don't want that.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static public void CallbackInitialization() {
            //Register the callback to be called everytime the scene is loaded.
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        //This is called each time a scene is loaded.
        static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1) {
            if(arg0.name == "Tutorial") {
                isThisTutorial = true;
                isThisQuest = false;
                isThisEndless = false;

                instance.callLevel++;
                instance.InitGame();
            }
            else if(arg0.name == "Quest") {
                isThisTutorial = false;
                isThisQuest = true;
                isThisEndless = false;

                instance.callLevel++;
                instance.InitGame();
            }
            else if(arg0.name == "Endless") {
                isThisTutorial = false;
                isThisQuest = false;
                isThisEndless = true;

                instance.level++;
                instance.InitGame();
            }
        }

		void InitGame() {
			doingSetup = true;

			levelImage = GameObject.Find("LevelChanger/Canvas/LevelImage");
			levelText = GameObject.Find("LevelChanger/Canvas/LevelImage/LevelText").GetComponent<Text>();

            if(callLevel == 21) levelText.text = "Movement";
            if(callLevel == 22) levelText.text = "Orcs";
            if(callLevel == 23) levelText.text = "Fireball";
            if(callLevel == 24) levelText.text = "Archers";
            if(callLevel == 25) levelText.text = "Shield";
            if(callLevel == 26) levelText.text = "Bats";
            if(callLevel == 27) levelText.text = "Wizards";
            if(callLevel == 28) levelText.text = "Flash";
            if(callLevel == 29) levelText.text = "Ouroboros";
            if(callLevel == 30) levelText.text = "Consumables";

            levelImage.SetActive(true);

			Invoke("HideLevelImage", levelStartDelay);

			enemies.Clear();
            if(callLevel == 0) {
                boardScript.SetupEndless(level);
            }
            else if(callLevel >= 1 && callLevel <= 20) {
                boardScript.SetupQuest(5);
            }
            else if(callLevel >= 21 && callLevel <= 30) {
                boardScript.SetupTutorial(callLevel);
            }
        }

		//Hides black image used between levels.
		void HideLevelImage() {
			levelImage.SetActive(false);
			doingSetup = false;
		}

		void Update() {
			if(playersTurn || enemiesMoving || doingSetup) {
				return;
			}

			StartCoroutine(MoveEnemies());
            Buttons.instance.Tick();
		}

		public void AddEnemyToList(Enemy script) {
			enemies.Add(script);
		}

		public void GameOver() {
			levelText.text = "After " + level + " days, you starved.";
			levelImage.SetActive(true);
			enabled = false;
		}
		
		//Coroutine to move enemies in sequence.
		IEnumerator MoveEnemies() {
			enemiesMoving = true;
			
			//Wait for turnDelay seconds, defaults to .1 (100 ms).
			yield return new WaitForSeconds(turnDelay);

			if(enemies.Count == 0) {
				//Wait for turnDelay seconds between moves, replaces delay caused by enemies moving when there are none.
				yield return new WaitForSeconds(turnDelay);
			}

            for(int i = 0; i < enemies.Count; ++i) {
                if(enemies[i].gameObject.activeSelf) {
                    enemies[i].MoveEnemy();
                }
				
				//Wait for Enemy's moveTime before moving next Enemy, 
				yield return new WaitForSeconds(enemies[i].moveTime);
			}

			playersTurn = true;
			enemiesMoving = false;
		}
	}
}