/****************************************************
	文件：Formula.cs
	作者：CaptainYun
	日期：2019/06/15 13:48   	
	功能：锻造配方
*****************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Formula {

    public int Item1ID { get; private set; } // 第一种材料
    public int Item1Amount { get; private set; }
    public int Item2ID { get; private set; } // 第二种材料
    public int Item2Amount { get; private set; }
    public int ResID { get; private set; } // 锻造结果的物品 
    private List<int> needIdList = new List<int>(); // 所需要的物品的 id，需要几个，就存几个 id

    public List<int> NeedIdList {
        get {
            return needIdList;
        }
    }

    public Formula(int item1ID, int item1Amount, int item2ID, int item2Amount, int resID) {
        this.Item1ID = item1ID;
        this.Item1Amount = item1Amount;
        this.Item2ID = item2ID;
        this.Item2Amount = item2Amount;
        this.ResID = resID;
        for (int i = 0; i < Item1Amount; i++) {
            needIdList.Add(Item1ID);
        }
        for (int i = 0; i < Item2Amount; i++) {
            needIdList.Add(Item2ID);
        }
    }

    /// <summary>
    /// 提供的物品是否符合某种锻造配方
    /// </summary>
    /// <param name="idList">提供的物品的 id</param>
    public bool Match(List<int> idList) {
        List<int> tempIDList = new List<int>(idList);
        // 判断 「提供的物品的 id」 是否包含 「所需要的物品的 id」
        foreach (int id in needIdList) {
            bool isSuccess = tempIDList.Remove(id);
            if (isSuccess == false) {
                return false;
            }
        }
        return true;
    }
}
