using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class HorizontalFOV : MonoBehaviour
{
	public float orthoSize = 5;
	public float defaultAspectRatio = 1.765f;
    public float positionScale = 5;

    void Update()
    {
        float aspectRatio = ((float)Camera.main.pixelWidth) / ((float)Camera.main.pixelHeight);

        Camera.main.orthographicSize = orthoSize / aspectRatio;

        float positionAdjust = (defaultAspectRatio - aspectRatio) * positionScale;
        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x,
                                                     positionAdjust,
                                                     Camera.main.transform.position.z);
    }
}
