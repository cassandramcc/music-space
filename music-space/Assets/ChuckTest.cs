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

            TriOsc foo => ADSR env1 =>  dac;
            env1 => Delay delay1 => dac;
            delay1 => delay1;
            0.3 => foo.gain;
            200::ms => delay1.max;
            50::ms => delay1.delay;
            0.5 => delay1.gain;

            (10::ms,100::ms,0,10::ms) => env1.set;

            [440,550,880,770,880,550,440,770] @=> int l[];


            for( 0 => int i; i < l.cap() ; i++ )
            {
                // debug-print value of 'foo'
                l[i] => foo.freq;
                1 => env1.keyOn;
                100::ms => now;
            }
        ");
    }
}
