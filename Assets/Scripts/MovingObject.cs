using UnityEngine;
using System.Collections;

namespace Completed {
	public abstract class MovingObject : MonoBehaviour {
		public float moveTime = 0.1f;			//Time it will take object to move, in seconds.
		public LayerMask blockingLayer;			//Layer on which collision will be checked.

		private BoxCollider2D boxCollider;
		private Rigidbody2D rb2D;
		private float inverseMoveTime;			//Used to make movement more efficient.

        protected bool busy;

        protected virtual void Start() {
			boxCollider = GetComponent<BoxCollider2D>();
			rb2D = GetComponent<Rigidbody2D>();
			
			//By storing the reciprocal of the move time we can use it by multiplying instead of dividing, this is more efficient.
			inverseMoveTime = 1f / moveTime;
		}

		//Move returns true if it is able to move and false if not. 
		//Move takes parameters for x direction, y direction and a RaycastHit2D to check collision.
		protected bool Move(int xDir, int yDir, bool flyer, out RaycastHit2D hit) {
			// Calculate end position based on the direction parameters passed in when calling Move.
			Vector2 start = transform.position;
			Vector2 end = start + new Vector2 (xDir, yDir);

			boxCollider.enabled = false;							//Disable the boxCollider so that linecast doesn't hit this object's own collider.
			hit = Physics2D.Linecast(start, end, blockingLayer);	//Cast a line from start point to end point checking collision on blockingLayer.
			boxCollider.enabled = true;								//Re-enable boxCollider after linecast.

			if(hit.transform == null || (flyer && hit.transform.tag == "Wall")) {
				StartCoroutine(SmoothMovement(end));
				return true;
			}

			return false;
		}

		//Co-routine for moving units from one space to next, takes a parameter end to specify where to move to.
		protected IEnumerator SmoothMovement(Vector3 end) {
			//Calculate the remaining distance to move based on the square magnitude of the difference between current position and end parameter. 
			//Square magnitude is used instead of magnitude because it's computationally cheaper.
			float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            busy = true;

			while(sqrRemainingDistance > float.Epsilon) {
				Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
				rb2D.MovePosition(newPosition);
				sqrRemainingDistance = (transform.position - end).sqrMagnitude;
				yield return null;
			}

            busy = false;
        }

        protected bool Attack(int xDir, int yDir, bool motion, out RaycastHit2D hit) {
            Vector2 start = transform.position;
            Vector2 end = start + new Vector2(xDir, yDir);

            boxCollider.enabled = false;                            //Disable the boxCollider so that linecast doesn't hit this object's own collider.
            hit = Physics2D.Linecast(start, end, blockingLayer);    //Cast a line from start point to end point checking collision on blockingLayer.
            boxCollider.enabled = true;                             //Re-enable boxCollider that was disabled when the linecast.
            
            if(hit.transform != null) {
                if(motion) {
                    StartCoroutine(SmoothAttack(end, start));
                }
                return true;
            }

            return false;
        }
        
        protected IEnumerator SmoothAttack(Vector3 end, Vector3 start) {
            //Calculate the remaining distance to move based on the square magnitude of the difference between current position and end parameter. 
            //Square magnitude is used instead of magnitude because it's computationally cheaper.
            float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            float sqrDistanceHalf = sqrRemainingDistance / 2;

            busy = true;

            while(sqrRemainingDistance > sqrDistanceHalf) {
                Vector3 newPostion = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
                rb2D.MovePosition(newPostion);
                sqrRemainingDistance = (transform.position - end).sqrMagnitude;
                yield return null;
            }

            while(sqrRemainingDistance > float.Epsilon) {
                Vector3 newPostion = Vector3.MoveTowards(rb2D.position, start, inverseMoveTime * Time.deltaTime);
                rb2D.MovePosition(newPostion);
                sqrRemainingDistance = (transform.position - start).sqrMagnitude;
                yield return null;
            }

            busy = false;
        }


        protected bool Jump(int xDir, int yDir, out RaycastHit2D[] hits) {
            Vector2 start = transform.position;
            Vector2 end = start + new Vector2(xDir, yDir);

            boxCollider.enabled = false;
            hits = Physics2D.LinecastAll(start, end, blockingLayer);
            boxCollider.enabled = true;

            bool cantJump = false;

            foreach(RaycastHit2D hit in hits) {
                if((hit.transform.position.x == end.x && hit.transform.position.y == end.y) &&
                   (hit.transform.tag == "Wall" || hit.transform.tag == "Enemy" || hit.transform.tag == "OuterWall" || hit.transform.tag == "Player"))
                {
                    cantJump = true;
                }
            }
            
            if(!cantJump) {
                StartCoroutine(SmoothJump(xDir, yDir));
                return true;
            }

            return false;
        }
        
        protected IEnumerator SmoothJump(int xDir, int yDir) {
            Vector2 start = transform.position;
            Vector2 end = start + new Vector2(xDir, yDir);
            
            Vector2 middle = new Vector2(start.x + (xDir / 2f), start.y + (yDir / 2f));

            float sqrRemainingDistance = (start - middle).sqrMagnitude;
            
            Vector3 startScale = transform.localScale;
            Vector3 endScale = new Vector3(1.25f, 1.25f, 1f);

            busy = true;

            boxCollider.enabled = false;

            while(sqrRemainingDistance > float.Epsilon) {
                Vector3 newPosition = Vector3.MoveTowards(rb2D.position, middle, inverseMoveTime * Time.deltaTime);
                rb2D.MovePosition(newPosition);
                
                //transform.localScale = Vector3.MoveTowards(startScale, endScale, inverseMoveTime * Time.deltaTime);

                sqrRemainingDistance = (new Vector2(transform.position.x, transform.position.y) - middle).sqrMagnitude;
                yield return null;
            }

            sqrRemainingDistance = (middle - end).sqrMagnitude;

            while(sqrRemainingDistance > float.Epsilon) {
                Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
                rb2D.MovePosition(newPosition);

                //transform.localScale = Vector3.MoveTowards(endScale, startScale, inverseMoveTime * Time.deltaTime);

                sqrRemainingDistance = (new Vector2(transform.position.x, transform.position.y) - end).sqrMagnitude;
                yield return null;
            }

            busy = false;

            boxCollider.enabled = true;
        }

        //The virtual keyword means AttemptMove can be overridden by inheriting classes using the override keyword.
        //AttemptMove takes a generic parameter T to specify the type of component we expect our unit to interact with if blocked (Player for Enemies, Wall for Player).
        protected virtual bool AttemptMove<T>(int xDir, int yDir, bool flyer) where T : Component {
			//Hit will store whatever our linecast hits when Move is called.
			RaycastHit2D hit;

			bool canMove = Move(xDir, yDir, flyer, out hit);
            
            if(hit.transform != null) {
                T hitComponent = hit.transform.GetComponent<T>();

                //If canMove is false and hitComponent is not equal to null, meaning MovingObject is blocked and has hit something it can interact with.
                if(!canMove && hitComponent != null) {
                    OnCantMove(hitComponent);
                }
            }

            return canMove;
		}

        protected virtual bool AttemptJump(int xDir, int yDir) {
            RaycastHit2D[] hits;

            bool canJump = Jump(xDir, yDir, out hits);

            return canJump;
        }

        protected virtual bool AttemptAttack<T>(int xDir, int yDir, bool motion) where T : Component {
            RaycastHit2D hit;

            bool canAttack = Attack(xDir, yDir, motion, out hit);
            
            if(hit.transform != null) {
                T hitComponent = hit.transform.GetComponent<T>();
                
                if(canAttack && hitComponent != null && (hitComponent as Player || hitComponent as Enemy)) {
                    OnCanAttack(hitComponent);
                    return true;
                }
            }

            return canAttack;
        }

        protected abstract void OnCantMove<T>(T component) where T : Component;
        protected abstract void OnCanAttack<T>(T component) where T : Component;
    }
}