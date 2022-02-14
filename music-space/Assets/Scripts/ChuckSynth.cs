using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChuckSynth : MonoBehaviour
{

    public class Note{
        public float freq;

        public Note(float _freq){
            freq = _freq;
        }
    }

    public List<float> buffer = new List<float>();
    // Start is called before the first frame update
    void Start()
    {

    }

    public void ReceiveFreqBuffer(List<float> freqs){
        foreach(float f in freqs){
            buffer.Add(f);
        }
    }

    public void playNotes(){
        GetComponent<ChuckSubInstance>().RunCode(@"

        TriOsc t => dac;

        440 => t.freq;
        500::ms => now;
        660 => t.freq;
        500::ms => now;

        "); 
    }
}
