using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ObjectPooling/ObjectData")]
public class PooledObjectData : ScriptableObject
{
    public string objectNames;
    public GameObject objectPrefabs;
    public int spawnAmount;

}
