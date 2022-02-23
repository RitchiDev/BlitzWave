using Andrich;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Avatar : MonoBehaviour
{
    [SerializeField] private GameObject m_Ship;
    [SerializeField] private ParticleSystem m_Shape, m_Trail, m_Burst; 
    [SerializeField] private float m_DeathCountDown = -1f;
    private Player m_Player;

    private void Awake()
    {
        m_Player = transform.root.GetComponent<Player>();
    }

    private void Update()
    {
        if (m_DeathCountDown >= 0f)
        {
            m_DeathCountDown -= Time.deltaTime;
            if (m_DeathCountDown <= 0f)
            {
                m_DeathCountDown = -1f;
                m_Shape.gameObject.SetActive(true);
                m_Trail.gameObject.SetActive(true);
                m_Ship.gameObject.SetActive(true);
                m_Player.Die();
            }
        }
    }

    [System.Obsolete]
    private void OnTriggerEnter(Collider other)
    {
        PipeItem item = other.GetComponentInParent<PipeItem>();

        if(item)
        {
            if(item.IsObstacle)
            {
                if(m_DeathCountDown < 0f)
                {
                    m_Player.TurnOffRotation();

                    m_Shape.gameObject.SetActive(false);
                    m_Trail.gameObject.SetActive(false);
                    m_Ship.gameObject.SetActive(false);

                    m_Burst.Play();
                    m_DeathCountDown = m_Burst.startLifetime;
                }
            }
            else
            {
                ObjectPooler.m_Instance.DeactivateObject(item.gameObject, item.GetComponent<PipeItem>().PrefabEnum);
                item.gameObject.SetActive(false);
                HUD.m_Instance.AddCrystals(item.Worth);
            }
        }
    }
}
