using Andrich;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiralPlacer : PipeItemGenerator
{
	//[SerializeField] private List<WhichPrefab> m_ItemList;
	[SerializeField] private WhichPrefab[] m_ItemTable;

	public override void GenerateItems(Pipe pipe)
	{
		float start = (UnityEngine.Random.Range(0, pipe.GetPipeSegmentCount()) + 0.5f);
		float direction = UnityEngine.Random.value < 0.5f ? 1f : -1f;

		float angleStep = pipe.GetCurveAngle() / pipe.GetCurveSegmentCount();

		for (int i = 0; i < pipe.GetCurveSegmentCount(); i++)
		{
			int rng = UnityEngine.Random.Range(0, m_ItemTable.Length);
			WhichPrefab randomItem = m_ItemTable[rng];

			PipeItem item = ObjectPooler.m_Instance.SetActiveFromPool(randomItem, Vector3.zero, Quaternion.identity).GetComponent<PipeItem>();

			float pipeRotation = (start + i * direction * 360f / pipe.GetPipeSegmentCount());
			item.Position(pipe, i * angleStep, pipeRotation);
		}
	}
}
