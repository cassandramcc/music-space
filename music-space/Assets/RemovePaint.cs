using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovePaint : MonoBehaviour
{

    public GameObject paints;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RemovePaint_(){
        foreach (Transform child in paints.transform){
            Destroy(child.gameObject);
        }
    }
}
