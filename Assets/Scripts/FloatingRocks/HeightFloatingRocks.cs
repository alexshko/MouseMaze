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
        private Vector3 curTarget;
        protected override void ApplyForce()
        {
            float t = InverseLerp(LowerLimit.position, UpperLimit.position, rb.position);
            float totDist = Vector3.Distance(LowerLimit.position, UpperLimit.position);
            float portion = (curTarget == UpperLimit.position? t / totDist : (1 - t / totDist));
            if (t >= 0.9)
            {
                curTarget = LowerLimit.position;
            }
            if (t <= 0.1)
            {
                curTarget = UpperLimit.position;
            }
            //float nextT = isGoingUp ? t + speedOfMove * Time.deltaTime : t- speedOfMove * Time.deltaTime;

            rb.MovePosition(Vector3.Lerp(rb.position, curTarget, speedOfMove * portion * Time.deltaTime));
            //rb.AddForce(new Vector3(0, 1, 0), ForceMode.Force);
        }

        public override void Start()
        {
            base.Start();
            isGoingUp = true;
            initPos = transform.position;
            curTarget = UpperLimit.position;
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
