using UnityEngine;
public class HpUISettings :MonoBehaviour{
    public float widthPerHp = 20;
    public float offsetPerItem = 5;
    public float offsetY = 5;
    public Transform hpBarItemPref;

    public static HpUISettings m;

    private void Awake() {
        m = this;
    }
}

