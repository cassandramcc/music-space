using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChuckTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //A sanity check to make sure Chuck is producing audio
        GetComponent<ChuckSubInstance>().RunCode(@"

            TriOsc t => dac;

            440 => t.freq;
            200::ms => now;
        ");
    }
}
