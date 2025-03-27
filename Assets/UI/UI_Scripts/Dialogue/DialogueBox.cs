using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueBox : MonoBehaviour
{
    // Start is called before the first frame update
    public void OnDialogueStartComplete()
    {
        Debug.Log("OnDialogueStartComplete");
        UI_Manager.instance.UpdateGameStateForUI(true);

    }

    public void OnDialogueEndComplete()
    {
        Debug.Log("OnDialogueEndComplete");
        UI_Manager.instance.UpdateGameStateForUI(false);
    }
}
