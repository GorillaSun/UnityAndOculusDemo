using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class birdfly1 : MonoBehaviour
{

    public Text myText;

    private float timeFly;
    private int flyDirection = 1;

    // Update is called once per frame
    void Update()
    {
        timeFly = timeFly + Time.deltaTime * 10;
        if (timeFly > 0.5)
        {
            timeFly = 0;

            //this.myText.text = "x=" + transform.position.x;
            if (flyDirection == 1)
            {
                transform.Translate(-2, 0, 2);
                if (transform.position.x < -120)
                {
                    flyDirection = 2;
                }
            }
            if (flyDirection == 2)
            {
                transform.Translate(2, 0, -2);
                if (transform.position.x > 120)
                {
                    flyDirection = 1;
                }
            }
        }
    }

    public void hide() {
        transform.Translate(0,-300,0);
    }
    public void show(){
        transform.Translate(0, 300, 0);
    }
}
