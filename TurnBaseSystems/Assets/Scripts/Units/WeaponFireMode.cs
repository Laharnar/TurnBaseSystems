using System.Collections;
using UnityEngine;
public class WeaponFireMode {

    /// <summary>
    /// Result from last WaitPlayerToSetAim.
    /// Don't modify elsewhere.
    /// </summary>
    public static Vector2 activeUnitAimDirection;

    public static IEnumerator WaitPlayerToSetAim(Unit source, Unit target, Transform aimConePref, float range) {
        if (!aimConePref) {
            Debug.Log("missing pref. breaking cone aim coroutine.");
            yield break;
        }
        Transform aimCone = GameObject.Instantiate(aimConePref, source.transform.position, new Quaternion());
        Transform aimTarget = aimCone.GetChild(0).Find("AIM").Find("AimTarget");
        aimTarget.localScale  = new Vector3(range, aimTarget.localScale.y, 1); // resize cone length.
        aimTarget.localPosition = new Vector3(range/2f, 0,0);
        aimCone.right = (Vector3)target.transform.position- source.transform.position;
        Transform aimObj = aimCone.GetChild(0).Find("AIM");
        aimObj.right = (Vector3)target.transform.position- source.transform.position;
        Vector2 otherDir = aimObj.up;
        yield return null;
        float time = Time.time;
        int pingPongDir = 1;
        float timeFrame = 1f;
        while (!Input.GetKeyDown(KeyCode.Mouse0)) {
            float timeDiff = Time.time - time;
            aimObj.right = 
                (Vector2)(target.transform.position - source.transform.position)
                + otherDir * timeDiff*pingPongDir -otherDir*timeFrame*pingPongDir;
            if (timeDiff > timeFrame*2) {
                time = Time.time;
                pingPongDir *= -1;
            }
            yield return null;
        }
        activeUnitAimDirection = aimObj.right * range;
        GameObject.Destroy(aimCone.gameObject);
    }
}
