using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarWheel : MonoBehaviour
{
    float maxTorque = 5f;

    public WheelCollider targetWheel;
    private Vector3 wheelPosition = new Vector3();
    private Quaternion wheelRotation = new Quaternion();

    // Use this for initialization
    void Start()
    {

    }

    void FixedUpdate()
    {
        float accelerate = Input.GetAxis("Vertical");
        float steer = Input.GetAxis("Horizontal");

        targetWheel.motorTorque = accelerate * maxTorque;
    }

    // Update is called once per frame 
    void Update()
    {
        targetWheel.GetWorldPose(out wheelPosition, out wheelRotation);
        transform.position = wheelPosition;
        transform.rotation = wheelRotation;
    }
}
