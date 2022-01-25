using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioSource audioSource;
    public float waitTime = 20f;

    float timeStamp;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timeStamp <= Time.time){
            audioSource.Play();
            timeStamp = Time.time + waitTime;
        }
    }
}
