/****************************************************
    文件：InventoryManager.cs
	作者：CaptainYun
    日期：2019/6/12 11:13:34
	功能：物品管理器
*****************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour {

    #region 单例模式
    private static InventoryManager _instance;

    public static InventoryManager Instance {
        get {
            if (_instance == null) {
                // 下面的代码只会执行一次，所以直接使用了 find
                _instance = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
            }
            return _instance;
        }
    }
    #endregion

    /// <summary>
    ///  物品信息的列表
    /// </summary>
    private List<Item> itemList;
    private Canvas canvas;

    #region ToolTip
    private ToolTip toolTip;
    private bool isToolTipShow = false;
    private Vector2 toolTipPosionOffset = new Vector2(20, -20); // pivot 的偏移
    #endregion

    #region PickedItem
    private bool isPickedItem = false;
    /// <summary>
    /// 鼠标是否选中物体
    /// </summary>
    public bool IsPickedItem {
        get {
            return isPickedItem;
        }
    }
    private ItemUI pickedItem;
    /// <summary>
    /// 鼠标选中的物体
    /// </summary>
    public ItemUI PickedItem {
        get {
            return pickedItem;
        }
    }
    #endregion

    void Awake() {
        ParseItemJson();
    }

    void Start() {
        toolTip = GameObject.FindObjectOfType<ToolTip>();
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        pickedItem = GameObject.Find("PickedItem").GetComponent<ItemUI>();
        pickedItem.Hide(); // 默认隐藏
    }

    void Update() {
        // 如果我们捡起了物品，我们就要让物品跟随鼠标
        if (isPickedItem) {
            Vector2 position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, null, out position);
            pickedItem.SetLocalPosition(position);
        }
        // 控制提示面板跟随鼠标
        else if (isToolTipShow) {
            Vector2 position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, null, out position);
            toolTip.SetLocalPotion(position + toolTipPosionOffset);
        }
        // 物品丢弃的处理，鼠标点击的地方没有任何的 UI
        if (isPickedItem && Input.GetMouseButtonDown(0) && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(-1) == false) {
            isPickedItem = false;
            PickedItem.Hide();
        }
    }

    /// <summary>
    /// 解析物品信息 - Json，添加到 list 里
    /// </summary>
    private void ParseItemJson() {
        itemList = new List<Item>();
        // 加载 Resources 下的 json 文件，文本为在 Unity 里面是 TextAsset 类型
        TextAsset itemText = Resources.Load<TextAsset>("Items");
        string itemsJson = itemText.text; // 物品信息的 Json 格式文本
        JSONObject jo = new JSONObject(itemsJson);
        foreach (JSONObject temp in jo.list) {
            // 解析 Items 里面的公有属性
            int id = (int)(temp["id"].n);
            string name = temp["name"].str;
            Item.ItemQuality quality = (Item.ItemQuality)System.Enum.Parse(typeof(Item.ItemQuality), temp["quality"].str);
            string description = temp["description"].str;
            int capacity = (int)(temp["capacity"].n);
            int buyPrice = (int)(temp["buyPrice"].n);
            int sellPrice = (int)(temp["sellPrice"].n);
            string sprite = temp["sprite"].str;
            string typeStr = temp["type"].str;
            // 解析 type，不同类型的物体拥有各自不同的属性
            Item.ItemType type = (Item.ItemType)System.Enum.Parse(typeof(Item.ItemType), typeStr); // 将 string 转 枚举
            Item item = null;
            switch (type) {
                case Item.ItemType.Consumable:
                    int hp = (int)(temp["hp"].n);
                    int mp = (int)(temp["mp"].n);
                    item = new Consumable(id, name, type, quality, description, capacity, buyPrice, sellPrice, sprite, hp, mp);
                    break;
                case Item.ItemType.Equipment:
                    int strength = (int)temp["strength"].n;
                    int intellect = (int)temp["intellect"].n;
                    int agility = (int)temp["agility"].n;
                    int stamina = (int)temp["stamina"].n;
                    Equipment.EquipmentType equipType = (Equipment.EquipmentType)System.Enum.Parse(typeof(Equipment.EquipmentType), temp["equipType"].str);
                    item = new Equipment(id, name, type, quality, description, capacity, buyPrice, sellPrice, sprite, strength, intellect, agility, stamina, equipType);
                    break;
                case Item.ItemType.Weapon:
                    int damage = (int)temp["damage"].n;
                    Weapon.WeaponType wpType = (Weapon.WeaponType)System.Enum.Parse(typeof(Weapon.WeaponType), temp["weaponType"].str);
                    item = new Weapon(id, name, type, quality, description, capacity, buyPrice, sellPrice, sprite, damage, wpType);
                    break;
                case Item.ItemType.Material:
                    item = new Material(id, name, type, quality, description, capacity, buyPrice, sellPrice, sprite);
                    break;
            }
            itemList.Add(item);
        }
    }

    public Item GetItemById(int id) {
        foreach (Item item in itemList) {
            if (item.ID == id) {
                return item;
            }
        }
        return null;
    }

    #region ToolTip
    public void ShowToolTip(string content) {
        if (this.isPickedItem) return;
        isToolTipShow = true;
        toolTip.Show(content);
    }

    public void HideToolTip() {
        isToolTipShow = false;
        toolTip.Hide();
    }
    #endregion

    /// <summary>
    /// 捡起物品槽指定数量的物品
    /// </summary>
    /// <param name="amount">默认一个</param>
    public void PickupItem(Item item, int amount) {
        PickedItem.SetItem(item, amount);
        isPickedItem = true;
        PickedItem.Show();
        this.toolTip.Hide();
        // 如果我们捡起了物品，我们就要让物品跟随鼠标
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, null, out position);
        pickedItem.SetLocalPosition(position);
    }

    /// <summary>
    /// 从手上拿掉指定数量物品放在物品槽里面
    /// </summary>
    /// <param name="amount">默认一个</param>
    public void RemoveItem(int amount = 1) {
        PickedItem.ReduceAmount(amount);
        if (PickedItem.Amount <= 0) {
            isPickedItem = false;
            PickedItem.Hide();
        }
    }

    public void SaveInventory() {
        Knapsack.Instance.SaveInventory();
        Chest.Instance.SaveInventory();
        CharacterPanel.Instance.SaveInventory();
        Forge.Instance.SaveInventory();
        PlayerPrefs.SetInt("CoinAmount", GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().CoinAmount);
    }

    public void LoadInventory() {
        Knapsack.Instance.LoadInventory();
        Chest.Instance.LoadInventory();
        CharacterPanel.Instance.LoadInventory();
        Forge.Instance.LoadInventory();
        if (PlayerPrefs.HasKey("CoinAmount")) {
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().CoinAmount = PlayerPrefs.GetInt("CoinAmount");
        }
    }

}