using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GUITexture))]
public class GUIT_Button : MonoBehaviour
{
	public Color labelColor;
	public Texture t_on,t_off,t_on_over,t_off_over;
	
	public GameObject callbackObject;
	public string callback;
	
	private bool over = false;
	public bool on;
	
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
		on = !on;
		callbackObject.SendMessage(callback);
		UpdateImage();
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
			this.GetComponent<GUITexture>().texture = on ? t_on_over : t_off_over;
		else
			this.GetComponent<GUITexture>().texture = on ? t_on : t_off;
	}
	
	public void UpdateState(bool b)
	{
		on = b;
		UpdateImage();
	}
}
