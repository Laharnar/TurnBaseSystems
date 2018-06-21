using UnityEngine;

/// <summary>
/// handles background and displayed hp objects.
/// Put it on unit.
/// </summary>
public class HpUIController: MonoBehaviour {
    public Transform canvasRoot;
    public Transform background;
    public Transform[] hpList;

    public void ShowHp(int curHp) {
        for (int i = 0; i < hpList.Length; i++) {
            hpList[i].gameObject.SetActive(i < curHp);
        }
    }

    public void InitHp(int maxHp, Unit source) {
        hpList = new Transform[maxHp];
        float offsetPerItem = HpUISettings.m.offsetPerItem;
        float widthPerHp = HpUISettings.m.widthPerHp;
        float offsetY = HpUISettings.m.offsetY;
        background.localScale = new Vector3((maxHp * offsetPerItem + maxHp * widthPerHp)*2+offsetPerItem, 1,1);
        background.position = new Vector3();
        Vector2 start = new Vector2(-(maxHp-1) * (offsetPerItem + widthPerHp) / 2, offsetY);
        for (int i = 0; i < hpList.Length; i++) {
            Vector2 pos = (Vector2)source.transform.position 
                + new Vector2(i*(offsetPerItem + widthPerHp), 0)+start;
            hpList[i] = Instantiate(HpUISettings.m.hpBarItemPref, pos, new Quaternion(), canvasRoot);
        }
        background.localPosition = new Vector3(0, hpList[0].localPosition.y, 0);
    }
}

