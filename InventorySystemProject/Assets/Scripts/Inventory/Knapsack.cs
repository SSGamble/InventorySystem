/****************************************************
	文件：Knapsack.cs
	作者：CaptainYun
	日期：2019/06/12 13:59   	
	功能：背包
*****************************************************/
using UnityEngine;
using System.Collections;

public class Knapsack : Inventory
{

    #region 单例模式
    private static Knapsack _instance;
    public static Knapsack Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance =  GameObject.Find("KnapsackPanel").GetComponent<Knapsack>();
            }
            return _instance;
        }
    }
    #endregion
}
