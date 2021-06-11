using Andrich;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPlacer : PipeItemGenerator
{
	[SerializeField] WhichPrefab[] m_ItemTable;

	public override void GenerateItems(Pipe pipe)
	{
		float angleStep = pipe.GetCurveAngle() / pipe.GetCurveSegmentCount();
		for (int i = 0; i < pipe.GetCurveSegmentCount(); i++)
		{
			//Array itemEnum = Enum.GetValues(typeof(WhichItem));
			//WhichItem randomItem = (WhichItem)itemEnum.GetValue(UnityEngine.Random.Range(1, itemEnum.Length));

			int rng = UnityEngine.Random.Range(0, m_ItemTable.Length);
			WhichPrefab randomItem = m_ItemTable[rng];

			PipeItem item = ObjectPooler.m_Instance.SetActiveFromPool(randomItem, Vector3.zero, Quaternion.identity).GetComponent<PipeItem>();

			float pipeRotation = (UnityEngine.Random.Range(0, pipe.GetPipeSegmentCount()) + 0.5f) * 360f / pipe.GetPipeSegmentCount();
			item.Position(pipe, i * angleStep, pipeRotation);
		}
	}
}
