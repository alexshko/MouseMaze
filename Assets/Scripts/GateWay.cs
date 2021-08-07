using System;
using UnityEngine;
using Cinemachine;
using System.Threading.Tasks;

namespace alexshko.colamazle.Entities
{
    public class GateWay : MonoBehaviour
    {
        public Transform regularlGroundRef;
        public Transform litGroundRef;

        public CinemachineVirtualCamera cameraRef;
        public int waitTime = 1500;
        public bool ActiveInIntitalState = false;
        public bool isActive { get; set; }

        private void Awake()
        {
            if (ActiveInIntitalState)
            {
                ActivateGateway();
            }
            else
            {
                DeactivateGateway();
            }
        }

        //need to change this so it will be more generic:
        public Action<Transform> actionToPerformWhenStepped;
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Open Gate");
            //if (actionToPerformWhenStepped != null &&isActive)
            //{
            //    actionToPerformWhenStepped(other.transform);
            //}
            Debug.Log("trigger is active");
            if (isActive)
            {
                GateWayAction gateAction = GetComponent<GateWayAction>();
                if (gateAction != null)
                {
                    gateAction.makeAction();
                }
            }
        }
        private void OnCollisionEnter(Collision collision)
        {
            //Debug.Log("Open Gate by collision");
            //if (actionToPerformWhenStepped != null && isActive)
            //{
            //    actionToPerformWhenStepped(collision.transform);
            //}
            if (isActive)
            {
                GateWayAction gateAction = GetComponent<GateWayAction>();
                if (gateAction != null)
                {
                    gateAction.makeAction();
                }
            }
        }

        public void ActivateGateway()
        {
            this.GetComponent<Renderer>().material.SetFloat(Shader.PropertyToID("Alpha"), 0.4f);
            this.GetComponent<Renderer>().material.SetFloat(Shader.PropertyToID("IsActive"), 1);
            this.GetComponent<MeshRenderer>().enabled = true;
            SetGroundOfPortalToActive();
            if (cameraRef)
            {
                ShowCameraOnEffect().ConfigureAwait(false);
            }

            isActive = true;
            Debug.Log("Activate Gateway");
        }

        private async Task ShowCameraOnEffect()
        {
            int curPriority = cameraRef.Priority;
            cameraRef.Priority = int.MaxValue;
            await Task.Delay(waitTime);
            cameraRef.Priority = curPriority;
        }

        public void DeactivateGateway()
        {
            this.GetComponent<Renderer>().material.SetFloat("Alpha", 0);
            this.GetComponent<Renderer>().material.SetFloat("IsActive", 0);
            this.GetComponent<MeshRenderer>().enabled = false;
            SetGroundOfPortalToActive(false);

            isActive = false;
            Debug.Log("Deactivate Gateway");
        }
        private void SetGroundOfPortalToActive(bool active = true)
        {
            litGroundRef.gameObject.SetActive(active);
            regularlGroundRef.gameObject.SetActive(!active);
        }
    }

    public interface GateWayAction
    {
        void makeAction();
    }
}
