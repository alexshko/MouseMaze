using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TutorialCheckPointEngine : MonoBehaviour
{
    public int waitTimeMillis;
    public GameObject UIMessageToShowRef;

    private Text txtRef;
    private Animator anim;
    //protected Task currMessage;
    //protected CancellationTokenSource tokenSource2;
    protected Coroutine curMessage;

    private static TutorialCheckPointEngine instance;

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
        anim = UIMessageToShowRef.GetComponent<Animator>();
    }

    public static void ShowMessageUI(string msg)
    {
        if (instance.curMessage != null)
        {
            instance.StopCoroutine(instance.curMessage);
        }
        instance.curMessage = instance.StartCoroutine(instance.ShowMessage(msg));
        
    }

    private IEnumerator ShowMessage(string msg)
    {
        UIMessageToShowRef.SetActive(true);
        txtRef.text = msg;
        anim.SetBool("isShowMessage", true);
        yield return new WaitForSeconds(waitTimeMillis /1000 / 2);
        anim.SetBool("isShowMessage", false);
        yield return new WaitForSeconds(waitTimeMillis / 1000 / 2);
        UIMessageToShowRef.gameObject.SetActive(false);
        yield return null;
    }
}
