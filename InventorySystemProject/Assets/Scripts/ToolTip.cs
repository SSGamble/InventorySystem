/****************************************************
	文件：ToolTip.cs
	作者：CaptainYun
	日期：2019/06/12 14:54   	
	功能：鼠标悬浮显示物品具体信息
*****************************************************/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour {

    private Text toolTipText; // 控制大小
    private Text contentText; // 控制内容
    private CanvasGroup canvasGroup; // 控制其 Aplha 值
    private float targetAlpha = 0; // 目标值
    public float smoothing = 1; // 渐变速度

    void Start() {
        toolTipText = GetComponent<Text>();
        contentText = transform.Find("Content").GetComponent<Text>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Update() {
        // 渐变
        if (canvasGroup.alpha != targetAlpha) {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, smoothing * Time.deltaTime);
            if (Mathf.Abs(canvasGroup.alpha - targetAlpha) < 0.01f) {
                canvasGroup.alpha = targetAlpha;
            }
        }
    }

    public void Show(string text) {
        toolTipText.text = text;
        contentText.text = text;
        targetAlpha = 1;
    }

    public void Hide() {
        targetAlpha = 0;
    }

    /// <summary>
    /// 设置位置，用于跟随鼠标
    /// </summary>
    /// <param name="position"></param>
    public void SetLocalPotion(Vector2 position) {
        transform.localPosition = position;
    }
}
