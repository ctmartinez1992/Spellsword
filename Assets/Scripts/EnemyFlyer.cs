using UnityEngine;
using System.Collections;

namespace Completed {
    //Enemy inherits from MovingObject, our base class for objects that can move, Player also inherits from this.
    public class EnemyFlyer : Enemy {
        private bool skipMove;                  //Boolean to determine whether or not enemy should skip a turn or move this turn.

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

            Vector3 dir = transform.position - target.position;
            int xDir = Mathf.RoundToInt(-dir.x);
            int yDir = Mathf.RoundToInt(-dir.y);

            xDir = Mathf.Clamp(xDir, -1, 1);
            yDir = Mathf.Clamp(yDir, -1, 1);

            if(Random.value < .5f) {
                xDir *= 2;
            }
            if(Random.value < .5f) {
                yDir *= 2;
            }

            bool attemptMove = false;
            bool attemptAttack = AttemptAttack<Player>(xDir, yDir, true);
            if(!attemptAttack) {
                attemptMove = AttemptMove<Wall>(xDir, yDir, true);
            }

            if(!attemptAttack && !attemptMove) {
                //Try to move/attack 10 times in random direction, if all fail do nothing.
                int counter = 0;

                while(counter < 10) {
                    int randomX = Random.Range((int)transform.position.x - 2, (int)transform.position.x + 2);
                    int randomY = Random.Range((int)transform.position.y - 2, (int)transform.position.y + 2);

                    attemptAttack = AttemptAttack<Player>(xDir, yDir, true);
                    if(!attemptAttack) {
                        attemptMove = AttemptMove<Wall>(xDir, yDir, true);
                    }

                    if(attemptAttack || attemptMove) {
                        break;
                    }

                    counter++;
                }
            }
        }

        protected override void OnCantMove<T>(T component) {
            //Do nothing...
        }

        protected override void OnCanAttack<T>(T component) {
            Player hitPlayer = component as Player;
            hitPlayer.LoseFood();

            GameObject blood = Instantiate(Resources.Load<GameObject>("Prefabs/Blood")) as GameObject;
            blood.transform.position = new Vector3(component.transform.position.x, component.transform.position.y, 0);
            blood.transform.SetParent(component.transform);
        }
    }
}