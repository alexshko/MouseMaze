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
        public int horizontalCamStandSpeed = 6;
        public int horizontalCamMinSpeed = 2;
        public int horizontalCamMaxSpeed = 3;
        public Transform CamRefObject;

        [SerializeField]
        private float speed => MoveToMakeNoGravityLocal.magnitude;

        private Animator anim;
        private CharacterController character;
        private Transform mouseRef;
        
        private Vector3 prevMovement;
        private Vector3 MoveToMake;
        private Vector3 MoveToMakeNoGravityLocal;
        private Vector3 gravitySpeed = Vector3.zero;
        private Vector3 jumpSpeed = Vector3.zero;
        private bool prevGrounded = false;
        private bool isJumping = false;
        private bool isAboutToJump = false; //for requesting to jump by button.
        private float CameraMoveAngleY = 0;
        float InputVal = 0;

        private bool isCameraMove;
        private Vector2 fingerMoveForCamera;
        private float adjustedVerticalAim
        {
            get => speed==0? horizontalCamStandSpeed : Mathf.Lerp(horizontalCamMaxSpeed, horizontalCamMinSpeed, speed / MaxForwardSpeed);
        }

        private void Start()
        {
            character = GetComponentInChildren<CharacterController>();
            if (!character)
            {
                Debug.LogError("missing CharacterController");
            }

            anim = GetComponentInChildren<Animator>();
            if (!anim)
            {
                Debug.LogError("Missing animator");
            }
            mouseRef = anim.transform;

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

        private void FixedUpdate()
        {
            TurnCharacter();
            MoveCharacter();
            TurnCamRefObject();
        }

        private void CalcMovementToMake()
        {
            //to make comparison to previous movement vector. will be used, for instance, to check if he reached the maximum height during jump
            prevMovement = MoveToMake;
            MoveToMake = Vector3.zero;

            if (GameController.Instance.acceptInputPlayer)
            {
                //take the Vertical input axis. and also the vertical valuue of the joystick:
                InputVal = Input.GetAxis("Vertical") + GameController.Instance.JoystickValue;
            }
            if (Mathf.Abs(InputVal) > 0.05f)
            {
                Debug.Log("Going forward");
                //if the character stands still and need to turn around, he will start moving only after finished turning.
                //CharAngleY = CamRefObject.rotation.eulerAngles.y;
                if (Quaternion.Angle(mouseRef.rotation, CamRefObject.rotation) < 1)
                {
                    Debug.Log("Move to make1: " + transform.forward);
                    MoveToMake += (InputVal > 0 ? MaxForwardSpeed : MaxBackwardSpeed) * Mathf.Clamp(InputVal, -1, 1) * mouseRef.forward;
                }
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
                        if (curTouch.phase == TouchPhase.Moved || curTouch.phase == TouchPhase.Stationary)
                        {
                            fingerMoveForCamera = curTouch.deltaPosition;
                            //fingerMoveForCamera = new Vector2(-0.5f, -0.5f);
                            //if he's during run then take only half the aiming speed
                            CameraMoveAngleY += Mathf.Clamp(fingerMoveForCamera.x, -1, 1) * adjustedVerticalAim * Time.deltaTime;
                            break;
                        }
                    }
                }
            }
        }

        private void TurnCharacter()
        {
            if (Mathf.Abs(InputVal) > 0.05f && Quaternion.Angle(mouseRef.rotation, CamRefObject.rotation) > 1)
            {
                //mouseRef.rotation = Quaternion.Lerp(mouseRef.rotation, CamRefObject.rotation, 10*Time.deltaTime);
                float yVelocity = 0;
                float curAngle = mouseRef.rotation.eulerAngles.y;
                float newAngle = Mathf.SmoothDampAngle(curAngle, CamRefObject.rotation.eulerAngles.y, ref yVelocity, 0.05f);
                mouseRef.rotation = Quaternion.Euler(new Vector3(0, newAngle, 0));
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
        private void TurnCamRefObject()
        {
            float yVelocity = 0;
            //move the reference
            if (CameraMoveAngleY > 1 || CameraMoveAngleY <-1)
            {
                float curAngle = CamRefObject.rotation.eulerAngles.y;
                float newAngle = Mathf.SmoothDampAngle(curAngle, curAngle + CameraMoveAngleY, ref yVelocity, 0.05f);
                CameraMoveAngleY -= newAngle - curAngle;
                CamRefObject.rotation = Quaternion.Euler(new Vector3(0, newAngle, 0));
            }
        }


        private void MakeMoveAnim(float speed)
        {
            anim.SetFloat("Speed", Mathf.Clamp(speed, -MaxBackwardSpeed, MaxForwardSpeed));
        }

        private void StartJumpAnim()
        {
            anim.SetTrigger("Jump");
        }

        private void FinshJumpAnim()
        {
            Debug.Log("Finish jump anim");
            anim.SetTrigger("FinishJump");
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
