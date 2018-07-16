using UnityEngine;
public class CameraController : MonoBehaviour {

    public float speed = 10f;

    private void Update() {
        Transform t = Camera.main.transform;
        t.Translate(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0)*Time.deltaTime*speed);
    }

}
