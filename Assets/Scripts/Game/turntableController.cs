using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class turntableController : MonoBehaviour
{
    const float START_SPEED = 4000;

    const float ENDING_SPEED = 100f;
    const float SLOWDOWN_RATE = 1.5f;
    const int ROTATENUM = 100;

    public Text[] options;
    public bool duringDrawing = false;

    public Image turntable;

    public int currentSelectedNum = 0;
    private float rotateSpeed;
    private float totalRotation;
    // Start is called before the first frame update
    void Start()
    {
        drawTurntable();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(rotateSpeed);
        if(duringDrawing){
            if(totalRotation >= 0){
                float thisRotation = rotateSpeed * Time.deltaTime;
                // if the rotation this time exceed the rest rotation, assign it to be the same as the rest rotation
                if(thisRotation >= totalRotation){
                    thisRotation = totalRotation;
                }
                
                turntable.GetComponent<Transform>().Rotate(new Vector3(0,0,thisRotation));
                totalRotation -= thisRotation;
                if(rotateSpeed > ENDING_SPEED - SLOWDOWN_RATE){
                    rotateSpeed -= SLOWDOWN_RATE;
                }
                else{
                    rotateSpeed = ENDING_SPEED;
                }
            }
            
            else{
                // rotation end
                duringDrawing = false;
            }
        }
    }

    public void drawTurntable(){
        int optionNum = options.Length;
        int selectedNum = Random.Range(0,optionNum);
        duringDrawing = true;
        currentSelectedNum = selectedNum;
        totalRotation = 360*ROTATENUM + currentSelectedNum*90;
        rotateSpeed = START_SPEED;
    }
}
