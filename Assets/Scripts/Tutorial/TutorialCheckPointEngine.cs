using alexshko.colamazle.core;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace alexshko.colamazle.tutorial
{
    public class TutorialCheckPointEngine : MonoBehaviour
    {
        public float waitTimeSeconds = 0.67f;
        public GameObject UIMessageToShowRef;
        public GameObject CanvasObjectAnimations;

        private Text txtRef;
        private Animator messageAnim;
        private Animator tutorialAnim;
        private Coroutine curMessage;

        public static TutorialCheckPointEngine instance;

        // Start is called before the first frame update
        void Start()
        {
            instance = this;
            if (UIMessageToShowRef == null)
            {
                Debug.LogError("Missing reference to MessageUI");
            }

            txtRef = UIMessageToShowRef.GetComponentInChildren<Text>();
            if (txtRef == null)
            {
                Debug.LogError("No text component to show");
            }
            curMessage = null;
            messageAnim = UIMessageToShowRef.GetComponent<Animator>();
            tutorialAnim = CanvasObjectAnimations.GetComponent<Animator>();
            //tutorialAnim.enabled = true;
        }

        public void ActivateCheckPoint(TutorialStep[] steps, bool acceptInput)
        {
            GameController.Instance.acceptInputPlayer = acceptInput;

            if (curMessage != null)
            {
                StopCoroutine(curMessage);
            }
            curMessage = StartCoroutine(ShowMessages(steps));
        }

        private IEnumerator ShowMessages(TutorialStep[] steps)
        {
            UIMessageToShowRef.SetActive(true);
            tutorialAnim.enabled = true;

            foreach (TutorialStep step in steps)
            {
                //if there is animation in this step, the step wait time should include it:
                if (step.canvasAnimationTrigger!= "")
                {
                    tutorialAnim.SetTrigger(step.canvasAnimationTrigger);
                }

                //show message with animation. the animation takes waitTimeMillis milliseconds:
                txtRef.text = step.messageToShow;
                messageAnim.SetBool("isShowMessage", true);
                yield return new WaitForSeconds(waitTimeSeconds / 2);

                messageAnim.SetBool("isShowMessage", false);
                yield return new WaitForSeconds(waitTimeSeconds / 2);

                //if the animation is still playing, wait for it to finish.
                while (tutorialAnim.GetCurrentAnimatorStateInfo(0).IsName(step.canvasAnimationTrigger))
                {
                    Debug.Log("still playing the transition");
                    yield return new WaitForSeconds(0.1f);
                }

                //wait awhile between steps.
                yield return new WaitForSeconds(0.2f);
            }

            UIMessageToShowRef.gameObject.SetActive(false);
            //release controll at the end of coroutine, just in case it was taken.
            GameController.Instance.acceptInputPlayer = true;
            tutorialAnim.enabled = false;
            yield return null;
        }
    }
}
