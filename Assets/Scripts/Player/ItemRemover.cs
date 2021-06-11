using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRemover : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PipeItem item = other.GetComponentInParent<PipeItem>();
        if (item.RemoveEarly)
        {
            Andrich.ObjectPooler.m_Instance.DeactivateObject(item.gameObject, item.GetComponent<PipeItem>().PrefabEnum);
        }
    }
}
