using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    public GameObject ButtonPrefab;
    public float distenceWithPlayer;
    public Vector3 buttonOffset;
    GameObject button;

    bool isTriggered = false;





    public void Update()
    {
        DistenceWithPlayer();
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            TriggerDialogue();
        }
    }

    public void FixedUpdate()
    {
        if (button != null)
        {
            button.transform.position = Camera.main.WorldToScreenPoint(transform.position + buttonOffset);
        }
    }

    public void DistenceWithPlayer()
    {
        if (!isTriggered)
        {
            float distance = Vector3.Distance(UI_Manager.instance.playerPosition, transform.position);
            if (distance < distenceWithPlayer && button == null)
            {
                button = Instantiate(ButtonPrefab, UI_Manager.instance.canvas.transform);
                button.transform.position = Camera.main.WorldToScreenPoint(transform.position + buttonOffset);
            }
            else if (distance > distenceWithPlayer && button != null)
            {
                Destroy(button);
            }
        }        
    }

    public void TriggerDialogue()
    {
        if (!isTriggered&& button != null)
        {
            DialogueManager.instance.StartDialogue(dialogue);
            isTriggered = true;
            Destroy(button);
        }
        else
        {
            DialogueManager.instance.DisplayNextSentence();
        }
    }

}
