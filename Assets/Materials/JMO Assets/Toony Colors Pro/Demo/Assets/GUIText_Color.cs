using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GUIText))]
public class GUIText_Color : MonoBehaviour
{
	public Color labelColor;
	void Awake()
	{
		this.GetComponent<GUIText>().material.color = labelColor;
	}
}
