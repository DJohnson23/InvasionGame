using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LookAtCamera : MonoBehaviour
{
    public bool backwards = false;

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = (Camera.main.transform.position - transform.position).normalized;
        
        if(backwards)
        {
            transform.LookAt(transform.position - direction);
        }
        else
        {
            transform.LookAt(transform.position + direction);
        }
    }
}
