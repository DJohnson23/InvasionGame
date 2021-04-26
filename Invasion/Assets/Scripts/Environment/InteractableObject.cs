using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    public GameObject interactionUI;

    bool playerInRange = false;

    // Start is called before the first frame update
    void Start()
    {
        if(interactionUI)
        {
            interactionUI.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            interactionUI.SetActive(true);
            playerInRange = true;
            InteractableStart();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            interactionUI.SetActive(false);
            playerInRange = false;
            InteractableEnd();
        }
    }

    private void Update()
    {
        if(playerInRange)
        {
            InteractableUpdate();
        }
    }

    public abstract void InteractableUpdate();
    public abstract void InteractableStart();
    public abstract void InteractableEnd();
}
