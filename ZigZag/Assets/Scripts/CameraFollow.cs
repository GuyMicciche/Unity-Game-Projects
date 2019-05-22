using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject Ball;
    public float LerpRate;
    public bool GameOver;

    Vector3 Offset;

    // Use this for initialization
    void Start()
    {
        Offset = Ball.transform.position - transform.position;
        GameOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!GameOver)
        {
            Follow();
        }
        else
        {
            EndFollow();
        }
    }

    void EndFollow()
    {
        Vector3 originalPos = transform.position;
        Vector3 targetPos = new Vector3(Ball.transform.position.x - Offset.x, transform.position.y, Ball.transform.position.z - Offset.z);
        transform.position = Vector3.Lerp(originalPos, targetPos, LerpRate * Time.deltaTime);
    }

    void Follow()
    {
        Vector3 originalPos = transform.position;
        Vector3 targetPos = Ball.transform.position - Offset;
        transform.position = Vector3.Lerp(originalPos, targetPos, LerpRate * Time.deltaTime);
    }
}
