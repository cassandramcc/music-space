using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PaintHolder : MonoBehaviour
{

    [SerializeField]
    public List<Vector3> meshVertices = new List<Vector3>();

    public List<int> triangles = new List<int>();

    Mesh mesh;
    // Start is called before the first frame update
    public void UpdateMesh(){
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        Debug.Log("UpdateMesh");
        mesh.Clear();

        //Debug.Log("Mesh vertices count: "+meshVertices.Count);
        
        int min = triangles.Min(t => t);
        
        mesh.vertices = meshVertices.ToArray();
        for (int i = 0 ; i < triangles.Count; i++){
            triangles[i] -= min;    
        }
        //Debug.Log("Mesh vertices count: "+meshVertices.Count);
        //Debug.Log("mesh.vertices count:" +mesh.vertices.Count());
        //Debug.Log("Min index: " + triangles.Min(t => t));
        //Debug.Log("Max index: "+ triangles.Max(t => t));
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }

    void Update(){
        meshVertices = mesh.vertices.ToList();
        triangles = mesh.triangles.ToList();
    }
}
