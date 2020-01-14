using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public abstract class MyCustomEditor : Editor {
	
	/*
	 * Some quick notes on implementation:
	 * 
	 * This is the base class for custom editors.
	 * StartEdit and EndEdit methods must be called at the beginning and end of OnInspectorGUI.
	 * A new section is started by calling Section() and all following fields will be indented under it. 
	 * Fields are created by calling PropertyField().
	 */
	

	/// Global spacing and layout:
	private const int defaultHeight = 18;
	private const int labelWidth = 90;
	private const int fieldWidth = 150;
	/// Horizontal indent of each section
	private const int indent = 12;
	
	/// Allows custom editor to specify property behaviour (e.g. clamping the char length of a string)
	/// This is specified in conjunction with a float param to hold the atual value/s
	public enum PropertyBehaviour {Default, ClampGreaterThan, ClampLesserThan, ClampBetween, ProgressBar}
	private PropertyBehaviour behaviour;
	private float[] behaviourValues;

	/// Section information
	private bool isWritingSection;
	private int currentSectionIndex;
	private int currentIndentIndex;
	private static Dictionary<string, bool> sectionFoldStates = new Dictionary<string, bool>();

	private Rect lastGUIBounds;

	// Methods called directly by the derived class to control functioning and appearance of the custom editor
	#region Editor Methods
	
	// Create a field in the inspector for given property
	protected void PropertyField (string label, string tooltip, SerializedProperty property,
	                              PropertyBehaviour propertyBehaviour = PropertyBehaviour.Default, params float[] propertyBehaviourValues) {
		behaviour = propertyBehaviour;
		behaviourValues = propertyBehaviourValues;
		
		/// Draw custom property fields depending on type
		switch (property.propertyType) {
		case SerializedPropertyType.Boolean: /// Bool field
			BoolField(label,tooltip,property);
			break;
		case SerializedPropertyType.Generic: /// Array field
			ArrayField(label,tooltip,property);
			break;
		case SerializedPropertyType.Integer: case SerializedPropertyType.Float: /// Integer/Float field
			NumberField(label,tooltip,property);
			break;
		default: /// Draw default field if no custom one is available
			DrawDefaultPropertyField(label, tooltip, property);
			break;
		}

		/// Clamp values based on specified behaviour (note behaviours that relate to graphical changes are dealt with individually when drawing the property field))
		switch (property.propertyType) {
		case SerializedPropertyType.Integer: case SerializedPropertyType.Float: /// Integer/Float field

			if (property.propertyType == SerializedPropertyType.Float)
				property.floatValue = ClampValue(property.floatValue);
			else
				property.intValue = (int)ClampValue(property.intValue);

			break;
		case SerializedPropertyType.Vector2: case SerializedPropertyType.Vector3: /// Vector field
			Vector3 vector = Vector3.zero;
			bool isVector3 = property.propertyType == SerializedPropertyType.Vector3;
			if (isVector3)
				vector = property.vector3Value;
			else
				vector = new Vector3(property.vector2Value.x,property.vector2Value.y);

			vector = new Vector3(ClampValue(vector.x),ClampValue(vector.y),ClampValue(vector.z));
			if (isVector3)
				property.vector3Value = vector;
			else 
				property.vector2Value = new Vector2(vector.x,vector.y);

			break;
		}
	}

	protected void NoteField(string note) {
		//GUI.color = new Color(.35f,.7f,.9f);
		EditorGUILayout.HelpBox(note,MessageType.Info);
		GUI.color = Color.white;
	}

	// Leave a blank line
	protected void VerticalBreak(int pixels) {
		GetBounds(pixels);
	}
	protected void VerticalBreak() {
		VerticalBreak(defaultHeight);	
	}
	
	// Create an indent
	protected bool Subsection(string label) {
		currentSectionIndex ++;
		SectionFoldState = EditorGUI.Foldout(GetBounds(500), SectionFoldState, new GUIContent(label), EditorStyles.foldout);
		if (SectionFoldState)
			currentIndentIndex++;
		return SectionFoldState;
	}
	protected void EndSubsection() {
		currentIndentIndex--;	
	}
	
	// Override in custom editor class to initialize properties
	protected abstract void Initialize();
	
	// To be called at the start of OnInspectorGUI
	protected void StartEdit() {
		EditorGUIUtility.LookLikeInspector();
		serializedObject.Update ();
		currentSectionIndex = 0;
		Initialize ();
	}
	
	// To be called at the end of OnInspectorGUI
	protected void EndEdit() {
		EndSection();
		serializedObject.ApplyModifiedProperties();
	}
	
	// Starts a new section with child properties indented beneath it.
	protected bool Section(string title, string tooltip, Color sectionColour) {
		if (isWritingSection)
			EndSection();

		GUI.color = sectionColour;
		bool foldState = EditorGUI.Foldout(GetBounds(), SectionFoldState, new GUIContent(title,tooltip), EditorStyles.foldout);
		SectionFoldState = foldState;
		
		isWritingSection = true;
		currentIndentIndex = 1;
		
		return foldState;
	}
	
	/// No tooltip, no colour
	protected bool Section(string title) { return Section(title,title,Color.white);}
	/// No tooltip, with colour
	protected bool Section(string title, Color sectionColour) {return Section(title,title,sectionColour);}
	#endregion

	
	// The customized property fields to display for various data types
	#region Property Fields
	
	// Boolean
	private void BoolField(string label, string tooltip, SerializedProperty property) {
		EditorGUI.PropertyField (GetBounds(), property, new GUIContent ("",tooltip));
		EditorGUI.LabelField(OverlayField(15), new GUIContent(label,tooltip));
	}
	
	// Array
	private void ArrayField(string label, string tooltip, SerializedProperty property) {
		int buttonWidth = 50;
		int buttonSpacing = -buttonWidth;
		int arrayLength = property.arraySize;
		
		if (EditorGUI.PropertyField(GetBounds().AddX(12), property, new GUIContent(label,tooltip))) {
			/// Draw size field
			EditorGUI.LabelField(GetBounds(),new GUIContent("\tSize: " + property.arraySize));

			if (GUI.Button(ExtendField(0,buttonWidth),"Reset",EditorStyles.miniButton))
				property.arraySize = 0;
			if (GUI.Button(ExtendField(0,buttonWidth),"Add",EditorStyles.miniButton))
				property.arraySize += 1;
			if (GUI.Button(ExtendField(0,buttonWidth),"Remove",EditorStyles.miniButton))
				property.arraySize -= 1;
			property.NextVisible(true);
			
			/// Draw object fields
			
			for (int i = 0; i < arrayLength; i ++) {
				property.NextVisible(true);
				EditorGUI.LabelField(GetBounds(),new GUIContent("\t"+i + ":"));
				EditorGUI.PropertyField(ExtendField(), property, new GUIContent(""));
			}
			VerticalBreak();
		}
	}

	// Int/Float slider
	private void NumberSliderField(string label, string tooltip, SerializedProperty property, float minVal,float maxVal) {
		EditorGUI.LabelField(GetBounds(),new GUIContent(label,tooltip));
		if (property.propertyType == SerializedPropertyType.Float)
			property.floatValue = EditorGUI.Slider(ExtendField(),new GUIContent(""), property.floatValue,minVal,maxVal);	
		if (property.propertyType == SerializedPropertyType.Integer)
			property.intValue = EditorGUI.IntSlider(ExtendField(),new GUIContent(""), property.intValue,(int)minVal,(int)maxVal);	
	}

	// Int/Float field
	private void NumberField(string label, string tooltip, SerializedProperty property) {
		if (behaviour == PropertyBehaviour.ClampBetween)
			NumberSliderField(label,tooltip,property,GetBehaviourValue(0),GetBehaviourValue(1));	
		else
			DrawDefaultPropertyField(label,tooltip,property);
	}
	
	// Default field
	private void DrawDefaultPropertyField(string label, string tooltip, SerializedProperty property) {
		EditorGUI.LabelField(GetBounds(),new GUIContent(label,tooltip));
		EditorGUI.PropertyField(ExtendField(), property, new GUIContent ("",tooltip));	
	}
	#endregion
	
	
	// System operations for managing the placement of GUI elements etc
	#region GUI bounds calculations
	
	// Reserve some space on the line for the next gui element and return its bounds
	private Rect GetBounds(int customWidth, int customHeight) {
		Rect bounds = GUILayoutUtility.GetRect(0,customHeight).AddX(indent * currentIndentIndex);
		bounds.width = customWidth;
		lastGUIBounds = bounds;
		return bounds;
	}

	private Rect GetBounds(int customWidth) {
		return GetBounds(customWidth, defaultHeight);
	}

	private Rect GetBounds() {
		return GetBounds(labelWidth, defaultHeight);
	}

	// Return a rect on the same line as the current field, starting where the field ends + padding
	private Rect ExtendField(float horizontalPadding, float width) {
		Rect bounds = lastGUIBounds;
		bounds = bounds.AddX(horizontalPadding + bounds.width);
		bounds.width = width;
		lastGUIBounds = bounds;
		return bounds;
	}
	private Rect ExtendField(float horizontalPadding) {
		return ExtendField(horizontalPadding,fieldWidth);
	}
	private Rect ExtendField() {
		return ExtendField(0,fieldWidth);
	}

	// Return a rect at the same position as the current field + padding
	private Rect OverlayField(float horizontalPadding, float width) {
		Rect bounds = GUILayoutUtility.GetLastRect();
		bounds = bounds.AddX(horizontalPadding + indent * currentIndentIndex);
		bounds.width = width;
		return bounds;
	}
	private Rect OverlayField(float horizontalPadding) {	
		return OverlayField(horizontalPadding,fieldWidth);
	}
	private Rect OverlayField() {
		return OverlayField(0,fieldWidth);
	}
	
	// End the current section
	private void EndSection() {
		isWritingSection = false;
		currentIndentIndex = 0;
		currentSectionIndex ++;
	}
	
	#endregion
	
	
	// Helper methods
	#region Helper methods
	
	// Get the behaviour value at the current index and return a helpful error if custom editor has not supplied the value to PropertyField method
	private float GetBehaviourValue(int index) {
		if (index < behaviourValues.Length) {
			return behaviourValues[index];
		}
		else {
			Debug.LogError("Too few behaviour values given for the specified parameter behaviour type " + "(" + behaviour.ToString() + ")");
			return 0;
		}
	}

	

	// Get/set folded state of current section
	private bool SectionFoldState {
		get {
			string key = this + "" + currentSectionIndex;
			bool result = false;
			sectionFoldStates.TryGetValue(key, out result);

			return result;
		}
		set {
			string key = this + "" + currentSectionIndex;

			if (sectionFoldStates.ContainsKey(key)) {
				sectionFoldStates[key] = value;
			}
			else {
				sectionFoldStates.Add(key,value);
			}
		}
	}

	// Clamp value based on behaviour
	private float ClampValue(float value) {
		if (behaviour == PropertyBehaviour.ClampGreaterThan)
			return Mathf.Clamp(value,GetBehaviourValue(0),float.MaxValue);
		else if (behaviour == PropertyBehaviour.ClampLesserThan)
			return Mathf.Clamp(value,float.MinValue,GetBehaviourValue(0));

		return value;
	}


	#endregion
}


