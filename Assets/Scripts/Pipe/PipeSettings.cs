using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Default Pipe Settings", menuName = "Create Pipe Settings")]
public class PipeSettings : ScriptableObject
{
    public float m_PipeRadius = 1f; // The width of the pipe
    public int m_PipeSegmentCount = 8; // The division of the pipe

    public float m_RingDistance = 2.3f; // The lenght of the pipe

    public float m_MinCurveRadius = 80f;
    public float m_MaxCurveRadius = 160f;

    public int m_MinCurveSegmentCount = 8;
    public int m_MaxCurveSegmentCount = 20;
}
