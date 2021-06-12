using alexshko.colamazle.core;
using System;
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
        public int horizontalAimingSpeed = 6;
        public Transform CamRefObject;


        private CharacterController character;
        private Vector3 MoveToMake;
        private float CameraMoveAngleY = 0;

        private void Start()
        {
            character = GetComponent<CharacterController>();
            if (!character)
            {
                Debug.LogError("missing CharacterController");
            }
            CameraMoveAngleY = 0;
            MoveToMake = Vector3.zero;

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

            CalcCamReferenceObject();
        }

        private void CalcCamReferenceObject()
        {
            //added by alexshko:
            //Android phone:
            bool isCameraMove = (GameController.Instance.JoystickIsTouching && Input.touchCount > 1) || (!GameController.Instance.JoystickIsTouching && Input.touchCount > 0);
            if (isCameraMove)
            {
                foreach (Touch curTouch in Input.touches)
                {
                    if (curTouch.fingerId != GameController.Instance.JoystickTouchId)
                    {
                        if ((curTouch.phase == TouchPhase.Moved || curTouch.phase == TouchPhase.Stationary) && curTouch.position.x > 512 && curTouch.position.y > 512)
                        {
                            Vector2 fingerMove = curTouch.deltaPosition;
                            CameraMoveAngleY += Mathf.Clamp(fingerMove.x, -1, 1) * horizontalAimingSpeed * 0.5f;
                            //angleV += Mathf.Clamp(fingerMove.y, -1, 1) * verticalAimingSpeed * 0.5f;
                            break;
                        }
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            MakeMove();
            MoveCamRefObject();
        }

        private void MoveCamRefObject()
        {
            //move the reference
            CamRefObject.rotation = Quaternion.Euler(0, CameraMoveAngleY, 0);
        }

        private void MakeMove()
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
