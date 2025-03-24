using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    public GameObject ButtonPrefab;
    public Transform buttonTransform;
    GameObject button;

    void Start()
    {
        UI_Manager.instance.startDialogue+= StartDialogue;
    }
    void OnDisable()
    {
        UI_Manager.instance.startDialogue -= StartDialogue;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")&&UI_Manager.instance.isTriggered==false)
        {
            GetDialogueButton();
            UI_Manager.instance.inDialogueRange = true;
            UI_Manager.instance.ShowInteractionText("«öE¹ï¸Ü");
            UI_Manager.instance.dialogue = dialogue;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(button);
            UI_Manager.instance.inDialogueRange = false;
            UI_Manager.instance.HideInteractionText();
        }
    }

    public void FixedUpdate()
    {
        if (button != null)
        {
            button.transform.position = Camera.main.WorldToScreenPoint(buttonTransform.position);
        }
    }


    void GetDialogueButton()
    {
        button = Instantiate(ButtonPrefab, UI_Manager.instance.canvas.transform);
        button.transform.position = Camera.main.WorldToScreenPoint(buttonTransform.position);
    }

    void StartDialogue()
    {
        Destroy(button);
    }

}
