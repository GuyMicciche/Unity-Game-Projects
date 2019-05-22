using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public float SmoothSpeed;
    public Vector3 Offset;
    public float ClampX;
    //private Vector3 offset;

    private Vector3 velocity = Vector3.zero;


    // Use this for initialization
    void Start()
    {
        //offset = transform.position - player.transform.position;
        Offset = (Offset + transform.position) - player.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 desiredPosition = player.transform.position + Offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, SmoothSpeed);
        //Vector3 smoothedPosition = Vector3.MoveTowards(transform.position, desiredPosition, SmoothSpeed);
        //Debug.Log("THIS IS THE DESIRED OFFSET => " + (desiredPosition - smoothedPosition));
        //Debug.Log("CURRENT POSITION => " + desiredPosition);

        transform.position = smoothedPosition;


        if (Math.Abs(desiredPosition.x - smoothedPosition.x) >= ClampX)
        {
            Vector3 p = player.transform.position;

            // NO SMOOTH
            if ((desiredPosition.x - smoothedPosition.x) > 0)
            {
                transform.position = desiredPosition + new Vector3(-ClampX, 0, 0);
            }
            else
            {
                transform.position = desiredPosition + new Vector3(ClampX, 0, 0);
            }
        }
        else
        {
            // SMOOTH
            transform.position = smoothedPosition;
        }



        //Vector3 aheadPoint = player.transform.position + new Vector3(player.transform.GetComponent<Rigidbody>().velocity.x, 0, 0);
        //Vector3 point = Camera.main.WorldToViewportPoint(aheadPoint);
        //Vector3 delta = aheadPoint - Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
        //Vector3 destination = transform.position + delta;
        //transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, SmoothSpeed);



        //Vector3 desiredPosition = player.transform.position + Offset;
        //Vector3 smoothedPosition = Vector3.MoveTowards(transform.position, desiredPosition, SmoothSpeed);
        //Debug.Log("THIS IS THE DESIRED OFFSET => " + (desiredPosition.z - smoothedPosition.z));
        //Debug.Log("CURRENT POSITION => " + desiredPosition);

        //if (Math.Abs(desiredPosition.z - smoothedPosition.z) > ClampZ)
        //{

        //    // NO SMOOTH
        //    if ((desiredPosition.z - smoothedPosition.z) > 0)
        //    {
        //        transform.position = desiredPosition + new Vector3(0, 0, -ClampZ);
        //    }
        //    else
        //    {
        //        transform.position = desiredPosition + new Vector3(0, 0, ClampZ);
        //    }
        //}
        //else
        //{
        //    Vector3 p = player.transform.position;

        //    if ((desiredPosition.z - smoothedPosition.z) > 0)
        //    {
        //        Debug.Log("GOING FORWARD");

        //        Vector3 o = p - desiredPosition;

        //        desiredPosition = new Vector3(transform.position.x, transform.position.y, o.z);
        //    }
        //    else
        //    {
        //        Debug.Log("GOING BACKWARD");

        //        Vector3 o = p + desiredPosition;

        //        desiredPosition = new Vector3(transform.position.x, transform.position.y, o.z);
        //    }

        //    // SMOOTH
        //    transform.position = Vector3.Lerp(transform.position, desiredPosition, SmoothSpeed);
        //}
    }
}
