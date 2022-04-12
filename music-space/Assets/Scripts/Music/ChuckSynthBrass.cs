using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChuckSynthBrass : MonoBehaviour
{

    public double[] noteBuffer;
    public string freqArrayName;
    public string timeArray;
    public string waitTime;

    public string pointerPos;
    ChuckIntSyncer positionSyncer;

    GameObject pointer;
    void Start(){
        
        GetComponent<ChuckSubInstance>().RunCode(string.Format(@"
            global int {3};
            fun void playNotes(float fs[], int times[], int wait){{
                SawOsc saw1  => LPF f1 => ADSR env1=> dac;
                SqrOsc saw2 => LPF f2 => ADSR env2=> dac;

                0.1 => saw1.gain;
                0.1 => saw2.gain;
                //30 => filter.freq;

                1600 => f1.freq;
                1600 => f2.freq;

                (15::ms,300::ms,-2.7,200::ms) => env1.set;
                (15::ms,300::ms,-2.7,200::ms) => env2.set;
                wait::ms => now;
                for (0 => int i; i < fs.cap(); i++){{
                    Std.mtof(fs[i]) => saw2.freq => saw1.freq;
                    1 => env1.keyOn;
                    1 => env2.keyOn;
                    times[i]::ms => now;
                    {3}+ times[i]/100 => {3};
                }}
                0 => {3};
            }}
            global float {0}[1000];
            global int {1}[1000];
            global Event start;
            global int {2};

            while (true) {{
                start => now;
                spork ~ playNotes({0},{1},{2});
            }}
        ",freqArrayName,timeArray,waitTime,pointerPos));

        positionSyncer = gameObject.AddComponent<ChuckIntSyncer>();
		positionSyncer.SyncInt( GetComponent<ChuckSubInstance>(), pointerPos );
        pointer = Instantiate(GetComponent<MeshHolder>().pointer);
        pointer.transform.parent = transform;
    }

    public void PlayChuck(){
        GetComponent<ChuckSubInstance>().BroadcastEvent("start");
    }

    void Update(){
        pointer.transform.position = GetComponent<MeshHolder>().centralVertices[positionSyncer.GetCurrentValue()].pos;
    }
}
