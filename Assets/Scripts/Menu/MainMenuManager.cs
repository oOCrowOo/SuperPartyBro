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
	
    private bool firstConnect = true;

    public AnimationClip fadeoutClip;
    public AnimationClip moveinClip;
    public AnimationClip popupClip;
    public AnimationClip disappearClip;


    // Start is called before the first frame update
    void Start()
    {   
        // TODO: debug usage, remove later
        PlayerPrefs.DeleteAll();
        name_panel.SetActive(true);
        
        name_panel.GetComponent<Animation>().Play();
        Debug.Log(name_panel.GetComponent<Animation>().clip);
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

    public IEnumerator switchPanel(GameObject oldPanel, GameObject newPanel, AnimationClip leaveClip, AnimationClip inClip)
    {
        // Play the fade out animation for the old panel
        Animation fadeoutAnimation = oldPanel.GetComponent<Animation>();
        float duration = leaveClip.length;
        fadeoutAnimation.clip = leaveClip;
        fadeoutAnimation.Play();
        // after the seconds of the clip length
        yield return new WaitForSeconds(duration);

        // Play the move in animation for the new panel
        newPanel.SetActive(true);
        Animation moveinAnimation = newPanel.GetComponent<Animation>();
        moveinAnimation.clip = inClip;
        moveinAnimation.Play();
        oldPanel.SetActive(false);
        oldPanel.GetComponent<Transform>().localPosition = new Vector3(0,0,0);
    }

    public void onClickStart(){
        // create_join_panel.SetActive(true);
        // //main_panel.GetComponent<Animation>().Play();
        // main_panel.SetActive(false);

        StartCoroutine(switchPanel(main_panel,create_join_panel,fadeoutClip,moveinClip));
         
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
        StartCoroutine(switchPanel(main_panel,about_panel,fadeoutClip,moveinClip));
        // about_panel.SetActive(true);
        // main_panel.SetActive(false);
    }

    public void onClickBack_Join(){
        StartCoroutine(switchPanel(pin_panel,create_join_panel,disappearClip,moveinClip));
    }

    public void onClickBack_Create_Join(){
        StartCoroutine(switchPanel(create_join_panel,main_panel,fadeoutClip,moveinClip));
        // create_join_panel.SetActive(false);
        // main_panel.SetActive(true);
    }

    public void onClickBack_Room(){
        //resetPlayers();
        RoomPanel.SetActive(false);
        create_join_panel.SetActive(true);
    }

    public string getPIN(){
        return PIN.text;
    }

   /* private void resetPlayers(){
        for (int i = 1;i<=4;i++){
            setPlayerName("player" + i.ToString(),"waiting...");
             Image readyImg = GameObject.Find("player" + i.ToString()).GetComponentsInChildren<Image>()[1];
            readyImg.enabled = false;
        }
    }*/

    public void onClickBack_About(){
        StartCoroutine(switchPanel(about_panel,main_panel,fadeoutClip,moveinClip));
        // about_panel.SetActive(false);
        // main_panel.SetActive(true);
    }

    public void onClickQuit(){
        Application.Quit();
    }

    public void onClickCreate(){
        //host = true;
        // player_num = "player1";
        // random pin number every time
        PIN.text = Random.Range(1000,9999).ToString();
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
        // create_join_panel.SetActive(false);
        // RoomPanel.SetActive(true);
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
        // create_join_panel.SetActive(false);
        // pin_panel.SetActive(true);
        
        StartCoroutine(switchPanel(create_join_panel,pin_panel,fadeoutClip,popupClip));
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
        if(firstConnect){
            firstConnect = false;
            //close name panel when connected to server
            StartCoroutine(switchPanel(name_panel,main_panel,disappearClip,moveinClip));
            // main_panel.SetActive(true);
            // name_panel.SetActive(false);
            Debug.Log(PhotonNetwork.LocalPlayer.NickName +" is Succuessfully connect to internet");
        }
        
    }
    public override void OnCreatedRoom()
    {
        Debug.Log("room with password" + PhotonNetwork.CurrentRoom.Name + "is create ");
        // create_join_panel.SetActive(false);
        // RoomPanel.SetActive(true);
        StartCoroutine(switchPanel(create_join_panel,RoomPanel,fadeoutClip,moveinClip));
    }
    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName +" is joined to password room:" + PhotonNetwork.CurrentRoom.Name+ "playercount  " + PhotonNetwork.CurrentRoom.PlayerCount);
        StartCoroutine(switchPanel(create_join_panel,RoomPanel,fadeoutClip,moveinClip));
        //查看选的游戏是什么，以后可以根据用户的选择把人带进不同的游戏
        /* if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("gm"))
        {
            object gamemodeName;
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gm",out gamemodeName))
            {
                Debug.Log(gamemodeName.ToString());
            }
        }*/
		// int teamSize = 4;
		// Text[] texts;
		// ExitGames.Client.Photon.Hashtable costomProperties;
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
