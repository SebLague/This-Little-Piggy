using UnityEngine;

namespace CodeStage.AntiCheat.Detectors
{
	/// <summary>
	/// Base class for all detectors.
	/// </summary>
	[AddComponentMenu("")]
	public abstract class ActDetectorBase : MonoBehaviour
	{
		protected const string MENU_PATH = "GameObject/Create Other/Code Stage/Anti-Cheat Toolkit/";

		/// <summary>
		/// Detector will be automatically disposed after firing callback if enabled 
		/// or it will just stop internal processes otherwise.
		/// </summary>
		public bool autoDispose = true;

		/// <summary>
		/// Allows to keep Detector's game object on new level (scene) load.
		/// </summary>
		public bool keepAlive = true;

		protected System.Action onDetection;
		
#region ComponentPlacement
#if UNITY_EDITOR
		protected static void SetupDetectorInScene(string detectorName)
		{
			SpeedHackDetector component = (SpeedHackDetector)FindObjectOfType(typeof(SpeedHackDetector));
			if (component != null)
			{
				if (component.IsPlacedCorrectly(detectorName))
				{
					if (UnityEditor.EditorUtility.DisplayDialog("Remove " + detectorName + "?", detectorName + " already exists in scene and placed correctly. Dou you wish to remove it?", "Yes", "No"))
					{
						DestroyImmediate(component.gameObject);
					}
				}
				else if (component.MayBePlacedHere())
				{
					int dialogResult = UnityEditor.EditorUtility.DisplayDialogComplex("Fix existing Game Object to work with " + detectorName + "?", detectorName + " already exists in scene and placed onto empty Game Object \"" + component.name + "\".\nDo you wish to let component configure and use this Game Object further? Press Delete to remove component from scene at all.", "Fix", "Delete", "Cancel");

					switch (dialogResult)
					{
						case 0:
							component.FixCurrentGameObject(detectorName);
							break;
						case 1:
							DestroyImmediate(component);
							break;
					}
				}
				else
				{
					int dialogResult = UnityEditor.EditorUtility.DisplayDialogComplex("Move existing " + detectorName + " to own Game Object?", "Looks like " + detectorName + " component already exists in scene and placed incorrectly on Game Object \"" + component.name + "\".\nDo you wish to let component move itself onto separate configured Game Object \"" + detectorName + "\"? Press Delete to remove plugin from scene at all.", "Move", "Delete", "Cancel");
					switch (dialogResult)
					{
						case 0:
							GameObject go = new GameObject(detectorName);
							SpeedHackDetector newComponent = go.AddComponent<SpeedHackDetector>();

							UnityEditor.EditorUtility.CopySerialized(component, newComponent);

							DestroyImmediate(component);
							break;
						case 1:
							DestroyImmediate(component);
							break;
					}
				}
			}
			else
			{
				GameObject go = new GameObject(detectorName);
				go.AddComponent<SpeedHackDetector>();
			}
		}

		private bool MayBePlacedHere()
		{
			return (gameObject.GetComponentsInChildren<Component>().Length == 2 && transform.childCount == 0 && transform.parent == null);
		}

		private void FixCurrentGameObject(string componentName)
		{
			gameObject.name = componentName;
			transform.position = Vector3.zero;
			transform.rotation = Quaternion.identity;
			transform.localScale = Vector3.one;
			tag = "Untagged";
			gameObject.layer = 0;
			gameObject.isStatic = true;
		}
#endif
		protected virtual bool IsPlacedCorrectly(string componentName)
		{
			return (name == componentName &&
					GetComponentsInChildren<Component>().Length == 2 &&
					transform.childCount == 0);
		}
#endregion

		protected virtual bool Init(ActDetectorBase instance, string detectorName)
		{
			if (instance != null)
			{
				Debug.LogWarning("[ACT] Only one " + detectorName + " instance allowed!");
				Destroy(gameObject);
				return false;
			}

			if (!IsPlacedCorrectly(detectorName))
			{
				Debug.LogWarning("[ACT] " + detectorName + " is placed in scene incorrectly and will be auto-destroyed!\nPlease, use \"" + MENU_PATH.Replace("/", "->") + detectorName + "\" menu to correct this!");
				Destroy(gameObject);
				return false;
			}

			DontDestroyOnLoad(gameObject);
			return true;
		}

		private void OnDisable()
		{
			StopDetectionInternal();
		}

		private void OnApplicationQuit()
		{
			DisposeInternal();
		}

		private void OnLevelWasLoaded(int index)
		{
			if (!keepAlive)
			{
				DisposeInternal();
			}
		}

		protected abstract void StopDetectionInternal();
		protected virtual void DisposeInternal()
		{
			StopDetectionInternal();
			Destroy(gameObject);
		}
	}
}
