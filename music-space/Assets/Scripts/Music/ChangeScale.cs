using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeScale : MonoBehaviour
{
    public GameObject painter;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DownScale(){
        painter.GetComponent<Painter>().currentScale = (painter.GetComponent<Painter>().currentScale - 1);
        if (painter.GetComponent<Painter>().currentScale < 0) {
            painter.GetComponent<Painter>().currentScale = 0;
        }
    }

    public void UpScale(){
        painter.GetComponent<Painter>().currentScale = (painter.GetComponent<Painter>().currentScale + 1);
        if (painter.GetComponent<Painter>().currentScale > Root.FMajor) {
            painter.GetComponent<Painter>().currentScale = Root.FMajor;
        }
    }
}
