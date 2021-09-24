using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace alexshko.colamazle.tutorial
{
    public class TutorialCheckPointEngine : MonoBehaviour
    {
        public int waitTimeMillis;
        public GameObject UIMessageToShowRef;
        public GameObject CanvasObjectAnimations;

        private Text txtRef;
        private Animator messageAnim;
        private Animator tutorialAnim;
        private Coroutine curMessage;

        public static TutorialCheckPointEngine instance;

        private void Awake()
        {
            instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
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
        }

        public void ActivateCheckPoint(string msg, string animationTrig = "")
        {
            if (curMessage != null)
            {
                StopCoroutine(curMessage);
            }
            curMessage = StartCoroutine(ShowMessage(msg));

            if (animationTrig != "")
            {
                tutorialAnim.SetTrigger(animationTrig);
            }
        }

        private IEnumerator ShowMessage(string msg)
        {
            UIMessageToShowRef.SetActive(true);
            txtRef.text = msg;
            messageAnim.SetBool("isShowMessage", true);
            yield return new WaitForSeconds(waitTimeMillis / 1000 / 2);
            messageAnim.SetBool("isShowMessage", false);
            yield return new WaitForSeconds(waitTimeMillis / 1000 / 2);
            UIMessageToShowRef.gameObject.SetActive(false);
            yield return null;
        }
    }
}
