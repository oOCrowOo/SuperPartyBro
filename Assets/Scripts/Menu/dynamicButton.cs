using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class dynamicButton : MonoBehaviour
{

    private MainMenuManager menuManager;
    private Text text;

    void Awake(){
        menuManager = GameObject.Find("MainMenuManager").GetComponent<MainMenuManager>();
        text = this.GetComponent<Text>();
        print("hello");
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable(){
        print("hello2");
        if (menuManager.isHost()){
            text.text = "Start!";
        }
        else{
            text.text = "Ready!";
        }
    }
}
