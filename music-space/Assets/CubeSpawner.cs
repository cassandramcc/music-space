using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSpawner : MonoBehaviour
{

    public GameObject musicCube;
    public GameObject camera;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)){
            Debug.Log("Mouse pressed");
            //Vector3 spawnPosition = transform.forward * 2f + transform.position;
            Vector3 spawnPosition = (camera.transform.forward + new Vector3(0,0.4f,0)) * 5f + transform.position;
            Instantiate(musicCube, spawnPosition, Quaternion.identity);
        }
    }
}
