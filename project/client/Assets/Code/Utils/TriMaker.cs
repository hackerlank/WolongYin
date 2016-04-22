using UnityEngine;
using System.Collections;

public class TriMaker : MonoBehaviour
{

    public static void MakeListTargetTri(GameObject parentObj, float angle, float Y)
    {
        GameObject newObj = new GameObject();
        newObj.transform.parent = parentObj.transform;
        newObj.transform.localPosition = new Vector3(0, Y, 0);
        newObj.transform.localRotation = Quaternion.Euler(0, -90, 0);
        newObj.transform.localScale = new Vector3(0.5f, 1, 0.5f);
        TriMaker triMaker = newObj.AddComponent<TriMaker>();
        triMaker.mAngle = angle * Mathf.Deg2Rad;
    }

    public float mAngle;

    // Use this for initialization
    void Start()
    {
        MeshRenderer render = gameObject.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = new Mesh();
        Mesh mesh = meshFilter.mesh;
        float cos = Mathf.Cos(mAngle);
        float sin = Mathf.Sin(mAngle);
        Vector3[] vertices = mesh.vertices;
        Vector3[] triV = new Vector3[]
        {
            new Vector3(0, 0, 0),
            new Vector3(cos, 0, sin),
            new Vector3(cos, 0, -sin),
        };

        int[] ides = new int[] { 0, 1, 2 };
        mesh.vertices = triV;
        mesh.triangles = ides;

        Material mat = new Material(Shader.Find("Custom/ColorShader"));
        render.material = mat;

        render.material.SetColor("_MultiplyColor", new Color(161 / 255.0f, 201 / 255.0f, 49 / 255.0f, 0.5f));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
