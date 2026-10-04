using UnityEngine;
using DialogueEditor;
public class ConvoStarter : MonoBehaviour
{
    [SerializeField] private NPCConversation JimConvo;

    //Originally had this as an OnTriggerStay, keep as Enter so it triggers when the player walks up instead of locking the player
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
           
                ConversationManager.Instance.StartConversation(JimConvo);
            

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            ConversationManager.Instance.EndConversation();
        }
    }

}
