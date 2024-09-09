
using System.Collections;
using UnityEngine;


public class SimpleSFM : FSM
{

    public enum FSMState
    {
        None, Patrol, Chase, Attack, Dead,
    }

    // Current state that the NPC is reaching
    public FSMState curState = FSMState.Patrol;

    // Speed of the tank
    private float curSpeed = 150f;

    // Tank Rotation Speed;
    private float curRotSpeed = 2f;

    // Bullet
    public GameObject Bullet;

    // Whether the NPC is destroyed or not
    private bool bDead = false;
    private int health = 100;

    // We overwrite the deprecated built-in rigdbody
    [SerializeField]
    protected Rigidbody rigidbody;

    // Player Transform
    [SerializeField]
    protected Transform playerTransform;

    // Next destination position of the NPC Tank
    protected Vector3 destPos;

    // List of points for patrolling;
    protected GameObject[] pointList;

    // Buttet shooting rate
    protected float shootRate = 3f;
    protected float elapsedTime = 0f;
    public float maxFireAimError = 0.001f;

    // Status Radius
    public float patrollingRadius = 100f;
    public float attackRadius = 200f;
    public float playerNearRadius = 300f;

    // Tank Turrent
    public Transform turret;
    public Transform bulletSpawnPoint;

    // Start is called before the first frame update
    
   
    protected override void Initialize()
    {
        Debug.Log("hi");
        // Get the target enemy(Player)
        GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");

        // Get the rigidbody
        rigidbody = GetComponent<Rigidbody>();

        // Get the list of points
        pointList = GameObject.FindGameObjectsWithTag("WandarPoint");

        // Set Random destination point first
        FindNextPoint();

        
        playerTransform = objPlayer.transform;
        if(!playerTransform)
        {
            print("Player doesn't exists. Please add one with Tag name 'Player'");
        }
    }

    protected override void FSMUpdate()
    {
        switch(curState)
        {
            case FSMState.Patrol:
                UpdatePatrolState();
                break;
            case FSMState.Chase:
                UpdateChaseState();
                break;
            case FSMState.Attack:
                UpdateAttackState();
                break;
            case FSMState.Dead:
                UpdateDeadState();
                break;
        }

        // Update the time
        elapsedTime += Time.deltaTime;

        // Go to dead state is no health left;
        if(health <= 0)
        {
            curState = FSMState.Dead;
        }
    }

    private void UpdateDeadState()
    {
        if(!bDead)
        {
            bDead = true;
            Explode();
        }
    }

    private void Explode()
    {
        float rndX = Random.Range(10.0f, 10.0f);
        float rndZ = Random.Range(10f, 30f);
        for(int i = 0; i < 3; i++)
        {
            rigidbody.AddExplosionForce(10000f, transform.position - new Vector3(rndX, 10f, rndZ), 40, 10);
            rigidbody.velocity = transform.TransformDirection(new Vector3(rndX, 20, rndZ));

        }
        Destroy(gameObject, 1.5f);
    }

    protected void UpdateAttackState()
    {
        //Set the target position as the player position
        destPos = playerTransform.position;

        //Check the distance with the player tank
        float dist = Vector3.Distance(transform.position, playerTransform.position);
        if (dist >= 200.0f && dist < 300.0f)
        {
            //Rotate to the target point
            Quaternion targetRotation = Quaternion.LookRotation(destPos - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * curRotSpeed);

            //Go Forward
            transform.Translate(Vector3.forward * Time.deltaTime * curSpeed);

            curState = FSMState.Attack;
        }
        //Transition to patrol is the tank become too far
        else if (dist >= 300.0f)
        {
            curState = FSMState.Patrol;
        }

        //Always Turn the turret towards the player
        Quaternion turretRotation = Quaternion.LookRotation(destPos - turret.position);
        turret.rotation = Quaternion.Slerp(turret.rotation, turretRotation, Time.deltaTime * curRotSpeed);

        //Shoot the bullets
        ShootBullet();
    }

    private void ShootBullet()
    {
        if(elapsedTime >= shootRate)
        {
            Instantiate(Bullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            elapsedTime = 0;
        }
    }

    private void UpdateChaseState()
    {
        // Set the target position as the player position
        destPos = playerTransform.position;

        // Check the distance with player tank when the distance is near, transition to attack state
        float dist = Vector3.Distance(transform.position, playerTransform.position);
        if(dist <= attackRadius)
        {
            curState = FSMState.Attack;
        } else if(dist >= playerNearRadius)
        {
            curState = FSMState.Patrol;
        }

        transform.Translate(Vector3.forward * Time.deltaTime * curSpeed);
    }

    private void UpdatePatrolState()
    {
        if(Vector3.Distance(transform.position, destPos) <= 
            patrollingRadius)
        {
            print("Reached to the destination point\n " +
                "calculating the next point");
            FindNextPoint();
        } else if(Vector3.Distance(transform.position, playerTransform.position) <= playerNearRadius)
        {
            print("Switch to Chase Position");
            curState = FSMState.Chase;
        }

        // Rotate to the target point 
        Quaternion targetRotation = Quaternion.LookRotation(destPos - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,Time.deltaTime * curRotSpeed);
        // Go Forward
        transform.Translate(Vector3.forward * Time.deltaTime *
        curSpeed);

    }

    private void FindNextPoint()
    {
        if (pointList == null || pointList.Length == 0)
        {
            Debug.LogError("No wander points found. Ensure that objects are tagged as 'WandarPoint'.");
            return;
        }

        print("Finding next point");
        int rndIndex = Random.Range(0, pointList.Length);
        float rndRadius = 10.0f;
        Vector3 rndPosition = Vector3.zero;
        destPos = pointList[rndIndex].transform.position + rndPosition;

        if (IsCurrentRange(destPos))
        {
            rndPosition = new Vector3(Random.Range(-rndRadius, rndRadius), 0.0f, Random.Range(-rndRadius, rndRadius));
            destPos = pointList[rndIndex].transform.position + rndPosition;
        }
    }


    private bool IsCurrentRange(Vector3 pos)
    {
        float xPos = Mathf.Abs(pos.x - transform.position.x);
        float zPos = Mathf.Abs(pos.z - transform.position.z);

        if (xPos <= 50 && zPos <= 50) return true;
        return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Bullet")
        {
            health -= collision.gameObject.GetComponent<Bullet>().damage;
        }
    }
}
