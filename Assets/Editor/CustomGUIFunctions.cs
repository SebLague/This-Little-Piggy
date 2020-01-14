using UnityEngine;
using System.Collections;

public static class CustomGUIFunctions {

	// Add Rectangles together
	public static Rect Add(this Rect r1, Rect r2) {
		return new Rect(r1.x + r2.x, r1.y + r2.y,r1.width + r2.width, r1.height + r2.height);	
	}
	
	// Add horizontally to a rectangle
	public static Rect AddX(this Rect r, float x) {
		Rect h = new Rect(x,0,0,0);
		return r.Add(h);
	}
	// Add vertically to a rectangle
	public static Rect AddY(this Rect r, float y) {
		Rect v = new Rect(0,y,0,0);
		return r.Add(v);
	}
}
