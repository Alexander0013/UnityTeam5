using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    public Queue<string> sentences;

    public GameObject dialogueBox;
    CanvasGroup canvasGroup;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    public Animator animator;
    public Animator animator2;


    void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        instance = this;
    }

    void Start()
    {
        sentences = new Queue<string>();
        dialogueBox.SetActive(true);
        canvasGroup = dialogueBox.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
    }

    public void StartDialogue(Dialogue dialogue)
    {
        canvasGroup.alpha = 1;
        animator.SetBool("IsTalking", true);
        nameText.text = dialogue.name;

        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        Debug.Log("sentence = " + sentence);
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.05f); 
        }
    }

    public void EndDialogue()
    {
        animator.SetBool("IsTalking", false);
    }
}
