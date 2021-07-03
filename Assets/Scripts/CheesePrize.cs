using alexshko.colamazle.core;
using UnityEngine;

namespace alexshko.colamazle.Entities
{
    public class CheesePrize : MonoBehaviour
    {
        //add the cheese to the Prizes List when it is enabled.
        private void OnEnable()
        {
            GameController.Instance.CheeseList.Add(this.transform);
        }

        //remove the cheese from the Prizes List when it is disabled.
        private void OnDisable()
        {
            GameController.Instance.CheeseList.Remove(this.transform);
        }

        private void OnTriggerEnter(Collider other)
        {
            CollectCheese();
        }

        private void CollectCheese()
        {
            Debug.Log("CheeseCollected");
            this.enabled = false;
            Destroy(this.gameObject);
        }
    }
}
