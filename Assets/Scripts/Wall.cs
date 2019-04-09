using UnityEngine;
using System.Collections;

namespace Completed {
	public class Wall : MonoBehaviour {
        public AudioClip chopSound1;
        public AudioClip chopSound2;
        public AudioClip fireExplosionSound1;
        public AudioClip fireExplosionSound2;
        public AudioClip burnSound1;
        public AudioClip burnSound2;

        public GameObject[] dmgSprite;					//Alternate sprite to display after Wall has been attacked by player.

		public int hp = 3;

        public bool breakable;

		void Awake() {
		}

        //Types of damage: explosion.
		public void TakeDamage(bool fullDamage, string typeDmg) {
            if(fullDamage) {
                hp = 0;

                if(typeDmg == "explosion") {
                    GameObject fireExplosion = Instantiate(Resources.Load<GameObject>("Prefabs/FireExplosion")) as GameObject;
                    fireExplosion.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 0);
                    fireExplosion.transform.SetParent(this.transform);

                    SoundManager.instance.RandomizeSfx(fireExplosionSound1, fireExplosionSound2);
                }

                Invoke("DestroyWallWithBurn", 0.2f);
            }
            else {
                SoundManager.instance.RandomizeSfx(chopSound1, chopSound2);

                if(hp < 0) {
                    Invoke("DestroyWall", 0.2f);
                }
                else {
                    GameObject wallDmg = Instantiate(dmgSprite[hp]) as GameObject;
                    wallDmg.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 0);
                    wallDmg.transform.SetParent(this.transform);
                }
            }
		}

        private void DestroyWallWithBurn() {
            GameObject burn = Instantiate(Resources.Load<GameObject>("Prefabs/Burn")) as GameObject;
            burn.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 0);
            burn.transform.SetParent(this.transform);

            SoundManager.instance.RandomizeSfx(burnSound1, burnSound2);

            Invoke("DestroyWall", 0.2f);
        }
        
        private void DestroyWall() {
            this.gameObject.SetActive(false);
        }
    }
}