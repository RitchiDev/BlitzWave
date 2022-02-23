using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public static HUD m_Instance { get; private set; }

    [SerializeField] private TMP_Text m_DistanceText, m_MovementSpeedText, m_CrystalsText;
    private Canvas m_Canvas;
    private int m_CurrentCrystals;

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

        m_Canvas = GetComponent<Canvas>();
    }

    public void SetValues(float distanceTraveled, float movementSpeed)
    {
        m_DistanceText.text = ((int)(distanceTraveled * 10f)).ToString();
        m_MovementSpeedText.text = ((int)(movementSpeed * 10f)).ToString();
        m_CrystalsText.text = m_CurrentCrystals.ToString();
    }

    public void AddCrystals(int crystals)
    {
        m_CurrentCrystals = Mathf.Clamp(m_CurrentCrystals + crystals, 0, 99999);
        m_CrystalsText.text = m_CurrentCrystals.ToString();
    }

    private void UISetActive(bool value)
    {
        m_Canvas.enabled = value;
    }
}
