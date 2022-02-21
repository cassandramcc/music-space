using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChuckSynth : MonoBehaviour
{

    public double[] noteBuffer;
    public string freqArrayName;
    // Start is called before the first frame update
    void Start(){
        
        GetComponent<ChuckSubInstance>().RunCode(string.Format(@"
            fun void playNotes(float fs[]){{
                TriOsc t => ADSR env1 => NRev rev1 =>  dac;
                env1 => Delay delay1 => dac;
                delay1 => delay1;
                0.3 => t.gain;
                200::ms => delay1.max;
                50::ms => delay1.delay;
                0.5 => delay1.gain;
                0.1 => rev1.mix;

                (10::ms,100::ms,0,10::ms) => env1.set;
                for (0 => int i; i < fs.cap(); i++){{
                    200::ms => now;
                    Std.mtof(fs[i]) => t.freq;
                    1 => env1.keyOn;
                    100::ms => now;
                }}
   
            }}
            global float {0}[1000];
            global Event start;
            global float startFreq;
            while (true) {{
                {0}[0] => startFreq;
                start => now;
                spork ~ playNotes({0});
            }}
        ",freqArrayName));
    }
}
