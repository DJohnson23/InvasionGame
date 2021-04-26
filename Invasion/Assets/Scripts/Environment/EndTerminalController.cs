using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTerminalController : InteractableObject
{
    public override void InteractableUpdate()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            GameManager gm = FindObjectOfType<GameManager>();

            if(gm)
            {
                gm.LoadNextLevel();
            }
        }
    }

    public override void InteractableStart()
    {
        
    }

    public override void InteractableEnd()
    {

    }
}
