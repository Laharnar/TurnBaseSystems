using System;
using UnityEngine;

/// <summary>
/// handles background and displayed hp objects.
/// Put it on unit.
/// </summary>
public class HpUIController: MonoBehaviour {
    public Transform canvasRoot;
    public Transform background;
    public Transform[] hpList;
    public Transform[] greyhpList;

    public void OnLoadedMission() {
        canvasRoot.gameObject.SetActive(true);
    }

    public void ShowHp(int curHp) {
        for (int i = 0; i < hpList.Length; i++) {
            hpList[i].gameObject.SetActive(i < curHp);
        }
    }
    public void ShowHpWithGrey(int curHp, int curGreyHp) {
        for (int i = 0; i < hpList.Length; i++) {
            hpList[i].gameObject.SetActive(i < curHp);
        }
        for (int i = 0; i < greyhpList.Length; i++) {
            greyhpList[i].gameObject.SetActive(i < curGreyHp);
        }
    }
    public void InitHp(int maxHp, Unit source) {
        hpList = InitBar(maxHp, source, HpUISettings.m.hpBarItemPref);
        
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

    public void InitBarWithGrey(int maxHp, int maxGreyHp, Unit source) {
        InitBarWithGrey(maxHp, maxGreyHp, source, HpUISettings.m.hpBarItemPref, HpUISettings.m.greyhpBarItemPref);
    }
    public void InitBarWithGrey(int maxHp,int maxGreyHp, Unit source, Transform pref, Transform pref2) {
        hpList = new Transform[maxHp];
        greyhpList = new Transform[maxGreyHp];
        float offsetPerItem = HpUISettings.m.offsetPerItem;
        float widthPerHp = HpUISettings.m.widthPerHp;
        float offsetY = HpUISettings.m.offsetY;
        background.localScale = new Vector3((maxHp * (offsetPerItem+ widthPerHp)) * 2  + offsetPerItem, 1, 1);
        background.position = new Vector3();
        Vector2 start = new Vector2(-(maxHp - 1) * (offsetPerItem + widthPerHp) / 2, offsetY);
        for (int i = 0; i < hpList.Length; i++) {
            Vector2 pos = (Vector2)source.transform.position
                + new Vector2(i * (offsetPerItem + widthPerHp), 0) + start;
            hpList[i] = Instantiate(pref, pos, new Quaternion(), canvasRoot);
        }
        Vector2 greyStart = start;
        greyStart.x += (maxGreyHp - 2) * (offsetPerItem + widthPerHp) / 2;
        for (int i = 0; i < greyhpList.Length; i++) {
            Vector2 pos = (Vector2)source.transform.position
                + new Vector2(i * (offsetPerItem + widthPerHp), 0) + greyStart;
            greyhpList[i] = Instantiate(pref2, pos, new Quaternion(), canvasRoot);
        }

        background.localPosition = new Vector3(0, hpList[0].localPosition.y, 0);
    }


    public Transform[] InitBar(int maxHp, Unit source, Transform pref, float offset = 0) {
        Transform[] hpList = new Transform[maxHp];
        float offsetPerItem = HpUISettings.m.offsetPerItem;
        float widthPerHp = HpUISettings.m.widthPerHp;
        float offsetY = HpUISettings.m.offsetY;
        background.localScale = new Vector3((maxHp * offsetPerItem + maxHp * widthPerHp) * 2 + offsetPerItem, 1, 1);
        background.position = new Vector3();
        Vector2 start = new Vector2(-(maxHp - 1) * (offsetPerItem + widthPerHp) / 2+offset, offsetY);
        for (int i = 0; i < hpList.Length; i++) {
            Vector2 pos = (Vector2)source.transform.position
                + new Vector2(i * (offsetPerItem + widthPerHp), 0) + start;
            hpList[i] = Instantiate(pref, pos, new Quaternion(), canvasRoot);
        }
        background.localPosition = new Vector3(0, hpList[0].localPosition.y, 0);
        return hpList;
    }

    public void ClearList(Transform[] list) {
        for (int i = 0; i < list.Length; i++) {
            Destroy(list[i].gameObject);
        }
    }
}

