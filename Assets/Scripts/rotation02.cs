﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class rotation02 : MonoBehaviour
{
    private Transform roolPointer;
    private float initSpeed;
    private float changeSpeed = 0.8f;
    private bool isPause = true;
    private Button button;
    //8个Item
    private List<RewardItem> allItems;

    //封装 Item
    private class RewardItem
    {
        public RewardItem(string name, float rank, float ang1, float ang2)
        {
            this.ItemName = name;
            this.ItemRank = rank;
            this.MinAngle = ang1;
            this.MaxAngle = ang2;
        }
        public string ItemName { get; set; }
        //奖项概率
        public float ItemRank { get; set; }
        public float MaxAngle { get; set; }
        public float MinAngle { get; set; }
    }

    
    private string Reward()
    {
        string result = "";
        float totalRank = 100;

        allItems = new List<RewardItem>()
        {
           //8个item，平分概率和夹角，图文尚未对应
            new RewardItem("reward1",13,0,45),
            new RewardItem("reward2",13,45,90),
            new RewardItem("reward3",13,90,135),
            new RewardItem("reward4",13,135,180),
            new RewardItem("reward5",13,180,225),
            new RewardItem("reward6",13,225,270),
            new RewardItem("reward7",13,270,315),
            new RewardItem("reward8",13,315,360),
        };

        foreach (RewardItem item in allItems)
        {
            //产生一个0到100之间的随机数
            float random = Random.Range(0, totalRank);
            //将该随机数和奖品的概率比较
            if (random <= item.ItemRank)
            {
                result = item.ItemName;
                float angle = Random.Range(item.MinAngle, item.MaxAngle);
                //旋转转盘指针

                ////....
                //roolPointer.Rotate(new Vector3(0, 0, -1) * initSpeed * Time.deltaTime);
                //initSpeed -= changeSpeed;
                //if (initSpeed <= 0)
                //{
                //    isPause = true;
                //}
                ////....

                break;
            }
            else
            {
                totalRank -= item.ItemRank;
            }


    
        }

        return "抽奖结果为:" + result;
    }

    void Awake()
    {
        roolPointer = GetComponent<Transform>();
        button = transform.parent.Find("StartButton").GetComponent<Button>();
    }

    /// 开始抽奖，注册OnClick事件
    void OnEnable()
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnButtonClick);
    }


    public void OnButtonClick()
    {
        if (isPause)
        {
            initSpeed = Random.Range(100, 400);
            isPause = false;
        }

    }

    void Update()
    {
        if (!isPause)
        {
            //转动转盘(-1为顺时针, 1为逆时针)
            roolPointer.Rotate(new Vector3(0, 0, -1) * initSpeed * Time.deltaTime);
            initSpeed -= changeSpeed;
            if (initSpeed <= 0)
            {
                isPause = true;
                Debug.Log(Reward());
            }

   


        }


}

    


}
