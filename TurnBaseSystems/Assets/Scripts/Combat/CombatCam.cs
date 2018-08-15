using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CombatCam : MonoBehaviour {

    Coroutine coro;
    public Camera cam;
    public float camSpeed=1f;
    Queue<Vector3> customPos = new Queue<Vector3>();

    internal void FollowCenterPos(Transform[] target, float followTime) {
        if (coro != null)
            StopCoroutine(coro);
        coro = StartCoroutine(FollowCenterUpdate(target, followTime));
    }

    internal void MoveToPos() {
        if (coro != null)
            StopCoroutine(coro);
        coro = StartCoroutine(MoveUntilThere(customPos.Dequeue(), null));
    }

    private IEnumerator FollowCenterUpdate(Transform[] target, float followTime) {
        float timeout = Time.time + followTime;
        while (Time.time < timeout) {
            Vector3 t = MissionManager.GetCenter(target);
            t.z = cam.transform.position.z;
            cam.transform.position = t;
            yield return null;
        }
    }

    private IEnumerator MoveUntilThere(Vector3 pos, Action callback) {
        pos.z = cam.transform.position.z;
        float timeRequired = RequiredTimeToMoveToPos(pos);
        float timePassed = 0;
        Vector3 start = cam.transform.position;
        while (Vector3.Distance(pos, cam.transform.position) >1f) {
            cam.transform.position = 
                Vector3.Lerp(start, pos, timePassed/timeRequired);
            timePassed += Time.deltaTime;
            yield return null;
        }
    }

    public float RequiredTimeToMoveToPos(Vector3 pos) {
        return Vector3.Distance(pos, cam.transform.position) / (camSpeed);
    }

    internal void AddPos(Vector3 vector3) {
        customPos.Enqueue(vector3);
    }
}