using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Completed {
    public class Bomb : MonoBehaviour {
        private AudioSource bombSizzleSource;
        private GameObject playerGO;
        private int lastPlayerTurnCount;

        public Vector3 target;

        public AudioClip bombSizzle;
        public AudioClip bombExplosion1;
        public AudioClip bombExplosion2;

        public float velocity;

        //0 = Fly to target. When it reaches target, pass to stage 1.
        //1 = Sizzling. When a turn is rolled, pass to stage 2.
        //2 = Explosion.
        public int stage;

        void Start() {
            bombSizzleSource = GetComponent<AudioSource>();
            bombSizzleSource.Stop();

            playerGO = GameObject.Find("Player");

            stage = 0;
        }

        public void Fly() {
            transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * velocity);

            Vector3 dir = target - transform.position;
            float angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg) - 45;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        public void Explode() {
            GameObject fireExplosionMiddle = Instantiate(Resources.Load<GameObject>("Prefabs/BombExplosion")) as GameObject;
            fireExplosionMiddle.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 0);
            GameObject fireExplosionTop = Instantiate(Resources.Load<GameObject>("Prefabs/BombExplosion")) as GameObject;
            fireExplosionTop.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 1, 0);
            GameObject fireExplosionBot = Instantiate(Resources.Load<GameObject>("Prefabs/BombExplosion")) as GameObject;
            fireExplosionBot.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - 1, 0);
            GameObject fireExplosionLeft = Instantiate(Resources.Load<GameObject>("Prefabs/BombExplosion")) as GameObject;
            fireExplosionLeft.transform.position = new Vector3(this.transform.position.x + 1, this.transform.position.y, 0);
            GameObject fireExplosionRight = Instantiate(Resources.Load<GameObject>("Prefabs/BombExplosion")) as GameObject;
            fireExplosionRight.transform.position = new Vector3(this.transform.position.x - 1, this.transform.position.y, 0);
            GameObject fireExplosionTopLeft = Instantiate(Resources.Load<GameObject>("Prefabs/BombExplosion")) as GameObject;
            fireExplosionTopLeft.transform.position = new Vector3(this.transform.position.x - 1, this.transform.position.y + 1, 0);
            GameObject fireExplosionTopRight = Instantiate(Resources.Load<GameObject>("Prefabs/BombExplosion")) as GameObject;
            fireExplosionTopRight.transform.position = new Vector3(this.transform.position.x + 1, this.transform.position.y + 1, 0);
            GameObject fireExplosionBotLeft = Instantiate(Resources.Load<GameObject>("Prefabs/BombExplosion")) as GameObject;
            fireExplosionBotLeft.transform.position = new Vector3(this.transform.position.x - 1, this.transform.position.y - 1, 0);
            GameObject fireExplosionBotRight = Instantiate(Resources.Load<GameObject>("Prefabs/BombExplosion")) as GameObject;
            fireExplosionBotRight.transform.position = new Vector3(this.transform.position.x + 1, this.transform.position.y - 1, 0);

            gameObject.SetActive(false);
            Destroy(gameObject, 1f);
        }

        void FixedUpdate() {
            if(stage == 0) {
                Fly();
            }
            else if(stage == 1) {
                bombSizzleSource.Play();
            }
            else if(stage == 2) {
                Explode();
            }

            if(stage == 0 && Vector3.Distance(this.transform.position, target) < Mathf.Epsilon) {
                stage = 1;
                lastPlayerTurnCount = playerGO.GetComponent<Player>().turnCount;
            }
            if(stage == 1 && playerGO.GetComponent<Player>().turnCount > lastPlayerTurnCount) {
                stage = 2;
            }
        }
    }
}