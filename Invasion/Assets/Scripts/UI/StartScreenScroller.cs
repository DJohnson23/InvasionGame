using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScreenScroller : MonoBehaviour
{
    public MapGenerator initObject;
    public float speed = 3f;
    public float zSize = 80;
    public float viewSize = 100;

    List<MapGenerator> scrollObjects = new List<MapGenerator>();

    // Update is called once per frame
    void Update()
    {
        MoveObjects();
        RefreshObjects();
    }

    void MoveObjects()
    {
        foreach(MapGenerator mapGen in scrollObjects)
        {
            mapGen.transform.position -= new Vector3(0, 0, speed * Time.deltaTime);
        }
    }

    void RefreshObjects()
    {
        float curViewable = 0;
        Vector3 lastPos = new Vector3(0, 0, -zSize);

        if(scrollObjects.Count > 0)
        {
            lastPos = scrollObjects[scrollObjects.Count - 1].transform.position;
            curViewable = lastPos.z + zSize;
        }

        while(curViewable < viewSize)
        {
            GameObject newObj = Instantiate(initObject.gameObject, Vector3.zero, Quaternion.identity, transform);
            newObj.transform.position =lastPos + new Vector3(0, 0, zSize);
            MapGenerator newMapGen = newObj.GetComponent<MapGenerator>();
            scrollObjects.Add(newMapGen);
            newMapGen.randomSeed = Random.Range(1, 9999);
            curViewable += zSize;
            lastPos = newObj.transform.position;
        }

        for(int i = 0; i < scrollObjects.Count; i++)
        {
            MapGenerator mapGen = scrollObjects[i];

            if (mapGen.transform.position.z < -viewSize)
            {
                Destroy(mapGen.gameObject);
                scrollObjects.RemoveAt(i);
                i--;
            }
        }
    }
}
