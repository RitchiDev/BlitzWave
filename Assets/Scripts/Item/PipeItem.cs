using Andrich;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeItem : MonoBehaviour
{
	[SerializeField] private int m_Worth = 1;
	[SerializeField] private bool m_IsObstacle = true;
	[SerializeField] private bool m_RemoveEarly = true;
	[SerializeField] private WhichPrefab m_PrefabEnum;

	public int Worth => m_Worth;
	public bool IsObstacle => m_IsObstacle;
	public bool RemoveEarly => m_RemoveEarly;
	public WhichPrefab PrefabEnum => m_PrefabEnum;

	private Transform m_Rotater;

	private void Awake()
	{
		m_Rotater = transform.GetChild(0);
		if(m_IsObstacle)
		{
			m_Worth = 0;
		}
	}

	public void Position(Pipe pipe, float curveRotation, float ringRotation)
	{
		transform.SetParent(pipe.transform, false);
		transform.localRotation = Quaternion.Euler(0f, 0f, -curveRotation);
		m_Rotater.localPosition = new Vector3(0f, pipe.GetCurveRadius());
		m_Rotater.localRotation = Quaternion.Euler(ringRotation, 0f, 0f);
	}
}
