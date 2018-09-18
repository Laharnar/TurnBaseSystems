using System;
using UnityEngine;
/// <summary>
/// Single visual effect.
/// 
/// </summary>
public class VfxController : MonoBehaviour {

    public Transform[] endSubeffectsPrefs;
    [Header("0:timeout, 1: trigger")]
    public int mode = 0;

    public float waitTime = 1f;
    float time = float.PositiveInfinity;

    bool triggered = false;

    public Transform projectilePref;
    Projectile projectile;

    Action onEnd;

    internal void Init(Action endVfx) {
        onEnd = endVfx;
        switch (mode) {
            case 0:
                time = Time.time+waitTime;
                break;
            case 1:
                if (projectilePref) {
                    projectile = Instantiate(projectilePref, transform.position, Quaternion.LookRotation(AbilityInfo.Instance.attackedSlot - AbilityInfo.Instance.attackStartedAt))
                        .GetComponent<Projectile>();
                    projectile.Init(AbilityInfo.Instance.attackedSlot);
                }
                break;
            default:
                Debug.Log("Unhandled "+mode);
                break;
        }
    }

    private void Update() {
        if (IsDone()) {
            if (onEnd != null)
                onEnd();
            // trigger effects when projectile is done.
            if (projectile) {
                projectile.CleanUp();
                for (int i = 0; i < endSubeffectsPrefs.Length; i++) {
                    Combat.Instance.RunVfx(projectile.transform.position, endSubeffectsPrefs[i].transform);
                }
            } else {
                // trigger timeout effects on self.s
                for (int i = 0; i < endSubeffectsPrefs.Length; i++) {
                    Combat.Instance.RunVfx(transform.position, endSubeffectsPrefs[i].transform);
                }
            }
            Destroy(gameObject);
        }
    }

    public bool IsDone() {
        // check for timeout or trigger, depending on mode
        switch (mode) {
            case 0:
                return Time.time >= time;
            case 1:
                if (projectile) {
                    return projectile.triggered;
                }
                break;
            default:
                Debug.Log("Unhandled " + mode);
                break;
        }
        return true;
    }
}
