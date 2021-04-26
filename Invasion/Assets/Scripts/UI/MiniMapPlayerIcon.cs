using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MiniMapPlayerIcon : MonoBehaviour
{
    GameObject player;

    // Update is called once per frame
    void Update()
    {
        if(!player)
        {
            player = GameObject.FindGameObjectWithTag("Player");

            if(!player)
            {
                return;
            }
        }

        transform.localRotation = Quaternion.Euler(0, 0, -player.transform.eulerAngles.y);
    }
}
