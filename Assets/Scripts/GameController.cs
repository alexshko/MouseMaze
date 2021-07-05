using System.Collections.Generic;
using UnityEngine;

namespace alexshko.colamazle.core
{

    public class GameController : MonoBehaviour
    {
        public static GameController Instance { get; set; }

        public DynamicJoystick JoystickPref;
        public Canvas UICanvasRef;

        //reference for the score text:
        public TMPro.TMP_Text ScoreRef;

        public float JoystickValue
        {
            get => JoystickPref.Vertical;
        }
        public int JoystickTouchId
        {
            get => JoystickPref.TouchID;
        }
        public bool JoystickIsTouching
        {
            get => JoystickPref.isTouching;
        }
        private void Awake()
        {
            Instance = this;
            AdjustScreenSize();
        }

        private void Start()
        {
        }

        //List of cheese. the cheeses will be added when Enabled (on game start).
        public List<Transform> CheeseList;

        private void AdjustScreenSize()
        {
            RectTransform rt = UICanvasRef.GetComponent<RectTransform>();
            float canvasHeight = rt.rect.height;
            float desiredCanvasWidth = canvasHeight * Camera.main.aspect;
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, desiredCanvasWidth);
        }
    }
}
