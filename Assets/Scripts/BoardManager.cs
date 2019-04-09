using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Completed {
	public class BoardManager : MonoBehaviour {
        public static BoardManager instance = null;

        //Using Serializable allows us to embed a class with sub properties in the inspector.
        [Serializable]
		public class Count {
			public int minimum;
			public int maximum;

			public Count(int min, int max) {
				minimum = min;
				maximum = max;
			}
		}
        
		public Count wallCount = new Count (2, 4);
        public Count foodCount = new Count(1, 1);
        public Count potionCount = new Count(1, 1);
        public Count enemyMeleeCount = new Count(1, 1);
        public Count enemyArcherCount = new Count(10, 10);

        public GameObject exit;
		public GameObject[] floorTiles;
		public GameObject[] wallTiles;
        public GameObject[] foodTiles;                      
        public GameObject[] potionTiles;
        public GameObject enemyMeleeTile;
        public GameObject enemyArcherTile;
        public GameObject enemyFlyerTile;
        public GameObject enemyWizardTile;
        public GameObject[] outerWallTiles;
        public GameObject emeraldSword;
		
		private Transform boardHolder;									//A variable to store a reference to the transform of our Board object.
		private List <Vector3> gridPositions = new List <Vector3> ();	//A list of possible locations to place tiles.

        [HideInInspector] public int columns;
        [HideInInspector] public int rows;

        void Awake() {
            if(instance == null) {
                instance = this;
            }
        }
        
        void InitialiseList(int columns, int rows) {
			gridPositions.Clear();
			for(int x = 1; x < columns - 1; ++x) {
				for(int y = 1; y < rows - 1; ++y) {
					gridPositions.Add(new Vector3(x, y, 0f));
				}
			}
		}
        
		void BoardSetup(int columns, int rows) {
			boardHolder = new GameObject("Board").transform;

			for(int x = -1; x < columns + 1; ++x) {
				for(int y = -1; y < rows + 1; ++y) {
					GameObject toInstantiate = floorTiles[Random.Range (0, floorTiles.Length)];
					
					//Check if we current position is at board edge, if so choose a random outer wall prefab from our array of outer wall tiles.
					if(x == -1 || x == columns || y == -1 || y == rows) {
						toInstantiate = outerWallTiles[Random.Range (0, outerWallTiles.Length)];
					}

					GameObject instance = Instantiate(toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
					instance.transform.SetParent(boardHolder);
				}
			}
		}

		//RandomPosition returns a random position from our list gridPositions.
		Vector3 RandomPosition() {
			int randomIndex = Random.Range(0, gridPositions.Count);
			Vector3 randomPosition = gridPositions[randomIndex];
			gridPositions.RemoveAt(randomIndex);
			return randomPosition;
		}
        
        void LayoutObjectsAtRandom(GameObject[] tileArray, int minimum, int maximum) {
            int objectCount = Random.Range(minimum, maximum + 1);
            for(int i = 0; i < objectCount; ++i) {
                Vector3 randomPosition = RandomPosition();
                GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
                Instantiate(tileChoice, randomPosition, Quaternion.identity);
            }
        }

        void LayoutObjectAtRandom(GameObject tile, int minimum, int maximum) {
            int objectCount = Random.Range(minimum, maximum + 1);
            for(int i = 0; i < objectCount; ++i) {
                Vector3 randomPosition = RandomPosition();
                Instantiate(tile, randomPosition, Quaternion.identity);
            }
        }

        //SetupScene initializes our level and calls the previous functions to lay out the game board.
        public void SetupEndless(int level) {
            columns = 11;
            rows = 15;

			BoardSetup(columns, rows);
			InitialiseList(columns, rows);

			//LayoutObjectsAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
            //LayoutObjectsAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);
            //LayoutObjectsAtRandom(potionTiles, potionCount.minimum, potionCount.maximum);

            int enemyMeleeCount = (int)Mathf.Log(level - 2, 2f) + GameManager.instance.extraEnemies;
            int enemyArcherCount = (int)Mathf.Log(level, 2f) + GameManager.instance.extraEnemies;
            int enemyWizardCount = (int)Mathf.Log(level + 2, 2f) + GameManager.instance.extraEnemies;
            int enemyFlyerCount = (int)Mathf.Log(level + 4, 2f) + GameManager.instance.extraEnemies;

            if(enemyWizardCount < 0)    enemyWizardCount = 0;
            if(enemyFlyerCount < 0)     enemyFlyerCount = 0;
            if(enemyMeleeCount < 0)     enemyMeleeCount = 0;
            if(enemyArcherCount < 0)    enemyArcherCount = 0;

            //LayoutObjectAtRandom(enemyWizardTile, enemyWizardCount, enemyWizardCount);
            //LayoutObjectAtRandom(enemyFlyerTile, enemyFlyerCount, enemyFlyerCount);
            //LayoutObjectAtRandom(enemyMeleeTile, enemyMeleeCount, enemyMeleeCount);
            //LayoutObjectAtRandom(enemyArcherTile, enemyArcherCount, enemyArcherCount);

            Vector3 randomPosition = RandomPosition();
            Instantiate(emeraldSword, new Vector3(5, 5, 0), Quaternion.identity);

            GameManager.instance.extraEnemies = 0;

            Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
        }

        //Movement.
        public void SetupTutorial1() {
            columns = 9;
            rows = 13;

            BoardSetup(columns, rows);

            GameObject playerGO = GameObject.Find("Player");
            playerGO.transform.position = new Vector3(1, 4);

            Instantiate(exit, new Vector3(7, 4, 0f), Quaternion.identity);
        }

        //Orcs.
        public void SetupTutorial2() {
            columns = 9;
            rows = 13;

            BoardSetup(columns, rows);

            Instantiate(enemyMeleeTile, new Vector3(2, 2, 0), Quaternion.identity);
            Instantiate(enemyMeleeTile, new Vector3(5, 5, 0), Quaternion.identity);

            GameObject playerGO = GameObject.Find("Player");
            playerGO.transform.position = new Vector3(1, 1);

            Instantiate(exit, new Vector3(7, 7, 0f), Quaternion.identity);
        }

        //Fireball.
        public void SetupTutorial3() {
            columns = 9;
            rows = 13;

            BoardSetup(columns, rows);
            
            Instantiate(enemyMeleeTile, new Vector3(4, 4, 0), Quaternion.identity);
            Instantiate(enemyMeleeTile, new Vector3(6, 2, 0), Quaternion.identity);
            Instantiate(enemyMeleeTile, new Vector3(2, 6, 0), Quaternion.identity);

            GameObject playerGO = GameObject.Find("Player");
            playerGO.transform.position = new Vector3(1, 1);

            Instantiate(exit, new Vector3(7, 7, 0f), Quaternion.identity);
        }

        //Archers.
        public void SetupTutorial4() {
            columns = 9;
            rows = 13;

            BoardSetup(columns, rows);

            Instantiate(enemyArcherTile, new Vector3(8, 4, 0), Quaternion.identity);

            GameObject playerGO = GameObject.Find("Player");
            playerGO.transform.position = new Vector3(1, 1);

            Instantiate(exit, new Vector3(1, 8, 0f), Quaternion.identity);
        }

        //Shield.
        public void SetupTutorial5() {
            columns = 9;
            rows = 13;

            BoardSetup(columns, rows);

            Instantiate(enemyArcherTile, new Vector3(8, 4, 0), Quaternion.identity);
            Instantiate(enemyArcherTile, new Vector3(7, 5, 0), Quaternion.identity);

            GameObject playerGO = GameObject.Find("Player");
            playerGO.transform.position = new Vector3(1, 1);

            Instantiate(exit, new Vector3(1, 8, 0f), Quaternion.identity);
        }

        //Bats.
        public void SetupTutorial6() {
            columns = 9;
            rows = 13;

            BoardSetup(columns, rows);

            Instantiate(enemyFlyerTile, new Vector3(2, 5, 0), Quaternion.identity);

            GameObject playerGO = GameObject.Find("Player");
            playerGO.transform.position = new Vector3(1, 1);

            Instantiate(exit, new Vector3(8, 1, 0f), Quaternion.identity);
        }

        //Wizards.
        public void SetupTutorial7() {
            columns = 9;
            rows = 13;

            BoardSetup(columns, rows);

            Instantiate(enemyWizardTile, new Vector3(6, 4, 0), Quaternion.identity);

            GameObject playerGO = GameObject.Find("Player");
            playerGO.transform.position = new Vector3(2, 4);

            Instantiate(wallTiles[Random.Range(2, wallTiles.Length)], new Vector3(1, 4, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(2, wallTiles.Length)], new Vector3(1, 3, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(2, wallTiles.Length)], new Vector3(1, 5, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(2, wallTiles.Length)], new Vector3(2, 3, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(2, wallTiles.Length)], new Vector3(2, 5, 0), Quaternion.identity);

            Instantiate(exit, new Vector3(1, 8, 0f), Quaternion.identity);
        }

        //Flash.
        public void SetupTutorial8() {
            columns = 9;
            rows = 13;

            BoardSetup(columns, rows);

            GameObject playerGO = GameObject.Find("Player");
            playerGO.transform.position = new Vector3(2, 4);

            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(1, 4, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(1, 3, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(1, 5, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(2, 3, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(2, 5, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(3, 4, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(3, 3, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(3, 5, 0), Quaternion.identity);

            Instantiate(exit, new Vector3(1, 8, 0f), Quaternion.identity);
        }

        //Ouroboros.
        public void SetupTutorial9() {
            columns = 9;
            rows = 13;

            BoardSetup(columns, rows);

            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(0, 4, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(1, 4, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(2, 4, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(3, 4, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(4, 4, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(5, 4, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(6, 4, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(8, 4, 0), Quaternion.identity);

            GameObject playerGO = GameObject.Find("Player");
            playerGO.transform.position = new Vector3(1, 3);

            Instantiate(exit, new Vector3(1, 5, 0f), Quaternion.identity);
        }

        //Consumables.
        public void SetupTutorial10() {
            columns = 9;
            rows = 13;

            BoardSetup(columns, rows);

            Instantiate(potionTiles[0], new Vector3(2, 4, 0), Quaternion.identity);
            Instantiate(foodTiles[0], new Vector3(4, 4, 0), Quaternion.identity);
            Instantiate(foodTiles[1], new Vector3(6, 4, 0), Quaternion.identity);

            GameObject playerGO = GameObject.Find("Player");
            playerGO.transform.position = new Vector3(0, 4);

            Instantiate(exit, new Vector3(8, 4, 0f), Quaternion.identity);
        }

        public void SetupQuest1() {
            columns = 9;
            rows = 11;
            BoardSetup(columns, rows);

            for(int x = 0; x < columns; ++x) {
                Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(x, 10, 0), Quaternion.identity);
                Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(x, 9, 0), Quaternion.identity);
                Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(x, 8, 0), Quaternion.identity);
            }
            for(int y = 0; y < rows; ++y) {
                Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(8, y, 0), Quaternion.identity);
            }

            Instantiate(enemyMeleeTile, new Vector3(0, 7, 0), Quaternion.identity);
            Instantiate(enemyMeleeTile, new Vector3(7, 0, 0), Quaternion.identity);

            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(1, 4, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(2, 3, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(3, 2, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(4, 1, 0), Quaternion.identity);

            GameObject.Find("Player").transform.position = new Vector3(1, 1, 0);

            Instantiate(exit, new Vector3(7, 7, 0f), Quaternion.identity);
        }

        public void SetupQuest2() {
            columns = 9;
            rows = 11;
            BoardSetup(columns, rows);

            for(int x = 0; x < columns; ++x) {
                Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(x, 0, 0), Quaternion.identity);
                Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(x, 10, 0), Quaternion.identity);
            }
            for(int y = 0; y < rows; ++y) {
                Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(0, y, 0), Quaternion.identity);
                Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(8, y, 0), Quaternion.identity);
            }

            Instantiate(enemyMeleeTile, new Vector3(4, 5, 0), Quaternion.identity);
            Instantiate(enemyMeleeTile, new Vector3(6, 2, 0), Quaternion.identity);

            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(7, 3, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(5, 1, 0), Quaternion.identity);

            GameObject.Find("Player").transform.position = new Vector3(2, 2, 0);

            Instantiate(foodTiles[0], new Vector3(7, 1, 0), Quaternion.identity);

            Instantiate(exit, new Vector3(6, 8, 0f), Quaternion.identity);
        }

        public void SetupQuest3() {
            columns = 9;
            rows = 11;

            BoardSetup(columns, rows);

            for(int x = 0; x < columns; ++x) {
                Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(x, 0, 0), Quaternion.identity);
                Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(x, 1, 0), Quaternion.identity);
                Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(x, 9, 0), Quaternion.identity);
                Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(x, 10, 0), Quaternion.identity);
            }

            Instantiate(enemyMeleeTile, new Vector3(6, 8, 0), Quaternion.identity);
            Instantiate(enemyMeleeTile, new Vector3(1, 3, 0), Quaternion.identity);
            Instantiate(enemyMeleeTile, new Vector3(5, 3, 0), Quaternion.identity);
            Instantiate(enemyMeleeTile, new Vector3(7, 5, 0), Quaternion.identity);

            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(0, 8, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(4, 7, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(2, 5, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(6, 4, 0), Quaternion.identity);
            
            GameObject.Find("Player").transform.position = new Vector3(1, 7, 0);

            Instantiate(potionTiles[0], new Vector3(8, 2, 0), Quaternion.identity);

            Instantiate(exit, new Vector3(7, 3, 0f), Quaternion.identity);
        }

        public void SetupQuest4() {
            columns = 9;
            rows = 11;

            BoardSetup(columns, rows);

            for(int x = 0; x < columns; ++x) {
                Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(x, 9, 0), Quaternion.identity);
                Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(x, 10, 0), Quaternion.identity);
            }
            for(int y = 0; y < rows; ++y) {
                Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(0, y, 0), Quaternion.identity);
            }

            Instantiate(enemyMeleeTile, new Vector3(5, 6, 0), Quaternion.identity);
            Instantiate(enemyMeleeTile, new Vector3(3, 3, 0), Quaternion.identity);

            Instantiate(wallTiles[Random.Range(2, 7)], new Vector3(6, 0, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(2, 7)], new Vector3(6, 1, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(2, 7)], new Vector3(6, 2, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(2, 7)], new Vector3(7, 2, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(2, 7)], new Vector3(8, 2, 0), Quaternion.identity);

            GameObject.Find("Player").transform.position = new Vector3(8, 0, 0);

            Instantiate(foodTiles[1], new Vector3(7, 7, 0), Quaternion.identity);
            Instantiate(foodTiles[1], new Vector3(2, 1, 0), Quaternion.identity);
            Instantiate(potionTiles[0], new Vector3(1, 8, 0), Quaternion.identity);

            Instantiate(exit, new Vector3(2, 7, 0f), Quaternion.identity);
        }

        public void SetupQuest5() {
            columns = 9;
            rows = 11;

            BoardSetup(columns, rows);

            for(int x = 1; x < columns - 1; ++x) {
                Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(x, 1, 0), Quaternion.identity);
                Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(x, 9, 0), Quaternion.identity);
            }
            for(int y = 1; y < rows - 1; ++y) {
                Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(1, y, 0), Quaternion.identity);
                Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(7, y, 0), Quaternion.identity);
            }

            Instantiate(enemyMeleeTile, new Vector3(4, 5, 0), Quaternion.identity);

            GameObject.Find("Player").transform.position = new Vector3(4, 10, 0);

            Instantiate(foodTiles[1], new Vector3(0, 0, 0), Quaternion.identity);
            Instantiate(foodTiles[1], new Vector3(8, 0, 0), Quaternion.identity);
            Instantiate(potionTiles[0], new Vector3(4, 0, 0), Quaternion.identity);

            Instantiate(exit, new Vector3(4, 3, 0f), Quaternion.identity);
        }

        public void SetupQuest6() {
            columns = 9;
            rows = 11;

            BoardSetup(columns, rows);
            
            for(int y = 1; y < rows - 1; ++y) {
                Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(0, y, 0), Quaternion.identity);
                Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(8, y, 0), Quaternion.identity);
            }

            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(1, 0, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(2, 0, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(2, 1, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(2, 2, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(2, 3, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(3, 3, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(4, 3, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(5, 3, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(6, 3, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(6, 2, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(6, 1, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(6, 0, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(7, 0, 0), Quaternion.identity);

            Instantiate(wallTiles[Random.Range(2, 7)], new Vector3(6, 10, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(2, 7)], new Vector3(3, 8, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(2, 7)], new Vector3(6, 0, 0), Quaternion.identity);

            Instantiate(enemyMeleeTile, new Vector3(6, 8, 0), Quaternion.identity);
            Instantiate(enemyArcherTile, new Vector3(1, 9, 0), Quaternion.identity);
            Instantiate(enemyWizardTile, new Vector3(4, 5, 0), Quaternion.identity);

            GameObject.Find("Player").transform.position = new Vector3(4, 10, 0);

            Instantiate(foodTiles[0], new Vector3(4, 4, 0), Quaternion.identity);
            Instantiate(foodTiles[1], new Vector3(7, 1, 0), Quaternion.identity);
            Instantiate(potionTiles[0], new Vector3(1, 1, 0), Quaternion.identity);

            Instantiate(exit, new Vector3(4, 3, 0f), Quaternion.identity);
        }

        public void SetupQuest7() {
            columns = 9;
            rows = 11;

            BoardSetup(columns, rows);

            for(int x = 1; x < columns - 1; ++x) {
                Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(x, 0, 0), Quaternion.identity);
                Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(x, 10, 0), Quaternion.identity);
            }

            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(4, 4, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(5, 4, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(6, 4, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(7, 4, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(8, 4, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(8, 3, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(8, 2, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(8, 1, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(8, 0, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(0, 2, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(1, 2, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(2, 2, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(3, 2, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(4, 2, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(5, 2, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(6, 2, 0), Quaternion.identity);

            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(0, 6, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(1, 6, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(2, 6, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(0, 7, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(1, 7, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(2, 7, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(0, 8, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(1, 8, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(2, 8, 0), Quaternion.identity);

            Instantiate(enemyMeleeTile, new Vector3(7, 7, 0), Quaternion.identity);
            Instantiate(enemyMeleeTile, new Vector3(4, 3, 0), Quaternion.identity);
            Instantiate(enemyArcherTile, new Vector3(3, 8, 0), Quaternion.identity);

            GameObject.Find("Player").transform.position = new Vector3(0, 1, 0);

            Instantiate(foodTiles[0], new Vector3(7, 9, 0), Quaternion.identity);
            Instantiate(foodTiles[1], new Vector3(7, 1, 0), Quaternion.identity);
            Instantiate(potionTiles[0], new Vector3(8, 5, 0), Quaternion.identity);

            Instantiate(exit, new Vector3(1, 8, 0f), Quaternion.identity);
        }

        public void SetupQuest8() {
            columns = 9;
            rows = 11;

            BoardSetup(columns, rows);

            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(2, 6, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(3, 6, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(5, 6, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(6, 6, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(2, 5, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(3, 5, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(4, 5, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(5, 5, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(6, 5, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(3, 4, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(4, 4, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(5, 4, 0), Quaternion.identity);

            Instantiate(enemyMeleeTile, new Vector3(7, 9, 0), Quaternion.identity);
            Instantiate(enemyArcherTile, new Vector3(1, 9, 0), Quaternion.identity);
            Instantiate(enemyArcherTile, new Vector3(7, 1, 0), Quaternion.identity);
            Instantiate(enemyWizardTile, new Vector3(1, 1, 0), Quaternion.identity);

            GameObject.Find("Player").transform.position = new Vector3(4, 3, 0);

            Instantiate(foodTiles[0], new Vector3(0, 10, 0), Quaternion.identity);
            Instantiate(foodTiles[0], new Vector3(8, 0, 0), Quaternion.identity);
            Instantiate(foodTiles[1], new Vector3(8, 10, 0), Quaternion.identity);
            Instantiate(potionTiles[0], new Vector3(0, 0, 0), Quaternion.identity);

            Instantiate(exit, new Vector3(4, 6, 0f), Quaternion.identity);
        }

        public void SetupQuest9() {
            columns = 9;
            rows = 11;

            BoardSetup(columns, rows);

            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(0, 10, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(1, 10, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(2, 10, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(3, 10, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(4, 10, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(5, 10, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(6, 10, 0), Quaternion.identity);

            for(int x = 0; x < columns; ++x) {
                Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(x, 0, 0), Quaternion.identity);
            }

            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(0, 9, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(1, 9, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(2, 9, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(3, 9, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(8, 9, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(5, 8, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(6, 8, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(7, 8, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(8, 8, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(0, 7, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(1, 7, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(2, 7, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(3, 7, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(5, 6, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(6, 6, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(7, 6, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(8, 6, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(2, 7)], new Vector3(0, 5, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(1, 5, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(2, 5, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(3, 5, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(5, 4, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(2, 7)], new Vector3(6, 4, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(7, 4, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(8, 4, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(0, 3, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(1, 3, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(2, 3, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(3, 3, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(5, 2, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(2, 7)], new Vector3(6, 2, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(7, 2, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(2, 7)], new Vector3(8, 2, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(0, 1, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(1, 1, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(2, 1, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(3, 1, 0), Quaternion.identity);

            Instantiate(wallTiles[Random.Range(2, 7)], new Vector3(7, 9, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(8, 9, 0), Quaternion.identity);

            Instantiate(enemyMeleeTile, new Vector3(8, 1, 0), Quaternion.identity);
            Instantiate(enemyMeleeTile, new Vector3(0, 2, 0), Quaternion.identity);
            Instantiate(enemyArcherTile, new Vector3(0, 4, 0), Quaternion.identity);
            Instantiate(enemyArcherTile, new Vector3(8, 5, 0), Quaternion.identity);
            Instantiate(enemyWizardTile, new Vector3(6, 7, 0), Quaternion.identity);
            Instantiate(enemyWizardTile, new Vector3(1, 8, 0), Quaternion.identity);

            GameObject.Find("Player").transform.position = new Vector3(4, 1, 0);

            Instantiate(foodTiles[0], new Vector3(8, 3, 0), Quaternion.identity);
            Instantiate(foodTiles[0], new Vector3(8, 7, 0), Quaternion.identity);
            Instantiate(foodTiles[0], new Vector3(0, 8, 0), Quaternion.identity);
            Instantiate(foodTiles[1], new Vector3(8, 10, 0), Quaternion.identity);
            Instantiate(foodTiles[1], new Vector3(7, 10, 0), Quaternion.identity);
            Instantiate(potionTiles[0], new Vector3(0, 6, 0), Quaternion.identity);

            Instantiate(exit, new Vector3(4, 9, 0f), Quaternion.identity);
        }

        public void SetupQuest10() {
            columns = 9;
            rows = 11;

            BoardSetup(columns, rows);

            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(6, 10, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(2, 7)], new Vector3(6, 9, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(6, 8, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(6, 7, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(6, 6, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(6, 5, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(6, 4, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(7, 10, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(7, 9, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(7, 8, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(7, 7, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(7, 6, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(7, 5, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(7, 4, 0), Quaternion.identity);

            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(4, 6, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(4, 5, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(4, 4, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(4, 3, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(4, 2, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(3, 3, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(3, 2, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(2, 3, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(2, 2, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(1, 3, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(2, 7)], new Vector3(1, 2, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(0, 3, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(0, 2, 0), Quaternion.identity);

            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(7, 0, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(8, 0, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(8, 1, 0), Quaternion.identity);

            Instantiate(enemyMeleeTile, new Vector3(4, 4, 0), Quaternion.identity);
            Instantiate(enemyMeleeTile, new Vector3(1, 1, 0), Quaternion.identity);
            Instantiate(enemyMeleeTile, new Vector3(7, 1, 0), Quaternion.identity);
            Instantiate(enemyWizardTile, new Vector3(1, 5, 0), Quaternion.identity);

            GameObject.Find("Player").transform.position = new Vector3(0, 10, 0);

            Instantiate(foodTiles[0], new Vector3(0, 1, 0), Quaternion.identity);
            Instantiate(foodTiles[1], new Vector3(0, 0, 0), Quaternion.identity);

            Instantiate(exit, new Vector3(8, 10, 0f), Quaternion.identity);
        }

        public void SetupQuest11() {
            columns = 9;
            rows = 11;

            BoardSetup(columns, rows);

            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(0, 10, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(2, 10, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(4, 10, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(6, 10, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(8, 10, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(1, 9, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(3, 9, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(5, 9, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(7, 9, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(0, 8, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(2, 7)], new Vector3(2, 8, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(4, 8, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(6, 8, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(8, 8, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(1, 7, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(3, 7, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(2, 7)], new Vector3(5, 7, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(7, 7, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(0, 6, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(2, 6, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(2, 7)], new Vector3(4, 6, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(6, 6, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(8, 6, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(1, 5, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(3, 5, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(5, 5, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(7, 5, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(0, 4, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(2, 4, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(4, 4, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(2, 7)], new Vector3(6, 4, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(8, 4, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(2, 7)], new Vector3(1, 3, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(3, 3, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(5, 3, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(7, 3, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(0, 2, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(2, 2, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(4, 2, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(6, 2, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(8, 2, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(1, 1, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(3, 1, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(5, 1, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(7, 1, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(0, 0, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(2, 0, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(4, 0, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(6, 0, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(8, 0, 0), Quaternion.identity);

            Instantiate(enemyMeleeTile, new Vector3(5, 10, 0), Quaternion.identity);
            Instantiate(enemyMeleeTile, new Vector3(7, 8, 0), Quaternion.identity);
            Instantiate(enemyMeleeTile, new Vector3(3, 6, 0), Quaternion.identity);
            Instantiate(enemyMeleeTile, new Vector3(5, 6, 0), Quaternion.identity);
            Instantiate(enemyMeleeTile, new Vector3(0, 5, 0), Quaternion.identity);
            Instantiate(enemyMeleeTile, new Vector3(7, 4, 0), Quaternion.identity);
            Instantiate(enemyMeleeTile, new Vector3(7, 4, 0), Quaternion.identity);
            Instantiate(enemyMeleeTile, new Vector3(8, 3, 0), Quaternion.identity);
            Instantiate(enemyMeleeTile, new Vector3(1, 2, 0), Quaternion.identity);
            Instantiate(enemyMeleeTile, new Vector3(8, 1, 0), Quaternion.identity);

            GameObject.Find("Player").transform.position = new Vector3(4, 1, 0);
            
            Instantiate(foodTiles[0], new Vector3(0, 3, 0), Quaternion.identity);
            Instantiate(foodTiles[1], new Vector3(8, 7, 0), Quaternion.identity);
            Instantiate(potionTiles[0], new Vector3(1, 8, 0), Quaternion.identity);
            Instantiate(potionTiles[0], new Vector3(6, 3, 0), Quaternion.identity);

            Instantiate(exit, new Vector3(4, 9, 0f), Quaternion.identity);
        }

        public void SetupQuest12() {
            columns = 9;
            rows = 11;

            BoardSetup(columns, rows);

            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(3, 8, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(2, 7)], new Vector3(4, 8, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(5, 8, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(3, 7, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(5, 7, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(3, 6, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(2, 7)], new Vector3(4, 6, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(5, 6, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(3, 5, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(5, 5, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(3, 4, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(2, 7)], new Vector3(4, 4, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(5, 4, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(3, 3, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(5, 3, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(3, 2, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(4, 2, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(5, 2, 0), Quaternion.identity);

            Instantiate(enemyFlyerTile, new Vector3(0, 10, 0), Quaternion.identity);
            Instantiate(enemyFlyerTile, new Vector3(7, 0, 0), Quaternion.identity);

            GameObject.Find("Player").transform.position = new Vector3(0, 0, 0);

            Instantiate(foodTiles[0], new Vector3(4, 7, 0), Quaternion.identity);
            Instantiate(foodTiles[0], new Vector3(4, 3, 0), Quaternion.identity);
            Instantiate(potionTiles[0], new Vector3(4, 5, 0), Quaternion.identity);

            Instantiate(exit, new Vector3(8, 10, 0f), Quaternion.identity);
        }

        public void SetupQuest13() {
            columns = 9;
            rows = 11;

            BoardSetup(columns, rows);

            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(0, 1, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(1, 1, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(2, 1, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(3, 1, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(4, 1, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(5, 1, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(6, 1, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(7, 1, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(8, 1, 0), Quaternion.identity);

            Instantiate(wallTiles[Random.Range(2, 7)], new Vector3(3, 5, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(2, 7)], new Vector3(4, 6, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(2, 7)], new Vector3(4, 4, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(2, 7)], new Vector3(5, 5, 0), Quaternion.identity);

            Instantiate(enemyMeleeTile, new Vector3(1, 2, 0), Quaternion.identity);
            Instantiate(enemyArcherTile, new Vector3(1, 9, 0), Quaternion.identity);
            Instantiate(enemyWizardTile, new Vector3(7, 9, 0), Quaternion.identity);
            Instantiate(enemyFlyerTile, new Vector3(7, 2, 0), Quaternion.identity);

            GameObject.Find("Player").transform.position = new Vector3(7, 7, 0);

            Instantiate(potionTiles[0], new Vector3(0, 10, 0), Quaternion.identity);
            Instantiate(potionTiles[0], new Vector3(8, 10, 0), Quaternion.identity);
            Instantiate(potionTiles[0], new Vector3(8, 0, 0), Quaternion.identity);

            Instantiate(exit, new Vector3(1, 0, 0f), Quaternion.identity);
        }

        public void SetupQuest14() {
            columns = 9;
            rows = 11;

            BoardSetup(columns, rows);

            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(2, 7, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(3, 7, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(5, 7, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(6, 7, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(2, 6, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(3, 6, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(4, 6, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(5, 6, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(6, 6, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(1, 5, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(2, 5, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(3, 5, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(4, 5, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(5, 5, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(6, 5, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(7, 5, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(2, 4, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(3, 4, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(4, 4, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(5, 4, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(6, 4, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(2, 3, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(3, 3, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(5, 3, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(6, 3, 0), Quaternion.identity);

            Instantiate(enemyMeleeTile, new Vector3(1, 9, 0), Quaternion.identity);
            Instantiate(enemyMeleeTile, new Vector3(4, 10, 0), Quaternion.identity);
            Instantiate(enemyMeleeTile, new Vector3(7, 9, 0), Quaternion.identity);
            Instantiate(enemyMeleeTile, new Vector3(1, 1, 0), Quaternion.identity);
            Instantiate(enemyMeleeTile, new Vector3(7, 1, 0), Quaternion.identity);
            Instantiate(enemyArcherTile, new Vector3(4, 0, 0), Quaternion.identity);

            GameObject.Find("Player").transform.position = new Vector3(4, 7, 0);

            Instantiate(foodTiles[1], new Vector3(0, 5, 0), Quaternion.identity);
            Instantiate(foodTiles[1], new Vector3(8, 5, 0), Quaternion.identity);

            Instantiate(exit, new Vector3(4, 3, 0f), Quaternion.identity);
        }

        public void SetupQuest15() {
            columns = 9;
            rows = 11;

            BoardSetup(columns, rows);

            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(3, 9, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(1, 3, 0), Quaternion.identity);

            Instantiate(wallTiles[Random.Range(2, 7)], new Vector3(4, 6, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(2, 7)], new Vector3(7, 3, 0), Quaternion.identity);

            Instantiate(enemyMeleeTile, new Vector3(6, 10, 0), Quaternion.identity);
            Instantiate(enemyArcherTile, new Vector3(7, 7, 0), Quaternion.identity);
            Instantiate(enemyMeleeTile, new Vector3(1, 6, 0), Quaternion.identity);
            Instantiate(enemyFlyerTile, new Vector3(4, 4, 0), Quaternion.identity);
            Instantiate(enemyWizardTile, new Vector3(2, 1, 0), Quaternion.identity);
            Instantiate(enemyArcherTile, new Vector3(5, 1, 0), Quaternion.identity);

            GameObject.Find("Player").transform.position = new Vector3(0, 10, 0);

            Instantiate(foodTiles[0], new Vector3(8, 6, 0), Quaternion.identity);
            Instantiate(foodTiles[0], new Vector3(4, 2, 0), Quaternion.identity);
            Instantiate(potionTiles[0], new Vector3(1, 2, 0), Quaternion.identity);

            Instantiate(exit, new Vector3(8, 0, 0f), Quaternion.identity);
        }

        public void SetupQuest16() {
            columns = 9;
            rows = 11;

            BoardSetup(columns, rows);

            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(2, 10, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(3, 10, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(4, 10, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(5, 10, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(6, 10, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(7, 10, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(8, 10, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(5, 9, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(6, 9, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(7, 9, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(8, 9, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(5, 8, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(6, 8, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(7, 8, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(8, 8, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(8, 6, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(8, 5, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(8, 4, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(8, 3, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(8, 2, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(3, 1, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(4, 1, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(5, 1, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(6, 1, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(7, 1, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(8, 1, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(3, 0, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(4, 0, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(5, 0, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(6, 0, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(7, 0, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(8, 0, 0), Quaternion.identity);

            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(10, 1, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(9, 1, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(2, 7)], new Vector3(8, 1, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(7, 1, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(6, 1, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(5, 1, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(5, 1, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(4, 1, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(3, 1, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(2, 1, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(8, 3, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(7, 3, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(2, 7)], new Vector3(6, 3, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(5, 3, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(4, 3, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(3, 3, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(2, 3, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(7, 5, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(6, 5, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(5, 5, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(2, 7)], new Vector3(4, 5, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(3, 5, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(6, 7, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(5, 7, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(4, 7, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(3, 7, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(2, 7, 0), Quaternion.identity);

            Instantiate(enemyMeleeTile, new Vector3(0, 4, 0), Quaternion.identity);
            Instantiate(enemyArcherTile, new Vector3(4, 3, 0), Quaternion.identity);
            Instantiate(enemyMeleeTile, new Vector3(6, 5, 0), Quaternion.identity);

            GameObject.Find("Player").transform.position = new Vector3(0, 10, 0);

            Instantiate(foodTiles[0], new Vector3(2, 3, 0), Quaternion.identity);
            Instantiate(foodTiles[1], new Vector3(0, 0, 0), Quaternion.identity);
            Instantiate(potionTiles[0], new Vector3(4, 2, 0), Quaternion.identity);

            Instantiate(exit, new Vector3(8, 7, 0f), Quaternion.identity);
        }

        public void SetupQuest17() {
            columns = 9;
            rows = 11;

            BoardSetup(columns, rows);

            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(3, 7, 0), Quaternion.identity);

            Instantiate(wallTiles[Random.Range(2, 7)], new Vector3(0, 3, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(2, 7)], new Vector3(1, 3, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(2, 7)], new Vector3(5, 3, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(2, 7)], new Vector3(0, 2, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(2, 7)], new Vector3(1, 2, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(2, 7)], new Vector3(2, 2, 0), Quaternion.identity);

            Instantiate(enemyMeleeTile, new Vector3(0, 5, 0), Quaternion.identity);
            Instantiate(enemyMeleeTile, new Vector3(2, 5, 0), Quaternion.identity);
            Instantiate(enemyMeleeTile, new Vector3(1, 4, 0), Quaternion.identity);
            Instantiate(enemyArcherTile, new Vector3(7, 4, 0), Quaternion.identity);
            Instantiate(enemyArcherTile, new Vector3(4, 1, 0), Quaternion.identity);
            Instantiate(enemyFlyerTile, new Vector3(7, 1, 0), Quaternion.identity);
            Instantiate(enemyWizardTile, new Vector3(7, 9, 0), Quaternion.identity);

            GameObject.Find("Player").transform.position = new Vector3(1, 9, 0);

            Instantiate(foodTiles[0], new Vector3(5, 7, 0), Quaternion.identity);
            Instantiate(foodTiles[0], new Vector3(3, 3, 0), Quaternion.identity);
            Instantiate(potionTiles[0], new Vector3(8, 6, 0), Quaternion.identity);
            Instantiate(potionTiles[0], new Vector3(6, 2, 0), Quaternion.identity);

            Instantiate(exit, new Vector3(1, 1, 0f), Quaternion.identity);
        }

        public void SetupQuest18() {
            columns = 9;
            rows = 11;

            BoardSetup(columns, rows);

            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(0, 2, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(0, 1, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(0, 0, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(1, 0, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(2, 0, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(6, 0, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(7, 0, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(8, 0, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(8, 1, 0), Quaternion.identity);
            Instantiate(outerWallTiles[Random.Range(0, outerWallTiles.Length)], new Vector3(8, 2, 0), Quaternion.identity);

            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(1, 9, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(7, 9, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(0, 3, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(8, 3, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(1, 1, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(7, 1, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(4, 2, 0), Quaternion.identity);

            Instantiate(enemyMeleeTile, new Vector3(3, 5, 0), Quaternion.identity);
            Instantiate(enemyMeleeTile, new Vector3(4, 4, 0), Quaternion.identity);
            Instantiate(enemyMeleeTile, new Vector3(5, 5, 0), Quaternion.identity);
            Instantiate(enemyArcherTile, new Vector3(1, 7, 0), Quaternion.identity);
            Instantiate(enemyArcherTile, new Vector3(7, 7, 0), Quaternion.identity);
            Instantiate(enemyFlyerTile, new Vector3(2, 6, 0), Quaternion.identity);
            Instantiate(enemyFlyerTile, new Vector3(6, 6, 0), Quaternion.identity);
            Instantiate(enemyWizardTile, new Vector3(4, 8, 0), Quaternion.identity);

            GameObject.Find("Player").transform.position = new Vector3(4, 0, 0);

            Instantiate(foodTiles[1], new Vector3(8, 10, 0), Quaternion.identity);
            Instantiate(foodTiles[1], new Vector3(8, 5, 0), Quaternion.identity);
            Instantiate(potionTiles[0], new Vector3(0, 10, 0), Quaternion.identity);
            Instantiate(potionTiles[0], new Vector3(0, 5, 0), Quaternion.identity);

            Instantiate(exit, new Vector3(4, 10, 0f), Quaternion.identity);
        }

        public void SetupQuest19() {
            columns = 9;
            rows = 11;

            BoardSetup(columns, rows);

            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(2, 8, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(2, 7)], new Vector3(4, 8, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(6, 8, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(2, 6, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(4, 6, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(6, 6, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(2, 4, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(2, 7)], new Vector3(4, 4, 0), Quaternion.identity);
            Instantiate(wallTiles[Random.Range(0, 1)], new Vector3(6, 4, 0), Quaternion.identity);
            
            Instantiate(enemyMeleeTile, new Vector3(1, 7, 0), Quaternion.identity);
            Instantiate(enemyMeleeTile, new Vector3(3, 7, 0), Quaternion.identity);
            Instantiate(enemyMeleeTile, new Vector3(5, 7, 0), Quaternion.identity);
            Instantiate(enemyMeleeTile, new Vector3(7, 7, 0), Quaternion.identity);
            Instantiate(enemyArcherTile, new Vector3(1, 9, 0), Quaternion.identity);
            Instantiate(enemyArcherTile, new Vector3(7, 1, 0), Quaternion.identity);
            Instantiate(enemyFlyerTile, new Vector3(3, 9, 0), Quaternion.identity);
            Instantiate(enemyFlyerTile, new Vector3(5, 5, 0), Quaternion.identity);
            Instantiate(enemyFlyerTile, new Vector3(7, 3, 0), Quaternion.identity);
            Instantiate(enemyWizardTile, new Vector3(5, 9, 0), Quaternion.identity);
            Instantiate(enemyWizardTile, new Vector3(7, 5, 0), Quaternion.identity);

            GameObject.Find("Player").transform.position = new Vector3(1, 1, 0);

            Instantiate(foodTiles[1], new Vector3(8, 10, 0), Quaternion.identity);
            Instantiate(foodTiles[1], new Vector3(8, 9, 0), Quaternion.identity);
            Instantiate(potionTiles[0], new Vector3(8, 10, 0), Quaternion.identity);
            Instantiate(potionTiles[0], new Vector3(0, 0, 0), Quaternion.identity);

            Instantiate(exit, new Vector3(7, 9, 0f), Quaternion.identity);
        }

        public void SetupQuest20() {
            columns = 9;
            rows = 11;

            BoardSetup(columns, rows);

            GameObject.Find("Player").transform.position = new Vector3(4, 2, 0);

            Instantiate(emeraldSword, new Vector3(4, 8, 0f), Quaternion.identity);
        }

        public void SetupQuest(int callLevel) {
            if(callLevel == 1) SetupQuest1();
            else if(callLevel == 2) SetupQuest2();
            else if(callLevel == 3) SetupQuest3();
            else if(callLevel == 4) SetupQuest4();
            else if(callLevel == 5) SetupQuest5();
            else if(callLevel == 6) SetupQuest6();
            else if(callLevel == 7) SetupQuest7();
            else if(callLevel == 8) SetupQuest8();
            else if(callLevel == 9) SetupQuest9();
            else if(callLevel == 10) SetupQuest10();
            else if(callLevel == 11) SetupQuest11();
            else if(callLevel == 12) SetupQuest12();
            else if(callLevel == 13) SetupQuest13();
            else if(callLevel == 14) SetupQuest14();
            else if(callLevel == 15) SetupQuest15();
            else if(callLevel == 16) SetupQuest16();
            else if(callLevel == 17) SetupQuest17();
            else if(callLevel == 18) SetupQuest18();
            else if(callLevel == 19) SetupQuest19();
            else if(callLevel == 20) SetupQuest20();
        }

        public void SetupTutorial(int callLevel) {
            if(callLevel == 21) SetupTutorial1();
            else if(callLevel == 22) SetupTutorial2();
            else if(callLevel == 23) SetupTutorial3();
            else if(callLevel == 24) SetupTutorial4();
            else if(callLevel == 25) SetupTutorial5();
            else if(callLevel == 26) SetupTutorial6();
            else if(callLevel == 27) SetupTutorial7();
            else if(callLevel == 28) SetupTutorial8();
            else if(callLevel == 29) SetupTutorial9();
            else if(callLevel == 30) SetupTutorial10();
        }
    }
}