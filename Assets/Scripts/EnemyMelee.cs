using UnityEngine;
using System.Collections;

namespace Completed {
    public class EnemyMelee : Enemy {
        private bool skipMove;

        protected override void Start() {
            base.Start();
        }

        //Override the AttemptMove function of MovingObject to include functionality needed for Enemy to skip turns.
        protected override bool AttemptMove<T>(int xDir, int yDir, bool flyer) {
            if(skipMove) {
                skipMove = false;
                return false;
            }

            bool didItMove = base.AttemptMove<T>(xDir, yDir, flyer);
            
            skipMove = true;

            return didItMove;
        }
        
        protected override bool AttemptAttack<T>(int xDir, int yDir, bool motion) {
            return base.AttemptAttack<T>(xDir, yDir, motion);
        }
        
        public override void MoveEnemy() {
            if(dead) {
                return;
            }

            int xDir = 0;
            int yDir = 0;
            
            if(Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon) {
                yDir = (target.position.y > transform.position.y) ? 1 : -1;
            }
            else {
                xDir = (target.position.x > transform.position.x) ? 1 : -1;
            }

            AttemptAttack<Player>(xDir, yDir, true);
            AttemptMove<Wall>(xDir, yDir, false);
        }

        //OnCantMove is called if Enemy attempts to move into a space occupied by a Player, it overrides the OnCantMove function of MovingObject.
        //and takes a generic parameter T which we use to pass in the component we expect to encounter, in this case Player.
        protected override void OnCantMove<T>(T component) {
            base.OnCantMove<T>(component);
        }

        protected override void OnCanAttack<T>(T component) {
            base.OnCanAttack<T>(component);
        }
    }
}