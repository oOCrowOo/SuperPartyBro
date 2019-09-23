using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{


    public GameObject main_panel;
    public GameObject about_panel;
    public GameObject create_join_panel;
    public GameObject room_panel;

    public GameObject name_panel;

    public GameObject pin_panel;

    public Text nameInput;
    public InputField passwordInput;

    public Text PIN;

    private string nickname;
    private string player_num = "player1";
    private bool host = true;



    // Start is called before the first frame update
    void Start()
    {   
        // TODO: debug usage, remove later
        PlayerPrefs.DeleteAll();

        // Find user's preset name
        if(!PlayerPrefs.HasKey("nickname")){
            name_panel.SetActive(true);
            main_panel.SetActive(false);

        }
        else{
            name_panel.SetActive(false);
            main_panel.SetActive(true);
            nickname = PlayerPrefs.GetString("nickname");

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool isHost(){
        return host;
    }

    public void onClickStart(){
        create_join_panel.SetActive(true);
        main_panel.SetActive(false);
    }

    public void onClickGameStart(){
        if (host){
            SceneManager.LoadScene("BoxScene");
        }
        else{
            setPlayerReady(player_num);
        }
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
        host = true;
        create_join_panel.SetActive(false);
        room_panel.SetActive(true);
        setPlayerName(player_num,nickname);
    }

    public void setPlayerName(string player, string name){
        GameObject.Find(player).GetComponentInChildren<Text>().text = name;
    }

    public void setPlayerReady(string player){
        print(GameObject.Find(player).GetComponentInChildren<Image>());
        GameObject.Find(player).GetComponentsInChildren<Image>()[1].enabled = true;
    }

    public void onClickJoin(){
        create_join_panel.SetActive(false);
        pin_panel.SetActive(true);
    }

    public void onClickJoinConfirm(){
        host = false;
        pin_panel.SetActive(false);
        room_panel.SetActive(true);
        PIN.text = passwordInput.text;
        
        // TODO: change to set the player name in correct position
        // when connection is ready
        player_num = "player2";
        setPlayerName(player_num,nickname);
    }


    public void onClickName(){
        if (nameInput.text !=""){
            nickname = nameInput.text;
            // Save player's name, don't ask next time
            PlayerPrefs.SetString("nickname",nickname);
            main_panel.SetActive(true);
            name_panel.SetActive(false);
        }
    }
}
