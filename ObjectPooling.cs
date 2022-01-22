using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectPooling : MonoBehaviour
{
    public static ObjectPooling instance;
    public List<PooledObjectData> objectData = new List<PooledObjectData>();
    public List<PooledObject> pooledObjects = new List<PooledObject>();
    public HashSet<string> objectNamesList = new HashSet<string>();
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }else
        {
            Destroy(gameObject);
        }
        Init();
    }
    public void Init()
    {
        for (int i = 0; i < objectData.Count; i++)
        {
            PooledObject pooledObj = new PooledObject(objectData[i].objectNames,
                                                objectData[i].objectPrefabs,
                                                objectData[i].spawnAmount );
            GameObject objParent = new GameObject(pooledObj.objectNames + " Parent");
            objParent.transform.parent = this.gameObject.transform;
            pooledObj.objectParent = objParent.transform;
            pooledObjects.Add(pooledObj);
            pooledObj.AddObject(objectData[i].spawnAmount);
            objectNamesList.Add(objectData[i].objectNames);
        }
        foreach (var item in objectNamesList)
        {
            Debug.Log(item);
        }
    }
    public bool IsObjectExists(string name)
    {
        if(objectNamesList.Contains(name))
        {
            return true;
        }else return false;
    }
    public GameObject CreateObject(string name)
    {
        GameObject o = null;
        if(IsObjectExists(name))
        {
            for (int i = 0; i < pooledObjects.Count; i++)
            {
                if(pooledObjects[i].objectNames == name)
                {
                    o = pooledObjects[i].Get();
                    o.SetActive(true);
                }
            }
            return o;
        }else
        {
            Debug.LogError("No game object named " + name +" exists" );
            return o;
        }
    }
    public GameObject CreateObject(string name, Vector3 position, Quaternion rotation)
    {
        GameObject obj = CreateObject(name) as GameObject;
        if(obj != null)
        {
            obj.transform.position = position;
            obj.transform.rotation = rotation;
        }
        return obj;

    }
    public GameObject CreateObject(string name, Vector3 position, Quaternion rotation, Transform parent)
    {
        GameObject obj = CreateObject(name, position, rotation) as GameObject;
        if(obj != null)
        {
            obj.transform.parent = parent;
        }
        return obj;

    }
    public void DestroyObject(GameObject obj)
    {
        StartCoroutine(DestroyObjectCoroutine(obj, 0f));
    }
    public void DestroyObject(GameObject obj, float time)
    {
        StartCoroutine(DestroyObjectCoroutine(obj, time));
    }
    public void DestroyObjectWithCallback(GameObject obj, float time, Action callback)
    {
        DestroyObject(obj, time);
        if(callback != null)
        {
            callback();
        }
    }
    public IEnumerator DestroyObjectCoroutine(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if(pooledObjects[i].objectDequeueList.Contains(obj))
            {
                pooledObjects[i].ReturnToPool(obj);
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
[System.Serializable]
public class PooledObject
{
    public string objectNames;
    public GameObject objectPrefabs;
    public Transform objectParent;
    public Vector3 objectScale;
    public int spawnAmount;
    public Queue<GameObject> objectQueue = new Queue<GameObject>();
    public List<GameObject> objectDequeueList = new List<GameObject>();

    public PooledObject(string name, GameObject prefabs, int amount)
    {
        objectNames = name;
        objectPrefabs = prefabs;
        spawnAmount = amount;
        objectScale = prefabs.transform.localScale;
    }
    public void AddObject(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject obj = GameObject.Instantiate(objectPrefabs) as GameObject;
            obj.transform.SetParent(objectParent, false);
            obj.SetActive(false);
            objectQueue.Enqueue(obj);
        }
    }
    public void AddObject(int amount, Transform parent)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject obj = GameObject.Instantiate(objectPrefabs) as GameObject;
            obj.transform.parent = parent;
            obj.SetActive(false);
            objectQueue.Enqueue(obj);
        }
    }
    public GameObject Get()
    {
        if(objectQueue.Count == 0)
        {
            AddObject(1, objectParent);
        }
        GameObject obj = objectQueue.Dequeue();
        obj.transform.localScale = objectScale;
        obj.SetActive(true);
        objectDequeueList.Add(obj);
        return obj;
    }
    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        objectQueue.Enqueue(obj);
        objectDequeueList.Remove(obj);
    }
}
/*
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ObjectPooling/ObjectData")]
public class PooledObjectData : ScriptableObject
{
    public string objectNames;
    public GameObject objectPrefabs;
    public int spawnAmount;

}*/
