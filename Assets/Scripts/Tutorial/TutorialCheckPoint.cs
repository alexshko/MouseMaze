using UnityEngine;
namespace alexshko.colamazle.tutorial
{
    [System.Serializable]
    public struct TutorialStep
    {
        public string messageToShow;
        public string canvasAnimationTrigger;
        //public int millisToStep;
    }
    public class TutorialCheckPoint : MonoBehaviour
    {
        public TutorialStep[] steps;
        public bool acceptInputFromPlayer = true;

        private bool isVisited = false;

        private static TutorialCheckPointEngine tutorial
        {
            get => TutorialCheckPointEngine.instance;
        }

        private void Start()
        {
            if (tutorial == null)
            {
                Debug.Log("Missing Tutorial module. will not show tutorial");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player" && !isVisited && tutorial!=null)
            {
                tutorial.ActivateCheckPoint(steps, acceptInputFromPlayer);
                isVisited = true;
            }
        }
    }
}
