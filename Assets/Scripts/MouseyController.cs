using alexshko.colamazle.core;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

namespace alexshko.colamazle.Entities
{
    public class MouseyController : MonoBehaviour
    {
        public float MaxForwardSpeed = 0f;
        public float MaxBackwardSpeed = 0f;


        private CharacterController character;
        private Vector3 MoveToMake;

        private void Start()
        {
            character = GetComponent<CharacterController>();
            if (!character)
            {
                Debug.LogError("missing CharacterController");
            }

        }

        private void Update()
        {
            MoveToMake = Vector3.zero;
            //take the Vertical input axis. and also the vertical valuue of the joystick:
            float InputVal = Input.GetAxis("Vertical") + GameController.Instance.JoystickValue;
            if (Mathf.Abs(InputVal) > 0.05f)
            {
                Debug.Log("Going forward");
                MoveToMake.z += (InputVal > 0 ? MaxForwardSpeed : MaxBackwardSpeed) * InputVal;
            }

            //add gravity to the speed
            if (character.isGrounded)
            {
                Debug.Log("Character grounder");
            }
            MoveToMake += character.isGrounded ? Vector3.zero : Physics.gravity;
        }

        private void FixedUpdate()
        {
            if (MoveToMake != Vector3.zero)
            {
                character.Move(MoveToMake * Time.deltaTime);
            }
            MakeMoveAnim(MoveToMake.z);
        }

        private void MakeMoveAnim(float speed)
        {
            character.GetComponent<Animator>().SetFloat("Speed", Mathf.Clamp(speed, -MaxBackwardSpeed, MaxForwardSpeed));
        }
    }
}
