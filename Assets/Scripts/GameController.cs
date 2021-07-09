using alexshko.colamazle.helpers;
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
        
        public int NumberOfCheesesInGame
        {
            get => CheeseList.Count;
        }


        //List of cheese. the cheeses will be added when Enabled (on game start).

        private ObservedList<Transform> cheeseList;
        public ObservedList<Transform> CheeseList

        {
            get
            {
                if (cheeseList == null)
                {
                    cheeseList = new ObservedList<Transform>();
                    cheeseList.OnListChanged = UpdateCheeseUI;
                }
                return cheeseList;
            }
            set
            {
                cheeseList = value;
            }
        }

        private void Awake()
        {
            Instance = this;
            Debug.Log("Screen:" + Camera.main.WorldToScreenPoint(UICanvasRef.transform.position));

            AdjustScreenSize();
        }

        private void AdjustScreenSize()
        {
            RectTransform rt = UICanvasRef.GetComponent<RectTransform>();
            float canvasHeight = rt.rect.height;
            float desiredCanvasWidth = canvasHeight * Camera.main.aspect;
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, desiredCanvasWidth);

        }

        public void UpdateCheeseUI()
        {
            ScoreRef.text = NumberOfCheesesInGame.ToString();
        }
    }
}
