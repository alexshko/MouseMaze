using System.Collections.Generic;
using UnityEngine;

namespace alexshko.colamazle.core
{

    public class GameController : MonoBehaviour
    {
        public static GameController Instance { get; set; }

        public DynamicJoystick JoystickPref;

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
        }

        //List of cheese. the cheeses will be added when Enabled (on game start).
        public List<Transform> CheeseList;
    }
}
