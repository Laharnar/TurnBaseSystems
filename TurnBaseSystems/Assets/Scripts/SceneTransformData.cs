using UnityEngine;
public class SceneTransformData : GridData {
    public Transform transform;

    public SceneTransformData(Transform transform) {
        this.transform = transform;
    }

    public override void Null() {
        if (transform!= null) {
            GameObject.Destroy(transform);
        }
    }
}