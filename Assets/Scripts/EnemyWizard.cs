using UnityEngine;
using System.Collections;

namespace Completed {
    public class EnemyWizard : Enemy {
        public int range;

        protected override void Start() {
            base.Start();
        }

        protected override bool AttemptMove<T>(int xDir, int yDir, bool flyer) {
            return base.AttemptMove<T>(xDir, yDir, flyer);
        }

        protected override bool AttemptAttack<T>(int xDir, int yDir, bool motion) {
            return base.AttemptAttack<T>(xDir, yDir, motion);
        }

        public override void MoveEnemy() {
            if(dead) {
                return;
            }

            bool didItAttack = false;
            
            int distance = Mathf.FloorToInt(Vector3Int.Distance(Vector3Int.RoundToInt(this.transform.position), Vector3Int.RoundToInt(target.transform.position)));
            Vector2 dir = target.position - transform.position;
            int xDir = Mathf.RoundToInt(dir.x);
            int yDir = Mathf.RoundToInt(dir.y);

            if(distance <= 2) {
                yDir = (target.position.y > transform.position.y) ? -1 : 1;
                xDir = (target.position.x > transform.position.x) ? -1 : 1;

                xDir = Mathf.Clamp(xDir, -1, 1);
                yDir = Mathf.Clamp(yDir, -1, 1);
                
                bool attempt1 = AttemptMove<Wall>(xDir, yDir, false);
                if(attempt1 == false) {
                    bool attempt2 = AttemptMove<Wall>(xDir, 0, false);
                    if(attempt2 == false) {
                        AttemptMove<Wall>(0, yDir, false);
                    }
                }
            }
            else if(distance > range) {
                dir = transform.position - target.position;
                xDir = Mathf.RoundToInt(dir.x);
                yDir = Mathf.RoundToInt(dir.y);

                if(Mathf.Abs(xDir) > Mathf.Abs(yDir)) {
                    if(yDir == 0) {
                        bool attempt1 = AttemptMove<Wall>(0, 1, false);
                        if(attempt1 == false) {
                            bool attempt2 = AttemptMove<Wall>(0, -1, false);
                            if(attempt2 == false) {
                                AttemptMove<Wall>((Mathf.Clamp(xDir, -1, 1) * -1), 0, false);
                            }
                        }
                    }
                    else {
                        bool attempt1 = AttemptMove<Wall>(0, Mathf.Clamp(yDir, -1, 1), false);
                        if(attempt1 == false) {
                            bool attempt2 = AttemptMove<Wall>(0, (Mathf.Clamp(yDir, -1, 1) * -1), false);
                            if(attempt2 == false) {
                                AttemptMove<Wall>((Mathf.Clamp(xDir, -1, 1) * -1), 0, false);
                            }
                        }
                    }
                }
                if(Mathf.Abs(yDir) > Mathf.Abs(xDir)) {
                    if(xDir == 0) {
                        bool attempt1 = AttemptMove<Wall>(1, 0, false);
                        if(attempt1 == false) {
                            bool attempt2 = AttemptMove<Wall>(-1, 0, false);
                            if(attempt2 == false) {
                                AttemptMove<Wall>(0, (Mathf.Clamp(yDir, -1, 1) * -1), false);
                            }
                        }
                    }
                    else {
                        bool attempt1 = AttemptMove<Wall>(Mathf.Clamp(xDir, -1, 1), 0, false);
                        if(attempt1 == false) {
                            bool attempt2 = AttemptMove<Wall>((Mathf.Clamp(xDir, -1, 1) * -1), 0, false);
                            if(attempt2 == false) {
                                AttemptMove<Wall>(0, (Mathf.Clamp(yDir, -1, 1) * -1), false);
                            }
                        }
                    }
                }
            }
            else if(distance > 1 && distance <= range) {
                didItAttack = AttemptAttack<Player>(xDir, yDir, false);

                xDir = Mathf.Clamp(xDir, -1, 1);
                yDir = Mathf.Clamp(yDir, -1, 1);

                if(!didItAttack) {
                    bool attempt1 = AttemptMove<Wall>(xDir, yDir, false);
                    if(attempt1 == false) {
                        bool attempt2 = AttemptMove<Wall>(xDir, 0, false);
                        if(attempt2 == false) {
                            AttemptMove<Wall>(0, yDir, false);
                        }
                    }
                }
            }
        }

        //OnCantMove is called if Enemy attempts to move into a space occupied by a Player, it overrides the OnCantMove function of MovingObject.
        //and takes a generic parameter T which we use to pass in the component we expect to encounter, in this case Player.
        protected override void OnCantMove<T>(T component) {
            //TODO:
        }

        protected override void OnCanAttack<T>(T component) {
            Debug.Log(component.GetType().Name);
            Player hitPlayer = component as Player;

            GameObject bomb = Instantiate(Resources.Load<GameObject>("Prefabs/Bomb")) as GameObject;
            bomb.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 0);

            //The bomb can only land in an adjacent tile to the player.
            int newX = 0;
            int newY = 0;
            while(newX == 0 && newY == 0) {
                int[] numbers = { -1, 0, 1 };
                newX = numbers[Random.Range(0, 3)];
                newY = numbers[Random.Range(0, 3)];
            }

            Bomb bombScript = bomb.GetComponent<Bomb>();
            bombScript.target = new Vector3(hitPlayer.transform.position.x + newX, hitPlayer.transform.position.y + newY, 0);
        }
    }
}