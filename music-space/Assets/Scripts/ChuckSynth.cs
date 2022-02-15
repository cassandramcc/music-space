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
    void Start(){
        GetComponent<ChuckSubInstance>().RunCode(@"
            fun void playNotes(float fs[]){
                SinOsc t => dac;
                chout <= ""In function"" <= IO.newline();
                chout <= fs.cap() <= IO.newline();
                for (0 => int i; i < fs.cap(); i++){
                    chout <= "" In for "" <= IO.newline();
                    chout <= fs.cap() <= IO.newline();
                    //chout <= i <= IO.newline();
                    //chout <= fs[i] <= IO.newline();
                    fs[i] => t.freq;
                    200::ms => now;
                    chout <= fs.cap() <= IO.newline();
                }
   
            }
            global float freqs[1000];
            global float freq3;
            global Event start;
            while (true) {
                start => now;
                spork ~ playNotes(freqs);
                freqs[2] => freq3;
            }
        ");
    }

    public void ReceiveFreqBuffer(List<float> freqs){
        foreach(float f in freqs){
            buffer.Add(f);
        }
    }
}
