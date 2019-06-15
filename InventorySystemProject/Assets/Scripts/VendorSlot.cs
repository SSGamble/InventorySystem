/****************************************************
	文件：VendorSlot.cs
	作者：CaptainYun
	日期：2019/06/13 22:19   	
	功能：商店物品槽
*****************************************************/

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class VendorSlot : Slot {

    public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData) {
        // 右键购买
        if (eventData.button == PointerEventData.InputButton.Right && InventoryManager.Instance.IsPickedItem == false) {
            if (transform.childCount > 0) {
                Item currentItem = transform.GetChild(0).GetComponent<ItemUI>().Item;
                transform.parent.parent.SendMessage("BuyItem", currentItem);
            }
        }
        // 左键出售
        else if (eventData.button == PointerEventData.InputButton.Left && InventoryManager.Instance.IsPickedItem == true) {
            transform.parent.parent.SendMessage("SellItem");
        }

    }
}
