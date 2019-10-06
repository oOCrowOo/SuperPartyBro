using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Photon;
using Photon.Pun;
using Photon.Realtime;

public class RoomPanelController : MonoBehaviourPun {

	public GameObject lobbyPanel;		//游戏大厅面板
	public GameObject roomPanel;		//游戏房间面板
	public Button backButton;			//返回按钮
	public GameObject[] Team;			//队伍面板（显示队伍信息）
	public Button readyButton;			//准备/开始游戏按钮
	public Text promptMessage;			//提示信息

	public Text PIN;


	private MainMenuManager mManager;

	PhotonView pView;
	int teamSize;
	Text[] texts;
	ExitGames.Client.Photon.Hashtable costomProperties;

	void OnEnable () {
		mManager = GameObject.Find("MainMenuManager").GetComponent<MainMenuManager>();
		PIN.text = mManager.getPIN();
		pView = GetComponent<PhotonView>();					//获取PhotonView组件
		if(!PhotonNetwork.IsConnected)return;
		promptMessage.text = "";							//提示信息

		// master client will ask all other players to load the same scene
		PhotonNetwork.AutomaticallySyncScene = true;
		

		backButton.onClick.RemoveAllListeners ();			//移除返回按钮绑定的所有监听事件
		backButton.onClick.AddListener (delegate() {		//为返回按钮绑定新的监听事件
			PhotonNetwork.LeaveRoom ();						//客户端离开游戏房间
			StartCoroutine(mManager.switchPanel(mManager.RoomPanel,mManager.create_join_panel,mManager.fadeoutClip,mManager.moveinClip));					//禁用游戏房间面板
		});

		teamSize = 4;		//计算每队人数
		DisableTeamPanel ();								//初始化队伍面板
		UpdateTeamPanel (false);							//更新队伍面板（false表示不显示本地玩家信息）

		//Debug.Log(PhotonNetwork.IsMasterClient);

		for (int i = 0; i < teamSize; i++) {	
			if (!Team [i].activeSelf) {		//在队伍找到空余位置
				Team [i].SetActive (true);		//激活对应的队伍信息UI
				texts = Team [i].GetComponentsInChildren<Text> ();
				texts [0].text = PhotonNetwork.LocalPlayer.NickName;				//显示玩家昵称
				if(PhotonNetwork.IsMasterClient)texts[1].text="Host";	//如果玩家是MasterClient，玩家状态显示"房主"
				else texts [1].text = "not ready";							//如果玩家不是MasterClient，玩家状态显示"未准备"
				costomProperties = new ExitGames.Client.Photon.Hashtable () {	//初始化玩家自定义属性
					{ "TeamNum",i },		//玩家队伍序号
					{ "isReady",false },	//玩家准备状态
					// { "Score",0 }			//玩家得分
				};
				PhotonNetwork.SetPlayerCustomProperties (costomProperties);	//将玩家自定义属性赋予玩家
				break;
			}
		}
		ReadyButtonControl ();	//设置ReadyButton的按钮事件
	}

	void Update(){
		UpdateTeamPanel (true);
	}

	
	// 禁用队伍面板
	void DisableTeamPanel(){
		for (int i = 0; i < Team.Length; i++) {
			Team [i].SetActive (false);
		}
	}

	/**在队伍面板显示玩家信息
	 * 函数参数表示是否显示本地玩家信息
	 */
	void UpdateTeamPanel(bool isUpdateSelf){
		GameObject go;
		foreach (Player p in PhotonNetwork.PlayerList) {	//获取房间里所有玩家信息
			if (!isUpdateSelf && p.IsLocal)	continue;			//判断是否更新本地玩家信息
			costomProperties = p.CustomProperties;				//获取玩家自定义属性
				go = Team [(int)costomProperties ["TeamNum"]];	//查询玩家的队伍序号
				go.SetActive (true);							//激活显示玩家信息的UI
				texts = go.GetComponentsInChildren<Text> ();	//获取显示玩家信息的Text组件
			texts [0].text = p.NickName;						//显示玩家姓名
			if(p.IsMasterClient)							//如果玩家是MasterClient
				texts[1].text="Host";						//玩家状态显示"房主"
			else if ((bool)costomProperties ["isReady"]) {	//如果玩家不是MasterClient，获取玩家的准备状态isReady
				texts [1].text = "Ready";					//isReady为true，显示"已准备"
			} else
				texts [1].text = "Not Ready";					//isReady为false，显示"未准备"
		}
	}

	// ReadyButton按钮事件设置
	void ReadyButtonControl(){
		if (PhotonNetwork.IsMasterClient) {									//如果玩家是MasterClient
			readyButton.GetComponentInChildren<Text> ().text = "Start";	//ReadyButton显示"开始游戏"
			readyButton.onClick.RemoveAllListeners ();						//移除ReadyButton所有监听事件
			readyButton.onClick.AddListener (delegate() {					//为ReadyButton绑定新的监听事件
				ClickStartGameButton ();									//开始游戏
			});
		} else {															//如果玩家不是MasterClient
			if((bool)PhotonNetwork.LocalPlayer.CustomProperties["isReady"])		//根据玩家准备状态显示对应的文本信息
				readyButton.GetComponentInChildren<Text> ().text = "Not Ready";		
			else 
				readyButton.GetComponentInChildren<Text> ().text = "Ready";
			readyButton.onClick.RemoveAllListeners ();						//移除ReadyButton所有监听事件
			readyButton.onClick.AddListener (delegate() {					//为ReadyButton绑定新的监听事件
				ClickReadyButton ();										//切换准备状态
			});
		}
	}

	

	// 准备按钮事件响应函数
	public void ClickReadyButton(){
		bool isReady = (bool)PhotonNetwork.LocalPlayer.CustomProperties ["isReady"];					//获取玩家准备状态
		costomProperties = new ExitGames.Client.Photon.Hashtable (){ { "isReady",!isReady } };	//重新设置玩家准备状态
		PhotonNetwork.SetPlayerCustomProperties (costomProperties);
		Text readyButtonText = readyButton.GetComponentInChildren<Text> ();	//获取ReadyButton的按钮文本
		if(isReady)readyButtonText.text="Ready";		//根据玩家点击按钮后的状态，设置按钮文本的显示
		else readyButtonText.text="Not Ready";
	}

	// 开始游戏按钮事件响应函数
	public void ClickStartGameButton(){
		foreach (Player p in PhotonNetwork.PlayerList) {		//遍历房间内所有玩家
			if (p.IsLocal) continue;								//不检查MasterClient房主的准备状态
			if ((bool)p.CustomProperties ["isReady"] == false) {	//如果有人未准备
				promptMessage.text = "有人未准备，游戏无法开始";		//提示信息显示"有人未准备，游戏无法开始"
				return;												//结束函数执行
			}
		}
		promptMessage.text = "";										//清空提示信息
		// PhotonNetwork.CurrentRoom.Open = false;								//设置房间的open属性，使游戏大厅的玩家无法加入此房间

		PhotonNetwork.LoadLevel("BoxScene");
																
	}

	
}
