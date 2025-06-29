using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementOnTheBoard : MonoBehaviour
{
    private InputAction moveAction;
    public float turnSpeed = 80f;

    private Rigidbody rb;
    void Start()
    {
        moveAction = new InputAction("Move", binding: "<XRController>{LeftHand}/thumbstick");
        moveAction.Enable();
        rb = GetComponent<Rigidbody>();
        transform.GetChild(0).GetChild(1).GetChild(1).gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
         rb.AddForce(transform.forward * turnSpeed * Time.fixedDeltaTime, ForceMode.Impulse);
    }
}
