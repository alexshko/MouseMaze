using alexshko.colamazle.core;
using System;
using UnityEngine;

namespace alexshko.colamazle.Entities
{
    public class MouseyController : MonoBehaviour
    {
        #region references for taking cola animation
        public Transform ColaRef { get; set; }
        public Transform MouseHandRef { get; set; }
        #endregion

        public float MaxForwardSpeed = 0f;
        public float MaxBackwardSpeed = 0f;
        public float JumpSpeed = 10f;
        public int horizontalAimingSpeed = 6;
        public Transform CamRefObject;

        [SerializeField]
        private float speed => MoveToMakeNoGravityLocal.magnitude;


        private CharacterController character;
        private Vector3 prevMovement;
        private Vector3 MoveToMake;
        private Vector3 MoveToMakeNoGravityLocal;
        private Vector3 gravitySpeed = Vector3.zero;
        private Vector3 jumpSpeed = Vector3.zero;
        private bool prevGrounded = false;
        private bool isJumping = false;
        private bool isAboutToJump = false; //for requesting to jump by button.
        private float CameraMoveAngleY = 0;
        private float CharAngleY = 0;

        private bool isCameraMove;
        private Vector2 fingerMoveForCamera;
        private float adjustedVerticalAim
        {
            get => speed==0? horizontalAimingSpeed : Mathf.Lerp(horizontalAimingSpeed/2.0f, horizontalAimingSpeed / 4.0f, speed / MaxForwardSpeed);
        }

        private void Start()
        {
            character = GetComponent<CharacterController>();
            if (!character)
            {
                Debug.LogError("missing CharacterController");
            }
            CameraMoveAngleY = transform.rotation.eulerAngles.y;
            CharAngleY = transform.rotation.eulerAngles.y;
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
            prevMovement = MoveToMake;
            MoveToMake = Vector3.zero;

            float InputVal = 0;
            if (GameController.Instance.acceptInputPlayer)
            {
                //take the Vertical input axis. and also the vertical valuue of the joystick:
                InputVal = Input.GetAxis("Vertical") + GameController.Instance.JoystickValue;
            }
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
                Debug.Log("Move to make1: " + transform.forward);
                MoveToMake += (InputVal > 0 ? MaxForwardSpeed : MaxBackwardSpeed) * Mathf.Clamp(InputVal, -1, 1) *transform.forward;
            }

            //if he's landed on ground, then cancel jumping.
            if (character.isGrounded && !prevGrounded)
            {
                isJumping = false;
            }
            //if he's on ground and presseed Jump button then it should be taken care in MoveToMake and activate the jump anim.
            if (character.isGrounded && isAboutToJump)
            {
                isJumping = true;
                StartJumpAnim();
            }
            MoveToMake += isJumping? (JumpSpeed * transform.up) : Vector3.zero;

            //calculate the speed affected by gravity and sum it to gravitySpeed.
            //if he;s on the ground then it shoukd be zero for the CharacterController to work
            if (character.isGrounded && !isJumping)
            {
                gravitySpeed = Vector3.zero;
            }
            gravitySpeed = gravitySpeed + Physics.gravity * Time.deltaTime;
            //add to the frame's speed:
            MoveToMake += gravitySpeed;

            //if he's in the height of the jump, and now starts falling, then start the second animation.
            if (isJumping && (Mathf.Sign(MoveToMake.y) == -1 && Mathf.Sign(prevMovement.y) == 1))
            {
                FinshJumpAnim();
            }

            prevGrounded = character.isGrounded;
            //at the end of the update, reset the request to jump.
            isAboutToJump = false;
        }

        private void CalcCamReferenceObject()
        {
            //added by alexshko:
            //Android phone:
            isCameraMove = GameController.Instance.acceptInputPlayer;
            isCameraMove = isCameraMove && (GameController.Instance.JoystickIsTouching && Input.touchCount > 1) || (!GameController.Instance.JoystickIsTouching && Input.touchCount > 0);
            if (isCameraMove)
            {
                foreach (Touch curTouch in Input.touches)
                {
                    if (curTouch.fingerId != GameController.Instance.JoystickTouchId)
                    {
                        if ((curTouch.phase == TouchPhase.Moved || curTouch.phase == TouchPhase.Stationary) && curTouch.position.x > 200 && curTouch.position.y > 200)
                        {
                            fingerMoveForCamera = curTouch.deltaPosition;
                            //if he's during run then take only half the aiming speed
                            CameraMoveAngleY += Mathf.Clamp(fingerMoveForCamera.x, -1, 1) * adjustedVerticalAim * Time.deltaTime;
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
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0,CharAngleY,0), 10*Time.deltaTime);
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
            Vector3 MoveToMakeNoGravityWorldSpace = new Vector3(MoveToMake.x, 0, MoveToMake.z);
            MoveToMakeNoGravityLocal = transform.InverseTransformDirection(MoveToMakeNoGravityWorldSpace);
            //Debug.Assert(MoveToMakeNoGravityWorldSpace.magnitude == MoveToMakeNoGravityLocal.magnitude);

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
            isAboutToJump = true;
        }

        public void GrabCola()
        {
            if (ColaRef && MouseHandRef)
            {
                ColaRef.parent = MouseHandRef;
                ColaRef.localPosition = Vector3.zero;
                ColaRef.localRotation = Quaternion.Euler(0, 0, 90);
            }
        }
    }
}
