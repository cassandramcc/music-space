using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshHolder : MonoBehaviour
{
    Mesh mesh;
    public List<Vector3> vertices;
    public List<int> triangles;

    public class Vertex{
        public Vector3 pos;
        public List<Vector3> edgeVertices = new List<Vector3>();
        public Vertex(Vector3 _pos){
            pos = _pos;
        }

    }

    public List<Vertex> centralVertices = new List<Vertex>();


    void UpdateMesh(){
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }
    // Start is called before the first frame update

    public void CalculateVertexDirection(){
        if (centralVertices.Count >= 2){
            Vector3 direction = centralVertices[centralVertices.Count-1].pos - centralVertices[centralVertices.Count - 2].pos;
            direction = direction/direction.magnitude; //normalised direction
            Vertex lastVertex = centralVertices[centralVertices.Count - 2];
            //this is the direction between first vertex and the next vertex
            //the first vertex's points need to be in the perpedincular direction
            Vector3 perpendicular = new Vector3();
            Vector3.OrthoNormalize(ref direction,ref perpendicular);
            Vector3 rotation = perpendicular * Mathf.Cos(Mathf.PI/2) + (Vector3.Cross(direction,perpendicular))*Mathf.Sin(Mathf.PI/2) + direction*(Vector3.Dot(perpendicular,direction))*(1-Mathf.Cos(Mathf.PI/2));
            //the point 180 degrees away from the perp vector
            
            //Create points on plane defined by this perpendicular by rotating the perp vector around the direction as the axis
            for (int i = 0; i <= 270; i+=90){
                Vector3 rot = perpendicular * Mathf.Cos(i * Mathf.Deg2Rad) + (Vector3.Cross(direction,perpendicular))*Mathf.Sin(i * Mathf.Deg2Rad) + direction*(Vector3.Dot(perpendicular,direction))*(1-Mathf.Cos(i * Mathf.Deg2Rad));
                Vector3 newPoint = lastVertex.pos - rot;
                Vector3 shrinkNewPoint = Vector3.MoveTowards(newPoint,lastVertex.pos,0.9f);
                lastVertex.edgeVertices.Add(shrinkNewPoint);
            }
        }
    }

    public void DrawTriangles(){
        if (centralVertices.Count >=3){
            Vertex start = centralVertices[centralVertices.Count - 3];
            Vertex end = centralVertices[centralVertices.Count - 2];
            vertices.AddRange(start.edgeVertices);
            vertices.AddRange(end.edgeVertices);
            int startVertex = vertices.Count - 8;
            //If you want different shapes, will need to make this number changeable.
            if (start.pos.x > end.pos.x){
                int[] tris = new int[]{
                    startVertex+5, startVertex,startVertex+1,
                    startVertex,startVertex+5,startVertex+4,

                    startVertex+3,startVertex,startVertex+7,
                    startVertex+7,startVertex,startVertex+4,

                    startVertex+2,startVertex+3,startVertex+6,
                    startVertex+6,startVertex+3,startVertex+7,

                    startVertex+5,startVertex+1,startVertex+2,
                    startVertex+6,startVertex+5,startVertex+2
                };
                triangles.AddRange(tris); 
            }
            else{
                int[] tris = new int[]{
                    startVertex, startVertex+1,startVertex+5,
                    startVertex,startVertex+5,startVertex+4,

                    startVertex,startVertex+7,startVertex+3,
                    startVertex,startVertex+4,startVertex+7,

                    startVertex+3,startVertex+6,startVertex+2,
                    startVertex+3,startVertex+7,startVertex+6,

                    startVertex+1,startVertex+2,startVertex+5,
                    startVertex+5,startVertex+2,startVertex+6
                };
                triangles.AddRange(tris);
            }
            
            //draw triangles between last vertex and last last vertex.
            UpdateMesh();
        }
    }
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        vertices = new List<Vector3>();
        triangles = new List<int>();
        UpdateMesh();
    }
}
