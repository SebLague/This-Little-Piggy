using UnityEngine;

using Debug = UnityEngine.Debug;

#if UNITY_FLASH
using UnityEngine.Flash;
#endif

namespace CodeStage.AntiCheat.Detectors
{
	/// <summary>
	/// Detects cheating of any Obscured type (except ObscuredPrefs) used in project.
	/// It allows cheaters to find desired (fake) values in memory and change them, keeping original values secure.
	/// It's like a cheese in the mouse trap - cheater try to change some obscured value and get caught on it.
	/// Just call ObscuredCheatingDetector.StartDetection() to use it.
	/// </summary>
	/// You also may add it to the scene in editor through the<br/>
	/// "GameObject->Create Other->Code Stage->Anti-Cheat Toolkit->Obscured Cheating Detector" menu.
	/// 
	/// It allows you to edit and store detector's settings in inspector.<br/>
	/// <strong>Please, keep in mind you still need to call ObscuredCheatingDetector.StartDetection() to start detector!</strong><br/><br/>
	[AddComponentMenu("")] // sorry, but you shouldn't add it via Component menu, read above comment please
	public class ObscuredCheatingDetector : ActDetectorBase
	{
		private const string COMPONENT_NAME = "Obscured Cheating Detector";

		internal static bool isRunning;
		private static ObscuredCheatingDetector instance;

		/// <summary>
		/// Max allowed difference between encrypted and fake values in ObscuredFloat. Increase in case of false positives.
		/// </summary>
		[HideInInspector]
		public float floatEpsilon = 0.0001f;

		/// <summary>
		/// Max allowed difference between encrypted and fake values in ObscuredVector2. Increase in case of false positives.
		/// </summary>
		[HideInInspector] 
		public float vector2Epsilon = 0.1f;

		/// <summary>
		/// Max allowed difference between encrypted and fake values in ObscuredVector3. Increase in case of false positives.
		/// </summary>
		[HideInInspector]
		public float vector3Epsilon = 0.1f;

		/// <summary>
		/// Max allowed difference between encrypted and fake values in ObscuredQuaternion. Increase in case of false positives.
		/// </summary>
		[HideInInspector]
		public float quaternionEpsilon = 0.1f;

		#region ComponentPlacement
#if UNITY_EDITOR
		[UnityEditor.MenuItem(MENU_PATH + COMPONENT_NAME, false)]
		private static void AddToScene()
		{
			SetupDetectorInScene(COMPONENT_NAME);
		}
#endif
		#endregion

		/// <summary>
		/// Allows to reach public properties from code.
		/// </summary>
		public static ObscuredCheatingDetector Instance
		{
			get
			{
				if (instance == null)
				{
					ObscuredCheatingDetector detector = (ObscuredCheatingDetector)FindObjectOfType(typeof(ObscuredCheatingDetector));
					if (detector == null)
					{
						GameObject go = new GameObject(COMPONENT_NAME);
						detector = go.AddComponent<ObscuredCheatingDetector>();
					}
					return detector;
				}
				return instance;
			}
		}

		/// <summary>
		/// Starts all Obscured types cheating detection.
		/// </summary>
		/// <param name="callback">Method to call after detection.</param>
		public static void StartDetection(System.Action callback)
		{
			Instance.StartDetectionInternal(callback);
		}

		/// <summary>
		/// Stops detector. Detector's GameObject remains in the scene. Use Dispose() to completely remove detector.
		/// </summary>
		public static void StopDetection()
		{
			Instance.StopDetectionInternal();
		}

		/// <summary>
		/// Stops and completely disposes detector including its GameObject.
		/// </summary>
		public static void Dispose()
		{
			Instance.DisposeInternal();
		}

		// preventing direct instantiation =P
		private ObscuredCheatingDetector() { }

		private void Awake()
		{
			if (Init(instance, COMPONENT_NAME))
			{
				instance = this;
			}
		}

		private void StartDetectionInternal(System.Action callback)
		{
			if (isRunning)
			{
				Debug.LogWarning("[ACT] " + COMPONENT_NAME + " already running!");
				return;
			}

			onDetection = callback;
			isRunning = true;
		}

		protected override void StopDetectionInternal()
		{
			if (isRunning)
			{
				onDetection = null;
				isRunning = false;
			}
		}

		protected override void DisposeInternal()
		{
			base.DisposeInternal();
			instance = null;
		}

		internal void OnCheatingDetected()
		{
			if (onDetection != null)
			{
				onDetection();

				if (autoDispose)
				{
					Dispose();
				}
				else
				{
					StopDetection();
				}
			}
		}
	}
}