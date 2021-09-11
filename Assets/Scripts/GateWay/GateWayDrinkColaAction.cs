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

            MouseyRef.GetComponent<MouseyController>().ColaRef = ColaRef;
            MouseyRef.GetComponent<MouseyController>().MouseHandRef = MouseHandRef;

            Animator anim = MouseyRef.GetComponent<Animator>();
            if (anim)
            {
                anim.SetTrigger("PickUp");
            }
        }

        //private void GrabCola()
        //{
        //    ColaRef.parent = MouseHandRef;
        //    ColaRef.localPosition = Vector3.zero;
        //    ColaRef.localRotation = Quaternion.Euler(0, 0, 90);
        //}

        private async Task ShowCameraOnEffect()
        {
            GameController.Instance.acceptInputPlayer = false;
            int curPriority = cameraRef.Priority;
            cameraRef.Priority = int.MaxValue;
            await Task.Delay(waitTime);
            cameraRef.Priority = curPriority;
            await Task.Delay(1000);
            AsyncOperation ao = SceneManager.LoadSceneAsync(0);
            ao.allowSceneActivation = true;
            while (!ao.isDone)
            {
                await Task.Delay(10);
            }
        }
    }
}
