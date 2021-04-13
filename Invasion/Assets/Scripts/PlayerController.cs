using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
	public float speed = 2;
	public float smoothRotSpeed = 100;
	public float mouseSensitivity = 100;
	public Transform camRig;
	public Transform character;
	public float maxCamRot = 30f;
	public Animator playerAnimator;

	CharacterController charController;
	float xRotation = 0;
	float yRotation = 0;

	Vector3 velocity = new Vector3();

    // Start is called before the first frame update
    void Start()
    {
		Cursor.lockState = CursorLockMode.Locked;
		charController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
		HandleMovement();
		HandleLook();

		HandlePhysics();
    }

	Vector3 FlatDirection(Vector3 dir)
	{
		return Vector3.Scale(dir, new Vector3(1, 0, 1)).normalized;
	}

	void HandleMovement()
	{
		float xMovement = Input.GetAxis("Horizontal");
		float zMovement = Input.GetAxis("Vertical");

		Vector3 forward = FlatDirection(camRig.forward);
		Vector3 right = FlatDirection(camRig.right);

		Vector3 flatVelocity = (xMovement * right + zMovement * forward).normalized * speed;

		if(xMovement != 0 || zMovement != 0)
		{
			character.rotation = Quaternion.RotateTowards(character.rotation, Quaternion.LookRotation(flatVelocity, character.up), smoothRotSpeed * Time.deltaTime);
			//character.LookAt(character.position + flatVelocity);
			playerAnimator.SetBool("walk", true);
		}
		else
		{
			playerAnimator.SetBool("walk", false);
		}

		velocity = new Vector3(flatVelocity.x, velocity.y, flatVelocity.z);
	}

	void HandleLook()
	{
		yRotation += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
		xRotation += -Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

		xRotation = Mathf.Clamp(xRotation, -maxCamRot, maxCamRot);
		camRig.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
	}

	void HandlePhysics()
	{
		charController.Move(velocity * Time.deltaTime);
	}
}
