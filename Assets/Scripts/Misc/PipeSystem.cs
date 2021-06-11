using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeSystem : MonoBehaviour
{
    public static PipeSystem m_Instance { get; private set; }
    [SerializeField] private Pipe m_PipePrefab;
    [SerializeField] private int m_PipeCount = 40;
    [SerializeField] private int m_EmptyPipeCount = 2;
    private Pipe[] m_Pipes;

    private void Awake()
    {
        if(m_Instance == null)
        {
            m_Instance = this;
        }
        else if(m_Instance != null)
        {
            Destroy(this);
        }

        m_Pipes = new Pipe[m_PipeCount];
        for (int i = 0; i < m_Pipes.Length; i++)
        {
            Pipe pipe = m_Pipes[i] = Instantiate<Pipe>(m_PipePrefab);
            pipe.transform.SetParent(transform, false);
        }
    }

    public Pipe SetUpNextPipe()
    {
        ShiftPipes();
        AlignNextPipeWithOrigin();
        m_Pipes[m_Pipes.Length - 1].Generate();
        m_Pipes[m_Pipes.Length - 1].AlignWith(m_Pipes[m_Pipes.Length - 2]);
        transform.localPosition = new Vector3(0f, -m_Pipes[1].GetCurveRadius());
        return m_Pipes[1];
    }

    public Pipe SetupFirstPipe(bool reset = true)
    {
        if(reset)
        {
            for (int i = 0; i < m_Pipes.Length; i++)
            {
                Pipe pipe = m_Pipes[i];
                pipe.Generate(i > m_EmptyPipeCount);
                if (i > 0)
                {
                    pipe.AlignWith(m_Pipes[i - 1]);
                }
            }
        }
        else
        {
            for (int i = 0; i < m_EmptyPipeCount + 1; i++)
            {
                for (int c = 0; c < m_Pipes[i].transform.childCount; c++)
                {
                    m_Pipes[i].transform.GetChild(c).gameObject.SetActive(false);
                }
            }
        }

        AlignNextPipeWithOrigin();

        transform.localPosition = new Vector3(0f, -m_Pipes[1].GetCurveRadius());
        return m_Pipes[1];
    }

    private void AlignNextPipeWithOrigin()
    {
        Transform transformToAlign = m_Pipes[1].transform;
        for (int i = 0; i < m_Pipes.Length; i++)
        {
            if(i != 1)
            {
                m_Pipes[i].transform.SetParent(transformToAlign);
            }
        }

        transformToAlign.localPosition = Vector3.zero;
        transformToAlign.localRotation = Quaternion.identity;

        for (int i = 0; i < m_Pipes.Length; i++)
        {
            if(i != 1)
            {
                m_Pipes[i].transform.SetParent(transform);
            }
        }
    }

    private void ShiftPipes()
    {
        Pipe temp = m_Pipes[0];
        for (int i = 0; i < m_Pipes.Length; i++)
        {
            m_Pipes[Mathf.Clamp(i - 1, 0, m_Pipes.Length)] = m_Pipes[i];
        }
        m_Pipes[m_Pipes.Length - 1] = temp;
    }
}
