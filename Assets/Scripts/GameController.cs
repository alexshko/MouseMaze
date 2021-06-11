using UnityEngine;

namespace alexshko.colamazle.core
{

    public class GameController : MonoBehaviour
    {
        public static GameController Instance { get; set; }

        public DynamicJoystick JoystickPref;

        public float JoystickValue
        {
            get => JoystickPref.Vertical;
           
        }

        private void Awake()
        {
            Instance = this;
        }
    }
}
