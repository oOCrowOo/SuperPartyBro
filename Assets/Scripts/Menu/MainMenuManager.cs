using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class MainMenuManager : MonoBehaviourPunCallbacks
{


    public GameObject main_panel;
    public GameObject about_panel;
    public GameObject create_join_panel;
    // public GameObject room_panel;

    public GameObject name_panel;
    public GameObject playerList;
    public GameObject pin_panel;
	public GameObject RoomPanel;

    public Text nameInput;
    public InputField passwordInput;
	public GameObject[] Team;
    public Text PIN;

    private string nickname;
    private string player_num = "player1";
    private bool host = true;
	



    // Start is called before the first frame update
    void Start()
    {   
        // TODO: debug usage, remove later
        PlayerPrefs.DeleteAll();
        name_panel.SetActive(true);
        main_panel.SetActive(false);
        // Find user's preset name
        //*******
        //这个方法有点问题，如果player想换个名字咋办，既然是网游就让他每次都登录吧
        //****
        /* if (!PlayerPrefs.HasKey("nickname")){
             name_panel.SetActive(true);
             main_panel.SetActive(false);

         }
         else{
             name_panel.SetActive(false);
             main_panel.SetActive(true);
             nickname = PlayerPrefs.GetString("nickname");

         } */




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

    public void onClickBack_Room(){
        //resetPlayers();
        RoomPanel.SetActive(false);
        create_join_panel.SetActive(true);
    }

   /* private void resetPlayers(){
        for (int i = 1;i<=4;i++){
            setPlayerName("player" + i.ToString(),"waiting...");
             Image readyImg = GameObject.Find("player" + i.ToString()).GetComponentsInChildren<Image>()[1];
            readyImg.enabled = false;
        }
    }*/

    public void onClickBack_About(){
        about_panel.SetActive(false);
        main_panel.SetActive(true);
    }

    public void onClickQuit(){
        Application.Quit();
    }

    public void onClickCreate(){
        //host = true;
        // player_num = "player1";
        string password = PIN.text;
        RoomOptions roomoptions = new RoomOptions();
        roomoptions.MaxPlayers = 4;
        string[] roompropsInLobby = { "gm" }; // game mode
        //party game mode
        //1. 丢纸团 = tp
        //2. 转盘= roll
        //3。 摇可乐 = shake
        ExitGames.Client.Photon.Hashtable customesproperties = new ExitGames.Client.Photon.Hashtable();
        roomoptions.CustomRoomPropertiesForLobby = roompropsInLobby;
        roomoptions.CustomRoomProperties = customesproperties;
        PhotonNetwork.CreateRoom(password, roomoptions);
        create_join_panel.SetActive(false);
        RoomPanel.SetActive(true);
        //setPlayerName(player_num, PhotonNetwork.LocalPlayer.NickName);

    }

    /*public void setPlayerName(string player, string name){
        GameObject.Find(player).GetComponentInChildren<Text>().text = name;
    }*/

    public void setPlayerReady(string player){
        Image readyImg = GameObject.Find(player).GetComponentsInChildren<Image>()[1];
        readyImg.enabled = !readyImg.enabled;
    }

    public void onClickJoin(){
        create_join_panel.SetActive(false);
        pin_panel.SetActive(true);
  
        
    }

    public void onClickJoinConfirm(){
       
        
        PIN.text = passwordInput.text;

 
        PhotonNetwork.JoinRoom(PIN.text);
        pin_panel.SetActive(false);
        

        // TODO: change to set the player name in correct position
        // when connection is ready
        // player_num = "player2";
        //setPlayerName(player_num,nickname);
        // TODO: remove harding coding owner name
        //setPlayerName("player1","Owner");
    }


    public void onClickName(){
        if (!string.IsNullOrEmpty( nameInput.text)){
            nickname = nameInput.text;
            //dante: try to save player name using server
            // Save player's name, don't ask next time
            PlayerPrefs.SetString("nickname",nickname);
            
           
            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LocalPlayer.NickName = nickname;
                PhotonNetwork.ConnectUsingSettings();
            }
            else
            {
                Debug.Log("player name is invalid");    
            }

        }

    }
    #region Photon callbacks
    public override void OnConnected()
    {
        Debug.Log("Succuessfully connect to internet");
    }
    public override void OnConnectedToMaster()
    {
        //close name panel when connected to server
        main_panel.SetActive(true);
        name_panel.SetActive(false);
        Debug.Log(PhotonNetwork.LocalPlayer.NickName +" is Succuessfully connect to internet");
    }
    public override void OnCreatedRoom()
    {
        Debug.Log("room with password" + PhotonNetwork.CurrentRoom.Name + "is create ");
    }
    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName +" is joined to password room:" + PhotonNetwork.CurrentRoom.Name+ "playercount  " + PhotonNetwork.CurrentRoom.PlayerCount);
        RoomPanel.SetActive(true);
        //查看选的游戏是什么，以后可以根据用户的选择把人带进不同的游戏
        /* if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("gm"))
        {
            object gamemodeName;
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gm",out gamemodeName))
            {
                Debug.Log(gamemodeName.ToString());
            }
        }*/
		int teamSize = 4;
		Text[] texts;
		ExitGames.Client.Photon.Hashtable costomProperties;
		/* DisableTeamPanel();
		UpdateTeamPanel();
        for(int i = 0;i<teamSize;i++)
        {
			if(!Team[i].activeSelf){
				Team[i].SetActive(true);
				texts=Team[i].GetComponentsInChildren<Text>();
				texts[0].text=PhotonNetwork.LocalPlayer.NickName;
				costomProperties = new ExitGames.Client.Photon.Hashtable()
				{
					{"TeamNum",i},
					{"isReady",false}
				};
			}
            PhotonNetwork.player.SetCustomProperties(costomProperties);
			break;
        }
		
		void DisableTeamPanel()
		{
			for(int i =0;i<4;i++)
			{
				Team[i].SetActive(false);
			}
		}
		
		void UpdateTeamPanel()
		{
			GameObject go;
			foreach(PhotonPlayer p in PhotonNetwork.playerList){
				costomProperties = p.customProperties;
				go = Team[(int)costomProperties["TeamNum"]];
				go.SetActive(true);
				texts=go.GetComponentsInChildren<Text>();
			}
			texts[0]=p.name;
		} */





    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log(message);
    }

    #endregion










}
