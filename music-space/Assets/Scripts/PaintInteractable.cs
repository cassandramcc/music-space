using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PaintInteractable : MonoBehaviour
{
    public Material paintColour;
    public GameObject paintHolder;
    public GameObject[] controllers;
    // Start is called before the first frame update
    void Start()
    {
        Assert.IsNotNull(paintHolder);
        Assert.IsNotNull(paintColour);
        Assert.AreNotEqual(controllers.Length,0);
    }

    public void ControllerHover(){
        Debug.Log("Controller here!");
    }

    public void ControllerSelect(){
        Debug.Log("Controller select");
        paintHolder.GetComponent<Painter>().paintColour = paintColour;
        foreach (GameObject controller in controllers){
            controller.GetComponent<MeshRenderer>().material = paintColour;
        }
    }
}
