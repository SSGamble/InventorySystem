/****************************************************
	文件：Inventory.cs
	作者：CaptainYun
	日期：2019/06/12 14:00   	
	功能：背包基类，背包，箱子
*****************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

public class Inventory : MonoBehaviour {

    protected Slot[] slotList;

    #region 控制显示隐藏
    private float targetAlpha = 1;
    private float smoothing = 4;
    private CanvasGroup canvasGroup;
    #endregion

    public virtual void Start() {
        slotList = GetComponentsInChildren<Slot>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update() {
        // 显示隐藏的渐变
        if (canvasGroup.alpha != targetAlpha) {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, smoothing * Time.deltaTime);
            if (Mathf.Abs(canvasGroup.alpha - targetAlpha) < .01f) {
                canvasGroup.alpha = targetAlpha;
            }
        }
    }

    /// <summary>
    /// 存储物品
    /// </summary>
    public bool StoreItem(int id) {
        Item item = InventoryManager.Instance.GetItemById(id);
        return StoreItem(item);
    }

    /// <summary>
    /// 存储物品
    /// </summary>
    /// <param name="item">物品</param>
    /// <returns>是否存储成功</returns>
    public bool StoreItem(Item item) {
        if (item == null) {
            Debug.LogWarning("要存储的物品不存在");
            return false;
        }
        // 一个槽只能放一个
        if (item.Capacity == 1) {
            Slot slot = FindEmptySlot();
            if (slot == null) {
                Debug.LogWarning("没有空的物品槽");
                return false;
            }
            else {
                slot.StoreItem(item);//把物品存储到这个空的物品槽里面
            }
        }
        // 一个槽可以放多个
        else {
            // 先找有没有槽已经存有这个 item 了
            Slot slot = FindSameIdSlot(item);
            if (slot != null) {
                slot.StoreItem(item);
            }
            else {
                Slot emptySlot = FindEmptySlot();
                if (emptySlot != null) {
                    emptySlot.StoreItem(item);
                }
                else {
                    Debug.LogWarning("没有空的物品槽");
                    return false;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// 找一个空的物品槽
    /// </summary>
    private Slot FindEmptySlot() {
        foreach (Slot slot in slotList) {
            if (slot.transform.childCount == 0) {
                return slot;
            }
        }
        return null;
    }

    /// <summary>
    /// 找同样 物品ID 的物品槽
    /// </summary>
    private Slot FindSameIdSlot(Item item) {
        foreach (Slot slot in slotList) {
            if (slot.transform.childCount >= 1 && slot.GetItemId() == item.ID && slot.IsFilled() == false) {
                return slot;
            }
        }
        return null;
    }

    public void Show() {
        canvasGroup.blocksRaycasts = true; // 可以交互
        targetAlpha = 1;
    }

    public void Hide() {
        canvasGroup.blocksRaycasts = false; // 不可以交互
        targetAlpha = 0;
    }

    /// <summary>
    /// 背包的显示和隐藏开关
    /// </summary>
    public void DisplaySwitch() {
        if (targetAlpha == 0) {
            Show();
        }
        else {
            Hide();
        }
    }

    #region save and load
    public void SaveInventory() {
        StringBuilder sb = new StringBuilder();
        foreach (Slot slot in slotList) {
            if (slot.transform.childCount > 0) {
                ItemUI itemUI = slot.transform.GetChild(0).GetComponent<ItemUI>();
                sb.Append(itemUI.Item.ID + "," + itemUI.Amount + "-");
            }
            else {
                sb.Append("0-");
            }
        }
        PlayerPrefs.SetString(this.gameObject.name, sb.ToString());
    }

    public void LoadInventory() {
        if (PlayerPrefs.HasKey(this.gameObject.name) == false) return;
        string str = PlayerPrefs.GetString(this.gameObject.name);
        string[] itemArray = str.Split('-');
        for (int i = 0; i < itemArray.Length - 1; i++) {
            string itemStr = itemArray[i];
            if (itemStr != "0") {
                string[] temp = itemStr.Split(',');
                int id = int.Parse(temp[0]);
                Item item = InventoryManager.Instance.GetItemById(id);
                int amount = int.Parse(temp[1]);
                for (int j = 0; j < amount; j++) {
                    slotList[i].StoreItem(item);
                }
            }
        }
    }
    #endregion
}
