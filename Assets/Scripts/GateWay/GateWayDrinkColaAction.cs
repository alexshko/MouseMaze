using alexshko.colamazle.core;
using alexshko.colamazle.Entities;
using Cinemachine;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace alexshko.colamazle.Entities
{
    public class GateWayDrinkColaAction : MonoBehaviour, GateWayAction
    {
        public Transform ColaRef;
        public Transform MouseyRef;
        public Transform MouseHandRef;
        public CinemachineVirtualCamera cameraRef;
        public int waitTime = 1500;
        public void makeAction()
        {
            Debug.Log("make acrion gateway");
            if (cameraRef)
            {
                ShowCameraOnEffect().ConfigureAwait(false);
            }
            MakeAnimation().ConfigureAwait(true);
        }

        private async Task MakeAnimation()
        {
            await Task.Delay(1000);

            Animator anim = MouseyRef.GetComponentInChildren<Animator>();
            if (anim)
            {
                anim.SetTrigger("PickUp");
            }
            //wait a second for the animation to reach the point of straight hand:
            await Task.Delay(1000*(1+4/60));
            MouseyRef.GetComponent<MouseyController>().ColaRef = ColaRef;
            MouseyRef.GetComponent<MouseyController>().MouseHandRef = MouseHandRef;
            MouseyRef.GetComponent<MouseyController>().GrabCola();
            await Task.Delay(300);
        }

        private async Task ShowCameraOnEffect()
        {
            GameController.Instance.acceptInputPlayer = false;
            int curPriority = cameraRef.Priority;
            cameraRef.Priority = int.MaxValue;
            await Task.Delay(waitTime);
            cameraRef.Priority = curPriority;
            await Task.Delay(1000);
            //make adjustment:
            AsyncOperation ao = SceneManager.LoadSceneAsync(0);
            ao.allowSceneActivation = true;
            while (!ao.isDone)
            {
                await Task.Delay(10);
            }
        }
    }
}
