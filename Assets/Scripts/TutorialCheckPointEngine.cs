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
    protected Task currMessage;
    protected CancellationTokenSource tokenSource2;

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
        currMessage = null;
        tokenSource2 = new CancellationTokenSource();
        anim = UIMessageToShowRef.GetComponent<Animator>();
    }

    public static void ShowMessageUI(string msg)
    {
        if (instance.currMessage != null)
        {
            instance.tokenSource2.Cancel();
        }
        instance.currMessage = Task.Run(async () => await instance.ShowMessage(msg), instance.tokenSource2.Token);
        instance.currMessage.ConfigureAwait(true);
    }

    private async Task ShowMessage(string msg)
    {
        UIMessageToShowRef.SetActive(true);
        txtRef.text = msg;
        anim.SetBool("isShowMessage", true);
        await Task.Delay(waitTimeMillis / 2);
        anim.SetBool("isShowMessage", false);
        await Task.Delay(waitTimeMillis / 2);
        UIMessageToShowRef.gameObject.SetActive(false);
    }
}
