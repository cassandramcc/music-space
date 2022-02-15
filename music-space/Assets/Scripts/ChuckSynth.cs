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

    //Chuck.FloatArrayCallback floatArrayCallback;

    public List<float> buffer = new List<float>();
    // Start is called before the first frame update
    void Start(){

        //floatArrayCallback = GetComponent<ChuckSubInstance>().CreateGetFloatArrayCallback( GetFreqs );
        GetComponent<ChuckSubInstance>().RunCode(@"
            fun void playNotes(float fs[]){
                chout <= "" play notes "" <= IO.newline();
                TriOsc t => ADSR env1 => NRev rev1 =>  dac;
                env1 => Delay delay1 => dac;
                delay1 => delay1;
                0.3 => t.gain;
                200::ms => delay1.max;
                50::ms => delay1.delay;
                0.5 => delay1.gain;
                0.1 => rev1.mix;

                (10::ms,100::ms,0,10::ms) => env1.set;
                for (0 => int i; i < fs.cap(); i++){
                    200::ms => now;
                    fs[i] => t.freq;
                    1 => env1.keyOn;
                    100::ms => now;
                }
   
            }
            global float freqs[1000];
            global Event start;
            while (true) {
                chout <= "" loop start "" <= IO.newline();
                start => now;
                chout <= "" about to play notes "" <= IO.newline();
                spork ~ playNotes(freqs);
            }
        ");
    }

    void Update(){
        //GetComponent<ChuckSubInstance>().GetFloatArray("freqs",floatArrayCallback);
    }

    void GetFreqs(float[] freqs){

    }

    public void ReceiveFreqBuffer(List<float> freqs){
        foreach(float f in freqs){
            buffer.Add(f);
        }
    }
}
