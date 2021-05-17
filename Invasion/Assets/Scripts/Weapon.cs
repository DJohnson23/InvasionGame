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
    public float reloadTime = 1;
    public int maxAmmo = 6;
    public AudioClip shootSound;
    public AudioClip reloadSound;

    public int ammo { get; private set; }
    float cooldownTime = 0;

    Coroutine currentCooldown;
    Coroutine currentReload;

    public bool canShoot { get; private set; }
    public bool reloading { get; private set; }

    AudioSource audio;

    private void Start()
    {
        audio = GetComponent<AudioSource>();
        canShoot = true;
        cooldownTime = 1 / attackSpeed;
        ammo = maxAmmo;
    }

    public void Shoot()
    {
        GameObject newObj = Instantiate(muzzleFlash, barrelLocation.position, barrelLocation.rotation, barrelLocation);
        Destroy(newObj, 2);

        audio.PlayOneShot(shootSound);

        ammo -= 1;

        if(ammo == 0)
        {
            Reload();
        }
        else
        {
            if (currentCooldown != null)
            {
                StopCoroutine(currentCooldown);
            }

            currentCooldown = StartCoroutine(WeaponCooldown());
        }
    }

    public void Reload()
    {
        if(ammo == maxAmmo)
        {
            return;
        }

        if (currentReload != null)
        {
            StopCoroutine(currentReload);
        }

        audio.PlayOneShot(reloadSound);

        currentReload = StartCoroutine(WeaponReload());
    }

    public void CancelReload()
    {
        if (currentReload != null)
        {
            StopCoroutine(currentReload);
        }
            reloading = false;
    }

    IEnumerator WeaponCooldown()
    {
        canShoot = false;

        yield return new WaitForSeconds(cooldownTime);

        canShoot = true;
    }

    IEnumerator WeaponReload()
    {
        SendMessageUpwards("OnReloadStart");

        reloading = true;

        yield return new WaitForSeconds(reloadTime);

        ammo = maxAmmo;

        reloading = false;


        SendMessageUpwards("OnReloadComplete");
    }
}
