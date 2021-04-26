using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringCamRig : MonoBehaviour
{
	public Camera cameraTarget;
	public Transform target;
	public LayerMask checkLayerMask;
	public float maxCamSpeed = 1f;

	Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
		offset = transform.InverseTransformPoint(cameraTarget.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
		Vector3 realLocation = transform.TransformPoint(offset);
		Vector3 direction = realLocation - target.position;
		float distance = direction.magnitude;
		direction.Normalize();

		Ray ray = new Ray(target.position, direction);
		RaycastHit hit;

		Vector3 newLocation;

		if (Physics.Raycast(ray, out hit, distance, checkLayerMask))
		{
			newLocation = hit.point - direction * 0.5f;
		}
		else
		{
			newLocation = realLocation;
		}

		//cameraTarget.transform.position = Vector3.MoveTowards(cameraTarget.transform.position, newLocation, maxCamSpeed * Time.deltaTime);
		cameraTarget.transform.position = newLocation;
    }

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;

		Gizmos.DrawLine(target.position, cameraTarget.transform.position);
	}
}
