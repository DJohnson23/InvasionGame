using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldItem : MonoBehaviour
{
    public int amount = 20;
    public AudioClip collectSound;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            GameManager gameManager = FindObjectOfType<GameManager>();

            if(gameManager.AddShield(amount))
            {
                AudioSource.PlayClipAtPoint(collectSound, transform.position);

                Destroy(gameObject);
            }
        }
    }
}
