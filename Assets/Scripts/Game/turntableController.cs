using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class turntableController : MonoBehaviour
{
    const float START_SPEED = 1000;

    const float ENDING_SPEED = 100f;
    const float SLOWDOWN_RATE = 2f;
    const int ROTATENUM = 100;

    public Text[] options;
    public bool duringDrawing = false;
    public bool reachingEnd = false;

    public Image turntable;

    public int currentSelectedNum = 0;
    private float rotateSpeed;
    private float totalRotation;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if(duringDrawing){
            Transform turntable_transform = turntable.GetComponent<Transform>();
            float thisRotation = rotateSpeed * Time.deltaTime;    

            // handle the case if the rotation this time exceed the total rotation left       
            if (thisRotation>= totalRotation){
                thisRotation = totalRotation;
            }
            turntable.GetComponent<Transform>().Rotate(new Vector3(0,0,thisRotation));


            if(rotateSpeed > ENDING_SPEED + SLOWDOWN_RATE){
                rotateSpeed -= SLOWDOWN_RATE;
            }
            else{
                // detect the rotation is reaching end
                if(!reachingEnd){
                    rotateSpeed = ENDING_SPEED;
                    totalRotation = 360 - turntable_transform.eulerAngles.z + currentSelectedNum*90;
                    reachingEnd = true;
                }
                
            }
            // if is reaching end of rotation, only rotate one more circle
            if (reachingEnd){
                totalRotation -= thisRotation;
                if (totalRotation <= 0){
                    duringDrawing = false;
                    reachingEnd = false;
                }
            }
            
        }
    }


    public int selectNum(){
        int optionNum = options.Length;
        int selectedNum = Random.Range(0,optionNum);
        return selectedNum;
    }

    public void drawTurntable(int select){
        // int optionNum = options.Length;
        // // randomly select one option
        // // TODO: don't select the same option every time
        // int selectedNum = Random.Range(0,optionNum);
        duringDrawing = true;
        currentSelectedNum = select;
        totalRotation = 360*ROTATENUM + currentSelectedNum*90;
        rotateSpeed = START_SPEED;
    }
}
