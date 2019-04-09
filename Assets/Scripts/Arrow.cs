using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Arrows can only be shot by enemies. So only the player can get hit by them.
//Ultimately, every arrow that is shot is going to hit the player, otherwise it's not shot at all.
namespace Completed {
    public class Arrow : MonoBehaviour {
        public GameObject player;
        public Vector3 target;

        public AudioClip arrowThud1;
        public AudioClip arrowThud2;

        public float velocity;

        private Rigidbody2D rb2d;

        void Start() {
            player = GameObject.Find("Player");
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
                Player hitPlayer = player.GetComponent<Player>();
                hitPlayer.LoseHealth(true);

                SoundManager.instance.RandomizeSfx(arrowThud1, arrowThud2);

                gameObject.SetActive(false);
                Destroy(gameObject, 1f);
            }
        }
    }
}