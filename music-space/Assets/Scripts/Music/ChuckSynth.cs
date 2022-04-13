using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChuckSynth : MonoBehaviour
{

    public double[] noteBuffer;
    public string freqArrayName;
    public string timeArray;
    public string waitTime;

    public string pointerPos;
    ChuckIntSyncer positionSyncer;
    //Need to know when to instantiate the pointer
    public string createPointer;
    ChuckIntSyncer createPointerSyncer;
    bool doCreatePointer;

    GameObject pointer;
    void Start(){
        
        GetComponent<ChuckSubInstance>().RunCode(string.Format(@"
            global int {3};
            global int {4};
            0 => {4};
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
                1 => {4};
                for (0 => int i; i < fs.cap(); i++){{
                    Std.mtof(fs[i]) => t.freq;
                    1 => env1.keyOn;
                    times[i]::ms => now;
                    {3}+ times[i]/100 => {3};
                }}
                0 => {3};
                0 => {4};
            }}
            global float {0}[1000];
            global int {1}[1000];
            global Event start;
            global int {2};

            while (true) {{
                start => now;
                spork ~ playNotes({0},{1},{2});
            }}
        ",freqArrayName,timeArray,waitTime,pointerPos,createPointer));

        positionSyncer = gameObject.AddComponent<ChuckIntSyncer>();
        createPointerSyncer = gameObject.AddComponent<ChuckIntSyncer>();
		positionSyncer.SyncInt( GetComponent<ChuckSubInstance>(), pointerPos );
        createPointerSyncer.SyncInt(GetComponent<ChuckSubInstance>(), createPointer);
        

    }

    public void PlayChuck(){
        GetComponent<ChuckSubInstance>().BroadcastEvent("start");
        doCreatePointer = true;
    }

    void Update(){
        if (doCreatePointer && createPointerSyncer.GetCurrentValue() > 0)
        {
            pointer = Instantiate(GetComponent<MeshHolder>().pointer);
            pointer.transform.parent = transform;
            pointer.transform.position = GetComponent<MeshHolder>().centralVertices[positionSyncer.GetCurrentValue()].pos;
            doCreatePointer = false;
        }

        if (!doCreatePointer && createPointerSyncer.GetCurrentValue() > 0)
        {
            pointer.transform.position = GetComponent<MeshHolder>().centralVertices[positionSyncer.GetCurrentValue()].pos;
        }

        if (!doCreatePointer && createPointerSyncer.GetCurrentValue() == 0)
        {
            Destroy(pointer);
        }
    }
}
