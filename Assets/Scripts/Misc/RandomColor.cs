using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomColor : MonoBehaviour
{
    [SerializeField] private List<Color> m_Colors;
    [SerializeField] private float m_TimeBeforeChange = 60f;
    [SerializeField] private float m_LerpTime = 5f;
    private Light m_Light;
    private Color m_PreviousColor;

    private void Awake()
    {
        m_Light = GetComponent<Light>();
        m_Light.color = m_Colors[Random.Range(0, m_Colors.Count)];
    }

    private void OnEnable()
    {
        m_PreviousColor = m_Light.color;
        StartCoroutine(ColorChangeTimer());
    }

    private IEnumerator ColorChangeTimer()
    {
        while(true)
        {
            float elapsedTime = 0;

            yield return new WaitForSeconds(m_TimeBeforeChange);

            int rng = Random.Range(0, m_Colors.Count);

            while(m_PreviousColor == m_Colors[rng])
            {
                rng = Random.Range(0, m_Colors.Count);
            }

            while(elapsedTime < m_LerpTime)
            {
                elapsedTime += Time.deltaTime;
                m_Light.color = Color.Lerp(m_PreviousColor, m_Colors[rng], elapsedTime / m_LerpTime);

                yield return null;
            }

            m_PreviousColor = m_Colors[rng];
        }
    }
}
