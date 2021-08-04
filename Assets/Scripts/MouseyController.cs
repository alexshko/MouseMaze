using alexshko.colamazle.core;
using System;
using UnityEngine;

namespace alexshko.colamazle.Entities
{
    public class MouseyController : MonoBehaviour
    {
        public float MaxForwardSpeed = 0f;
        public float MaxBackwardSpeed = 0f;
        public float JumpSpeed = 10f;
        public int horizontalAimingSpeed = 6;
        public Transform CamRefObject;


        private CharacterController character;
        private Vector3 MoveToMake;
        private Vector3 gravitySpeed = Vector3.zero;
        private Vector3 jumpSpeed = Vector3.zero;
        private bool isJumping = false;
        private float CameraMoveAngleY = 0;
        private float CharAngleY = 0;

        private bool isCameraMove;
        private Vector2 fingerMoveForCamera;

        private void Start()
        {
            character = GetComponent<CharacterController>();
            if (!character)
            {
                Debug.LogError("missing CharacterController");
            }
            CameraMoveAngleY = 0;
            MoveToMake = Vector3.zero;
            gravitySpeed = Vector3.zero;
            jumpSpeed = Vector3.zero;
            isJumping = false;
        }

        private void Update()
        {
            CalcMovementToMake();
            CalcCamReferenceObject();
        }

        private void CalcMovementToMake()
        {
            //to make comparison to previous movement vector. will be used, for instance, to check if he reached the maximum height during jump
            Vector3 prevMovement = MoveToMake;
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
                MoveToMake += (InputVal > 0 ? MaxForwardSpeed : MaxBackwardSpeed) * Mathf.Clamp(InputVal, -1, 1) *transform.forward;
            }

            //calculate the speed affected by gravity in this frame and sum it to gravitySpeed:
            gravitySpeed = character.isGrounded ? Vector3.zero : (gravitySpeed + Physics.gravity * Time.deltaTime);
            //add to the frame's speed:
            MoveToMake += gravitySpeed;

            //if he jumps then add it to the MoveToMake
            //if he's on ground and presseed Jump button then he should get velocity:
            if (character.isGrounded && isJumping)
            {
                StartJumpAnim();
            }
            //if he landed back on the ground then he has no velocity anymore
            else if (character.isGrounded)
            {
                isJumping = false;
            }
            MoveToMake += isJumping? (JumpSpeed * transform.up) : Vector3.zero;
            //if he's in the height of the jump, then start the second animation.
            if (isJumping && (Mathf.Sign(MoveToMake.y) == -1 && Mathf.Sign(prevMovement.y) ==1))
            {
                FinshJumpAnim();
            }
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
                    if (curTouch.fingerId != GameController.Instance.JoystickTouchId)
                    {
                        if ((curTouch.phase == TouchPhase.Moved || curTouch.phase == TouchPhase.Stationary) && curTouch.position.x > 200 && curTouch.position.y > 200)
                        {
                            fingerMoveForCamera = curTouch.deltaPosition;
                            CameraMoveAngleY += Mathf.Clamp(fingerMoveForCamera.x, -1, 1) * horizontalAimingSpeed * Time.deltaTime;
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
            //calculate the speed of wich to make the run animation:
            Vector3 MoveToMakeNoGravity = new Vector3(MoveToMake.x, 0, MoveToMake.z);
            Vector3 MoveToMakeNoGravityLocal = transform.InverseTransformDirection(MoveToMakeNoGravity);
            Debug.Log("Run: " + MoveToMakeNoGravityLocal);
            float sign = Mathf.Sign(MoveToMakeNoGravityLocal.z);
            MakeMoveAnim(sign * MoveToMakeNoGravityLocal.magnitude);
        }

        private void MakeMoveAnim(float speed)
        {
            character.GetComponent<Animator>().SetFloat("Speed", Mathf.Clamp(speed, -MaxBackwardSpeed, MaxForwardSpeed));
        }

        private void StartJumpAnim()
        {
            character.GetComponent<Animator>().SetTrigger("Jump");
        }

        private void FinshJumpAnim()
        {
            Debug.Log("Finish jump anim");
            character.GetComponent<Animator>().SetTrigger("FinishJump");
        }

        public void MakeJumpButton()
        {
            if (!isJumping)
            {
                //will be checked during update function and perform jump. once it's grounded he will become flase again.
                isJumping = true;
            }
        }
    }
}
