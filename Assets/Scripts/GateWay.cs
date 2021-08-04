using System;
using UnityEngine;

namespace alexshko.colamazle.Entities
{
    public class GateWay : MonoBehaviour
    {
        //public Transform AffectedGameObject;
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
            foreach (var item in this.GetComponentsInChildren<Transform>())
            {
                if (item.gameObject.name == "GateWayGround")
                {
                    item.GetComponent<Renderer>().material.SetFloat(Shader.PropertyToID("Alpha"), 0.4f);
                    item.GetComponent<Renderer>().material.SetFloat(Shader.PropertyToID("IsActive"), 1);
                    break;
                }
            }
            this.GetComponent<MeshRenderer>().enabled = true;
            isActive = true;
            Debug.Log("Activate Gateway");
        }
        public void DeactivateGateway()
        {
            this.GetComponent<Renderer>().material.SetFloat(Shader.PropertyToID("Alpha"), 0);
            this.GetComponent<Renderer>().material.SetFloat(Shader.PropertyToID("IsActive"), 0);
            foreach (var item in this.GetComponentsInChildren<Transform>())
            {
                if (item.gameObject.name == "GateWayGround")
                {
                    item.GetComponent<Renderer>().material.SetFloat(Shader.PropertyToID("Alpha"), 1);
                    item.GetComponent<Renderer>().material.SetFloat(Shader.PropertyToID("IsActive"), 0);
                    break;
                }
            }
            this.GetComponent<MeshRenderer>().enabled = false;
            isActive = false;
            Debug.Log("Deactivate Gateway");
        }
    }
}
