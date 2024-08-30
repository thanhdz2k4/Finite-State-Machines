
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
    protected Rigidbody rigidbody;

    // Player Transform
    protected Transform playerTransform;

    // Next destination position of the NPC Tank
    protected Vector3 destPos;

    // List of points for patrolling;
    protected GameObject[] poinList;

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
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void Initialize()
    {
        // Get the list of points
        poinList = GameObject.FindGameObjectsWithTag("WandarPoint");

        // Set Random destination point first
        FindNextPoint();

        // Get the target enemy(Player)
        GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");

        // Get the rigidbody
        rigidbody = GetComponent<Rigidbody>();
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
        throw new System.NotImplementedException();
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
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * curRotSpeed);

    }

    private void FindNextPoint()
    {
        print("Finding next point");
        int rndIndex = Random.Range(0, poinList.Length);
        float rndRadius = 10f;
        Vector3 rndPosition = Vector3.zero;
        destPos = poinList[rndIndex].transform.position + rndPosition;
        
        // Check Range to decide the random point as the same as before
        if(IsCurrentRange(destPos))
        {
            rndPosition = new Vector3(Random.Range(-rndRadius, rndRadius), 0, Random.Range(-rndRadius, rndRadius));
            destPos = poinList[rndIndex].transform.position + rndPosition;
        }


    }

    private bool IsCurrentRange(Vector3 pos)
    {
        float xPos = Mathf.Abs(pos.x - transform.position.x);
        float zPos = Mathf.Abs(pos.z - transform.position.z);

        if (xPos <= 50 && zPos <= 50) return true;
        return false;
    }
}
