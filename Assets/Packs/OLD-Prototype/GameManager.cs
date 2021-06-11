using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager m_Instance;

    [SerializeField] private float m_GameSpeed = 50f;

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

    public float GetGameSpeed()
    {
        return m_GameSpeed;
    }
}
