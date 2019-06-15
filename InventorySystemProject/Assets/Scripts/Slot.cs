/****************************************************
	文件：Slot.cs
	作者：CaptainYun
	日期：2019/06/12 13:57   	
	功能：物品槽
*****************************************************/

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

/// <summary>
/// 物品槽，子物体就是 Item 物品
/// </summary>
public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler {

    public GameObject itemPrefab;

    /// <summary>
    /// 把 item 放在 槽 里
    /// </summary>
    public void StoreItem(Item item) {
        // 槽里还没有东西，根据 itemPrefab 去实例化一个 item，放在下面
        if (transform.childCount == 0) {
            GameObject itemGameObject = Instantiate(itemPrefab) as GameObject;
            itemGameObject.transform.SetParent(this.transform);
            itemGameObject.transform.localScale = Vector2.one;
            itemGameObject.transform.localPosition = Vector2.zero;
            itemGameObject.transform.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            itemGameObject.GetComponent<ItemUI>().SetItem(item);
        }
        // 槽里已经有 item了，数量++
        else {
            transform.GetChild(0).GetComponent<ItemUI>().AddAmount();
        }
    }

    /// <summary>
    /// 得到当前物品槽 存储的物品类型
    /// </summary>
    /// <returns></returns>
    public Item.ItemType GetItemType() {
        return transform.GetChild(0).GetComponent<ItemUI>().Item.Type;
    }

    /// <summary>
    /// 得到物品的 id
    /// </summary>
    /// <returns></returns>
    public int GetItemId() {
        return transform.GetChild(0).GetComponent<ItemUI>().Item.ID;
    }

    /// <summary>
    /// 物品槽有没有满
    /// </summary>
    /// <returns></returns>
    public bool IsFilled() {
        ItemUI itemUI = transform.GetChild(0).GetComponent<ItemUI>();
        return itemUI.Amount >= itemUI.Item.Capacity; // 当前的数量大于等于容量
    }

    #region 鼠标事件
    public void OnPointerEnter(PointerEventData eventData) {
        if (transform.childCount > 0) {
            string toolTipText = transform.GetChild(0).GetComponent<ItemUI>().Item.GetToolTipText();
            InventoryManager.Instance.ShowToolTip(toolTipText);
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (transform.childCount > 0) {
            InventoryManager.Instance.HideToolTip();
        }
    }

    public virtual void OnPointerDown(PointerEventData eventData) {
        // 右键点击，穿戴装备
        if (eventData.button == PointerEventData.InputButton.Right) {
            if (InventoryManager.Instance.IsPickedItem == false && transform.childCount > 0) {
                ItemUI currentItemUI = transform.GetChild(0).GetComponent<ItemUI>();
                if (currentItemUI.Item is Equipment || currentItemUI.Item is Weapon) {
                    currentItemUI.ReduceAmount(1);
                    Item currentItem = currentItemUI.Item;
                    if (currentItemUI.Amount <= 0) {
                        DestroyImmediate(currentItemUI.gameObject);
                        InventoryManager.Instance.HideToolTip();
                    }
                    CharacterPanel.Instance.PutOn(currentItem);
                }
            }
        }

        if (eventData.button != PointerEventData.InputButton.Left) return;
        // 自身不是空
        if (transform.childCount > 0) {
            ItemUI currentItem = transform.GetChild(0).GetComponent<ItemUI>();
            // 当前鼠标没有选中任何物品
            if (InventoryManager.Instance.IsPickedItem == false) {
                // ctrl 按下，取得当前物品槽中物品的一半
                if (Input.GetKey(KeyCode.LeftControl)) {
                    int amountPicked = (currentItem.Amount + 1) / 2; // 捡起的数量
                    InventoryManager.Instance.PickupItem(currentItem.Item, amountPicked);
                    int amountRemained = currentItem.Amount - amountPicked; // 余下的数量
                    if (amountRemained <= 0) {
                        Destroy(currentItem.gameObject); // 销毁当前物品
                    }
                    else {
                        currentItem.SetAmount(amountRemained); // 更新数量
                    }
                }
                // ctrl 没有按下，把当前物品槽里面的物品放到鼠标上
                else {
                    InventoryManager.Instance.PickupItem(currentItem.Item, currentItem.Amount);
                    Destroy(currentItem.gameObject); // 销毁当前物品
                }
            }
            // 当前鼠标有选中物品
            else {
                // 自身的 id == pickedItem.id，整合物品
                if (currentItem.Item.ID == InventoryManager.Instance.PickedItem.Item.ID) {
                    // 按下 ctrl，放置当前鼠标上的物品的一个，一个一个放置物品
                    if (Input.GetKey(KeyCode.LeftControl)) {
                        // 当前物品槽还有容量
                        if (currentItem.Item.Capacity > currentItem.Amount) {
                            currentItem.AddAmount();
                            InventoryManager.Instance.RemoveItem();
                        }
                        else {
                            return;
                        }
                    }
                    // 没有按下 ctrl，放置当前鼠标上的物品的所有
                    else {
                        if (currentItem.Item.Capacity > currentItem.Amount) {
                            int amountRemain = currentItem.Item.Capacity - currentItem.Amount; // 当前物品槽剩余的空间
                            // 容量够，可以完全放下
                            if (amountRemain >= InventoryManager.Instance.PickedItem.Amount) {
                                currentItem.SetAmount(currentItem.Amount + InventoryManager.Instance.PickedItem.Amount);
                                InventoryManager.Instance.RemoveItem(InventoryManager.Instance.PickedItem.Amount);
                            }
                            // 容量不够，只能放下其中一部分
                            else {
                                currentItem.SetAmount(currentItem.Amount + amountRemain);
                                InventoryManager.Instance.RemoveItem(amountRemain);
                            }
                        }
                        else {
                            return;
                        }
                    }
                }
                // 自身的 id != pickedItem.id，pickedItem 跟当前物品交换
                else {
                    Item item = currentItem.Item;
                    int amount = currentItem.Amount;
                    currentItem.SetItem(InventoryManager.Instance.PickedItem.Item, InventoryManager.Instance.PickedItem.Amount);
                    InventoryManager.Instance.PickedItem.SetItem(item, amount);
                }
            }
        }
        // 自身是空  
        else {
            // IsPickedItem == true，pickedItem 放在这个位置
            if (InventoryManager.Instance.IsPickedItem == true) {
                // 按下 ctrl，放置当前鼠标上的物品的一个，一个一个放
                if (Input.GetKey(KeyCode.LeftControl)) {
                    this.StoreItem(InventoryManager.Instance.PickedItem.Item);
                    InventoryManager.Instance.RemoveItem();
                }
                // 没有按下 ctrl ，放置当前鼠标上所有的物品
                else {
                    for (int i = 0; i < InventoryManager.Instance.PickedItem.Amount; i++) {
                        this.StoreItem(InventoryManager.Instance.PickedItem.Item);
                    }
                    InventoryManager.Instance.RemoveItem(InventoryManager.Instance.PickedItem.Amount); // 把手上的清空
                }
            }
            // IsPickedItem == false，不做任何处理
            else {
                return;
            }
        }
    }
    #endregion

}
