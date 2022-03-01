using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Assertions;
using System.Linq;

public class Painter : MonoBehaviour
{

    public class Vertex{
        public Vector3 pos;
        public Vertex(Vector3 _pos){
            pos = _pos;
        }
    }

    List<Vertex> mostRecentVertices = new List<Vertex>();
    int numRecentVertices;
    Vector3 lastPoint;

    public GameObject controller;
    public GameObject rController;

    int chuckCounter = 1;
    public GameObject meshHolder;

    GameObject currentMesh;

    void CreatePoint(){
        Vector3 point = controller.transform.position;
        //To stop too many vertices spawning in the same place
        if ((lastPoint - point).magnitude > 0.01f){
            mostRecentVertices.Add(new Vertex(point));
            numRecentVertices += 1;
            currentMesh.GetComponent<MeshHolder>().centralVertices.Add(new MeshHolder.Vertex(point));
        }
    }


    //The range are the values the notes are allowed to have
    //Turning the range of vertex heights into notes
    int VertexToNote(Vertex v, List<int> range){
        //output start, output end, input start, input end
        int output = (int)Mathf.Lerp (range[0], range[range.Count - 1], Mathf.InverseLerp (0, 1f, v.pos.y));

        //If note from lerp inverselerp is not in range, just minus 1 to get a correct note.
        if (!range.Contains(output)){
            output-=1;
        }

        //Add 48 to translate into midi notes. 48 is C3.
        return output+48;
    }

    double[] PaintToNotes(List<Vertex> vertices){
        // ! Will need to make this variable
        Scale scale = new Scale(Root.FMajor);
        
        //Generating the base notes for the vertices
        IEnumerable<int> baseNotes = scale.notes;
        List<int> rangeNotes = new List<int>();
        for (int i = 0; i < 4; i++){
            rangeNotes.AddRange(baseNotes.Select(n => n + i*12));
        }

        //Converting each vertex into a note based on y value
        List<int> noteBuffer = new List<int>();
        foreach(Vertex v in vertices){   
            noteBuffer.Add(VertexToNote(v,rangeNotes));
        }

        List<double> notesAsDouble = new List<double>();
        //have to convert the floats to doubles because a chuck float is actually a double
        foreach(int n in noteBuffer){
            notesAsDouble.Add((double)n);
        }
        return notesAsDouble.ToArray();  
    }


    public Tuple<List<Double>, List<long>> NotesToTimes(double[] notes){
        List<Double> newNotes = new List<double>();
        //Because chuck wants ints to be longs
        List<long> times = new List<long>();
        double prevNote = notes[0];
        long time = 100;
        int scale = 100;
        //Counting the number of repeat digits in a row in the notes list
        foreach(double n in notes){
            if (prevNote == n){
                time = time+scale;
            }
            else{
                newNotes.Add(prevNote);
                prevNote = n;
                times.Add(time);
                time = 100;
            }
        }
        newNotes.Add(notes[notes.Count()-1]);
        times.Add(time);
        times[0] -= 100; 
        return new Tuple<List<Double>, List<long>>(newNotes,times);
    }

    void GiveNotesToChuck(){
        double[] notesForChuck = PaintToNotes(mostRecentVertices);
        //Item 1 = notes, item 2 = times
        Tuple<List<Double>, List<long>> correctedNotes = NotesToTimes(notesForChuck);

        currentMesh.AddComponent<ChuckSynth>();
        currentMesh.GetComponent<ChuckSynth>().freqArrayName = "freqs" + chuckCounter.ToString();
        currentMesh.GetComponent<ChuckSynth>().timeArray = "times" + chuckCounter.ToString();

        ChuckSubInstance newChuckSubInstance = currentMesh.GetComponent<ChuckSubInstance>();

        //Different chuck sub instances have to have different array names for the frequencies, so this is the crude way to do it.
        newChuckSubInstance.SetFloatArray("freqs" + chuckCounter.ToString(),correctedNotes.Item1.ToArray());
        newChuckSubInstance.SetIntArray("times" + chuckCounter.ToString(),correctedNotes.Item2.ToArray());
        chuckCounter++;
    }

    void Update()
    {
        Vector3 point = controller.transform.position;

        if (controller.GetComponent<ActionBasedController>().activateAction.action.WasPressedThisFrame()){
            currentMesh = Instantiate(meshHolder,Vector3.zero,Quaternion.identity);
        }

        if (controller.GetComponent<ActionBasedController>().activateAction.action.ReadValue<float>() > 0.8){
            CreatePoint();
            currentMesh.GetComponent<MeshHolder>().CalculateVertexDirection();
            currentMesh.GetComponent<MeshHolder>().DrawTriangles();
        }

        else if(controller.GetComponent<ActionBasedController>().activateAction.action.WasReleasedThisFrame()){
            numRecentVertices = 0;
            GiveNotesToChuck();
            mostRecentVertices.Clear();
            currentMesh = null;
        }

        if (rController.GetComponent<ActionBasedController>().activateAction.action.WasPressedThisFrame()){
            ChuckSynth[] chucks = GameObject.FindObjectsOfType<ChuckSynth>();
            //Perhaps put all chucks into one parent and call on children at once? Possible faster?

            foreach(ChuckSynth c in chucks){
                c.gameObject.GetComponent<ChuckSubInstance>().BroadcastEvent("start");
            }
        }
        lastPoint = point;
    }




    void DebugSpheres(Vector3 point){
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.name = "@"+sphere.name;
        sphere.transform.position = new Vector3(point.x,point.y,point.z);
        sphere.transform.localScale = new Vector3(0.02f,0.02f,0.02f);
    }
}
