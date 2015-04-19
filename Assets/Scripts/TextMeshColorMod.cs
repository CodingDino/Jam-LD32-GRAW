using UnityEngine;
using System.Collections;

public class TextMeshColorMod : MonoBehaviour {

	public TextMesh textMesh;
	public Color color;

	// Update is called once per frame
	void Update () {
		textMesh.color = color;
	}
}
