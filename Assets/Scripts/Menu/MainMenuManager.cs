using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{


    public GameObject main_panel;
    public GameObject about_panel;
    public GameObject create_join_panel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onClickStart(){
        create_join_panel.SetActive(true);
        main_panel.SetActive(false);
    }

    public void onClickAbout(){
        about_panel.SetActive(true);
        main_panel.SetActive(false);
    }

    public void onClickBack_Create_Join(){
        create_join_panel.SetActive(false);
        main_panel.SetActive(true);
    }

    public void onClickBack_About(){
        about_panel.SetActive(false);
        main_panel.SetActive(true);
    }

    public void onClickQuit(){
        Application.Quit();
    }

    public void onClickCreate(){
        //TODO: Temp code just loading the ar scene
        // Should create the room
        SceneManager.LoadScene("HelloAR");
    }

    public void onClickJoin(){
        //TODO: Temp code just loading the ar scene
        // should search for room can be joined
        SceneManager.LoadScene("HelloAR");
    }
}
