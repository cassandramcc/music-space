using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChuckSynth : MonoBehaviour
{

    public double[] noteBuffer;
    public string freqArrayName;
    public string timeArray;
    public string waitTime;
    // Start is called before the first frame update
    void Start(){
        
        GetComponent<ChuckSubInstance>().RunCode(string.Format(@"
            fun void playNotes(float fs[], int times[], int wait){{
                TriOsc t => ADSR env1 => NRev rev1 =>  dac;
                env1 => Delay delay1 => dac;
                delay1 => delay1;
                0.3 => t.gain;
                200::ms => delay1.max;
                50::ms => delay1.delay;
                0.5 => delay1.gain;
                0.1 => rev1.mix;

                (10::ms,600::ms,0,100::ms) => env1.set;
                wait::ms => now;
                for (0 => int i; i < fs.cap(); i++){{
                    Std.mtof(fs[i]) => t.freq;
                    1 => env1.keyOn;
                    times[i]::ms => now;
                }}
            }}
            global float {0}[1000];
            global int {1}[1000];
            global Event start;
            global int {2};
            while (true) {{
                start => now;
                spork ~ playNotes({0},{1},{2});
            }}
        ",freqArrayName,timeArray,waitTime));
    }

    public void PlayChuck(){
        GetComponent<ChuckSubInstance>().BroadcastEvent("start");
    }
}
