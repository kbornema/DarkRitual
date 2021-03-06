using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class EnemyBehaviour_Ranged : NetworkBehaviour {

	public bool playerInSight = false;
	public float playerDistance;
	[HideInInspector]
	public GameObject player;
	public GameObject bulletPrefab;
	private SpriteRenderer spriteRenderer;

	public Animator animator;

	public int enemyHealth = 50;

	// Movement variables
	float movementRange = 1;
	int stepMultiplier;
	bool isMoving = true;
	float movementTime;
	Vector2 movementDirection;
	Vector2 nullMovement2 = new Vector2 (0, 0);
	Vector3 nullMovement3 = new Vector3 (0, 0, 0);

	// Attack variables
	public float attackThreshold_maxValue = 4;
	float attackThreshold;
	//	public int enemyDamage = 10;

	public float speed = 1.0f;

    public float timeSpeedUp = 1.0f;


    public float bulletSpeed = 5.0f;


	// Use this for initialization
	void Start () 
    {
        if (isServer)
        {
            //playerDistance = gameObject.transform.child gameObject.GetComponent<
            stepMultiplier = Random.Range(1, 4);
            movementRange *= stepMultiplier;
            movementTime = movementRange;
            movementDirection = GetNewDirection();

            attackThreshold = attackThreshold_maxValue;
            animator = GetComponent<Animator>();
            spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        }
	}


	void FixedUpdate () 
    {
        if (isServer)
        {



            attackThreshold -= Time.deltaTime * timeSpeedUp;
            if (attackThreshold < 0)
                attackThreshold = 0;

            // Run towards Player if in sight. Does only work with 1 Player right now
            if (playerInSight)
            {

                if (player == null)
                {
                    playerInSight = false;
                    return;
                }

                var heading = player.transform.position - gameObject.transform.position;
                var distance = heading.magnitude;



                if (attackThreshold < 0.1f)
                {
                    GameObject spawnedBulled = (GameObject)GameObject.Instantiate(bulletPrefab, gameObject.transform.position, Quaternion.identity);

         
                    
                    Vector3 direction = heading / distance;
                    direction.z = 0;
                    //spawnedBulled.GetComponent<Movement>().direction = direction;
                    //            Mathf.Atan2(direction.y, direction.x);
                    spawnedBulled.transform.rotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90.0f);

                    spawnedBulled.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed;
                    //TODO: "attack"
                    //player.GetComponent<Hea>().playerHealth-= enemyDamage;
                    attackThreshold += attackThreshold_maxValue;
                    NetworkServer.Spawn(spawnedBulled);
                }


                // Attack
                if (distance < 1.5f)
                {


                }

                // Hold distance. Is already done via colliders
                if (distance < playerDistance)
                {
                    Vector3 direction = heading / distance;
                    direction.z = 0;
                    gameObject.transform.Translate(-direction * Time.deltaTime * speed);
                    HandleFlip(direction);
                }
                else
                {
                    gameObject.transform.Translate(movementDirection * Time.deltaTime * speed, 0);
                    HandleFlip(movementDirection);
                    movementTime -= Time.deltaTime;
                    if (movementTime < 0)
                    {
                        if (Random.Range(0, 3) > 0)
                        {
                            movementDirection = GetNewDirection();
                            animator.SetBool("IsMoving", true);
                        }

                        else
                        {
                            movementDirection = nullMovement2;
                            animator.SetBool("IsMoving", false);
                        }

                        movementTime = movementRange;
                    }

                }
                animator.SetBool("IsMoving", true);
            }
            else
            {
                gameObject.transform.Translate(movementDirection * Time.deltaTime * speed, 0);
                HandleFlip(movementDirection);
                movementTime -= Time.deltaTime;
                if (movementTime < 0)
                {
                    if (Random.Range(0, 2) < 1)
                    {
                        movementDirection = GetNewDirection();
                        animator.SetBool("IsMoving", true);
                    }

                    else
                    {
                        movementDirection = nullMovement2;
                        animator.SetBool("IsMoving", false);
                    }

                    movementTime = movementRange;
                }
            }
        }
	}

	private void HandleFlip(Vector2 movement)
	{
		if (movement.x < 0.0f)
			spriteRenderer.flipX = false;

		else if (movement.x > 0.0f)
			spriteRenderer.flipX = true;
	}

	Vector2 GetNewDirection(){

		return( Random.insideUnitCircle);
	}

}
