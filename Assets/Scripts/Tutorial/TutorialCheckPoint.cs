using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TutorialCheckPoint : MonoBehaviour
{
    public string messageToShow;
    public string canvasAnimationTrigger="";
    private bool isVisited = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !isVisited)
        {
            TutorialCheckPointEngine.ActivateCheckPoint(messageToShow, canvasAnimationTrigger);
            isVisited = true;
        }
    }
}
