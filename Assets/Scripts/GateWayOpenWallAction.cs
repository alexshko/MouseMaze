using UnityEngine;
using alexshko.colamazle.Entities;
using Cinemachine;
using System.Threading.Tasks;

public class GateWayOpenWallAction : MonoBehaviour, GateWayAction
{
    public Transform wallRef;
    public CinemachineVirtualCamera cameraRef;
    public int waitTime = 1500;
    public void makeAction()
    {
        if (cameraRef)
        {
            ShowCameraOnEffect().ConfigureAwait(false);
        }
        MakeAnimation().ConfigureAwait(true);
    }

    private async Task MakeAnimation()
    {
        await Task.Delay(1000);
        Animator anim = wallRef.GetComponent<Animator>();
        if (anim)
        {
            anim.SetTrigger("makeAnim");
        }
    }

    private async Task ShowCameraOnEffect()
    {
        int curPriority = cameraRef.Priority;
        cameraRef.Priority = int.MaxValue;
        await Task.Delay(waitTime);
        cameraRef.Priority = curPriority;
    }
}
