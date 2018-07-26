using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhoneCamera : MonoBehaviour {

    bool camAvalible;
    WebCamTexture backCam;
    Texture defaultBackground;

    public RawImage background;
    public AspectRatioFitter fit;
    public int cams = 0;
	// Use this for initialization
	void Start () {
        defaultBackground = background.texture;
        WebCamDevice[] devices = WebCamTexture.devices;
        
        if (devices.Length == 0) {
            Debug.Log("no cam detected");
            camAvalible = false;
            return;
        }
        backCam = new WebCamTexture(devices[cams].name, Screen.width, Screen.height);

        for (int i = 0; i < devices.Length; i++) {
            break;
            if (!devices[i].isFrontFacing) {
                break;
            }
        }

        if (backCam == null) {
            Debug.Log("no back cam");
            return;
        }

        backCam.Play();
        background.texture = backCam;

        camAvalible = true;
    }
	
	// Update is called once per frame
	void Update () {
        if (!camAvalible) {
            return;
        }

        float ration = (float)backCam.width / (float)backCam.height;
        fit.aspectRatio = ration;

        float scaleY = backCam.videoVerticallyMirrored ? -1f : 1f;
        background.rectTransform.localEulerAngles = new Vector3(1, scaleY, 1);

        int orient = -backCam.videoRotationAngle;
        background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);
	}

    public void NextCam() {
        cams++;
    }
}
