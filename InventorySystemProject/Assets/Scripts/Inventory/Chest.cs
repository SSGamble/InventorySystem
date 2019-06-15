/****************************************************
	文件：Chest.cs
	作者：CaptainYun
	日期：2019/06/12 22:21   	
	功能：箱子
*****************************************************/
using UnityEngine;
using System.Collections;

public class Chest : Inventory {
    #region 单例模式
    private static Chest _instance;
    public static Chest Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("ChestPanel").GetComponent<Chest>();
            }
            return _instance;
        }
    }
    #endregion
}
