using Andrich;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static MainMenu m_Instance { get; private set; }

    [SerializeField] private Player m_Player;
    [SerializeField] private Text m_ScoreText;
   
    private Canvas m_Canvas;

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
        Application.targetFrameRate = 1000;
    }

    public void StartGame(int mode)
    {
        m_Player.StartGame(mode);
        GameOver.m_Instance.AllowContinue();
        UISetActive(false);
    }

    public void OpenMainMenu()
    {
        ObjectPooler.m_Instance.DeactivateAllObjects();
        UISetActive(true);
    }

    public void SetHighscore(float distanceTraveled)
    {
        m_ScoreText.text = "Furthest Traveled: " + ((int)(distanceTraveled * 10f)).ToString();
    }

    private void UISetActive(bool value)
    {
        m_Canvas.enabled = value;
    }
}
