using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Roller : MonoBehaviour
{
    public float Speed;
    public float Gravity;
    public float JumpingPower;

    private Rigidbody rb;
    private GameObject unicycle;
    private bool isGrounded = true;


    private bool isMovingForward;
    private bool isMovingBackward;
    private Vector3 LastPOS;
    private Vector3 NextPOS;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        unicycle = transform.parent.gameObject;
    }

    private void Update()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //OrientPlayer();

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        //Vector3 movement = new Vector3(-moveHorizontal, -.4f, 0.0f);
        Vector3 movement = new Vector3(-moveHorizontal, 0, 0);

        rb.AddForce(movement * Speed * Time.deltaTime);
        //rb.AddForce(Vector3.down * Gravity * rb.mass * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Debug.Log("JUNMPING!!!");
            //Vector3 jump = new Vector3(0.0f, 200000.0f, 0.0f);
            Vector3 jump = new Vector3(0.0f, JumpingPower, 0.0f);

            rb.AddForce(jump);
        }

        if (transform.position.y <= -10.0f)
        {
            //RELOAD SCENE
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("HIT GROUND => " + collision.gameObject.name);

        if (collision.gameObject.name == "ZoomZoo")
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("LEAVE GROUND => " + collision.gameObject.name);

        if (collision.gameObject.name == "ZoomZoo")
        {
            isGrounded = false;

            Debug.Log("VELOCITY => " + rb.velocity);
        }
    }

    void OrientPlayer()
    {

        // C# and UnityScript
        var velocity = rb.velocity;
        var localVel = transform.InverseTransformDirection(velocity);

        if (localVel.x > 0)
        {
            //unicycle.transform.rotation = Quaternion.Lerp(unicycle.transform.rotation, Quaternion.Euler(unicycle.transform.rotation.x, 90f, unicycle.transform.rotation.z), Time.deltaTime * 0.1f);
            unicycle.transform.rotation = Quaternion.Euler(unicycle.transform.rotation.x, 90f, unicycle.transform.rotation.z);
        }
        else if (localVel.x < 0)
        {
            //unicycle.transform.rotation = Quaternion.Lerp(unicycle.transform.rotation, Quaternion.Euler(unicycle.transform.rotation.x, -90f, unicycle.transform.rotation.z), Time.deltaTime * 0.1f);
            unicycle.transform.rotation = Quaternion.Euler(unicycle.transform.rotation.x, -90f, unicycle.transform.rotation.z);
        }
        else
        {

        }
    }
}
