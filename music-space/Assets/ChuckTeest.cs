using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChuckTeest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<ChuckSubInstance>().RunCode(@"
            
            // make our patch
            SinOsc s => dac;
            
            261.63 => s.freq;
            1000::ms => now;
            392 => s.freq;
            1000::ms => now;
	    ");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
