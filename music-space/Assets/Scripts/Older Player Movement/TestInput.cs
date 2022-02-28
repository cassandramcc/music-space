using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TestInput : MonoBehaviour
{
    private ActionBasedController controller;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<ActionBasedController>();

        controller.activateAction.action.performed += Action_performed;
    }

    private void Action_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        //Debug.Log(controller.activateAction.action.ReadValue<float>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
