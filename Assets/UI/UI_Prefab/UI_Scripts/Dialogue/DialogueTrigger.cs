using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    public Dialogue dialogue2;
    public Dialogue dialogue3;
    public GameObject ButtonPrefab;
    public Transform buttonTransform;
    public ItemGiver itemGivers;
    GameObject button;

    //Misson
    int currentItemCount;

    void Start()
    {
        UI_Manager.instance.startDialogue+= StartDialogue;
        currentItemCount = InventoryManager.instance.GetItemAmount(Item.ItemType.Other);
    }
    void OnDisable()
    {
        UI_Manager.instance.startDialogue -= StartDialogue;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GetDialogueButton();
            UI_Manager.instance.ShowInteractionText("«öE¹ï¸Ü");
            UI_Manager.instance.inDialogueRange = true;
            UI_Manager.instance.npcDT = this;
            if (!UI_Manager.instance.getMission)
            {
                UI_Manager.instance.dialogue = dialogue;
            }
            else
            {
                if (currentItemCount == 5)
                {
                    UI_Manager.instance.dialogue = dialogue2;
                    UI_Manager.instance.missionDone = true;
                }
                else
                {
                    UI_Manager.instance.dialogue = dialogue3;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(button);
            UI_Manager.instance.inDialogueRange = false;
            UI_Manager.instance.dialogue = null;
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
        if(button != null)
            Destroy(button);
    }

    public void GetItemFromNPC()
    {
        itemGivers.AddNewItems();
    }
}
