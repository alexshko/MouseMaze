using System;
using UnityEngine;

namespace alexshko.colamazle.Entities
{
    public class CheesePrize : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            CollectCheese();
            Debug.Log("CheeseCollected");
        }

        private void CollectCheese()
        {
            throw new NotImplementedException();
        }
    }
}
