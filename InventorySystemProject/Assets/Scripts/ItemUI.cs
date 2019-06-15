/****************************************************
	文件：ItemUI.cs
	作者：CaptainYun
	日期：2019/06/12 13:59   	
	功能：物品 UI 信息
*****************************************************/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour {

    #region Data
    public Item Item { get; private set; }
    public int Amount { get; private set; }
    #endregion

    #region UI Component
    private Image itemImage;
    private Image ItemImage {
        get {
            if (itemImage == null) {
                itemImage = GetComponent<Image>();
            }
            return itemImage;
        }
    }
    private Text amountText;
    private Text AmountText {
        get {
            if (amountText == null) {
                amountText = GetComponentInChildren<Text>();
            }
            return amountText;
        }
    }
    #endregion

    #region 动画
    private float targetScale = 1f; // 物品的目标大小
    private Vector3 animationScale = new Vector3(1.4f, 1.4f, 1.4f); // 动画缩放大小
    private float smoothing = 4; // 渐变速度
    #endregion

    void Update() {
        // 动画
        if (transform.localScale.x != targetScale) {
            float scale = Mathf.Lerp(transform.localScale.x, targetScale, smoothing * Time.deltaTime);
            transform.localScale = new Vector3(scale, scale, scale);
            // 临界值判断，防止插值运算的性能损耗
            if (Mathf.Abs(transform.localScale.x - targetScale) < 0.02f) {
                transform.localScale = new Vector3(targetScale, targetScale, targetScale);
            }
        }
    }

    /// <summary>
    /// 设置显示 Item
    /// </summary>
    public void SetItem(Item item, int amount = 1) {
        transform.localScale = animationScale;
        this.Item = item;
        this.Amount = amount;
        // update ui 
        ItemImage.sprite = Resources.Load<Sprite>(item.Sprite);
        if (Item.Capacity > 1)
            AmountText.text = Amount.ToString();
        else
            AmountText.text = "";
    }

    /// <summary>
    /// 增加 Item 数量
    /// </summary>
    public void AddAmount(int amount = 1) {
        transform.localScale = animationScale;
        this.Amount += amount;
        // update ui 
        if (Item.Capacity > 1)
            AmountText.text = Amount.ToString();
        else
            AmountText.text = "";
    }

    /// <summary>
    /// 减少 Item 数量
    /// </summary>
    public void ReduceAmount(int amount = 1) {
        transform.localScale = animationScale;
        this.Amount -= amount;
        //update ui 
        if (Item.Capacity > 1)
            AmountText.text = Amount.ToString();
        else
            AmountText.text = "";
    }

    /// <summary>
    /// 更新数量
    /// </summary>
    /// <param name="amount"></param>
    public void SetAmount(int amount) {
        transform.localScale = animationScale;
        this.Amount = amount;
        //update ui 
        if (Item.Capacity > 1)
            AmountText.text = Amount.ToString();
        else
            AmountText.text = "";
    }

    /// <summary>
    /// 当前物品 跟 另一个物品 交换显示
    /// </summary>
    /// <param name="itemUI"></param>
    public void Exchange(ItemUI itemUI) {
        Item itemTemp = itemUI.Item;
        int amountTemp = itemUI.Amount;
        itemUI.SetItem(this.Item, this.Amount);
        this.SetItem(itemTemp, amountTemp);
    }

    public void Show() {
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 设置位置，用于跟随鼠标
    /// </summary>
    /// <param name="position"></param>
    public void SetLocalPosition(Vector2 position) {
        transform.localPosition = position;
    }
}
