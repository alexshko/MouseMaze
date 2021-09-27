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
        public float EpsilonAngleCheck = 1.0f;
        public float timeForPlayerTurn = 0.04f;
        public float timeForCamTurn = 0.1f;
        public Transform CamRefObject;
        public float MinDistanceOFSwipe = 0.002f;
        public float minAngleJoystickNotUp = 0.1f;

        [SerializeField]
        private float speed => MoveToMakeNoGravityLocal.magnitude;

        private Animator anim;
        private CharacterController character;
        private Transform mouseRef;
        
        private Vector3 prevMovement;
        private Vector3 MoveToMake;
        private Vector3 MoveToMakeNoGravityLocal;
        private Vector3 gravitySpeed = Vector3.zero;
        private bool prevGrounded = false;
        private bool isJumping = false;
        private bool isAboutToJump = false; //for requesting to jump by button.

        private Vector2 InputVal = Vector2.zero;
        private bool isCameraMove;
        private Vector2 fingerMoveForCamera;
        private float CameraMoveAngleY = 0;
        private float CameraMoveAngleYTotal = 0;

        private float joystickRotation;
        private Quaternion mouseDesiredRotation;
        private float mouseDesiredSpeed;

        private int fingerIdCameraControl = -1;
        [SerializeField]
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

            CameraMoveAngleYTotal = 0;
            MoveToMake = Vector3.zero;
            gravitySpeed = Vector3.zero;
            isJumping = false;
            fingerIdCameraControl = -1;

            Debug.LogFormat("Screen size: {0}", Screen.width);

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

        #region functions executed in Update:
        private void CalcMovementToMake()
        {
            //to make comparison to previous movement vector. will be used, for instance, to check if he reached the maximum height during jump
            prevMovement = MoveToMake;
            MoveToMake = Vector3.zero;

            if (GameController.Instance.acceptInputPlayer)
            {
                //take the Vertical input axis. and also the vertical valuue of the joystick:
                InputVal = GameController.Instance.JoystickValue;
            }
            else
            {
                InputVal = Vector2.zero;
            }

            //calculate the movement and rotation of mousey by the joystick:
            CalcJoystickRotationMagnitude(out mouseDesiredRotation, out mouseDesiredSpeed);

            //if there is joystick moemvent, then needs to go forward.
            //if it starts from standing (no speed), then it shoud wait for the turn of the mouse to finsh first.
            if (InputVal.magnitude > 0.05f)
            {

                if ((speed == 0 && Quaternion.Angle(mouseRef.rotation, mouseDesiredRotation) < EpsilonAngleCheck) || speed!=0)
                {
                    Debug.Log("Move to make1: " + mouseRef.forward);
                    MoveToMake = MaxForwardSpeed * Mathf.Clamp(mouseDesiredSpeed, 0, 1) * mouseRef.forward;
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
                    if (checkIfCameraMoveTouch(curTouch))
                    {
                        if (curTouch.phase == TouchPhase.Began)
                        {
                            fingerIdCameraControl = curTouch.fingerId;
                        }
                        else if (curTouch.phase == TouchPhase.Ended || curTouch.phase == TouchPhase.Canceled)
                        {
                            fingerIdCameraControl = -1;
                        }
                        else if (curTouch.phase == TouchPhase.Moved)
                        {
                            fingerMoveForCamera = curTouch.deltaPosition / Screen.width;
                            Debug.LogFormat("Touch input x: {0}", fingerMoveForCamera.x);
                            CameraMoveAngleY += Mathf.Clamp(fingerMoveForCamera.x, -1, 1);
                            //if the angley has passed the MinAngle only then start adding to the total, so it will turn:
                            if (Mathf.Abs(CameraMoveAngleY) > MinDistanceOFSwipe/Screen.width)
                            {
                                CameraMoveAngleYTotal += Mathf.Clamp(fingerMoveForCamera.x, -1, 1) * adjustedVerticalAim * Time.deltaTime;
                                Debug.LogFormat("camera Move: {0}", CameraMoveAngleY);
                            }
                            break;
                        }
                        else
                        {
                            CameraMoveAngleY = 0;
                        }
                    }
                }
            }
            else
            {
                CameraMoveAngleYTotal = 0;
            }
        }

        private bool checkIfCameraMoveTouch(Touch curTouch)
        {
            if (fingerIdCameraControl != -1)
            {
                return (curTouch.fingerId == fingerIdCameraControl);
            }
            return (curTouch.fingerId != GameController.Instance.JoystickTouchId);
        }
        
        private void CalcJoystickRotationMagnitude(out Quaternion rot, out float magntiude)
        {
            //calculate the movement and rotation of mousey by the joystick:
            float jRot = -Vector2.SignedAngle(Vector2.up, InputVal);
            Debug.LogFormat("jRot before the miDistance: {0}", jRot);
            jRot = Mathf.Abs(jRot) < minAngleJoystickNotUp ? 0 : jRot;
            jRot = Mathf.Abs(jRot) > 180 - minAngleJoystickNotUp ? 180 : jRot;

            rot = Quaternion.Euler(0, jRot, 0) * CamRefObject.rotation;
            magntiude = InputVal.magnitude;
        }
        
        #endregion

        #region functions executed in FixedUpdate. they make changes to the objects so they will move.
        private void TurnCharacter()
        {
            if (InputVal.magnitude > 0.05f && Quaternion.Angle(mouseRef.rotation, mouseDesiredRotation) > EpsilonAngleCheck)
            {
                float yVelocity = 0;
                float curAngle = mouseRef.rotation.eulerAngles.y;
                float newAngle = Mathf.SmoothDampAngle(curAngle, mouseDesiredRotation.eulerAngles.y, ref yVelocity, timeForPlayerTurn,Mathf.Infinity,Time.fixedDeltaTime);
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
            MoveToMakeNoGravityLocal = mouseRef.InverseTransformDirection(MoveToMakeNoGravityWorldSpace);

            Debug.Log("Run: " + MoveToMakeNoGravityLocal);
            float sign = Mathf.Sign(MoveToMakeNoGravityLocal.z);
            MakeMoveAnim(sign * MoveToMakeNoGravityLocal.magnitude);
        }
        private void TurnCamRefObject()
        {
            float yVelocity = 0;
            //move the reference
            if (CameraMoveAngleYTotal > EpsilonAngleCheck || CameraMoveAngleYTotal <-EpsilonAngleCheck)
            {
                float curAngle = CamRefObject.rotation.eulerAngles.y;
                float newAngle = Mathf.SmoothDampAngle(curAngle, curAngle + CameraMoveAngleYTotal, ref yVelocity, timeForCamTurn, Mathf.Infinity, Time.fixedDeltaTime);
                CameraMoveAngleYTotal -= newAngle - curAngle;
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

        #endregion
        public void MakeJumpButton()
        {
            if (!isAboutToJump && !isJumping)
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
