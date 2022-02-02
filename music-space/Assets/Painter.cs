using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Painter : MonoBehaviour
{

    class Vertex{
        public Vector3 pos;
        public Vector3[] edgeVertices = new Vector3[]{};

        public Vector3 direction;
        public Vertex(Vector3 _pos){
            pos = _pos;
        }

    }

    Mesh mesh;
    List<Vector3> vertices;

    List<Vertex> centralVertices;
    int newestVertexDrawn = 0;
    List<int> triangles;

    public Camera cam;

    public GameObject mouseCursorUI;
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
        Vector3 mousePos = Input.mousePosition;
        Vector3 point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10));
        mouseCursorUI.transform.position = point;
        Debug.DrawRay(cam.transform.position,cam.transform.forward*10,Color.green);
        centralVertices.Add(new Vertex(point));
    }

    void CalculateVertexDirection(){
        if (centralVertices.Count >= 2){
            Vector3 direction = centralVertices[centralVertices.Count-1].pos - centralVertices[centralVertices.Count - 2].pos;
            direction = direction/direction.magnitude; //normalised direction
            Vertex lastVertex = centralVertices[centralVertices.Count - 2];
            lastVertex.direction = direction;
            //this is the direction between first vertex and the next vertex
            //the first vertex's points need to be in the perpedincular direction
            //The perpendicular direction is the negative reciprical gradient, so need to calculate gradient of direction
            // then turn perpendicular gradient into a vector direction.
            //Create points on plane defined by this perpendicular gradient
            //Before doing triangles, add debug spheres to test points spawn correctly.
        }
    }

    void DrawTriangles(){
        if (centralVertices.Count >=3){
            //draw triangles between last vertex and last last vertex.
        }
    }
    // Update is called once per frame
    void Update()
    {
        DrawPoint();
        CalculateVertexDirection();
        DrawTriangles();
    }
}
