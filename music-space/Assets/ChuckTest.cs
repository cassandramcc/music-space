using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChuckTest : MonoBehaviour
{

    public class Note{
        public float freq;

        public Note(float _freq){
            freq = _freq;
        }
    }

    public List<Note> buffer = new List<Note>();
    // Start is called before the first frame update
    void Start()
    {
        //buffer.Add(new Note(261.63f));
        //buffer.Add(new Note(392f));
        /*GetComponent<ChuckSubInstance>().RunCode(@"
            
            // make our patch
            SinOsc s => dac;
            
            261.63 => s.freq;
            1000::ms => now;
            392 => s.freq;
            1000::ms => now;
	    ");*/
        //StartCoroutine(playNotes());
    }

    public void ReceiveFreqBuffer(List<float> freqs){
        foreach(float f in freqs){
            buffer.Add(new Note(f));
        }
    }

    public IEnumerator playNotes(){
        foreach (Note note in buffer ){
            GetComponent<ChuckSubInstance>().RunCode(string.Format(@"

            SinOsc s => dac;
            {0} => s.freq;
            200::ms => now;
            ",note.freq));
            yield return new WaitForSeconds(0.4f);
        }  
    }
}
