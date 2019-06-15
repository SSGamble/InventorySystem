/****************************************************
	文件：EquipmentSlot.cs
	作者：CaptainYun
	日期：2019/06/13 15:04   	
	功能：装备物品槽，只能存放对应类型的装备
*****************************************************/

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class EquipmentSlot : Slot {

    public Equipment.EquipmentType equipType; // 当前物品槽可以存放的装备类型
    public Weapon.WeaponType wpType; // 当前物品槽可以放什么类型的武器

    public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData) {
        // 鼠标右键卸下装备
        if (eventData.button == PointerEventData.InputButton.Right) {
            if (InventoryManager.Instance.IsPickedItem == false && transform.childCount > 0) {
                ItemUI currentItemUI = transform.GetChild(0).GetComponent<ItemUI>();
                Item itemTemp = currentItemUI.Item;
                DestroyImmediate(currentItemUI.gameObject);
                // 脱掉放到背包里面
                transform.parent.SendMessage("PutOff", itemTemp);
                InventoryManager.Instance.HideToolTip();
            }
        }

        if (eventData.button != PointerEventData.InputButton.Left) return;
        
        bool isUpdateProperty = false; // 是否需要更新人物属性

        // 手上有东西
        if (InventoryManager.Instance.IsPickedItem == true) {
            ItemUI pickedItem = InventoryManager.Instance.PickedItem; // 手上的物品
            // 当前装备槽有装备，交换
            if (transform.childCount > 0) {
                ItemUI currentItemUI = transform.GetChild(0).GetComponent<ItemUI>(); // 当前装备槽里面的物品
                if (IsRightItem(pickedItem.Item)) {
                    InventoryManager.Instance.PickedItem.Exchange(currentItemUI);
                    isUpdateProperty = true;
                }
            }
            // 当前装备槽无装备，放下
            else {
                if (IsRightItem(pickedItem.Item)) {
                    this.StoreItem(InventoryManager.Instance.PickedItem.Item);
                    InventoryManager.Instance.RemoveItem(1);
                    isUpdateProperty = true;
                }

            }
        }
        // 手上没东西
        else {
            // 当前装备槽有装备，捡起
            if (transform.childCount > 0) {
                ItemUI currentItemUI = transform.GetChild(0).GetComponent<ItemUI>();
                InventoryManager.Instance.PickupItem(currentItemUI.Item, currentItemUI.Amount);
                Destroy(currentItemUI.gameObject);
                isUpdateProperty = true;
            }
        }
        // 更新人物属性
        if (isUpdateProperty) {
            transform.parent.SendMessage("UpdatePropertyText");
        }
    }

    /// <summary>
    /// 判断 item 是否适合放在这个位置
    /// </summary>
    public bool IsRightItem(Item item) {
        if ((item is Equipment && ((Equipment)(item)).EquipType == this.equipType) ||
                    (item is Weapon && ((Weapon)(item)).WpType == this.wpType)) {
            return true;
        }
        return false;
    }
}
