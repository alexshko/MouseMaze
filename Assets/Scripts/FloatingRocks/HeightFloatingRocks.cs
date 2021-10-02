using System;
using UnityEngine;

namespace alexshko.colamazle.Entities.Rocks
{
    public class HeightFloatingRocks : FloatingRock
    {
        public Transform UpperLimit;
        public Transform LowerLimit;

        private bool isGoingUp;
        private Vector3 initPos;
        protected override void ApplyForce()
        {
            float t = InverseLerp(LowerLimit.position, UpperLimit.position, rb.position);
            if (t >= 1)
            {
                isGoingUp = false;
            }
            if (t <= 0)
            {
                isGoingUp = true;
            }
            float nextT = isGoingUp ? t + speedOfMove * Time.deltaTime : t- speedOfMove * Time.deltaTime;

            rb.MovePosition(Vector3.Lerp(LowerLimit.position,UpperLimit.position, nextT));
            //rb.AddForce(new Vector3(0, 1, 0), ForceMode.Force);
        }

        public override void Start()
        {
            base.Start();
            isGoingUp = true;
            initPos = transform.position;
        }

        //taken from https://answers.unity.com/questions/1271974/inverselerp-for-vector3.html
        private float InverseLerp(Vector3 a, Vector3 b, Vector3 value)
        {
            Vector3 AB = b - a;
            Vector3 AV = value - a;
            return Mathf.Clamp01(Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB));
        }
    }
}
