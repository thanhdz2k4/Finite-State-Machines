using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTankController : MonoBehaviour
{
    public GameObject Bullet;
    public GameObject Turret;
    public GameObject bulletSpawnPoint;

    public float rotSpeed = 150f;
    public float turretRotSpeed = 10f;
    public float maxForwardSpeed = 300f;
    public float maxBackwardSoeed = -300f;
    public float shootRate = 0.5f;

    private float curSpeed, targetSpeed;

    protected float elapsedTime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateWeapon();
        UpdateControl();
    }

    private void UpdateWeapon()
    {
        elapsedTime += Time.deltaTime;
        if(Input.GetMouseButton(0))
        {
            if(elapsedTime >= shootRate)
            {
                // Reset the time
                elapsedTime = 0f;
                // Instantiate the bullet;
                Instantiate(Bullet, transform.position, bulletSpawnPoint.transform.rotation);
            }
        }
    }

    private void UpdateControl()
    {
        // AIMING WITH THE MOUSE
        // Generate a plane that intersects the Transform's
        // position with an upwards normal
        Plane playerPlane = new Plane(Vector3.up, transform.position + new Vector3(0, 0, 0));

        // Generate a ray from the cursor position
        Ray RayCast = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Determine the point where the cursor ray intersects
        // the plane.
        float HitDist = 0;

        // If the ray is parallet to the plane, Raycast will return false
        if(playerPlane.Raycast(RayCast, out HitDist))
        {
            // Get the point along the ray that hits the calculates distance
            Vector3 RayHitPoint = RayCast.GetPoint(HitDist);
            Quaternion targetRotation = Quaternion.LookRotation(RayHitPoint - transform.position);
            Turret.transform.rotation = Quaternion.Slerp(Turret.transform.rotation, targetRotation, Time.deltaTime * turretRotSpeed);
        }

        if(Input.GetKey(KeyCode.W))
        {
            targetSpeed = maxForwardSpeed;
        } else if(Input.GetKey(KeyCode.S))
        {
            targetSpeed = maxBackwardSoeed;
        } else
        {
            targetSpeed = 0;
        }

        if(Input.GetKey(KeyCode.A))
        {
            transform.Rotate(0, -rotSpeed * Time.deltaTime, 0f);
        } else if(Input.GetKey(KeyCode.D))
        {
            transform.Rotate(0, rotSpeed * Time.deltaTime, 0);
        }


        // Determin current speed;
        curSpeed = Mathf.Lerp(curSpeed, targetSpeed, 7.0f * Time.deltaTime);
        transform.Translate(Vector3.forward * Time.deltaTime * curSpeed);
 
    }

    
}
