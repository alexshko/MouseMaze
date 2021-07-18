using alexshko.colamazle.core;
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
                MoveToMake += (InputVal > 0 ? MaxForwardSpeed : MaxBackwardSpeed) * Mathf.Clamp(InputVal, -1, 1) *transform.forward;
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
    }
}
