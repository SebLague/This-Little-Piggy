using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GUITexture))]
public class GUIT_Button_Simple : MonoBehaviour
{
	public Color labelColor;
	public Texture text,text_over;
	
	public GameObject callbackObject;
	public string callback;
	
	private bool over = false;
	
	void Awake()
	{
		this.GetComponentInChildren<GUIText>().material.color = labelColor;
		UpdateImage();
	}
	
	void Update ()
	{
		if(this.GetComponent<GUITexture>().GetScreenRect().Contains(Input.mousePosition))
		{
			if(!over)
			{
				OnOver();
			}
			
			if(Input.GetMouseButtonDown(0))
			{
				OnClick();
			}
		}
		else if(over)
		{
			OnOut();
		}
		
	}
	
	void OnClick()
	{
		callbackObject.SendMessage(callback);
	}
	
	void OnOver()
	{
		over = true;
		UpdateImage();
	}
	
	void OnOut()
	{
		over = false;
		UpdateImage();
	}
	
	void UpdateImage()
	{
		if(over)
			this.GetComponent<GUITexture>().texture = text_over;
		else
			this.GetComponent<GUITexture>().texture = text;
	}
}
