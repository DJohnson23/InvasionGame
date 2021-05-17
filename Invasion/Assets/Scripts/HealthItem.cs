using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HealthItem : MonoBehaviour
{
    public int amount;
    public AudioClip collectSound;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            GameManager gameManager = FindObjectOfType<GameManager>();

            if(gameManager.HealPlayer(amount))
            {
                AudioSource.PlayClipAtPoint(collectSound, transform.position);

                Destroy(gameObject);
            }
        }
    }
}
