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
    public GameObject lController;
    public GameObject rController;
    int chuckCounter = 1;
    public GameObject meshHolder;
    GameObject currentMesh;
    public GameObject chuckSubParent;

    public Material paintColour;
    public string instrument;

    public Root currentScale = Root.CMajor;

    void CreatePoint(){
        Vector3 point = rController.transform.position;
        //To stop too many vertices spawning in the same place
        if ((lastPoint - point).magnitude > 0.001f){
            mostRecentVertices.Add(new Vertex(point));
            numRecentVertices += 1;
            currentMesh.GetComponent<MeshHolder>().centralVertices.Add(new MeshHolder.Vertex(point));
        }
    }


    //The range are the values the notes are allowed to have
    //Turning the range of vertex heights into notes
    int VertexToNote(Vertex v, List<int> range){
        //output start, output end, input start, input end
        int output = (int)Mathf.Lerp (range[0], range[range.Count - 1], Mathf.InverseLerp (0.4f, 2.5f, v.pos.y));

        //If note from lerp inverselerp is not in range, just minus 1 to get a correct note.
        if (!range.Contains(output)){
            output-=1;
        }

        //Add 48 to translate into midi notes. 48 is C3.
        return output+48;
    }

    //Generating the base notes for the vertices
    List<int> GenerateAllNotesFromBase(Scale scale){
        IEnumerable<int> baseNotes = scale.notes;
        List<int> rangeNotes = new List<int>();
        for (int i = 0; i < 4; i++){
            rangeNotes.AddRange(baseNotes.Select(n => n + i*12));
        }
        return rangeNotes;
    }

    double[] PaintToNotes(List<Vertex> vertices){
        // ! Will need to make this variable
        //Scale scale = new Scale(Root.FMajor);
        Scale scale = new Scale(currentScale);
        
        List<int> rangeNotes = GenerateAllNotesFromBase(scale);

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
        long time = 50;
        int scale = 50;
        //Counting the number of repeat digits in a row in the notes list
        foreach(double n in notes){
            if (prevNote == n){
                time = time+scale;
            }
            else{
                newNotes.Add(prevNote);
                prevNote = n;
                times.Add(time);
                time = 50;
            }
        }
        newNotes.Add(notes[notes.Count()-1]);
        times.Add(time);
        times[0] -= 50; 
        return new Tuple<List<Double>, List<long>>(newNotes,times);
    }

    void GiveNotesToChuck(){
        double[] notesForChuck = PaintToNotes(mostRecentVertices);
        //Item 1 = notes, item 2 = times
        Tuple<List<Double>, List<long>> correctedNotes = NotesToTimes(notesForChuck);

        if (paintColour.name == "Red Paint"){
            GameObject currentMeshChild = currentMesh.transform.GetChild(0).gameObject;
            currentMeshChild.AddComponent<ChuckSynthPiano>();
            currentMeshChild.GetComponent<ChuckSynthPiano>().freqArrayName = "freqs" + chuckCounter.ToString();
            currentMeshChild.GetComponent<ChuckSynthPiano>().timeArray = "times" + chuckCounter.ToString();
            currentMeshChild.GetComponent<ChuckSynthPiano>().waitTime = "wait" + chuckCounter.ToString();
            currentMeshChild.GetComponent<ChuckSynthPiano>().pointerPos = "pos" + chuckCounter.ToString();
            currentMeshChild.GetComponent<ChuckSynthPiano>().createPointer = "createPointer" + chuckCounter.ToString();
        }
        else if (paintColour.name == "Blue Paint"){
            GameObject currentMeshChild = currentMesh.transform.GetChild(0).gameObject;
            currentMeshChild.AddComponent<ChuckSynthBass>();
            currentMeshChild.GetComponent<ChuckSynthBass>().freqArrayName = "freqs" + chuckCounter.ToString();
            currentMeshChild.GetComponent<ChuckSynthBass>().timeArray = "times" + chuckCounter.ToString();
            currentMeshChild.GetComponent<ChuckSynthBass>().waitTime = "wait" + chuckCounter.ToString();
            currentMeshChild.GetComponent<ChuckSynthBass>().pointerPos = "pos" + chuckCounter.ToString();
            currentMeshChild.GetComponent<ChuckSynthBass>().createPointer = "createPointer" + chuckCounter.ToString();
        }
        else if (paintColour.name == "Yellow Paint"){
            GameObject currentMeshChild = currentMesh.transform.GetChild(0).gameObject;
            currentMeshChild.AddComponent<ChuckSynthBrass>();
            currentMeshChild.GetComponent<ChuckSynthBrass>().freqArrayName = "freqs" + chuckCounter.ToString();
            currentMeshChild.GetComponent<ChuckSynthBrass>().timeArray = "times" + chuckCounter.ToString();
            currentMeshChild.GetComponent<ChuckSynthBrass>().waitTime = "wait" + chuckCounter.ToString();
            currentMeshChild.GetComponent<ChuckSynthBrass>().pointerPos = "pos" + chuckCounter.ToString();
            currentMeshChild.GetComponent<ChuckSynthBrass>().createPointer = "createPointer" + chuckCounter.ToString();
        }
        else
        {
            GameObject currentMeshChild = currentMesh.transform.GetChild(0).gameObject;
            currentMeshChild.AddComponent<ChuckSynth>();
            currentMeshChild.GetComponent<ChuckSynth>().freqArrayName = "freqs" + chuckCounter.ToString();
            currentMeshChild.GetComponent<ChuckSynth>().timeArray = "times" + chuckCounter.ToString();
            currentMeshChild.GetComponent<ChuckSynth>().waitTime = "wait" + chuckCounter.ToString();
            currentMeshChild.GetComponent<ChuckSynth>().pointerPos = "pos" + chuckCounter.ToString();
            currentMeshChild.GetComponent<ChuckSynth>().createPointer = "createPointer" + chuckCounter.ToString();
        }
        

        float waitDistance;
        if (currentMesh.GetComponent<MeshHolder>().closestMesh == null){
            waitDistance = 0;
        }
        else{
            //Mapping distance into 100ms
            waitDistance = Vector3.Distance(currentMesh.GetComponent<MeshHolder>().startPoint,currentMesh.GetComponent<MeshHolder>().closestMesh.GetComponent<MeshHolder>().startPoint);
            waitDistance = Mathf.Floor((waitDistance*1000)/100)*100 + currentMesh.GetComponent<MeshHolder>().closestMesh.GetComponent<MeshHolder>().waitTime;
        }
        
        currentMesh.GetComponent<MeshHolder>().waitTime = (int)waitDistance;

        ChuckSubInstance newChuckSubInstance = currentMesh.transform.GetChild(0).GetComponent<ChuckSubInstance>();

        //Different chuck sub instances have to have different array names for the frequencies, so this is the crude way to do it.
        newChuckSubInstance.SetFloatArray("freqs" + chuckCounter.ToString(),correctedNotes.Item1.ToArray());
        newChuckSubInstance.SetIntArray("times" + chuckCounter.ToString(),correctedNotes.Item2.ToArray());
        newChuckSubInstance.SetInt("wait" + chuckCounter.ToString(),currentMesh.GetComponent<MeshHolder>().waitTime);
        chuckCounter++;
    }

    void FindClosestOtherMesh(){
        MeshHolder[] meshes = GameObject.FindObjectsOfType<MeshHolder>();
        GameObject closest = null;
        float prevDistance = Mathf.Infinity;
        foreach(MeshHolder chuck in meshes){
            float currDistance = Vector3.Distance(chuck.gameObject.GetComponent<MeshHolder>().startPoint,currentMesh.GetComponent<MeshHolder>().startPoint);
            if (chuck.gameObject != currentMesh && currDistance < prevDistance){
                prevDistance = currDistance;
                closest = chuck.gameObject;
            }
        }
        currentMesh.GetComponent<MeshHolder>().closestMesh = closest;
    }

    void Update()
    {
        Vector3 point = rController.transform.position;
        
        //Create a new sub chuck to hold the new mesh
        if (rController.GetComponent<ActionBasedController>().activateAction.action.WasPressedThisFrame()){
            currentMesh = Instantiate(meshHolder,point,Quaternion.identity);
            currentMesh.GetComponent<MeshHolder>().paintColour = paintColour;
            currentMesh.transform.parent = chuckSubParent.transform;
            //new sub chuck needs to hold its starting point to find closest other sub chuck
            currentMesh.GetComponent<MeshHolder>().startPoint = point;
            FindClosestOtherMesh();
        }

        //Create a central that will get turned into a note, point is also given to new sub chuck to create shape around for mesh vertices
        if (rController.GetComponent<ActionBasedController>().activateAction.action.ReadValue<float>() > 0.8){
            CreatePoint();
            currentMesh.GetComponent<MeshHolder>().CalculateVertexDirection();
            currentMesh.GetComponent<MeshHolder>().DrawTriangles();
        }

        //Clear variables and give notes to chuck
        else if(rController.GetComponent<ActionBasedController>().activateAction.action.WasReleasedThisFrame()){
            numRecentVertices = 0;
            GiveNotesToChuck();
            mostRecentVertices.Clear();
            currentMesh = null;
        }

        if (lController.GetComponent<ActionBasedController>().activateAction.action.WasPressedThisFrame()){
            chuckSubParent.BroadcastMessage("PlayChuck");
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
