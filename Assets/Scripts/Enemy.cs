using UnityEngine;
using System.Collections;

namespace Completed {
    //Enemy inherits from MovingObject, our base class for objects that can move, Player also inherits from this.
    public abstract class Enemy : MovingObject {
        public AudioClip attackSound1;          //First of two audio clips to play when attacking the player.
        public AudioClip attackSound2;          //Second of two audio clips to play when attacking the player.

        public bool dead;

        protected Transform target;             //Transform to attempt to move toward each turn.

        protected override void Start() {
            dead = false;

            //Register this enemy with our instance of GameManager by adding it to a list of Enemy objects. 
            //This allows the GameManager to issue movement commands.
            GameManager.instance.AddEnemyToList(this);

            target = GameObject.FindGameObjectWithTag("Player").transform;

            base.Start();
        }

        //Override the AttemptMove function of MovingObject to include functionality needed for Enemy to skip turns.
        protected override bool AttemptMove<T>(int xDir, int yDir, bool flyer) {
            return base.AttemptMove<T>(xDir, yDir, flyer);
        }

        protected override bool AttemptAttack<T>(int xDir, int yDir, bool motion) {
            return base.AttemptAttack<T>(xDir, yDir, motion);
        }

        //MoveEnemy is called by the GameManger each turn to tell each Enemy to try to move towards the player.
        public abstract void MoveEnemy();
        
        protected override void OnCantMove<T>(T component) {
            Wall hitWall = component as Wall;
            if(hitWall.breakable) {
                hitWall.TakeDamage(true, "");
            }
        }
        
        protected override void OnCanAttack<T>(T component) {
            Player hitPlayer = component as Player;
            hitPlayer.LoseHealth(true);
            SoundManager.instance.RandomizeSfx(attackSound1, attackSound2);
        }
        
        //OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
        private void OnTriggerEnter2D(Collider2D other) {
            if(other.tag == "Exit") {
                GameManager.instance.extraEnemies++;
                this.gameObject.SetActive(false);
            }
        }

        //Types of damage: blood, explosion.
        public void TakeDamage(string typeDmg) {
            dead = true;

            if(typeDmg == "blood") {
                GameObject blood = Instantiate(Resources.Load<GameObject>("Prefabs/Blood")) as GameObject;
                blood.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 0);
            }
            else if(typeDmg == "explosion") {
                GameObject fireExplosion = Instantiate(Resources.Load<GameObject>("Prefabs/FireExplosion")) as GameObject;
                fireExplosion.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 0);
            }

            Invoke("DestroyEnemy", 0.2f);
        }

        private void DestroyEnemy() {
            this.gameObject.SetActive(false);
        }
    }
}