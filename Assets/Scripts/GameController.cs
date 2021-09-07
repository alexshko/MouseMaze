using alexshko.colamazle.Entities;
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

        //to know if to accept input from player:
        public bool acceptInputPlayer { get; set; }

        private List<GateWay> listOfGateWays;


        //List of cheese. the cheeses will be added when Enabled (on game start).

        private ObservedList<Transform> cheeseList;
        public ObservedList<Transform> CheeseList

        {
            get
            {
                if (cheeseList == null)
                {
                    cheeseList = new ObservedList<Transform>();
                    cheeseList.OnListChanged += UpdateCheeseUI;
                    cheeseList.OnListChanged += CheckIfOpenPortal;
                }
                return cheeseList;
            }
            set
            {
                cheeseList = value;
            }
        }

        public Transform GateWaysHir;

        private void Awake()
        {
            Instance = this;
            Debug.Log("Screen:" + Camera.main.WorldToScreenPoint(UICanvasRef.transform.position));
            listOfGateWays = new List<GateWay>();
            foreach (var item in GateWaysHir.GetComponentsInChildren<GateWay>())
            {
                listOfGateWays.Add(item);
            }
            acceptInputPlayer = true;

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

        private void CheckIfOpenPortal()
        {
            if (NumberOfCheesesInGame == 0)
            {
                foreach (var gateway in listOfGateWays)
                {
                    gateway.ActivateGateway();
                }
            }
        }

        private void OnDestroy()
        {
            listOfGateWays.Clear();
            listOfGateWays = null;
            cheeseList.Clear();
            cheeseList = null;
        }
    }
}
