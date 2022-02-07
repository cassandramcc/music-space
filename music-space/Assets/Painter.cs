using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.XR.Interaction.Toolkit;

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
    int newestVertexDrawn = 0;
    List<int> triangles;

    bool drawTriangles;
    int numRecentVertices;

    public Camera cam;

    Vector3 lastPoint;

    public GameObject mouseCursorUI;

    public GameObject controller;
    // Start is called before the first frame update
    void Start()
    {
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
        //Vector3 mousePos = Input.mousePosition;
        //Vector3 point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10));
        Vector3 point = controller.transform.position;
        //Debug.Log(point);
        //Debug.Log(lastPoint);
        //Debug.Log((lastPoint - point).magnitude);
        //To stop too many vertices spawning in the same place
        if ((lastPoint - point).magnitude > 0.01f){
            centralVertices.Add(new Vertex(point));
            DebugSpheres(point);
            numRecentVertices += 1;
        }
        
        mouseCursorUI.transform.position = point;
        Debug.DrawRay(cam.transform.position,cam.transform.forward*10,Color.green);
        
    }

    void CalculateVertexDirection(){
        if (centralVertices.Count >= 2){
            Vector3 direction = centralVertices[centralVertices.Count-1].pos - centralVertices[centralVertices.Count - 2].pos;
            direction = direction/direction.magnitude; //normalised direction
            Vertex lastVertex = centralVertices[centralVertices.Count - 2];
            lastVertex.direction = direction;
            //Debug.DrawRay(lastVertex.pos,lastVertex.direction*2f,Color.red);
            //this is the direction between first vertex and the next vertex
            //the first vertex's points need to be in the perpedincular direction
            Vector3 perpendicular = new Vector3();
            Vector3.OrthoNormalize(ref direction,ref perpendicular);
            lastVertex.orthoDirection = perpendicular;
            //Debug.DrawRay(lastVertex.pos,perpendicular*5,Color.cyan);
            Vector3 rotation = perpendicular * Mathf.Cos(Mathf.PI/2) + (Vector3.Cross(direction,perpendicular))*Mathf.Sin(Mathf.PI/2) + direction*(Vector3.Dot(perpendicular,direction))*(1-Mathf.Cos(Mathf.PI/2));
            lastVertex.rotated = rotation;
            //Debug.DrawRay(lastVertex.pos,rotation*2f,Color.magenta);
            
            //The perpendicular direction is the negative reciprical gradient, so need to calculate gradient of direction
            // then turn perpendicular gradient into a vector direction.
            //Create points on plane defined by this perpendicular gradient
            for (int i = 0; i <= 270; i+=90){
                Vector3 rot = perpendicular * Mathf.Cos(i * Mathf.Deg2Rad) + (Vector3.Cross(direction,perpendicular))*Mathf.Sin(i * Mathf.Deg2Rad) + direction*(Vector3.Dot(perpendicular,direction))*(1-Mathf.Cos(i * Mathf.Deg2Rad));
                Vector3 newPoint = lastVertex.pos - rot;
                Vector3 shrinkNewPoint = Vector3.MoveTowards(newPoint,lastVertex.pos,0.9f);
                lastVertex.edgeVertices.Add(shrinkNewPoint);
                //DebugSpheres(shrinkNewPoint);
            }
            //Before doing triangles, add debug spheres to test points spawn correctly.
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
    // Update is called once per frame
    void Update()
    {
        //Vector3 mousePos = Input.mousePosition;
        //Vector3 point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10));
        //mouseCursorUI.transform.position = point;
        Vector3 point = controller.transform.position;
        
        /*if (Input.GetMouseButton(0)){
            DrawPoint();
            CalculateVertexDirection();
            DrawTriangles();
        }

        if (Input.GetMouseButtonUp(0)){
            drawTriangles = false;
            numRecentVertices = 0;
        }*/

        if (controller.GetComponent<ActionBasedController>().activateAction.action.ReadValue<float>() > 0.8)
        {
            DrawPoint();
            CalculateVertexDirection();
            DrawTriangles();
        }
        else
        {
            numRecentVertices = 0;
        }

        foreach (Vertex v in centralVertices){
            //Debug.DrawRay(v.pos,v.direction*0.4f,Color.red);
            //Debug.DrawRay(v.pos,v.orthoDirection*0.5f,Color.cyan);
            //Debug.DrawRay(v.pos,v.rotated*0.5f,Color.magenta);
        }
        
        
        lastPoint = point;
    }

    void DebugSpheres(Vector3 point){
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        cube.transform.position = new Vector3(point.x,point.y,point.z);
        cube.transform.localScale = new Vector3(0.02f,0.02f,0.02f);
    }

    void OnDrawGizmos() {
        for (int i = 0; i < 9; i++){
            Handles.Label(vertices[i],i.ToString());
        }
    }
}
