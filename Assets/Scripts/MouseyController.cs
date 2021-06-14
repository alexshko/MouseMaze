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
        private float CharAngleY = 0;

        private bool isCameraMove;
        private string PhaseMove;
        private Vector2 TouchPos;
        private Vector2 fingerMove;
        private int JoystickTouchId;
        private int TouchID;

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
            CalcMovementToMake();
            CalcCamReferenceObject();
        }

        private void CalcMovementToMake()
        {
            MoveToMake = Vector3.zero;

            //take the Vertical input axis. and also the vertical valuue of the joystick:
            float InputVal = Input.GetAxis("Vertical") + GameController.Instance.JoystickValue;
            if (Mathf.Abs(InputVal) > 0.05f)
            {
                Debug.Log("Going forward");
                if (CamRefObject.rotation.eulerAngles.magnitude > 1)
                {
                    //transform.rotation *= CamRefObject.rotation;
                    //CamRefObject.rotation = Quaternion.Euler(0, 0, 0);
                    CharAngleY = CamRefObject.rotation.eulerAngles.y;
                    //CameraMoveAngleY = 0;
                }
                MoveToMake += (InputVal > 0 ? MaxForwardSpeed : MaxBackwardSpeed) * InputVal *transform.forward;
            }

            //add gravity to the speed
            if (character.isGrounded)
            {
                Debug.Log("Character grounder");
            }
            MoveToMake += character.isGrounded ? Vector3.zero : Physics.gravity;
        }

        private void CalcCamReferenceObject()
        {
            //added by alexshko:
            //Android phone:
            isCameraMove = (GameController.Instance.JoystickIsTouching && Input.touchCount > 1) || (!GameController.Instance.JoystickIsTouching && Input.touchCount > 0);
            if (isCameraMove)
            {
                foreach (Touch curTouch in Input.touches)
                {
                    TouchID = curTouch.fingerId;
                    JoystickTouchId = GameController.Instance.JoystickTouchId;
                    if (curTouch.fingerId != GameController.Instance.JoystickTouchId)
                    {
                        PhaseMove = curTouch.phase.ToString();
                        TouchPos = curTouch.position;
                        if ((curTouch.phase == TouchPhase.Moved || curTouch.phase == TouchPhase.Stationary) && curTouch.position.x > 10 && curTouch.position.y > 10)
                        {
                            fingerMove = curTouch.deltaPosition;
                            CameraMoveAngleY += Mathf.Clamp(fingerMove.x, -1, 1) * horizontalAimingSpeed * 0.5f * Time.deltaTime;
                            //angleV += Mathf.Clamp(fingerMove.y, -1, 1) * verticalAimingSpeed * 0.5f;
                            break;
                        }
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            TurnCharacter();
            MoveCharacter();
            TurnCamRefObject();
        }

        private void TurnCharacter()
        {
            if (Quaternion.Angle(transform.rotation, Quaternion.Euler(0, CharAngleY,0)) > 1)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0,CharAngleY,0), 5*Time.deltaTime);
            }
        }

        private void TurnCamRefObject()
        {
            //move the reference
            if (Quaternion.Angle(CamRefObject.rotation, Quaternion.Euler(0,CameraMoveAngleY,0))> 1)
            {
                CamRefObject.rotation = Quaternion.Lerp(CamRefObject.rotation, Quaternion.Euler(0, CameraMoveAngleY, 0), 5*Time.deltaTime);
            }
        }

        private void MoveCharacter()
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
