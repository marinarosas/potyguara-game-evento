using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SpherePhysicsToBoard : MonoBehaviour
{
    public GameObject sphere;

    public float speed = 5f;
    public float stopDistance = 0.5f;
    public float rotationSpeed = 5f;
    private float distance;

    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    
    void FixedUpdate()
    {
        Transform currentTarget = sphere.transform;
        distance = Vector3.Distance(transform.position, currentTarget.position);
        if (distance > stopDistance)
        {
            float speedNow = speed;
            Vector3 direction = (currentTarget.position - transform.position).normalized;
            rb.MovePosition(transform.position + direction * speedNow * Time.fixedDeltaTime);

            Quaternion lookRotation = Quaternion.LookRotation(direction);
            Quaternion adjustedRotation = lookRotation * Quaternion.Euler(0, 0, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, adjustedRotation, rotationSpeed * Time.fixedDeltaTime);
        }
        
    }
}
