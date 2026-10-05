using UnityEngine;
using DialogueEditor;
public class ConvoStarter : MonoBehaviour
{
    // Conversation assigned through inspector
    [SerializeField] private NPCConversation JimConvo;

    //Originally had this as an OnTriggerStay, keep as Enter so it triggers when the player walks up instead of locking the player
    private void OnTriggerEnter(Collider other)
    {
        // Make sure the entering object is the player or anything tagged player
        if (other.CompareTag("Player"))
        {
                // Display dialogue for the character
                ConversationManager.Instance.StartConversation(JimConvo);
        }

 
    }

    // Runs whenever the player leaves the trigger collider
    private void OnTriggerExit(Collider other)
    {
        // Make sure the exiting object is the player or anything tagged player
        if (other.CompareTag("Player")) 
        {
            // End the dialogue on the screen
            ConversationManager.Instance.EndConversation();
        }
    }

}
