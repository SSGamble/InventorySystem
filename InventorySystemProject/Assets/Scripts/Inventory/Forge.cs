/****************************************************
	文件：Forge.cs
	作者：CaptainYun
	日期：2019/06/15 13:53   	
	功能：锻造
*****************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Forge : Inventory {

    #region 单例模式
    private static Forge _instance;
    public static Forge Instance {
        get {
            if (_instance == null) {
                _instance = GameObject.Find("ForgePanel").GetComponent<Forge>();
            }
            return _instance;
        }
    }
    #endregion

    private List<Formula> formulaList; // 配方

    public override void Start() {
        base.Start();
        ParseFormulaJson();
    }

    void ParseFormulaJson() {
        formulaList = new List<Formula>();
        TextAsset formulasText = Resources.Load<TextAsset>("Formulas");
        string formulasJson = formulasText.text;//配方信息的Json数据
        JSONObject jo = new JSONObject(formulasJson);
        foreach (JSONObject temp in jo.list) {
            int item1ID = (int)temp["Item1ID"].n;
            int item1Amount = (int)temp["Item1Amount"].n;
            int item2ID = (int)temp["Item2ID"].n;
            int item2Amount = (int)temp["Item2Amount"].n;
            int resID = (int)temp["ResID"].n;
            Formula formula = new Formula(item1ID, item1Amount, item2ID, item2Amount, resID);
            formulaList.Add(formula);
        }
        //Debug.Log(formulaList[1].ResID);
    }

    /// <summary>
    /// 锻造物品
    /// </summary>
    public void ForgeItem() {
        // 得到当前有哪些材料
        List<int> haveMaterialIDList = new List<int>(); // 当前拥有的 材料 id
        foreach (Slot slot in slotList) {
            if (slot.transform.childCount > 0) {
                ItemUI currentItemUI = slot.transform.GetChild(0).GetComponent<ItemUI>();
                for (int i = 0; i < currentItemUI.Amount; i++) {
                    haveMaterialIDList.Add(currentItemUI.Item.ID); //  这个格子里面有多少个物品 就存储多少个 id
                }
            }
        }
        // 判断满足哪一个秘籍的要求
        Formula matchedFormula = null; // 匹配的秘籍
        foreach (Formula formula in formulaList) {
            bool isMatch = formula.Match(haveMaterialIDList);
            if (isMatch) {
                matchedFormula = formula;
                break;
            }
        }
        // 获得成品 并 去掉消耗的材料
        if (matchedFormula != null) {
            Knapsack.Instance.StoreItem(matchedFormula.ResID);
            foreach (int id in matchedFormula.NeedIdList) {
                foreach (Slot slot in slotList) {
                    if (slot.transform.childCount > 0) {
                        ItemUI itemUI = slot.transform.GetChild(0).GetComponent<ItemUI>();
                        if (itemUI.Item.ID == id && itemUI.Amount > 0) {
                            itemUI.ReduceAmount();
                            if (itemUI.Amount <= 0) {
                                DestroyImmediate(itemUI.gameObject);
                            }
                            break;
                        }
                    }
                }
            }
        }
    }
}
