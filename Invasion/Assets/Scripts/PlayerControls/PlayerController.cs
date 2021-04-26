using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : Damageable
{
	public float speed = 2;
	public float smoothRotSpeed = 100;
	public float mouseSensitivity = 1;
	public float maxCamRot = 30f;
	public Camera playerCamera;
	public Weapon currentWeapon;
	public LayerMask shootMask;
	public GameObject sparkParticle;

	CharacterController charController;
	float xRotation = 0;
	float yRotation = 0;

	Vector3 velocity = new Vector3();

	bool isDead = false;

	// Start is called before the first frame update
	void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
		charController = GetComponent<CharacterController>();
		FindObjectOfType<GameManager>().SetGameUIActive(true);
		yRotation = transform.eulerAngles.y;
	}

	// Update is called once per frame
	void Update()
	{
		if(isDead || GameManager.paused)
		{
			return;
		}

		HandleMovement();
		HandleLook();
		HandleWeapon();

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
		Transform camTransform = playerCamera.transform;

		Vector3 forward = FlatDirection(camTransform.forward);
		Vector3 right = FlatDirection(camTransform.right);

		Vector3 flatVelocity = (xMovement * right + zMovement * forward).normalized * speed;

		velocity = new Vector3(flatVelocity.x, velocity.y, flatVelocity.z);
	}

	void HandleLook()
	{
		yRotation += Input.GetAxis("Mouse X") * mouseSensitivity * 100 * Time.deltaTime;
		xRotation += -Input.GetAxis("Mouse Y") * mouseSensitivity * 100 * Time.deltaTime;

		xRotation = Mathf.Clamp(xRotation, -maxCamRot, maxCamRot);
		playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
		transform.rotation = Quaternion.Euler(0, yRotation, 0);
	}

	void HandleWeapon()
	{

		if(Input.GetButtonDown("Fire1") && currentWeapon.canShoot)
		{
			RaycastHit hit;
			Vector3 direction = playerCamera.transform.forward;

			if (Physics.Raycast(playerCamera.transform.position, direction, out hit, currentWeapon.range, shootMask))
			{
				Damageable hitObj = hit.collider.GetComponent<Damageable>();

				if(hitObj != null)
				{
					hitObj.TakeDamage(currentWeapon.damage, hit.point, playerCamera.transform.forward);
				}
				else {

					Vector3 reflection = Vector3.Reflect(direction, hit.normal);
					GameObject newSpark = Instantiate(sparkParticle, hit.point, Quaternion.LookRotation(reflection, Vector3.up));
					Destroy(newSpark, 2);
				}
			}

			currentWeapon.Shoot();
		}
	}

	void HandlePhysics()
	{
		charController.Move(velocity * Time.deltaTime);
	}

	public override void TakeDamage(float damage, Vector3 hitPosition, Vector3 hitDirection)
	{
		TakeDamage(damage);
	}

	public override void TakeDamage(float damage)
	{
		if(isDead)
		{
			return;
		}

		GameManager gameManager = FindObjectOfType<GameManager>();
		gameManager.DamagePlayer(damage);

		Health health = gameManager.GetComponent<Health>();
		isDead = health.isDead;
	}
}
