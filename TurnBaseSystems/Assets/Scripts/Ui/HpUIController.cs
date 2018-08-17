using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// handles background and displayed hp objects.
/// Put it on unit.
/// </summary>
public class HpUIController: MonoBehaviour {

    internal bool visible = true;

    public Transform canvasRoot;
    public Transform background;
    public Transform[] hpList;
    public Transform[] greyhpList;
    private Unit source;

    private void Update() {
        canvasRoot.eulerAngles = HpUISettings.m.angle;
        canvasRoot.localScale = HpUISettings.m.canvasScale;
        canvasRoot.localPosition = new Vector3(0, HpUISettings.m.offsetY);
        Vector2 start = new Vector2(
            /*Mathf.Sin(canvasRoot.right.x) * */
            (-(source.maxHp-1) * (HpUISettings.m.offsetPerItem + HpUISettings.m.widthPerHp) / 2),
            /*Mathf.Cos(canvasRoot.right.y) * HpUISettings.m.offsetY*/0f);

        for (int i = 0; i < hpList.Length; i++) {
            Vector2 pos = new Vector2(i * (HpUISettings.m.offsetPerItem 
                + HpUISettings.m.widthPerHp)
                , 0) + start;
            hpList[i].localPosition = pos + (Vector2)HpUISettings.m.offset;
            Image img1 = hpList[i].GetComponent<Image>();
            Color col1 = img1.color;
            img1.color = new Color(col1.r, col1.g, col1.b, HpUISettings.m.alphaHp);
        }

        background.localPosition = new Vector3(0, 0, 0) + HpUISettings.m.offset;
        background.localScale = HpUISettings.m.hpScale* new Vector3((source.maxHp * (HpUISettings.m.offsetPerItem + HpUISettings.m.widthPerHp)) * 2 + HpUISettings.m.edgesOffset, 1, 1);
        Image img = background.GetComponent<Image>();
        Color col = img.color;
        img.color = new Color(col.r, col.g, col.b, HpUISettings.m.alphaBackground);

    }

    public void ShowHp(int curHp) {
        for (int i = 0; i < hpList.Length; i++) {
            hpList[i].gameObject.SetActive(i < curHp && visible);
        }
    }
    public void ShowHpWithGrey(int curHp, int curGreyHp) {
        for (int i = 0; i < hpList.Length; i++) {
            hpList[i].gameObject.SetActive(i < curHp && visible);
        }
        for (int i = 0; i < greyhpList.Length; i++) {
            greyhpList[i].gameObject.SetActive(i < curGreyHp && visible);
        }
    }
    private Transform LastActiveHP() {
        for (int i = 0; i < hpList.Length; i++) {
            if (!hpList[i].gameObject.activeSelf) {
                if (i == 0)
                    return null;
                return hpList[i-1];
            }

        }
        return hpList[hpList.Length-1];
    }

    public void InitHiddenHpObjects(int maxHp, int maxGreyHp, Unit source) {
        InitHpBarWithGrey(maxHp, maxGreyHp, source, HpUISettings.m.hpBarItemPref, HpUISettings.m.greyhpBarItemPref);
    }
    public void InitHpBarWithGrey(int maxHp,int maxGreyHp, Unit source, Transform pref, Transform pref2) {
        if (!visible) {
            return;
        }
        this.source = source;
        canvasRoot.rotation = Quaternion.Euler(HpUISettings.m.angle);
        canvasRoot.localScale = HpUISettings.m.canvasScale;

        hpList = new Transform[maxHp];
        greyhpList = new Transform[maxGreyHp];
        
        float offsetPerItem = HpUISettings.m.offsetPerItem;
        float widthPerHp = HpUISettings.m.widthPerHp;
        float offsetY = HpUISettings.m.offsetY;

        // set up background
        background.localScale = new Vector3((source.maxHp * (HpUISettings.m.offsetPerItem + HpUISettings.m.widthPerHp)) * 2  + HpUISettings.m.edgesOffset, 1, 1);
        background.position = new Vector3();
        
        // create hp instance
        Vector2 start = new Vector2(-(maxHp - 1) * (offsetPerItem + widthPerHp) / 2, offsetY);
        for (int i = 0; i < hpList.Length; i++) {
            Vector2 pos = (Vector2)source.transform.position
                + new Vector2(i * (offsetPerItem + widthPerHp), 0) + start;
            hpList[i] = Instantiate(pref, pos, new Quaternion(), canvasRoot);
        }

        // create grey hp instances
        Vector2 greyStart = start;
        greyStart.x += (maxGreyHp - 2) * (offsetPerItem + widthPerHp) / 2;
        for (int i = 0; i < greyhpList.Length; i++) {
            Vector2 pos = (Vector2)source.transform.position
                + new Vector2(i * (offsetPerItem + widthPerHp), 0) + greyStart;
            greyhpList[i] = Instantiate(pref2, pos, new Quaternion(), canvasRoot);
        }

        // position background
        background.localPosition = new Vector3(0, hpList[0].localPosition.y, 0);
    }

    

    public void ClearList(Transform[] list) {
        for (int i = 0; i < list.Length; i++) {
            Destroy(list[i].gameObject);
        }
    }
}

