using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TutorialCheckPoint : MonoBehaviour
{
    public string messageToShow;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            TutorialCheckPointEngine.ShowMessageUI(messageToShow);
        }
    }
}
