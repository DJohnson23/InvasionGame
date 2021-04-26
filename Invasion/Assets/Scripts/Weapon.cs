using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float range = 10;
    public Transform barrelLocation;
    public float damage = 5;
    public GameObject muzzleFlash;
    public float attackSpeed = 2;

    float cooldownTime = 0;

    Coroutine currentCooldown;

    public bool canShoot { get; private set; }

    private void Start()
    {
        canShoot = true;
        cooldownTime = 1 / attackSpeed;
    }

    public void Shoot()
    {
        if(currentCooldown != null)
        {
            StopCoroutine(currentCooldown);
        }

        GameObject newObj = Instantiate(muzzleFlash, barrelLocation.position, barrelLocation.rotation, barrelLocation);
        Destroy(newObj, 2);

        currentCooldown = StartCoroutine(WeaponCooldown());
    }

    IEnumerator WeaponCooldown()
    {
        canShoot = false;

        yield return new WaitForSeconds(cooldownTime);

        canShoot = true;
    }
}
