using UnityEngine;

namespace alexshko.colamazle.Entities.Rocks
{
    public abstract class FloatingRock : MonoBehaviour
    {
        public float speedOfMove;
        public bool isMooving;

        protected Rigidbody rb;


        protected abstract void ApplyForce();

        public virtual void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        //public void tes();
        public void FixedUpdate()
        {
            if (isMooving)
            {
                ApplyForce();
            }
        }

    }
}
