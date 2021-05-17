using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTerminalController : InteractableObject
{
    public GameObject nextLevelUI;
    public GameObject notCompleteUI;

    public override void InteractableUpdate()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            GameManager gm = GameManager.instance;

            if(GameManager.enemiesLeft == 0)
            {
                gm.LoadNextLevel();
            }
        }
    }

    public override void InteractableStart()
    {
        bool complete = GameManager.enemiesLeft == 0;

        nextLevelUI.SetActive(complete);
        notCompleteUI.SetActive(!complete);
    }

    public override void InteractableEnd()
    {

    }
}
