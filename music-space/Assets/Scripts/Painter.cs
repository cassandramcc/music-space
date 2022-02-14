using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Assertions;

public class Painter : MonoBehaviour
{

    class Vertex{
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
    List<Vector3> vertices;

    List<Vertex> centralVertices = new List<Vertex>();

    List<Vertex> mostRecentVertices = new List<Vertex>();
    int newestVertexDrawn = 0;
    List<int> triangles;

    bool drawTriangles;
    int numRecentVertices;

    public Camera cam;

    Vector3 lastPoint;

    public GameObject mouseCursorUI;

    public GameObject controller;
    public GameObject rController;

    public GameObject chuckControls;
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
            DebugSpheres(point);
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
            vertices.AddRange(end.edgeVertices);
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
            }
            
            //draw triangles between last vertex and last last vertex.
            UpdateMesh();
        }
    }

    void PaintToFreq(){
        List<float> freqBuffer = new List<float>();
        foreach(Vertex v in mostRecentVertices){   
            freqBuffer.Add(VertexToFreq(v));
        }
        //chuckControls.GetComponent<ChuckSynth>().ReceiveFreqBuffer(freqBuffer);
        List<double> dFreq = new List<double>();
        foreach(float f in freqBuffer){
            dFreq.Add((double)f);
        }
        //chuckControls.GetComponent<ChuckSubInstance>().SetFloatArray("freqs",dFreq.ToArray());
    }

    void Update()
    {

        Vector3 point = controller.transform.position;

        if (controller.GetComponent<ActionBasedController>().activateAction.action.ReadValue<float>() > 0.8){
            DrawPoint();
            CalculateVertexDirection();
            DrawTriangles();
        }

        else{
            numRecentVertices = 0;
            PaintToFreq();
            mostRecentVertices.Clear();
        }

        if (rController.GetComponent<ActionBasedController>().activateAction.action.WasPressedThisFrame()){
            Debug.Log("Play notes");
            chuckControls.GetComponent<ChuckSubInstance>().SetFloatArray("freqs",new double[]{440,550,660});
            chuckControls.GetComponent<ChuckSubInstance>().BroadcastEvent("start");
            //chuckControls.GetComponent<ChuckSynth>().playNotes();
        }
        
        lastPoint = point;
    }

    float VertexToFreq(Vertex v){
        //output = output_start + ((output_end - output_start) / (input_end - input_start)) * (input - input_start)
        float output = 261.63f + ((466.16f - 261.63f)/1f - 0f) * (v.pos.y - 0f);
        return output;
    }

    void DebugSpheres(Vector3 point){
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.name = "@"+sphere.name;
        sphere.transform.position = new Vector3(point.x,point.y,point.z);
        sphere.transform.localScale = new Vector3(0.02f,0.02f,0.02f);
    }
}
