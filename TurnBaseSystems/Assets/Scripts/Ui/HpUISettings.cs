using UnityEngine;
public class HpUISettings :MonoBehaviour{
    public float widthPerHp = 20;
    public float offsetPerItem = 5;
    public float offsetY = 5;
    public Transform hpBarItemPref;
    public Vector3 angle = new Vector3();
    public Vector3 canvasScale = new Vector3(1, 1, 1);
    public float hpScale = 1;

    public static HpUISettings m;
    public Transform greyhpBarItemPref;
    
    public float edgesOffset = 0.1f;
    public float alphaBackground = 1f;
    public float alphaHp = 1f;
    public Vector3 offset;

    private void Awake() {
        m = this;
    }
}

