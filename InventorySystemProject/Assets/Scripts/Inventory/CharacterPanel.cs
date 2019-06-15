/****************************************************
	文件：CharacterPanel.cs
	作者：CaptainYun
	日期：2019/06/13 15:02   	
	功能：角色面板
*****************************************************/
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CharacterPanel : Inventory {
    #region 单例模式
    private static CharacterPanel _instance;
    public static CharacterPanel Instance {
        get {
            if (_instance == null) {
                _instance = GameObject.Find("CharacterPanel").GetComponent<CharacterPanel>();
            }
            return _instance;
        }
    }
    #endregion

    private Text propertyText; // 属性面板的角色属性信息
    private Player player;

    public override void Start() {
        base.Start();
        propertyText = transform.Find("PropertyPanel/Text").GetComponent<Text>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        UpdatePropertyText();
        Hide();
    }

    /// <summary>
    /// 穿上装备
    /// </summary>
    /// <param name="item"></param>
    public void PutOn(Item item) {
        Item exitItem = null; // 如果槽里已有装备了，要卸下
        foreach (Slot slot in slotList) {
            EquipmentSlot equipmentSlot = (EquipmentSlot)slot;
            // 找到合适的槽
            if (equipmentSlot.IsRightItem(item)) {
                // 槽里有装备了
                if (equipmentSlot.transform.childCount > 0) {
                    ItemUI currentItemUI = equipmentSlot.transform.GetChild(0).GetComponent<ItemUI>();
                    exitItem = currentItemUI.Item; // 卸下的装备
                    currentItemUI.SetItem(item, 1); // 放入新装备
                }
                // 槽里没装备，直接穿上
                else {
                    equipmentSlot.StoreItem(item);
                }
                break;
            }
        }
        // 将卸下的装备放入背包中
        if (exitItem != null) {
            Knapsack.Instance.StoreItem(exitItem);
        }
        UpdatePropertyText();
    }

    /// <summary>
    /// 卸下装备
    /// </summary>
    /// <param name="item"></param>
    public void PutOff(Item item) {
        Knapsack.Instance.StoreItem(item);
        UpdatePropertyText();
    }

    /// <summary>
    /// 更新属性信息，穿戴装备会有影响
    /// </summary>
    private void UpdatePropertyText() {
        int strength = 0, intellect = 0, agility = 0, stamina = 0, damage = 0;
        // 装备属性加成
        foreach (EquipmentSlot slot in slotList) {
            if (slot.transform.childCount > 0) {
                Item item = slot.transform.GetChild(0).GetComponent<ItemUI>().Item;
                if (item is Equipment) {
                    Equipment e = (Equipment)item;
                    strength += e.Strength;
                    intellect += e.Intellect;
                    agility += e.Agility;
                    stamina += e.Stamina;
                }
                else if (item is Weapon) {
                    damage += ((Weapon)item).Damage;
                }
            }
        }
        strength += player.BasicStrength;
        intellect += player.BasicIntellect;
        agility += player.BasicAgility;
        stamina += player.BasicStamina;
        damage += player.BasicDamage;
        string text = string.Format("力量：{0}\n智力：{1}\n敏捷：{2}\n体力：{3}\n攻击力：{4} ", strength, intellect, agility, stamina, damage);
        propertyText.text = text;
    }

}
