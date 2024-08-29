
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
                UpdateDeatState();
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

    private void UpdateDeatState()
    {
        
    }

    private void UpdateAttackState()
    {
        
    }

    private void UpdateChaseState()
    {
        
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

    }
}
