using UnityEngine;
using System.Collections;

namespace Completed {
    public class EnemyArcher : Enemy {
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

            //Declare variables for X and Y axis move directions, these range from -1 to 1.
            //These values allow us to choose between the cardinal directions: up, down, left and right.
            int xDir = 0;
            int yDir = 0;
            
            int distance = Mathf.FloorToInt(Vector3Int.Distance(Vector3Int.RoundToInt(this.transform.position), Vector3Int.RoundToInt(target.transform.position)));

            if(distance <= 2) {
                yDir = (target.position.y > transform.position.y) ? -1 : 1;
                xDir = (target.position.x > transform.position.x) ? -1 : 1;
            }
            else if(distance > range) {
                yDir = (target.position.y > transform.position.y) ? 1 : -1;
                xDir = (target.position.x > transform.position.x) ? 1 : -1;
            }
            else if(distance > 2 && distance <= range) {
                Vector2 dir = target.position - transform.position;
                yDir = Mathf.RoundToInt(dir.y);
                xDir = Mathf.RoundToInt(dir.x);

                //Archers only fire in vertical and horizontal lines.
                if(xDir == 0 || yDir == 0) {
                    didItAttack = AttemptAttack<Player>(xDir, yDir, false);
                } else {
                    //Favor horizontal movement.
                    if((xDir > 1 || xDir < -1) && (yDir > 1 || yDir < -1)) {
                        xDir = (target.position.x > transform.position.x) ? 1 : -1;
                    }
                    else {
                        if(xDir > 1 || xDir < -1) {
                            xDir = 0;
                        }
                        if(yDir > 1 || yDir < -1) {
                            yDir = 0;
                        }
                    }
                }
            }

            xDir = Mathf.Clamp(xDir, -1, 1);
            yDir = Mathf.Clamp(yDir, -1, 1);

            bool attempt1 = false;
            bool attempt2 = false;
            bool attempt3 = false;

            if(!didItAttack) {
                attempt1 = AttemptMove<Wall>(xDir, yDir, false);
                if(attempt1 == false) {
                    attempt2 = AttemptMove<Wall>(xDir, 0, false);
                    if(attempt2 == false) {
                        attempt3 = AttemptMove<Wall>(0, yDir, false);
                    }
                }
            }

            if(!didItAttack && !attempt1 && !attempt2 && !attempt3) {
                Vector2Int position = new Vector2Int(xDir * 2, yDir * 2);
                bool attemptJump1 = AttemptJump(position.x, position.y);
                if(attemptJump1 == false) {
                    bool attemptJump2 = AttemptJump(0, position.y);
                    if(attemptJump2 == false) {
                        AttemptJump(position.x, 0);
                    }
                }
            }
        }
        
        protected override void OnCantMove<T>(T component) {
            //Do nothing...
        }

        protected override void OnCanAttack<T>(T component) {
            Player hitPlayer = component as Player;

            GameObject arrow = Instantiate(Resources.Load<GameObject>("Prefabs/Arrow")) as GameObject;
            arrow.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 0);

            Arrow arrowScript = arrow.GetComponent<Arrow>();
            arrowScript.target = new Vector3(hitPlayer.transform.position.x, hitPlayer.transform.position.y, 0);
        }
    }
}