using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading;

public class ChangeScale : MonoBehaviour
{
    public GameObject painter;
    public TextMeshPro scaleUI;
    float timer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PopUpScaleUI()
    {
        scaleUI.gameObject.SetActive(true);
        scaleUI.SetText(painter.GetComponent<Painter>().currentScale.ToString());
        Invoke("DisableScaleUI", 4f); 
    }

    void DisableScaleUI()
    {
        scaleUI.gameObject.SetActive(false);
    }

    public void DownScale(){
        painter.GetComponent<Painter>().currentScale = (painter.GetComponent<Painter>().currentScale - 1);
        if (painter.GetComponent<Painter>().currentScale < 0) {
            painter.GetComponent<Painter>().currentScale = 0;
        }
        PopUpScaleUI();
    }

    public void UpScale(){
        painter.GetComponent<Painter>().currentScale = (painter.GetComponent<Painter>().currentScale + 1);
        if (painter.GetComponent<Painter>().currentScale > Root.FMajor) {
            painter.GetComponent<Painter>().currentScale = Root.FMajor;
        }
        PopUpScaleUI();
    }
}
