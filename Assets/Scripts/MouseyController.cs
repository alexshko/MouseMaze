using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class MouseyController : MonoBehaviour
{
    public float Speed = 0f;


    private CharacterController character;
    private Vector3 MoveToMake;

    private void Start()
    {
        character = GetComponent<CharacterController>();
        if (!character)
        {
            Debug.LogError("missing CharacterController");
        }

    }

    private void Update()
    {
        MoveToMake = Vector3.zero;
        if (Mathf.Abs(Input.GetAxis("Vertical")) >0.3f)
        {
            Debug.Log("Going forward");
            MoveToMake.z += Speed * Input.GetAxis("Vertical");
        }

        //add gravity to the speed
        if (character.isGrounded) {
            Debug.Log("Character grounder");
        }
        MoveToMake += character.isGrounded ? Vector3.zero : Physics.gravity;  
    }

    private void FixedUpdate()
    {
        if (MoveToMake != Vector3.zero)
        {
            character.Move(MoveToMake * Time.deltaTime);
        }
        MakeMoveAnim(MoveToMake.z);
    }

    private void MakeMoveAnim(float speed)
    {
        character.GetComponent<Animator>().SetFloat("Speed",Mathf.Clamp(speed, 0, 3f));
    }
}
