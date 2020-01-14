using System;
using System.Text;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	/// <summary>
	/// This is an Obscured analogue of the <a href="http://docs.unity3d.com/Documentation/ScriptReference/PlayerPrefs.html">PlayerPrefs</a> class.
	/// </summary>
	/// Saves data in encrypted state, optionally locking it to the current device.<br/>
	/// Automatically encrypts PlayerPrefs on first read (auto migration), has tampering detection and more.
	public static class ObscuredPrefs
	{
		private static string encryptionKey = "e806f6";
		private static bool savesAlterationReported; 

#if !UNITY_FLASH
		private static bool foreignSavesReported;
		private static string deviceHash;
		private static string DeviceHash
		{
			get
			{
				if (String.IsNullOrEmpty(deviceHash)) deviceHash = GetDeviceID();
				return deviceHash;
			}
		}
#endif

		/// <summary>
		/// Allows to react on saves alteration. May be helpful for banning potential cheaters.
		/// </summary>
		/// Fires only once.
		public static System.Action onAlterationDetected = null;

		/// <summary>
		/// Allows to save original PlayerPrefs values while migrating to ObscuredPrefs.
		/// </summary>
		/// In such case, original value still will be readable after switching from PlayerPrefs to 
		/// ObscuredPrefs and it should be removed manually as it became unneeded.<br/>
		/// Original PlayerPrefs value will be automatically removed after read by default.
		public static bool preservePlayerPrefs = false;

#if UNITY_EDITOR
		/// <summary>
		/// Allows to disable written data obscuration. Works in Editor only.
		/// </summary>
		/// Please note, it breaks PlayerPrefs to ObscuredPrefs migration (in Editor).
		public static bool unobscuredMode = false;
#endif

		/// <summary>
		/// Allows to react on detection of possible saves from some other device. 
		/// </summary>
		/// May be helpful to ban potential cheaters, trying to use someone's purchased in-app goods for example.<br/>
		/// May fire on same device in case cheater manipulates saved data in some special way.<br/>
		/// Fires only once.
		/// 
		/// <strong>\htmlonly<font color="7030A0">IMPORTANT:</font>\endhtmlonly May be called if same device ID was changed (pretty rare case though).</strong><br/>
		/// Same device ID change will be reported with separate callback in future.
		public static System.Action onPossibleForeignSavesDetected = null;

#if !UNITY_FLASH

		/// <summary>
		/// Allows to lock saved data to the current device.
		/// </summary>
		/// Use it to prevent cheating via 100% game progress save games usage or sharing purchased in-app items for example.<br/>
		/// Set to \link ObscuredPrefs::Soft DeviceLockLevel.Soft \endlink to allow reading of not locked data.<br/>
		/// Set to \link ObscuredPrefs::Strict DeviceLockLevel.Strict \endlink to disallow reading of not locked data (any not locked data will be lost).<br/>
		/// Set to \link ObscuredPrefs::None DeviceLockLevel.None \endlink to disable data lock feature and to read both previously locked and not locked data.<br/>
		/// Read more in #DeviceLockLevel description.
		/// 
		/// Relies on <a href="http://docs.unity3d.com/Documentation/ScriptReference/SystemInfo-deviceUniqueIdentifier.html">SystemInfo.deviceUniqueIdentifier</a>.
		/// Please note, it may change in some rare cases, so one day all locked data may became inaccessible on same device, and here comes #emergencyMode and #readForeignSaves to rescue.<br/>
		/// I'm going to implement separate detection of the same device ID change in future, allowing to easily re-lock data to the new id without extra head pain.
		/// 
		/// <strong>\htmlonly<font color="FF4040">WARNING:</font>\endhtmlonly Flash Player is not supported!</strong><br/>
		/// <strong>\htmlonly<font color="7030A0">IMPORTANT #1:</font>\endhtmlonly On iOS it tries to receive vendorIdentifier in first place, to avoid device id change while updating from iOS6 to iOS7. It leads to device ID change while updating from iOS5, but such case is lot rarer.<br/></strong>
		/// <strong>\htmlonly<font color="7030A0">IMPORTANT #2:</font>\endhtmlonly Main thread may lock up for a noticeable time while obtaining device ID first time on some devices (~ sec on my PC)! Consider using ForceLockToDeviceInit() to prevent undesirable behavior in such cases.</strong>
		/// \sa readForeignSaves, emergencyMode, ForceLockToDeviceInit(), ForceDeviceID()
		public static DeviceLockLevel lockToDevice = DeviceLockLevel.None;

		/// <summary>
		/// Allows to read saves locked to other device. #onPossibleForeignSavesDetected action still will be fired.
		/// </summary>
		/// \sa lockToDevice
		public static bool readForeignSaves = false;

		/// <summary>
		/// Allows to ignore #lockToDevice to recover saved data in case of some unexpected issues, like unique device ID change for the same device.
		/// </summary>
		/// \sa lockToDevice
		public static bool emergencyMode = false;

		/// <summary>
		/// Allows to force device id obtaining on demand. Otherwise, it will be obtained automatically on first usage.
		/// </summary>
		/// Device id obtaining process may be noticeably slow when called first time on some devices.<br/>
		/// This method allows you to force this process at comfortable time (while splash screen is showing for example).
		/// \sa lockToDevice
		public static void ForceLockToDeviceInit()
		{
			if (String.IsNullOrEmpty(deviceHash))
			{
				deviceHash = GetDeviceID();
			}
			else
			{
				Debug.LogWarning("[ACT] ObscuredPrefs.ForceLockToDeviceInit() is called, but device ID is already obtained!");
			}
		}

		/// <summary>
		/// Allows to use custom device ID to lock saves to the current device. ForceLockToDeviceInit() 
		/// </summary>
		/// <strong>\htmlonly<font color="FF4040">WARNING:</font>\endhtmlonly All data saved with previous device ID will be considered foreign!</strong>
		/// \sa lockToDevice
		public static void ForceDeviceID(string newDeviceID)
		{
			deviceHash = newDeviceID;
		}
#endif

		/// <summary>
		/// Allows to change default crypto key.
		/// </summary>
		/// <strong>\htmlonly<font color="FF4040">WARNING:</font>\endhtmlonly Any data saved with one encryption key will not be accessible with any other encryption key!</strong>
		public static void SetNewCryptoKey(string newKey)
		{
			encryptionKey = newKey;
		}

		/// <summary>
		/// Sets the <c>value</c> of the preference identified by <c>key</c>.
		/// </summary>
		public static void SetInt(string key, int value)
		{
			SetStringValue(key, value.ToString());
		}

		/// <summary>
		/// Returns the value corresponding to <c>key</c> in the preference file if it exists.
		/// If it doesn't exist, it will return 0.
		/// </summary>
		public static int GetInt(string key)
		{
			return GetInt(key,0);
		}

		/// <summary>
		/// Returns the value corresponding to <c>key</c> in the preference file if it exists.
		/// If it doesn't exist, it will return <c>defaultValue</c>.
		/// </summary>
		public static int GetInt(string key, int defaultValue)
		{
			string encryptedKey = EncryptKey(key);

#if UNITY_EDITOR
			if (!PlayerPrefs.HasKey(encryptedKey) && !unobscuredMode)
#else
			if (!PlayerPrefs.HasKey(encryptedKey))
#endif
			{
				if (PlayerPrefs.HasKey(key))
				{
					int unencrypted = PlayerPrefs.GetInt(key, defaultValue);
					if (!preservePlayerPrefs)
					{
						SetInt(key, unencrypted);
						PlayerPrefs.DeleteKey(key);
					}
					return unencrypted;
				}
			}

			string rawData = GetData(encryptedKey, defaultValue.ToString());
			int result;
			int.TryParse(rawData, out result);
			return result;
		}

		/// <summary>
		/// Sets the <c>value</c> of the preference identified by <c>key</c>.
		/// </summary>
		public static void SetString(string key, string value)
		{
			SetStringValue(key, value);
		}

		/// <summary>
		/// Returns the value corresponding to <c>key</c> in the preference file if it exists.
		/// If it doesn't exist, it will return "".
		/// </summary>
		public static string GetString(string key)
		{
			return GetString(key, "");
		}

		/// <summary>
		/// Returns the value corresponding to <c>key</c> in the preference file if it exists.
		/// If it doesn't exist, it will return <c>defaultValue</c>.
		/// </summary>
		public static string GetString(string key, string defaultValue)
		{
			string encryptedKey = EncryptKey(key);

#if UNITY_EDITOR
			if (!PlayerPrefs.HasKey(encryptedKey) && !unobscuredMode)
#else
			if (!PlayerPrefs.HasKey(encryptedKey))
#endif
			{
				if (PlayerPrefs.HasKey(key))
				{
					string unencrypted = PlayerPrefs.GetString(key, defaultValue);
					if (!preservePlayerPrefs)
					{
						SetString(key, unencrypted);
						PlayerPrefs.DeleteKey(key);
					}
					return unencrypted;
				}
			}

			string rawData = GetData(encryptedKey, defaultValue);
			return rawData;
		}

		/// <summary>
		/// Sets the <c>value</c> of the preference identified by <c>key</c>.
		/// </summary>
		public static void SetFloat(string key, float value)
		{
			SetStringValue(key, value.ToString());
		}

		/// <summary>
		/// Returns the value corresponding to <c>key</c> in the preference file if it exists.
		/// If it doesn't exist, it will return 0.
		/// </summary>
		public static float GetFloat(string key)
		{
			return GetFloat(key, 0);
		}

		/// <summary>
		/// Returns the value corresponding to <c>key</c> in the preference file if it exists.
		/// If it doesn't exist, it will return <c>defaultValue</c>.
		/// </summary>
		public static float GetFloat(string key, float defaultValue)
		{
			string encryptedKey = EncryptKey(key);

#if UNITY_EDITOR
			if (!PlayerPrefs.HasKey(encryptedKey) && !unobscuredMode)
#else
			if (!PlayerPrefs.HasKey(encryptedKey))
#endif
			{
				if (PlayerPrefs.HasKey(key))
				{
					float unencrypted = PlayerPrefs.GetFloat(key, defaultValue);
					if (!preservePlayerPrefs)
					{
						SetFloat(key, unencrypted);
						PlayerPrefs.DeleteKey(key);
					}
					return unencrypted;
				}
			}

			string rawData = GetData(encryptedKey, defaultValue.ToString());
			float result;
			float.TryParse(rawData, out result);
			return result;
		}

		/// <summary>
		/// Sets the <c>value</c> of the preference identified by <c>key</c>.
		/// </summary>
		public static void SetDouble(string key, double value)
		{
			SetStringValue(key, value.ToString());
		}

		/// <summary>
		/// Returns the value corresponding to <c>key</c> in the preference file if it exists.
		/// If it doesn't exist, it will return 0.
		/// </summary>
		public static double GetDouble(string key)
		{
			return GetDouble(key, 0);
		}

		/// <summary>
		/// Returns the value corresponding to <c>key</c> in the preference file if it exists.
		/// If it doesn't exist, it will return <c>defaultValue</c>.
		/// </summary>
		public static double GetDouble(string key, double defaultValue)
		{
			string rawData = GetData(EncryptKey(key), defaultValue.ToString());
			double result;
			double.TryParse(rawData, out result);
			return result;
		}

		/// <summary>
		/// Sets the <c>value</c> of the preference identified by <c>key</c>.
		/// </summary>
		public static void SetLong(string key, long value)
		{
			SetStringValue(key, value.ToString());
		}

		/// <summary>
		/// Returns the value corresponding to <c>key</c> in the preference file if it exists.
		/// If it doesn't exist, it will return 0.
		/// </summary>
		public static long GetLong(string key)
		{
			return GetLong(key, 0);
		}

		/// <summary>
		/// Returns the value corresponding to <c>key</c> in the preference file if it exists.
		/// If it doesn't exist, it will return <c>defaultValue</c>.
		/// </summary>
		public static long GetLong(string key, long defaultValue)
		{
			string rawData = GetData(EncryptKey(key), defaultValue.ToString());
			long result;
			long.TryParse(rawData, out result);
			return result;
		}

		/// <summary>
		/// Sets the <c>value</c> of the preference identified by <c>key</c>.
		/// </summary>
		public static void SetBool(string key, bool value)
		{
			SetInt(key, value ? 1 : 0);
		}

		/// <summary>
		/// Returns the value corresponding to <c>key</c> in the preference file if it exists.
		/// If it doesn't exist, it will return false.
		/// </summary>
		public static bool GetBool(string key)
		{
			return GetBool(key, false);
		}

		/// <summary>
		/// Returns the value corresponding to <c>key</c> in the preference file if it exists.
		/// If it doesn't exist, it will return <c>defaultValue</c>.
		/// </summary>
		public static bool GetBool(string key, bool defaultValue)
		{
			int defValue = defaultValue ? 1 : 0;
			string rawData = GetData(EncryptKey(key), defValue.ToString());
			int result;
			int.TryParse(rawData, out result);
			return result == 1;
		}

		/// <summary>
		/// Sets the <c>value</c> of the preference identified by <c>key</c>.
		/// </summary>
		public static void SetVector3(string key, Vector3 value)
		{
			string vector = value.x + "|" + value.y + "|" + value.z;
			SetStringValue(key, vector);
		}

		/// <summary>
		/// Returns the value corresponding to <c>key</c> in the preference file if it exists.
		/// If it doesn't exist, it will return Vector3.zero.
		/// </summary>
		public static Vector3 GetVector3(string key)
		{
			return GetVector3(key, Vector3.zero);
		}

		/// <summary>
		/// Returns the value corresponding to <c>key</c> in the preference file if it exists.
		/// If it doesn't exist, it will return <c>defaultValue</c>.
		/// </summary>
		public static Vector3 GetVector3(string key, Vector3 defaultValue)
		{
			Vector3 result;
			string rawData = GetData(EncryptKey(key), "{not_found}");
			if (rawData == "{not_found}")
			{
				result = defaultValue;
			}
			else
			{
				string[] values = rawData.Split('|');
				float x;
				float y;
				float z;
				float.TryParse(values[0], out x);
				float.TryParse(values[1], out y);
				float.TryParse(values[2], out z);

				result = new Vector3(x,y,z);
			}
			return result;
		}

		/// <summary>
		/// Sets the <c>value</c> of the preference identified by <c>key</c>.
		/// </summary>
		public static void SetByteArray(string key, byte[] value)
		{
			SetStringValue(key, Encoding.UTF8.GetString(value, 0, value.Length));
		}

		/// <summary>
		/// Returns the value corresponding to <c>key</c> in the preference file if it exists.
		/// If it doesn't exist, it will return new byte[0].
		/// </summary>
		public static byte[] GetByteArray(string key)
		{
			return GetByteArray(key, 0, 0);
		}

		/// <summary>
		/// Returns the value corresponding to <c>key</c> in the preference file if it exists.
		/// If it doesn't exist, it will return <c>byte[defaultLength]</c> filled with <c>defaultValue</c>.
		/// </summary>
		public static byte[] GetByteArray(string key, byte defaultValue, int defaultLength)
		{
			byte[] result;
			string rawData = GetData(EncryptKey(key), "{not_found}");
			if (rawData == "{not_found}")
			{
				result = new byte[defaultLength];
				for (int i = 0; i < defaultLength; i++)
				{
					result[i] = defaultValue;
				}
			}
			else
			{
				result = Encoding.UTF8.GetBytes(rawData);
			}
			return result;
		}

		/// <summary>
		/// Returns true if <c>key</c> exists in the ObscuredPrefs.
		/// </summary>
		public static bool HasKey(string key)
		{
			if (PlayerPrefs.HasKey(key))
			{
				return true;
			}
			else
			{
				return PlayerPrefs.HasKey(EncryptKey(key));
			}
		}

		/// <summary>
		/// Removes <c>key</c> and its corresponding value from the ObscuredPrefs.
		/// </summary>
		public static void DeleteKey(string key)
		{
			PlayerPrefs.DeleteKey(EncryptKey(key));
			PlayerPrefs.DeleteKey(key);
		}

		/// <summary>
		/// Removes all keys and values from the preferences, including anything saved with regular PlayerPrefs. Use with caution!
		/// </summary>
		public static void DeleteAll()
		{
			PlayerPrefs.DeleteAll();
		}

		/// <summary>
		/// Writes all modified preferences to disk.
		/// </summary>
		/// By default, Unity writes preferences to disk on Application Quit.<br/>
		/// In case when the game crashes or otherwise prematurely exits, you might want to write the preferences at sensible 'checkpoints' in your game.<br/>
		/// This function will write to disk potentially causing a small hiccup, therefore it is not recommended to call during actual game play.
		public static void Save()
		{
			PlayerPrefs.Save();
		}

		private static void SetStringValue(string key, string value)
		{	
#if UNITY_EDITOR
			if (unobscuredMode)
			{
				PlayerPrefs.SetString(key, value);
				return;
			}
#endif

#if UNITY_WEBPLAYER
			try
			{
				PlayerPrefs.SetString(EncryptKey(key), EncryptValue(value));
			}
			catch (PlayerPrefsException exception)
			{
				Debug.LogException(exception);
			}
#else
			PlayerPrefs.SetString(EncryptKey(key), EncryptValue(value));
#endif
		}

		private static string GetData(string key, string defaultValueRaw)
		{
#if UNITY_EDITOR
			if (unobscuredMode)
			{
				string originalKey = DecryptKey(key);
				if (PlayerPrefs.HasKey(originalKey))
				{
					return PlayerPrefs.GetString(originalKey, defaultValueRaw);
				}
			}
#endif

			string result = PlayerPrefs.GetString(key, defaultValueRaw);

			if (result != defaultValueRaw)
			{
				result = DecryptValue(result);
				if (result == "") result = defaultValueRaw;
			}
			else
			{
				string originalKey = DecryptKey(key);
				string originalKeyEncryptedDeprecated = EncryptKeyDeprecated(originalKey);
				result = PlayerPrefs.GetString(originalKeyEncryptedDeprecated, defaultValueRaw);
				if (result != defaultValueRaw)
				{
					result = DecryptValueDeprecated(result);
					PlayerPrefs.DeleteKey(originalKeyEncryptedDeprecated);

					SetStringValue(originalKey, result);
				}
				else
				{
					if (PlayerPrefs.HasKey(originalKey))
					{
						Debug.LogWarning("[ACT] Are you trying to read data saved with regular PlayerPrefs using ObscuredPrefs (key = " + originalKey + ")?");
					}
				}
			}

			return result;
		}

		private static string EncryptKey(string key)
		{
			key = ObscuredString.EncryptDecrypt(key, encryptionKey);
#if UNITY_FLASH
			key = Crypto.Base64Tools.ToBase64String(Encoding.UTF8.GetBytes(key));
#else
			key = Convert.ToBase64String(Encoding.UTF8.GetBytes(key));
#endif
			return key;
		}

		private static string DecryptKey(string key)
		{
#if UNITY_FLASH
			byte[] bytes = Crypto.Base64Tools.FromBase64String(key);
#else
			byte[] bytes = Convert.FromBase64String(key);
#endif
			key = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
			key = ObscuredString.EncryptDecrypt(key, encryptionKey);
			return key;
		}

		private static string EncryptValue(string value)
		{
			string encryptedValue = ObscuredString.EncryptDecrypt(value, encryptionKey);

#if UNITY_FLASH
			encryptedValue = Crypto.Base64Tools.ToBase64String(Encoding.UTF8.GetBytes(encryptedValue));
			encryptedValue += ":" + CalculateChecksum(encryptedValue);
#else
			encryptedValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(encryptedValue));

			if (lockToDevice != DeviceLockLevel.None)
			{
				encryptedValue += ':' + CalculateChecksum(encryptedValue + DeviceHash) + ":" + DeviceHash;
			}
			else
			{
				encryptedValue += ':' + CalculateChecksum(encryptedValue);
			}
#endif

			return encryptedValue;
		}

		private static string DecryptValue(string value)
		{
			string[] rawParts = value.Split(':');

			if (rawParts.Length < 2)
			{
				SavesTampered();
				return "";
			}

			string b64EncryptedValue = rawParts[0];
			string checksum = rawParts[1];

			byte[] bytes;

			try
			{
#if UNITY_FLASH
				bytes = Crypto.Base64Tools.FromBase64String(b64EncryptedValue);
#else
				bytes = Convert.FromBase64String(b64EncryptedValue);
#endif
			}
			catch
			{
				SavesTampered();
				return "";
			}

			string encryptedValue = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
			string clearValue = ObscuredString.EncryptDecrypt(encryptedValue, encryptionKey);
			//return ((byte)clearValue[0]).ToString("X2") + ((byte)clearValue[1]).ToString("X2") + ((byte)clearValue[2]).ToString("X2");
			//return clearValue;
			// checking saves for tamperation
			if (rawParts.Length == 3)
			{
#if UNITY_FLASH
				SavesTampered();
#else
				if (checksum != CalculateChecksum(b64EncryptedValue + DeviceHash))
				{
					SavesTampered();
				}
#endif
			}
			else if (rawParts.Length == 2)
			{
				if (checksum != CalculateChecksum(b64EncryptedValue))
				{
					SavesTampered();
				}
			}
			else
			{
				SavesTampered();
			}

#if !UNITY_FLASH
			// checking saves for foreignness
			if (lockToDevice != DeviceLockLevel.None && !emergencyMode)
			{
				if (rawParts.Length >= 3)
				{
					string deviceID = rawParts[2];
					if (deviceID != DeviceHash)
					{
						if (!readForeignSaves) clearValue = "";
						PossibleForeignSavesDetected();
					}
				}
				else if (lockToDevice == DeviceLockLevel.Strict)
				{
					if (!readForeignSaves) clearValue = "";
					PossibleForeignSavesDetected();
				}
				else
				{
					if (checksum != CalculateChecksum(b64EncryptedValue))
					{
						if (!readForeignSaves) clearValue = "";
						PossibleForeignSavesDetected();
					}
				}
			}
#endif
			return clearValue;
		}

		private static string CalculateChecksum(string input)
		{
			int result = 0;

			byte[] inputBytes = Encoding.UTF8.GetBytes(input + encryptionKey);
			int len = inputBytes.Length;
			int encryptionKeyLen = encryptionKey.Length^64;
			for (int i = 0; i < len; i++)
			{
				byte b = inputBytes[i];
				result += b + b * (i + encryptionKeyLen) % 3;
			}
			
			return result.ToString("X2");
		}

		private static void SavesTampered()
		{
			if (onAlterationDetected != null && !savesAlterationReported)
			{
				savesAlterationReported = true;
				onAlterationDetected();
			}
		}
#if !UNITY_FLASH
		private static void PossibleForeignSavesDetected()
		{
			if (onPossibleForeignSavesDetected != null && !foreignSavesReported)
			{
				foreignSavesReported = true;
				onPossibleForeignSavesDetected();
			}
		}

		private static string GetDeviceID()
		{
			string deviceID = "";
#if UNITY_IPHONE
			deviceID = iPhone.vendorIdentifier;
#endif
			if (String.IsNullOrEmpty(deviceID)) deviceID = SystemInfo.deviceUniqueIdentifier;
			return CalculateChecksum(deviceID);
		}
#endif

		//////////////////////////////////////////////////////
		// methods for legacy PlayerPrefsObscured data reading
		//////////////////////////////////////////////////////
		
		private static string EncryptKeyDeprecated(string key)
		{
			key = ObscuredString.EncryptDecrypt(key);
#if !UNITY_FLASH
			if (lockToDevice != DeviceLockLevel.None)
			{
				key = ObscuredString.EncryptDecrypt(key, GetDeviceIDDeprecated());
			}

			key = Convert.ToBase64String(Encoding.UTF8.GetBytes(key));
#else
			key = Crypto.Base64Tools.ToBase64String(Encoding.UTF8.GetBytes(key));
#endif
			return key;
		}

		private static string DecryptValueDeprecated(string value)
		{
#if !UNITY_FLASH
			byte[] bytes = Convert.FromBase64String(value);
			value = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
#else
			value = Encoding.UTF8.GetString(Crypto.Base64Tools.FromBase64String(value));
#endif

#if !UNITY_FLASH
			if (lockToDevice != DeviceLockLevel.None)
			{
				value = ObscuredString.EncryptDecrypt(value, GetDeviceIDDeprecated());
			}
#endif
			value = ObscuredString.EncryptDecrypt(value, encryptionKey);
			return value;
		}

#if !UNITY_FLASH
		private static string GetDeviceIDDeprecated()
		{
			return SystemInfo.deviceUniqueIdentifier;
		}
#endif

		/// <summary>
		/// Used to specify level of the device lock feature strictness.
		/// </summary>
		public enum DeviceLockLevel : byte
		{
			/// <summary>
			/// Both locked and not locked to any device data can be read.
			/// </summary>
			None,

			/// <summary>
			/// Performs checks for locked data and still allows reading not locked data (useful when you decided to lock your saves in one of app updates and wish to keep user data).
			/// </summary>
			Soft,

			/// <summary>
			/// Only locked to the current device data can be read. This is a preferred mode, but it should be enabled right from the first app release. If you released app without data lock consider using Soft lock or all previously saved data will not be accessible.
			/// </summary>
			Strict
		}
	}
}