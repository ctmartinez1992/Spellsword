using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Completed {
    public class BombExplosion : MonoBehaviour {
        public AudioClip bombExplosion1;
        public AudioClip bombExplosion2;
        
        void Start() {
        }
        
        void Update() {
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if(other.tag == "Player") {
                Player player = other.GetComponent<Player>();
                player.LoseHealth(false);

                SoundManager.instance.RandomizeSfx(bombExplosion1, bombExplosion2);
            }
            else if(other.tag == "Enemy") {
                Enemy enemy = other.GetComponent<Enemy>();
                enemy.TakeDamage("explosion");

                SoundManager.instance.RandomizeSfx(bombExplosion1, bombExplosion2);
            }
            else if(other.tag == "Wall") {
                Wall wall = other.GetComponent<Wall>();
                wall.TakeDamage(true, "explosion");

                SoundManager.instance.RandomizeSfx(bombExplosion1, bombExplosion2);
            }
        }

        public void DestroyThis() {
            Destroy(this.gameObject);
        }
    }
}