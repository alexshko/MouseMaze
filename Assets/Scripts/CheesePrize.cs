using alexshko.colamazle.core;
using UnityEngine;
using UnityEngine.VFX;

namespace alexshko.colamazle.Entities
{
    public class CheesePrize : MonoBehaviour
    {
        public Transform CheeseCollectEffectPref;

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
            Transform newEffect = Instantiate(CheeseCollectEffectPref, this.transform.position, CheeseCollectEffectPref.rotation);
            newEffect.GetComponent<VisualEffect>().Play();
            Destroy(newEffect.gameObject, 3);
            Destroy(this.gameObject);
        }
    }
}
