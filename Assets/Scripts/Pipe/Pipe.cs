using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour
{
    [SerializeField] private PipeSettings m_Settings;
    [SerializeField] private PipeItemGenerator[] m_Generators;
    private float m_CurveRadius;
    private int m_CurveSegmentCount;
    private float m_CurveAngle;

    private float m_RelativeRotation;

    private Mesh m_Mesh;
    private Vector3[] m_Vertices;
    private int[] m_Triangles;
    private string m_MeshName = "Pipe";
    private Vector2[] m_UV;

    private void Awake()
    {
        GetComponent<MeshFilter>().mesh = m_Mesh = new Mesh();
        m_Mesh.name = m_MeshName;
    }

    public void Generate(bool withItems = true)
    {
        m_CurveRadius = Random.Range(m_Settings.m_MinCurveRadius, m_Settings.m_MaxCurveRadius);
        m_CurveSegmentCount = Random.Range(m_Settings.m_MinCurveSegmentCount, m_Settings.m_MaxCurveSegmentCount + 1);

        m_Mesh.Clear();
        SetVertices();
        SetUV();
        SetTriangles();
        m_Mesh.RecalculateNormals();

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        if(withItems)
        {
            m_Generators[Random.Range(0, m_Generators.Length)].GenerateItems(this);
        }
    }

    private void SetUV()
    {
        m_UV = new Vector2[m_Vertices.Length];
        for (int i = 0; i < m_Vertices.Length; i += 4)
        {
            m_UV[i] = Vector2.zero;
            m_UV[i + 1] = Vector2.right;
            m_UV[i + 2] = Vector2.up;
            m_UV[i + 3] = Vector2.one;
        }
        m_Mesh.uv = m_UV;
    }

    public float GetRelativeRotation()
    {
        return m_RelativeRotation;
    }

    public float GetCurveRadius()
    {
        return m_CurveRadius;
    }

    public float GetCurveAngle()
    {
        return m_CurveAngle;
    }

    public int GetCurveSegmentCount()
    {
        return m_CurveSegmentCount;
    }

    public int GetPipeSegmentCount()
    {
        return m_Settings.m_PipeSegmentCount;
    }

    public void AlignWith(Pipe pipe)
    {
        m_RelativeRotation = Random.Range(0, m_CurveSegmentCount) * 360f / m_Settings.m_PipeSegmentCount;

        transform.SetParent(pipe.transform, false);

        transform.localPosition = Vector3.zero;

        transform.localRotation = Quaternion.Euler(0f, 0f, -pipe.m_CurveAngle);

        transform.Translate(0f, pipe.m_CurveRadius, 0f);
        transform.Rotate(m_RelativeRotation, 0f, 0f);
        transform.Translate(0f, -m_CurveRadius, 0f);

        transform.SetParent(pipe.transform.parent);

        transform.localScale = Vector3.one;
    }

    private void SetVertices()
    {
        m_Vertices = new Vector3[m_Settings.m_PipeSegmentCount * m_CurveSegmentCount * 4];

        float uStep = m_Settings.m_RingDistance / m_CurveRadius;
        m_CurveAngle = uStep * m_CurveSegmentCount * (360f / (2f * Mathf.PI));

        CreateFirstQuadRing(uStep);

        int iDelta = m_Settings.m_PipeSegmentCount * 4;
        for (int u = 2, i = iDelta; u <= m_CurveSegmentCount; u++, i += iDelta)
        {
            CreateQuadRing(u * uStep, i);
        }

        m_Mesh.vertices = m_Vertices;
    }

    private void SetTriangles()
    {
        m_Triangles = new int[m_Settings.m_PipeSegmentCount * m_CurveSegmentCount * 6]; // Each quad has 2 triangles so six vertex indices

        for (int t = 0, i = 0; t < m_Triangles.Length; t += 6, i += 4) // Order the vertices to show up on the outside
        {
            m_Triangles[t] = i;
            m_Triangles[t + 1] = m_Triangles[t + 4] = i + 2; // i + 2 for the outside
            m_Triangles[t + 2] = m_Triangles[t + 3] = i + 1; // i + 1 for the outside 
            m_Triangles[t + 5] = i + 3;
        }

        m_Mesh.triangles = m_Triangles;
    }

    private void CreateFirstQuadRing(float u)
    {
        float vStep = (2f * Mathf.PI) / m_Settings.m_PipeSegmentCount;

        Vector3 vertexA = GetPointOnTorus(0f, 0f);
        Vector3 vertexB = GetPointOnTorus(u, 0f);

        for (int v = 1, i = 0; v <= m_Settings.m_PipeSegmentCount; v++, i += 4)
        {
            m_Vertices[i] = vertexA;
            m_Vertices[i + 1] = vertexA = GetPointOnTorus(0f, v * vStep);
            m_Vertices[i + 2] = vertexB;
            m_Vertices[i + 3] = vertexB = GetPointOnTorus(u, v * vStep);
        }
    }

    private void CreateQuadRing(float u, int i)
    {
        float vStep = (2f * Mathf.PI) / m_Settings.m_PipeSegmentCount;
        int ringOffset = m_Settings.m_PipeSegmentCount * 4;

        Vector3 vertex = GetPointOnTorus(u, 0f);
        for (int v = 1; v <= m_Settings.m_PipeSegmentCount; v++, i += 4)
        {
            m_Vertices[i] = m_Vertices[i - ringOffset + 2];
            m_Vertices[i + 1] = m_Vertices[i - ringOffset + 3];
            m_Vertices[i + 2] = vertex;
            m_Vertices[i + 3] = vertex = GetPointOnTorus(u, v * vStep);
        }
    }

    private Vector3 GetPointOnTorus(float u, float v)
    {
        Vector3 p = Vector3.zero;

        float r = (m_CurveRadius + m_Settings.m_PipeRadius * Mathf.Cos(v));
        p.x = r * Mathf.Sin(u);
        p.y = r * Mathf.Cos(u);
        p.z = m_Settings.m_PipeRadius * Mathf.Sin(v);

        return p;
    }

    //private void OnDrawGizmosSelected()
    //{
    //    float uStep = (2f * Mathf.PI) / m_CurveSegmentCount;
    //    float vStep = (2f * Mathf.PI) / m_PipeSegmentCount;

    //    for (int u = 0; u < m_CurveSegmentCount; u++)
    //    {
    //        for (int v = 0; v < m_PipeSegmentCount; v++)
    //        {
    //            Vector3 point = GetPointOnTorus(u * uStep, v * vStep);
    //            Gizmos.color = new Color(
    //                1f,
    //                (float)v / m_PipeSegmentCount,
    //                (float)u / m_CurveSegmentCount);
    //            Gizmos.DrawSphere(point, 0.1f);
    //        }
    //    }
    //}
}

//private void Update()
//{
//    transform.position = new Vector3(0, 0, transform.position.z - GameManager.m_Instance.GetGameSpeed() * Time.deltaTime);
//}