using UnityEngine;
namespace alexshko.colamazle.tutorial
{
    public class TutorialCheckPoint : MonoBehaviour
    {
        public string messageToShow;
        public string canvasAnimationTrigger = "";
        private bool isVisited = false;

        private static TutorialCheckPointEngine tutorial
        {
            get => TutorialCheckPointEngine.instance;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player" && !isVisited)
            {
                tutorial.ActivateCheckPoint(messageToShow, canvasAnimationTrigger);
                isVisited = true;
            }
        }
    }
}
