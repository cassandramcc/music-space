using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(MeshFilter))]
public class PaintCreator : MonoBehaviour
{
    Mesh mesh;
    List<Vector3> vertices;
    int newestVertexDrawn = 0;
    List<int> triangles;
    public Camera cam;

    public GameObject mouseCursorUI;
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

    void DrawVertices(){
        Vector3 mousePos = Input.mousePosition;
        Vector3 point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10));
        Vector3[] newVertices = {
            new Vector3(point.x,point.y,point.z),
            new Vector3(point.x,point.y+1,point.z),
            new Vector3(point.x,point.y+1,point.z+1),
            new Vector3(point.x,point.y,point.z+1)
        };
        vertices.AddRange(newVertices);
        newestVertexDrawn = vertices.Count-8;
    }

    void DrawTriangles(){
        int vertexIndex = newestVertexDrawn;
        List<int> newTriangles = new List<int>();
        
        if (vertices[vertexIndex].x > vertices[vertexIndex+4].x){
            newTriangles.AddRange(
                new int[]{
                    vertexIndex,vertexIndex+5,vertexIndex+1,
                    vertexIndex,vertexIndex+4,vertexIndex+5,
                    vertexIndex+6,vertexIndex+7,vertexIndex+3,
                    vertexIndex+2,vertexIndex+6,vertexIndex+3,
                    vertexIndex+1,vertexIndex+6,vertexIndex+2,
                    vertexIndex+1,vertexIndex+5,vertexIndex+6,
            //bottom
                    vertexIndex,vertexIndex+3,vertexIndex+7,
                    vertexIndex,vertexIndex+7,vertexIndex+4
                }
            );
        }
        else{
            newTriangles.AddRange(
                new int[]{
                    vertexIndex,vertexIndex+1,vertexIndex+5,
                    vertexIndex,vertexIndex+5,vertexIndex+4,
                    vertexIndex+6,vertexIndex+3,vertexIndex+7,
                    vertexIndex+2,vertexIndex+3,vertexIndex+6,
                    vertexIndex+1,vertexIndex+2,vertexIndex+6,
                    vertexIndex+1,vertexIndex+6,vertexIndex+5,
                    //bottom
                    vertexIndex,vertexIndex+7,vertexIndex+3,
                    vertexIndex,vertexIndex+4,vertexIndex+7
                }
            );
        }
        /**int[] newTriangles = {
            //left
            
            vertexIndex,vertexIndex+1,vertexIndex+5,
            vertexIndex,vertexIndex+5,vertexIndex+4,
            //right
            vertexIndex+6,vertexIndex+3,vertexIndex+7,
            vertexIndex+2,vertexIndex+3,vertexIndex+6,
            //top
            vertexIndex+1,vertexIndex+2,vertexIndex+6,
            vertexIndex+1,vertexIndex+6,vertexIndex+5,
            //bottom
            vertexIndex,vertexIndex+7,vertexIndex+3,
            vertexIndex,vertexIndex+4,vertexIndex+7
        };**/
        triangles.AddRange(newTriangles);
        UpdateMesh();
    }


    void DrawFrontFace(){
        newestVertexDrawn = vertices.Count;
        Vector3 mousePos = Input.mousePosition;
        Vector3 point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10));
        Vector3[] newVertices = {
            new Vector3(point.x,point.y,point.z),
            new Vector3(point.x,point.y+1,point.z),
            new Vector3(point.x,point.y+1,point.z+1),
            new Vector3(point.x,point.y,point.z+1)
        };
        vertices.AddRange(newVertices);
        triangles.AddRange(new int[]{newestVertexDrawn,newestVertexDrawn+3,newestVertexDrawn+1,newestVertexDrawn+3,newestVertexDrawn+2,newestVertexDrawn+1});
        triangles.AddRange(new int[]{newestVertexDrawn,newestVertexDrawn+1,newestVertexDrawn+3,newestVertexDrawn+3,newestVertexDrawn+1,newestVertexDrawn+2});
        UpdateMesh();
    }

    void DrawBackFace(){
        int endIndex = vertices.Count - 4;
        triangles.AddRange(new int[]{endIndex,endIndex+1,endIndex+2,endIndex,endIndex+2,endIndex+3});
        UpdateMesh();
    }
    void Update() {
        Debug.Log(cam.transform.forward);
        Vector3 mousePos = Input.mousePosition;
        Vector3 point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10));
        mouseCursorUI.transform.position = point;
        Debug.DrawRay(cam.transform.position,cam.transform.forward*10,Color.green);

        if (Input.GetMouseButtonDown(1)){
            DrawFrontFace();
        }
        if (Input.GetMouseButton(1)){
            DrawVertices();
            DrawTriangles();
        }
        else if (Input.GetMouseButtonUp(1)){
            DrawBackFace();
        }
    }
    void DebugSpheres(Vector3 point){
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        cube.transform.position = new Vector3(point.x,point.y,point.z);
        cube.transform.localScale = new Vector3(0.2f,0.2f,0.2f);
        cube = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        cube.transform.position = new Vector3(point.x,point.y+1,point.z);
        cube.transform.localScale = new Vector3(0.2f,0.2f,0.2f);
        cube = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        cube.transform.position = new Vector3(point.x,point.y+1,point.z+1);
        cube.transform.localScale = new Vector3(0.2f,0.2f,0.2f);
        cube = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        cube.transform.position = new Vector3(point.x,point.y,point.z+1);
        cube.transform.localScale = new Vector3(0.2f,0.2f,0.2f);
    }
}
