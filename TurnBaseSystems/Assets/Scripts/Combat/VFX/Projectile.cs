using System;
using UnityEngine;
/// <summary>
/// Stops when collides(trigger) or in range
/// </summary>
public class Projectile : MonoBehaviour {
    public float flySpeed = 10f;
    Vector3 flyTo;
    Unit target;
    public bool triggered { get; private set; }

    internal void Init(Vector3 flyTo) {
        this.flyTo = flyTo;
        this.target = GridAccess.GetUnitAtPos(flyTo);
        triggered = false;
    }

    private void Update() {
        if (triggered) return;

        transform.Translate((flyTo - transform.position).normalized*Time.deltaTime*flySpeed);
        if (Vector3.Distance(flyTo, transform.position) < 1f) {
            triggered = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject == target.transform) {
            triggered = true;
        }
    }

    internal void CleanUp() {
        Destroy(gameObject);
    }
}
