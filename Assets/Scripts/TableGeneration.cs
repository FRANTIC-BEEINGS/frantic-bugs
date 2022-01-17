using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableGeneration : MonoBehaviour
{

/*
      Mesh mesh;
      Vector3[] vertices;
      int[] triangles;

      void Start()
      {
          mesh = new Mesh();
          GetComponent<MeshFilter>().mesh = mesh;

          CreateShape();
          UpdateMesh();
      }

      void CreateShape()
      {
          vertices = new Vector3[]
          {
              new Vector3 (0, 0, 0),
              new Vector3 (0, 0, 1),
              new Vector3 (1, 0, 0)
          };

          triangles = new int[]
          {
              0, 1, 2
          };
      }

      void UpdateMesh()
      {
          mesh.Clear();

          mesh.vertices = vertices;
          mesh.triangles = triangles;
      }
*/

    [SerializeField] private GameObject _map;
    private MapGeneration _mapGeneration;

    [SerializeField] private float _borderUpDown;
    [SerializeField] private float _borderLeftRight;
    [SerializeField] private float _plancWidth;
    [SerializeField] private bool UpdateButton;
    private bool prevboolvar;

    private float _width, _height;

    Mesh mesh;
    List<Vector3> vertices;
    List<int> triangles;

    private void Start()
    {
        _width = 100;
        _height = 100;

        if (_map == null)
        {
            Initialize();
        }
        else
        {
            Initialize(_map);
        }
    }

    private void Update()
    {
        if (prevboolvar != UpdateButton)
        {
            Initialize();
        }
        prevboolvar = UpdateButton;
    }

    private void Initialize()
    {
        GenerateMesh();
    }

    public void Initialize(GameObject map)
    {
        _mapGeneration = _map.GetComponent<MapGeneration>();
        _width = _mapGeneration.GetMapUnityWidth();
        _height = _mapGeneration.GetMapUnityHeight();
        Initialize();
    }

    private void GenerateMesh()
    {
        if (mesh != null)
        {
            mesh.Clear();
        }
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();

        mesh.Clear();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        Vector2[] uvs = new Vector2[vertices.Count];

        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].y);
        }
        mesh.uv = uvs;
    }

    private void CreateShape()
    {

        if (vertices != null)
        {
            vertices.Clear();
        }
        if (triangles != null)
        {
            triangles.Clear();;
        }

        vertices = new List<Vector3>();
        vertices.Add(new Vector3(0, 0, 0));
        vertices.Add(new Vector3(0, 1, 0));
        vertices.Add(new Vector3(1, 0, 0));
        vertices.Add(new Vector3(1, 1, 0));

        triangles = new List<int>();
        triangles.Add(0);
        triangles.Add(1);
        triangles.Add(2);
        triangles.Add(1);
        triangles.Add(3);
        triangles.Add(2);
    }

}
