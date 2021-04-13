using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
	public Vector3 angularVelocity;
	public bool isGlobal;

    // Update is called once per frame
    void Update()
    {
        if(isGlobal)
		{
			transform.rotation = Quaternion.Euler(transform.eulerAngles + angularVelocity * Time.deltaTime);
		}
		else
		{
			transform.localRotation = Quaternion.Euler(transform.localEulerAngles + angularVelocity * Time.deltaTime);
		}
    }
}
