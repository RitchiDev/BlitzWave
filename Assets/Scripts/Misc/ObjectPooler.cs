using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Andrich
{ 
    public enum WhichPrefab
    {
        notSet,
        bomb,
        pillar,
        normalCrystal,
        mediumCrystal,
        largeCrystal,
        doritos,
        halfDisc,
        oneSlice,
        halfAndHalf
    }

    public class ObjectPooler : MonoBehaviour
    {
        public static ObjectPooler m_Instance { get; private set; }

        private Dictionary<WhichPrefab, Queue<GameObject>> m_PrefabDictionary;
        private Dictionary<WhichPrefab, Queue<PrefabPool>> m_NestedClassDictionary;

        [SerializeField] private List<PrefabPool> m_PrefabPools = new List<PrefabPool>();

        [Serializable]
        public class PrefabPool
        {
            [SerializeField] private string m_ElementName = "Name";

            public WhichPrefab m_WhichPrefab = WhichPrefab.notSet;

            public GameObject m_Prefab = null;
            public Transform m_Parent = null;
            public int m_CopyAmount = 50;
        }

        private void Awake()
        {
            if(m_Instance == null)
            {
                m_Instance = this;
            }
            else if(m_Instance != null)
            {
                Destroy(this);
            }


            InstantiatePrefabs();
        }

        private void InstantiatePrefabs()
        {
            m_PrefabDictionary = new Dictionary<WhichPrefab, Queue<GameObject>>();
            m_NestedClassDictionary = new Dictionary<WhichPrefab, Queue<PrefabPool>>();

            foreach (PrefabPool pool in m_PrefabPools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();
                Queue<PrefabPool> nestedClass = new Queue<PrefabPool>();

                for (int i = 0; i < pool.m_CopyAmount; i++)
                {
                    GameObject copy = Instantiate(pool.m_Prefab, pool.m_Parent);
                    copy.SetActive(false);

                    objectPool.Enqueue(copy);
                    nestedClass.Enqueue(pool);
                }

                m_PrefabDictionary.Add(pool.m_WhichPrefab, objectPool);
                m_NestedClassDictionary.Add(pool.m_WhichPrefab, nestedClass);
            }
        }

        public void DeactivateAllObjects()
        {
            Array prefabsEnum = Enum.GetValues(typeof(WhichPrefab));

            for (int i = 1; i < prefabsEnum.Length; i++)
            {
                foreach (GameObject gameObject in m_PrefabDictionary[(WhichPrefab)i])
                {
                    gameObject.transform.SetParent(m_NestedClassDictionary[(WhichPrefab)i].Peek().m_Parent);
                    gameObject.SetActive(false);
                }
            }
        }

        public void DeactivateObject(GameObject gameObject, WhichPrefab whichPrefab = WhichPrefab.notSet)
        {
            gameObject.transform.SetParent(m_NestedClassDictionary[whichPrefab].Peek().m_Parent);
            gameObject.SetActive(false);
        }

        private void IncreasePoolSize(WhichPrefab prefabEnum)
        {
            PrefabPool pool = m_NestedClassDictionary[prefabEnum].Peek();

            for (int i = 0; i < pool.m_CopyAmount; i++)
            {
                GameObject copy = Instantiate(pool.m_Prefab, pool.m_Parent);
                copy.SetActive(false);

                m_PrefabDictionary[prefabEnum].Enqueue(copy);
                m_NestedClassDictionary[prefabEnum].Enqueue(pool);
            }
        }

        public GameObject SetActiveFromPool(WhichPrefab prefabEnum, Vector3 position, Quaternion rotation)
        {
            GameObject objectToSetActive;

            if (!m_PrefabDictionary.ContainsKey(prefabEnum))
            {
                Debug.LogError("The enum: " + prefabEnum + " hasn't been assigned yet");
                return null;
            }

            if(m_PrefabDictionary[prefabEnum].Peek().activeSelf)
            {
                Debug.Log("Increasing Pool Size");
                IncreasePoolSize(prefabEnum);
            }

            objectToSetActive = m_PrefabDictionary[prefabEnum].Dequeue();

            objectToSetActive.SetActive(true);
            objectToSetActive.transform.position = position;
            objectToSetActive.transform.rotation = rotation;

            m_PrefabDictionary[prefabEnum].Enqueue(objectToSetActive);

            return objectToSetActive;
        }
    }
}
