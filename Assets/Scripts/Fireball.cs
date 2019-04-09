using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Completed {
    public class Fireball : MonoBehaviour {
        public Vector3 target;

        public AudioClip fireballHit1;
        public AudioClip fireballHit2;
        public AudioClip fireExplosion1;
        public AudioClip fireExplosion2;

        public float velocity;

        private Rigidbody2D rb2d;

        void Start() {
        }

        public void Fly() {
            transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * velocity);

            Vector3 dir = target - transform.position;
            float angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg) - 45;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        void FixedUpdate() {
            Fly();

            if(Vector3.Distance(this.transform.position, target) < Mathf.Epsilon) {
                GameObject fireExplosion = Instantiate(Resources.Load<GameObject>("Prefabs/FireExplosion")) as GameObject;
                fireExplosion.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 0);

                SoundManager.instance.RandomizeSfx(fireExplosion1, fireExplosion2);

                gameObject.SetActive(false);
                Destroy(gameObject, 1f);
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other) {
            if(other.tag == "Enemy") {
                this.target = new Vector3(other.transform.position.x, other.transform.position.y, 0);

                Enemy enemy = other.GetComponent<Enemy>();
                enemy.TakeDamage("explosion");

                SoundManager.instance.RandomizeSfx(fireballHit1, fireballHit2);
            }
            else if(other.tag == "Wall") {
                this.target = new Vector3(other.transform.position.x, other.transform.position.y, 0);

                Wall wall = other.GetComponent<Wall>();
                wall.TakeDamage(true, "explosion");

                SoundManager.instance.RandomizeSfx(fireballHit1, fireballHit2);
            }
        }
    }
}