using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class PaintCreator : MonoBehaviour
{
    Mesh mesh;
    List<Vector3> vertices;
    List<int> triangles;
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
        UpdateMesh();
    }

    void CreateShape(){
        vertices = new List<Vector3>();
        triangles = new List<int>();
        for(int i = 0; i <= 10; i++){
            vertices.Add(new Vector3(0,0,i));
            vertices.Add(new Vector3(0,1,i));
            vertices.Add(new Vector3(1,1,i));
            vertices.Add(new Vector3(1,0,i));
        }
        int[] frontFace = {0,1,2,0,2,3};
        triangles.AddRange(frontFace);
        for(int i = 0; i < 10; i++){
            int corner = i*4;
            int[] bottom1 = {corner,corner+7,corner+4};
            int[] bottom2 = {corner,corner+3,corner+7};
            int[] left1 = {corner,corner+5,corner+1};
            int[] left2 = {corner,corner+4,corner+5};
            int[] top1 = {corner+1,corner+5,corner+6};
            int[] top2 = {corner+1,corner+6,corner+2};
            int[] right1 = {corner+3,corner+2,corner+6};
            int[] right2 = {corner+3,corner+6,corner+7};
            triangles.AddRange(bottom1);
            triangles.AddRange(bottom2);
            triangles.AddRange(left1);
            triangles.AddRange(left2);
            triangles.AddRange(top1);
            triangles.AddRange(top2);
            triangles.AddRange(right1);
            triangles.AddRange(right2);
        }
    }

    void UpdateMesh(){
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }

}
