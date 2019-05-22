using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    public GameObject Platform;
    public GameObject Diamond;

    public bool GameOver;

    Vector3 lastPos;
    float size;

    // Use this for initialization
    void Start()
    {
        lastPos = Platform.transform.position;
        size = Platform.transform.localScale.x;

        for (var i = 0; i < 100; i++)
        {
            SpawnPlatform();
        }

        StartSpawningPlatforms();
    }

    public void StartSpawningPlatforms()
    {
        // Wait 2 seconds, then run SpawnPlatforms every 0.2 seconds.
        InvokeRepeating("SpawnPlatform", 2f, 0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        if(GameOver)
        {
            CancelInvoke("SpawnPlatform");
        }
    }

    void SpawnPlatform()
    {
        int rand = Random.Range(0, 6);
        if(rand < 2)
        {
            SpawnX();
        }
        else if(rand >= 2)
        {
            SpawnZ();
        }
    }

    void SpawnX()
    {
        Vector3 pos = lastPos;

        if (Random.value >= 0.5)
        {
            pos.x += size;

        }
        else
        {
            pos.x += size;
        }

        lastPos = pos;

        //Spawn a platform
        Instantiate(Platform, pos, Quaternion.identity);

        //Spawn a Diamond
        if (Random.value >= 0.8)
        {
            Instantiate(Diamond, new Vector3(pos.x, pos.y + 1, pos.z), Diamond.transform.rotation);
        }
    }

    void SpawnZ()
    {
        Vector3 pos = lastPos;
        pos.z += size;
        lastPos = pos;

        //Spawn a platform
        Instantiate(Platform, pos, Quaternion.identity);

        //Spawn a Diamond
        if (Random.value >= 0.8)
        {
            Instantiate(Diamond, new Vector3(pos.x, pos.y + 1, pos.z), Diamond.transform.rotation);
        }
    }
}
