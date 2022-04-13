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
                1 => {4};
                for (0 => int i; i < fs.cap(); i++){{
                    Std.mtof(fs[i]) => saw2.freq => saw1.freq;
                    1 => env1.keyOn;
                    1 => env2.keyOn;
                    times[i]::ms => now;
                    {3}+ times[i]/50 => {3};
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
        ", freqArrayName,timeArray,waitTime,pointerPos, createPointer));

        positionSyncer = gameObject.AddComponent<ChuckIntSyncer>();
        createPointerSyncer = gameObject.AddComponent<ChuckIntSyncer>();
        positionSyncer.SyncInt(GetComponent<ChuckSubInstance>(), pointerPos);
        createPointerSyncer.SyncInt(GetComponent<ChuckSubInstance>(), createPointer);
    }

    public void PlayChuck(){
        GetComponent<ChuckSubInstance>().BroadcastEvent("start");
        doCreatePointer = true;

    }

    void Update()
    {
        if (doCreatePointer && createPointerSyncer.GetCurrentValue() > 0)
        {
            pointer = Instantiate(GetComponentInParent<MeshHolder>().pointer);
            pointer.transform.parent = transform;
            pointer.transform.position = GetComponentInParent<MeshHolder>().centralVertices[positionSyncer.GetCurrentValue()].pos;
            doCreatePointer = false;
        }

        if (!doCreatePointer && createPointerSyncer.GetCurrentValue() > 0)
        {
            pointer.transform.position = GetComponentInParent<MeshHolder>().centralVertices[positionSyncer.GetCurrentValue()].pos;
            transform.position = GetComponentInParent<MeshHolder>().centralVertices[positionSyncer.GetCurrentValue()].pos;
        }

        if (!doCreatePointer && createPointerSyncer.GetCurrentValue() == 0)
        {
            Destroy(pointer);
        }
    }
}
