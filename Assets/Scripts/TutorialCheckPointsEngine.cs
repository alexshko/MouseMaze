using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TutorialCheckPointsEngine : MonoBehaviour
{
    public int waitTimeMillis;
    public RectTransform UIMessageToShowRef;
    public string messageToShow;

    private Text txtRef;

    //private static TutorialCheckPointsEngine instance;
    //public TutorialCheckPointsEngine Instance
    //{
    //    get
    //    {
    //        if (!instance)
    //        {
    //            instance = this;
    //        }
    //        return instance;
    //    }
    //}

    //private string msg="";

    //public string Message {
    //    get => msg;
    //    set
    //    {
    //        msg = value;
    //        txtRef.text = value;
    //    } 
    //}


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
    }

    public void ShowMessageUI()
    {
        ShowMessage().ConfigureAwait(true);
    }

    private async Task ShowMessage()
    {
        txtRef.text = messageToShow;

        UIMessageToShowRef.gameObject.SetActive(true);
        UIMessageToShowRef.GetComponent<Animator>().SetBool("isShowMessage", true);
        await Task.Delay(waitTimeMillis/2);
        UIMessageToShowRef.GetComponent<Animator>().SetBool("isShowMessage", false);
        await Task.Delay(waitTimeMillis / 2);
        UIMessageToShowRef.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            ShowMessageUI();
        }
    }
}
