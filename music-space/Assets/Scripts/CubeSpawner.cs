using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSpawner : MonoBehaviour
{

    public GameObject musicCube;
    public GameObject camera;
    public GameObject cubeUIElement;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 spawnPosition = (camera.transform.forward + new Vector3(transform.position.x,camera.transform.position.y,transform.position.z));
        Debug.DrawRay(camera.transform.position,camera.transform.forward*10,Color.green);
        cubeUIElement.transform.position = spawnPosition;
        if (Input.GetMouseButtonDown(0)){
            Debug.Log("Mouse pressed");
            
            Instantiate(musicCube, spawnPosition, Quaternion.identity);
            
        }
    }
}
