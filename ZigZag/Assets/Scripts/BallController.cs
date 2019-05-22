using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public GameObject Particle;

    [SerializeField]
    private float Speed;

    bool Started;
    bool GameOver;
    Rigidbody rb;

    void Awake()
    {
        // Access the game object's RigidBody component
        rb = GetComponent<Rigidbody>();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Initial start
        if (!Started)
        {
            if (Input.GetMouseButtonDown(0))
            {
                rb.velocity = new Vector3(Speed, 0, 0);
                Started = true;

                GameManager.Instance.GameStart();
            }
        }

        // Game over if ball not touching floor
        if (!Physics.Raycast(transform.position, Vector3.down, 1f))
        {
            GameOver = true;

            // Ball falls over the edge
            rb.velocity = rb.velocity * 0.99f;
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.FreezeRotation;

            // Camera follow ends, GAME OVER!!!!!
            Camera.main.GetComponent<CameraFollow>().GameOver = true;
            GameObject.Find("PlatformSpawner").GetComponent<PlatformSpawner>().GameOver = true;

            GameManager.Instance.GameOver();
        }

        // Switch directions
        if (Input.GetMouseButtonDown(0) && !GameOver)
        {
            SwitchDirection();
            //SwitchDirectionRight();
        }
        else if (Input.GetMouseButtonDown(1) && !GameOver)
        {
            SwitchDirection();
            //SwitchDirectionLeft();
        }
    }

    void SwitchDirection()
    {
        if (rb.velocity.z > 0)
        {
            rb.velocity = new Vector3(Speed, 0, 0);
        }
        else if (rb.velocity.x > 0)
        {
            rb.velocity = new Vector3(0, 0, Speed);
        }
    }

    void SwitchDirectionLeft()
    {
        if (rb.velocity.z > 0)
        {
            rb.velocity = new Vector3(Speed, 0, 0);
        }
        else if (rb.velocity.x > 0 || rb.velocity.x < 0)
        {
            rb.velocity = new Vector3(0, 0, Speed);
        }
    }

    void SwitchDirectionRight()
    {
        if (rb.velocity.z > 0)
        {
            rb.velocity = new Vector3(-(Speed), 0, 0);
        }
        else if (rb.velocity.x > 0 || rb.velocity.x < 0)
        {
            rb.velocity = new Vector3(0, 0, Speed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Diamond")
        {
            GameObject particle = Instantiate(Particle, other.gameObject.transform.position, Quaternion.identity);
            Destroy(other.gameObject);
            Destroy(particle, 1f);
        }
    }
}
