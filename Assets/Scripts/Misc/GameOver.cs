using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public static GameOver m_Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private Player m_Player;
    [SerializeField] private Text m_ScoreText;
    [SerializeField] private GameObject m_ContinueButton;
    [SerializeField] private Color m_ButtonColor;
    [SerializeField] private Slider m_Slider;
    private Canvas m_Canvas;

    private float m_TimeBeforeLeave = -1f;

    private static float m_PreviousDistance;
    private bool m_AllowContinue;
    private bool m_AllowLeave;

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

    private void Update()
    {
        if (m_TimeBeforeLeave >= 0f)
        {
            m_TimeBeforeLeave -= Time.deltaTime;
            m_Slider.value = m_TimeBeforeLeave;
            if (m_TimeBeforeLeave <= 0f && m_AllowLeave)
            {
                m_TimeBeforeLeave = -1f;
                OpenMainMenu();
            }
        }
    }

    public void ContinueGame()
    {
        if (m_AllowContinue)
        {
            m_AllowLeave = false;
            m_Player.ContinueGame();
            AllowContinue(false);
        }
    }

    public void EndGame(float distanceTraveled)
    {
        m_AllowLeave = true;
        m_ScoreText.text = "Traveled: " + ((int)(distanceTraveled * 10f)).ToString();

        if (distanceTraveled > m_PreviousDistance)
        {
            m_PreviousDistance = distanceTraveled;
            MainMenu.m_Instance.SetHighscore(distanceTraveled);
        }

        if(m_AllowContinue)
        {
            m_ContinueButton.GetComponent<Button>().interactable = true;
            m_ContinueButton.GetComponent<Image>().color = m_ButtonColor;
        }
        else
        {
            m_ContinueButton.GetComponent<Button>().interactable = false;
            m_ContinueButton.GetComponent<Image>().color = Color.gray;
        }

        if(m_TimeBeforeLeave < 0)
        {
            m_Slider.value = m_Slider.maxValue;
            m_TimeBeforeLeave = m_Slider.maxValue;
        }

        UISetActive(true);
    }

    public void OpenMainMenu()
    {
        m_TimeBeforeLeave = -1f;
        m_AllowLeave = false;
        MainMenu.m_Instance.OpenMainMenu();
        UISetActive(false);
    }

    public void AllowContinue(bool value = true)
    {
        UISetActive(false);
        m_AllowContinue = value;
    }

    private void UISetActive(bool value)
    {
        m_Canvas.enabled = value;
    }
}
