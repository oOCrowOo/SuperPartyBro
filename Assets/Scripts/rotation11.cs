using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class rotation11 : MonoBehaviour
{
    /// 转盘指针父对象的Transform
    private Transform roolPointer;

    /// 初始旋转速度
    private float initSpeed;

    /// 速度变化值
    private float changeSpeed = 0.5f;

    /// 转盘是否暂停
    private bool isPause = true;

    /// button事件，控制指针
    private Button button;

    void Awake()
    {
        //获取幸运转盘指针父对象的Transform
        roolPointer = GetComponent<Transform>();
        //获取button按钮
        button = transform.parent.Find("StartButton").GetComponent<Button>();
    }

    /// 开始抽奖，注册OnClick事件
    void OnEnable()
    {
        //清空button注册事件
        button.onClick.RemoveAllListeners();
        //注册OnClick事件
        button.onClick.AddListener(OnButtonClick);
    }

    /// 点击事件
    public void OnButtonClick()
    {
        if (isPause)
        {
            //随机生成一个初始速度
            initSpeed = Random.Range(100, 500);
            //开始旋转
            isPause = false;
        }
    }

    void Update()
    {
        if (!isPause)
        {
            //转动转盘(-1为顺时针,1为逆时针)
            roolPointer.Rotate(new Vector3(0, 0, -1) * initSpeed * Time.deltaTime);

            //让转动的速度缓缓降低
            initSpeed -= changeSpeed;

            //当转动的速度为0时转盘停止转动
            if (initSpeed <= 0)
            {
                //转动停止
                isPause = true;
            }
        }
    }
}
