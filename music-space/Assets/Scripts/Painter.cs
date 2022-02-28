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
        public List<Vector3> edgeVertices = new List<Vector3>();

        public Vector3 direction;
        public Vector3 orthoDirection;

        public Vector3 rotated;
        public Vertex(Vector3 _pos){
            pos = _pos;
        }

    }

    Mesh mesh;
    public List<Vector3> vertices;

    List<Vertex> centralVertices = new List<Vertex>();

    List<Vertex> mostRecentVertices = new List<Vertex>();

    List<Vector3> mostRecentEdgeVertices = new List<Vector3>();
    int newestVertexDrawn = 0;
    public List<int> triangles;

    bool drawTriangles;
    int numRecentVertices;

    public Camera cam;

    Vector3 lastPoint;

    public GameObject mouseCursorUI;

    public GameObject controller;
    public GameObject rController;

    public GameObject chuckControls;

    int chuckCounter = 1;

    List<int> mostRecentTriangles = new List<int>();
    

    void Start() {
        Assert.IsNotNull(chuckControls);
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        vertices = new List<Vector3>();
        triangles = new List<int>();
        UpdateMesh();
    }

    void UpdateMesh(){
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        newestVertexDrawn = vertices.Count;
    }

    void DrawPoint(){
        Vector3 point = controller.transform.position;
        //To stop too many vertices spawning in the same place
        if ((lastPoint - point).magnitude > 0.01f){
            centralVertices.Add(new Vertex(point));
            mostRecentVertices.Add(new Vertex(point));
            //DebugSpheres(point);
            numRecentVertices += 1;
        }
        mouseCursorUI.transform.position = point;
    }

    void CalculateVertexDirection(){
        if (centralVertices.Count >= 2){
            Vector3 direction = centralVertices[centralVertices.Count-1].pos - centralVertices[centralVertices.Count - 2].pos;
            direction = direction/direction.magnitude; //normalised direction
            Vertex lastVertex = centralVertices[centralVertices.Count - 2];
            lastVertex.direction = direction;
            //this is the direction between first vertex and the next vertex
            //the first vertex's points need to be in the perpedincular direction
            Vector3 perpendicular = new Vector3();
            Vector3.OrthoNormalize(ref direction,ref perpendicular);
            lastVertex.orthoDirection = perpendicular;
            Vector3 rotation = perpendicular * Mathf.Cos(Mathf.PI/2) + (Vector3.Cross(direction,perpendicular))*Mathf.Sin(Mathf.PI/2) + direction*(Vector3.Dot(perpendicular,direction))*(1-Mathf.Cos(Mathf.PI/2));
            //the point 180 degrees away from the perp vector
            lastVertex.rotated = rotation;
            
            //Create points on plane defined by this perpendicular by rotating the perp vector around the direction as the axis
            for (int i = 0; i <= 270; i+=90){
                Vector3 rot = perpendicular * Mathf.Cos(i * Mathf.Deg2Rad) + (Vector3.Cross(direction,perpendicular))*Mathf.Sin(i * Mathf.Deg2Rad) + direction*(Vector3.Dot(perpendicular,direction))*(1-Mathf.Cos(i * Mathf.Deg2Rad));
                Vector3 newPoint = lastVertex.pos - rot;
                Vector3 shrinkNewPoint = Vector3.MoveTowards(newPoint,lastVertex.pos,0.9f);
                lastVertex.edgeVertices.Add(shrinkNewPoint);
            }
        }
    }

    void DrawTriangles(){
        if (numRecentVertices >=3){
            Vertex start = centralVertices[centralVertices.Count - 3];
            Vertex end = centralVertices[centralVertices.Count - 2];
            vertices.AddRange(start.edgeVertices);
            mostRecentEdgeVertices.AddRange(start.edgeVertices);
            vertices.AddRange(end.edgeVertices);
            mostRecentEdgeVertices.AddRange(end.edgeVertices);
            int startVertex = vertices.Count - 8;
            //If you want different shapes, will need to make this number changeable.
            if (start.pos.x > end.pos.x){
                triangles.AddRange(new int[]{
                    startVertex+5, startVertex,startVertex+1,
                    startVertex,startVertex+5,startVertex+4,

                    startVertex+3,startVertex,startVertex+7,
                    startVertex+7,startVertex,startVertex+4,

                    startVertex+2,startVertex+3,startVertex+6,
                    startVertex+6,startVertex+3,startVertex+7,

                    startVertex+5,startVertex+1,startVertex+2,
                    startVertex+6,startVertex+5,startVertex+2
                }); 
                mostRecentTriangles.AddRange(new int[]{
                    startVertex+5, startVertex,startVertex+1,
                    startVertex,startVertex+5,startVertex+4,

                    startVertex+3,startVertex,startVertex+7,
                    startVertex+7,startVertex,startVertex+4,

                    startVertex+2,startVertex+3,startVertex+6,
                    startVertex+6,startVertex+3,startVertex+7,

                    startVertex+5,startVertex+1,startVertex+2,
                    startVertex+6,startVertex+5,startVertex+2
                });
            }
            else{
                triangles.AddRange(new int[]{
                    startVertex, startVertex+1,startVertex+5,
                    startVertex,startVertex+5,startVertex+4,

                    startVertex,startVertex+7,startVertex+3,
                    startVertex,startVertex+4,startVertex+7,

                    startVertex+3,startVertex+6,startVertex+2,
                    startVertex+3,startVertex+7,startVertex+6,

                    startVertex+1,startVertex+2,startVertex+5,
                    startVertex+5,startVertex+2,startVertex+6
                });
                mostRecentTriangles.AddRange(new int[]{
                    startVertex, startVertex+1,startVertex+5,
                    startVertex,startVertex+5,startVertex+4,

                    startVertex,startVertex+7,startVertex+3,
                    startVertex,startVertex+4,startVertex+7,

                    startVertex+3,startVertex+6,startVertex+2,
                    startVertex+3,startVertex+7,startVertex+6,

                    startVertex+1,startVertex+2,startVertex+5,
                    startVertex+5,startVertex+2,startVertex+6
                });
            }
            
            //draw triangles between last vertex and last last vertex.
            UpdateMesh();
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

        GameObject newChuck = Instantiate(chuckControls,Vector3.zero,Quaternion.identity);
        newChuck.GetComponent<ChuckSynth>().freqArrayName = "freqs" + chuckCounter.ToString();
        newChuck.GetComponent<ChuckSynth>().timeArray = "times" + chuckCounter.ToString();
        //newChuck.GetComponent<PaintHolder>().centralVertices = mostRecentVertices;
        newChuck.GetComponent<PaintHolder>().meshVertices = mostRecentEdgeVertices;
        Debug.Log("Most recent edge vertices: "+mostRecentEdgeVertices.Count);
        newChuck.GetComponent<PaintHolder>().triangles = mostRecentTriangles;
        Debug.Log("Give notes to chuck");
        newChuck.GetComponent<PaintHolder>().UpdateMesh();
        
        ChuckSubInstance newChuckSubInstance = newChuck.GetComponent<ChuckSubInstance>();
        //the sub instance component is for some reason disabled on instantiation
        newChuckSubInstance.enabled = true;
        //Different chuck sub instances have to have different array names for the frequencies, so this is the crude way to do it.
        newChuckSubInstance.SetFloatArray("freqs" + chuckCounter.ToString(),correctedNotes.Item1.ToArray());
        newChuckSubInstance.SetIntArray("times" + chuckCounter.ToString(),correctedNotes.Item2.ToArray());
        chuckCounter++;

        // ? Do the chucks still need this array?
        newChuck.GetComponent<ChuckSynth>().noteBuffer = notesForChuck;
    }

    void Update()
    {
        Vector3 point = controller.transform.position;

        if (controller.GetComponent<ActionBasedController>().activateAction.action.ReadValue<float>() > 0.8){
            DrawPoint();
            CalculateVertexDirection();
            DrawTriangles();
        }

        else if(controller.GetComponent<ActionBasedController>().activateAction.action.WasReleasedThisFrame()){
            numRecentVertices = 0;
            GiveNotesToChuck();
            mostRecentVertices.Clear();
            mostRecentTriangles.Clear();
            mostRecentEdgeVertices.Clear();
        }

        if (rController.GetComponent<ActionBasedController>().activateAction.action.WasPressedThisFrame()){
            ChuckSynth[] chucks = GameObject.FindObjectsOfType<ChuckSynth>();
            //Perhaps put all chucks into one parent and call on children at once? Possible faster?

            foreach(ChuckSynth c in chucks){
                Debug.Log(c.GetComponent<ChuckSubInstance>());
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
