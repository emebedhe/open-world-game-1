using UnityEngine;

using TMPro;
public class npcscript : MonoBehaviour
{
    public GameObject text;


    void Start()
    {
        text.SetActive(false);
    }
    void Update()
    {
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
