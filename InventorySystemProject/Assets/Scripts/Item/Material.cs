/****************************************************
    文件：Material.cs
	作者：CaptainYun
    日期：2019/6/11 20:49:34
	功能：材料类
*****************************************************/
using UnityEngine;
using System.Collections;

/// <summary>
/// 材料类
/// </summary>
public class Material : Item {

    public Material(int id, string name, ItemType type, ItemQuality quality, string des, int capacity, int buyPrice, int sellPrice, string sprite)
        : base(id, name, type, quality, des, capacity, buyPrice, sellPrice, sprite) {
    }
}
