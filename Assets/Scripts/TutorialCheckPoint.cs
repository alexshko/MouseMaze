using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TutorialCheckPoint : MonoBehaviour
{
    public string messageToShow;
    private bool isVisited = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !isVisited)
        {
            TutorialCheckPointEngine.ShowMessageUI(messageToShow);
            isVisited = true;
        }
    }
}
