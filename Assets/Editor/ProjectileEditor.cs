using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof (Projectile))]
public class ProjectileEditor :  Editor{

	private bool foldout;

	public override void OnInspectorGUI () {
		base.DrawDefaultInspector();

	}

}
