using System;
using UnityEngine;
using Debug = UnityEngine.Debug;

#if UNITY_FLASH
using UnityEngine.Flash;
#endif

namespace CodeStage.AntiCheat.Detectors
{
	/// <summary>
	/// Allows to detect Cheat Engine's speed hack (and maybe some other speed hack tools) usage.
	/// Just call SpeedHackDetector.StartDetection() to use it.
	/// </summary>
	/// You also may add it to the scene in editor through the<br/>
	/// "GameObject->Create Other->Code Stage->Anti-Cheat Toolkit->Speed Hack Detector" menu.
	/// 
	/// Resulting GameObject allows to override default detector's settings.<br/>
	/// <strong>Please, keep in mind you still need to call SpeedHackDetector.StartDetection() to start detector!</strong>
	[AddComponentMenu("")] // sorry, but you shouldn't add it via Component menu, read above comment please
	public class SpeedHackDetector : ActDetectorBase
	{
		private const string COMPONENT_NAME = "Speed Hack Detector";
		private const long TICKS_PER_SECOND = TimeSpan.TicksPerMillisecond * 1000;

		// maximum allowed time difference (in ticks)
		// used to compare difference between genuine ticks and vulnerable ticks
		private const int THRESHOLD = 5000000; // = 500 ms

		/// <summary> 
		/// Time (in seconds) between detector checks.
		/// </summary>
		public float interval = 1f;

		/// <summary>
		/// Maximum false positives count allowed before registering speed hack.
		/// </summary>
		public byte maxFalsePositives = 3;

		/// <summary>
		/// Amount of sequential successful checks before clearing internal false positives counter.<br/>
		/// Set 0 to disable Cool Down feature.
		/// </summary>
		public int coolDown = 30;

		internal static bool isRunning;
		private static SpeedHackDetector instance;

		private byte currentFalsePositives;
		private int currentCooldownShots;
		private long ticksOnStart;
		private long vulnerableTicksOnStart;
		private long prevTicks;
		private long prevIntervalTicks;

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
		public static SpeedHackDetector Instance
		{
			get
			{
				if (instance == null)
				{
					SpeedHackDetector detector = (SpeedHackDetector)FindObjectOfType(typeof(SpeedHackDetector));
					if (detector == null)
					{
						GameObject go = new GameObject(COMPONENT_NAME);
						detector = go.AddComponent<SpeedHackDetector>();
					}
					return detector;
				}
				return instance;
			}
		}

		/// <summary>
		/// Starts speed hack detection using settings from inspector or defaults:<br/>
		/// interval = 1, maxFalsePositives = 3, coolDown = 10.
		/// </summary>
		/// <param name="callback">Method to call after detection.</param>
		public static void StartDetection(System.Action callback)
		{
			StartDetection(callback, Instance.interval);
		}

		/// <summary>
		/// Starts speed hack detection using passed checkInterval.<br/>
		/// Other settings used from inspector or defaults:<br/>
		/// maxFalsePositives = 3, coolDown = 10.
		/// </summary>
		/// <param name="callback">Method to call after detection.</param>
		/// <param name="checkInterval">Time in seconds between speed hack checks.</param>
		public static void StartDetection(System.Action callback, float checkInterval)
		{
			StartDetection(callback, checkInterval, Instance.maxFalsePositives);
		}

		/// <summary>
		/// Starts speed hack detection using passed checkInterval and maxErrors.<br/>
		/// Default (10) or inspector value used for coolDown.
		/// </summary>
		/// <param name="callback">Method to call after detection.</param>
		/// <param name="checkInterval">Time in seconds between speed hack checks.</param>
		/// <param name="falsePositives">Amount of possible false positives.</param>
		public static void StartDetection(System.Action callback, float checkInterval, byte falsePositives)
		{
			StartDetection(callback, checkInterval, falsePositives, Instance.coolDown);
		}

		/// <summary>
		/// Starts speed hack detection using passed checkInterval, maxErrors and coolDown. 
		/// </summary>
		/// <param name="callback">Method to call after detection.</param>
		/// <param name="checkInterval">Time in seconds between speed hack checks.</param>
		/// <param name="falsePositives">Amount of possible false positives.</param>
		/// <param name="shotsTillCooldown">Amount of sequential successful checks before resetting false positives counter.</param>
		public static void StartDetection(System.Action callback, float checkInterval, byte falsePositives, int shotsTillCooldown)
		{
			Instance.StartDetectionInternal(callback, checkInterval, falsePositives, shotsTillCooldown);
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
		private SpeedHackDetector() { }

		private void Awake()
		{
			if (Init(instance, COMPONENT_NAME))
			{
				instance = this;
			}
		}

		private void StartDetectionInternal(System.Action callback, float checkInterval, byte falsePositives, int shotsTillCooldown)
		{
			if (isRunning)
			{
				Debug.LogWarning("[ACT] " + COMPONENT_NAME + " already running!");
				return;
			}

			onDetection = callback;
			interval = checkInterval;
			maxFalsePositives = falsePositives;
			coolDown = shotsTillCooldown;

			ResetStartTicks();
			currentFalsePositives = 0;
			currentCooldownShots = 0;

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

		private void ResetStartTicks()
		{
#if UNITY_FLASH && !UNITY_EDITOR
			ticksOnStart = ActionScript.Expression<long>("ACTFlashGate.GetMillisecondsFromDate();");
			vulnerableTicksOnStart = ActionScript.Expression<long>("ACTFlashGate.GetMillisecondsFromStart();");

			ticksOnStart *= TimeSpan.TicksPerMillisecond;
			vulnerableTicksOnStart *= TimeSpan.TicksPerMillisecond;
#else
			ticksOnStart = DateTime.UtcNow.Ticks;
			vulnerableTicksOnStart = System.Environment.TickCount * TimeSpan.TicksPerMillisecond;
#endif
			prevTicks = ticksOnStart;
			prevIntervalTicks = ticksOnStart;
		}

		private void OnApplicationPause(bool pause)
		{
			if (!pause)
			{
				//Debug.LogWarning("UNPAUSE");
				ResetStartTicks();
			}
		}

		private void Update()
		{
			if (!isRunning) return;

			long ticks = 0;

#if UNITY_FLASH && !UNITY_EDITOR
			ticks = ActionScript.Expression<long>("ACTFlashGate.GetMillisecondsFromDate();");
			ticks *= TimeSpan.TicksPerMillisecond;
#else
			ticks = DateTime.UtcNow.Ticks;
#endif

			long ticksSpentSinceLastUpdate = ticks - prevTicks;
			long intervalTicks = (long)(interval * TICKS_PER_SECOND);

			if (ticksSpentSinceLastUpdate < 0 || ticksSpentSinceLastUpdate > TICKS_PER_SECOND)
			{
				if (Debug.isDebugBuild) Debug.LogWarning("[ACT] SpeedHackDetector: System DateTime change detected!");
				ResetStartTicks();
				return;
			}

			prevTicks = ticks;
/*
			Debug.LogWarning("ticks - prevIntervalTicks = " + (ticks - prevIntervalTicks));
			Debug.LogWarning("intervalTicks = " + intervalTicks);*/

			if (ticks - prevIntervalTicks >= intervalTicks)
			{
				long vulnerableTicks = 0;

#if UNITY_FLASH && !UNITY_EDITOR
				vulnerableTicks = ActionScript.Expression<long>("ACTFlashGate.GetMillisecondsFromStart();");
				vulnerableTicks *= TimeSpan.TicksPerMillisecond;
#else
				vulnerableTicks = System.Environment.TickCount * TimeSpan.TicksPerMillisecond;
#endif
				if (Mathf.Abs((vulnerableTicks - vulnerableTicksOnStart) - (ticks - ticksOnStart)) > THRESHOLD)
				{
					currentFalsePositives++;
					if (currentFalsePositives > maxFalsePositives)
					{
						if (Debug.isDebugBuild) Debug.LogWarning("[ACT] SpeedHackDetector: final detection!");
						if (onDetection != null)
						{
							onDetection();
						}

						if (autoDispose)
						{
							Dispose();
						}
						else
						{
							StopDetection();
						}
					}
					else
					{
						if (Debug.isDebugBuild) Debug.LogWarning("[ACT] SpeedHackDetector: detection! Allowed false positives left: " + (maxFalsePositives - currentFalsePositives));
						currentCooldownShots = 0;
						ResetStartTicks();
					}
				}
				else if (currentFalsePositives > 0 && coolDown > 0)
				{
					if (Debug.isDebugBuild) Debug.LogWarning("[ACT] SpeedHackDetector: success shot! Shots till Cooldown: " + (coolDown - currentCooldownShots));
					currentCooldownShots++;
					if (currentCooldownShots >= coolDown)
					{
						if (Debug.isDebugBuild) Debug.LogWarning("[ACT] SpeedHackDetector: Cooldown!");
						currentFalsePositives = 0;
					}
				}

				prevIntervalTicks = ticks;
			}
		}
	}
}