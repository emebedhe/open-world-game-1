using UnityEngine;
using System.Collections.Generic;
using TMPro;
public class npcscript : MonoBehaviour
{
    public GameObject text;
    public List<string> Dialogues = new List<string>();
    public GameObject inDialogue;
    public GameObject dialogueCanvas;

    void Start()
    {
        Dialogues.Add("Hello there, traveler!");
        text.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && text.activeSelf)
        {
            Debug.Log("Entering Dialogue...");
            text.SetActive(false);
            dialogueCanvas.SetActive(true);
            inDialogue.SetActive(true);
            dialogueCanvas.GetComponentInChildren<TextMeshProUGUI>().text = Dialogues[0];
            Invoke("ExitDialogue", 3f);
        }
    }
    void ExitDialogue()
    {
        inDialogue.SetActive(false);
        dialogueCanvas.SetActive(false);
        Time.timeScale = 1f; 
        Debug.Log("Exiting Dialogue...");
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Player entered trigger zone.");
            text.SetActive(true);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Player exited trigger zone.");
            text.SetActive(false);
        }
    }
}
