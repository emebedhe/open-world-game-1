using UnityEngine;
using System.Collections.Generic;
using TMPro;
public class npcscript : MonoBehaviour
{
    public GameObject text;
    public List<string> Dialogues = new List<string>();
    public GameObject inDialogue;
    public GameObject dialogueCanvas;
    private int dialogueIndex = 0;
    private bool nextDialogue = false;
    private bool insideTrigger = false;
    void Start()
    {
        Dialogues.Add(".  ");
        Dialogues.Add("Hello there, traveler!");
        Dialogues.Add("The world is vast and full of wonders.");
        Dialogues.Add("Beware of the dangers that lurk in the shadows.");
        Dialogues.Add("May your journey be safe and prosperous.");
        text.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && insideTrigger)
        {
            if (inDialogue.activeSelf == false)
            {
                inDialogue.SetActive(true);
                dialogueCanvas.SetActive(true);
                dialogueCanvas.GetComponentInChildren<TextMeshProUGUI>().text = Dialogues[dialogueIndex];
                text.SetActive(false);
                nextDialogue = false;
            }
            else if (dialogueIndex == Dialogues.Count - 1)
            {
                dialogueCanvas.SetActive(false);
                inDialogue.SetActive(false);
                dialogueIndex = 0;
                nextDialogue = false;
                text.SetActive(true);
                inDialogue.SetActive(false);
            }
            else
            {
                Debug.Log(dialogueIndex + " " + Dialogues.Count);
                Debug.Log(Dialogues[dialogueIndex]);
                dialogueIndex++;
                dialogueCanvas.GetComponentInChildren<TextMeshProUGUI>().text = Dialogues[dialogueIndex];
                nextDialogue = false;
            }
            // Debug.Log(dialogueIndex);
            // if (nextDialogue)
            // {
            //     if (dialogueIndex <= Dialogues.Count - 1 && dialogueIndex > 0)
            //     {
            //         dialogueIndex++;
            //         dialogueCanvas.GetComponentInChildren<TextMeshProUGUI>().text = Dialogues[dialogueIndex];
            //         nextDialogue = false;
            //         Invoke("NextDialogue", 3f);
            //     }
            //     else if (dialogueIndex == Dialogues.Count)
            //     {
            //         dialogueCanvas.SetActive(false);
            //         inDialogue.SetActive(false);
            //         dialogueIndex = 0;
            //         nextDialogue = false;
            //         text.SetActive(true);
            //         inDialogue.SetActive(false);
            //     }
            //     else
            //     {
            //         dialogueIndex = 1;
            //         nextDialogue = false;
            //         text.SetActive(false);
            //         dialogueCanvas.SetActive(true);
            //         inDialogue.SetActive(true);
            //     }
            // }
            Invoke("NextDialogue", 3f);
        }
    }
    void NextDialogue()
    {
        nextDialogue = true;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Player entered trigger zone.");
            text.SetActive(true);
            insideTrigger = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Player exited trigger zone.");
            text.SetActive(false);
            insideTrigger = false;
        }
    }
}
