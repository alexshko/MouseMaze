using System;
using UnityEngine;

namespace alexshko.colamazle.Entities
{
    public class GateWay : MonoBehaviour
    {
        public Transform regularlGroundRef;
        public Transform litGroundRef;
        public bool isActive { get; set; }

        private void Awake()
        {
            DeactivateGateway();
        }

        //need to change this so it will be more generic:
        public Action<Transform> actionToPerformWhenStepped;
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Open Gate");
            if (actionToPerformWhenStepped != null &&isActive)
            {
                actionToPerformWhenStepped(other.transform);
            }
        }
        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log("Open Gate by collision");
            if (actionToPerformWhenStepped != null && isActive)
            {
                actionToPerformWhenStepped(collision.transform);
            }
        }

        public void ActivateGateway()
        {
            this.GetComponent<Renderer>().material.SetFloat(Shader.PropertyToID("Alpha"), 0.4f);
            this.GetComponent<Renderer>().material.SetFloat(Shader.PropertyToID("IsActive"), 1);
            this.GetComponent<MeshRenderer>().enabled = true;
            SetGroundOfPortalToActive();

            isActive = true;
            Debug.Log("Activate Gateway");
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
}
