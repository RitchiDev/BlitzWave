using Andrich;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager m_Instance { get; private set; }

    [SerializeField] private float m_Offset = 59.83f;
    [SerializeField] private int m_MaxEmpty = 4;
    [SerializeField] private int m_MinEmpty = 2;
    [SerializeField] private List<GameObject> m_Tubes;
    private GameObject m_NewTube;
    private int m_EmptyCounter;
    private int m_RandomEmptyAmount;

    private void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
        }
        else if (m_Instance != null)
        {
            Destroy(this);
        }
    }

    void Start()
    {
        if (m_Tubes != null && m_Tubes.Count > 0)
        {
            m_Tubes = m_Tubes.OrderBy(r => r.transform.position.z).ToList();
        }

        m_RandomEmptyAmount = 0;
    }

    public void SpawnTriggerEntered(GameObject tube)
    {
        MoveTubes(tube);
    }

    private void MoveTubes(GameObject tube)
    {
        m_NewTube = m_Tubes[0];
        m_Tubes.Remove(m_NewTube);

        tube.SetActive(false);
        if (m_EmptyCounter < m_RandomEmptyAmount)
        {
            //m_NewTube = ObjectPooler.m_Instance.SetActiveFromPool(WhichTube.empty, Vector3.zero, tube.transform.rotation);

            m_EmptyCounter++;
        }
        else
        {
            //Array tubeEnum = Enum.GetValues(typeof(WhichTube));
            //WhichTube randomTube = (WhichTube)tubeEnum.GetValue(UnityEngine.Random.Range(1, tubeEnum.Length));
            //m_NewTube = ObjectPooler.m_Instance.SetActiveFromPool(randomTube, Vector3.zero, tube.transform.rotation);

            m_EmptyCounter = 0;
            m_RandomEmptyAmount = UnityEngine.Random.Range(m_MinEmpty, m_MaxEmpty);
        }

        float newZ = m_Tubes[m_Tubes.Count - 1].transform.position.z + m_Offset;
        m_NewTube.transform.position = new Vector3(0, 0, newZ);
        m_Tubes.Add(m_NewTube);
    }
}
