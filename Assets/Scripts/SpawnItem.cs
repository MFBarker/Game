using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using Random = UnityEngine.Random;



public class SpawnItem : MonoBehaviour
{
    [SerializeField] List<GameObject> itemPrefabList;

    [SerializeField] List<Transform> spawnpointList;

    // Start is called before the first frame update
    void Start()
    {
        PlaceItem();
    }

    /// <summary>
    /// Loops through all spawn points, instantiating a random item at that spawn point
    /// </summary>
    public void PlaceItem()
    {
        //loop through all spawn points
        foreach (var item in spawnpointList) 
        {
            //choose an item and spawn it at the coords
            int ranItemIndex = Random.Range(0, itemPrefabList.Count - 1);
            GameObject.Instantiate(itemPrefabList[ranItemIndex],item);
        }
    }
}
