using System;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	/// <summary>
	/// Use it instead of regular <c>Quaternion</c> for any cheating-sensitive variables.
	/// </summary>
	/// <strong>\htmlonly<font color="FF4040">WARNING:</font>\endhtmlonly Doesn't mimic regular type API, thus should be used with extra caution.</strong> Cast it to regular, not obscured type to work with regular APIs.<br/>
	/// <strong><em>Regular type is faster and memory wiser comparing to the obscured one!</em></strong>
	[Serializable]
	public struct ObscuredQuaternion
	{
		private static int cryptoKey = 120205;
		private static readonly Quaternion initialFakeValue = Quaternion.identity;

		private int currentCryptoKey;
		private RawEncryptedQuaternion hiddenValue;
		public Quaternion fakeValue;
		private bool inited;

		private ObscuredQuaternion(RawEncryptedQuaternion value)
		{
			currentCryptoKey = cryptoKey;
			hiddenValue = value;
			fakeValue = initialFakeValue;
			inited = true;
		}

		/// <summary>
		/// Allows to change default crypto key of this type instances. All new instances will use specified key.<br/>
		/// All current instances will use previous key unless you call ApplyNewCryptoKey() on them explicitly.
		/// </summary>
		public static void SetNewCryptoKey(int newKey)
		{
			cryptoKey = newKey;
		}

		/// <summary>
		/// Use it after SetNewCryptoKey() to re-encrypt current instance using new crypto key.
		/// </summary>
		public void ApplyNewCryptoKey()
		{
			if (currentCryptoKey != cryptoKey)
			{
				hiddenValue = Encrypt(InternalDecrypt(), cryptoKey);
				currentCryptoKey = cryptoKey;
			}
		}

		/// <summary>
		/// Use this simple encryption method to encrypt any Quaternion value, uses default crypto key.
		/// </summary>
		public static RawEncryptedQuaternion Encrypt(Quaternion value)
		{
			return Encrypt(value, 0);
		}

		/// <summary>
		/// Use this simple encryption method to encrypt any Quaternion value, uses passed crypto key.
		/// </summary>
		public static RawEncryptedQuaternion Encrypt(Quaternion value, int key)
		{
			if (key == 0)
			{
				key = cryptoKey;
			}

			RawEncryptedQuaternion result;
			result.x = ObscuredFloat.Encrypt(value.x, key);
			result.y = ObscuredFloat.Encrypt(value.y, key);
			result.z = ObscuredFloat.Encrypt(value.z, key);
			result.w = ObscuredFloat.Encrypt(value.w, key);

			return result;
		}

		/// <summary>
		/// Use it to decrypt RawEncryptedQuaternion you got from Encrypt(), uses default crypto key.
		/// </summary>
		public static Quaternion Decrypt(RawEncryptedQuaternion value)
		{
			return Decrypt(value, 0);
		}

		/// <summary>
		/// Use it to decrypt RawEncryptedQuaternion you got from Encrypt(), uses passed crypto key.
		/// </summary>
		public static Quaternion Decrypt(RawEncryptedQuaternion value, int key)
		{
			if (key == 0)
			{
				key = cryptoKey;
			}

			Quaternion result;
			result.x = ObscuredFloat.Decrypt(value.x, key);
			result.y = ObscuredFloat.Decrypt(value.y, key);
			result.z = ObscuredFloat.Decrypt(value.z, key);
			result.w = ObscuredFloat.Decrypt(value.w, key);

			return result;
		}

		/// <summary>
		/// Allows to pick current obscured value as is.
		/// </summary>
		/// Use it in conjunction with SetEncrypted().<br/>
		/// Useful for saving data in obscured state.
		public RawEncryptedQuaternion GetEncrypted()
		{
			ApplyNewCryptoKey();
			return hiddenValue;
		}

		/// <summary>
		/// Allows to explicitly set current obscured value.
		/// </summary>
		/// Use it in conjunction with GetEncrypted().<br/>
		/// Useful for loading data stored in obscured state.
		public void SetEncrypted(RawEncryptedQuaternion encrypted)
		{
			hiddenValue = encrypted;
			if (Detectors.ObscuredCheatingDetector.isRunning)
			{
				fakeValue = InternalDecrypt();
			}
		}

		private Quaternion InternalDecrypt()
		{
			if (!inited)
			{
				currentCryptoKey = cryptoKey;
				hiddenValue = Encrypt(initialFakeValue);
				fakeValue = initialFakeValue;
				inited = true;
			}

			int key = cryptoKey;

			if (currentCryptoKey != cryptoKey)
			{
				key = currentCryptoKey;
			}

			Quaternion value;

			value.x = ObscuredFloat.Decrypt(hiddenValue.x, key);
			value.y = ObscuredFloat.Decrypt(hiddenValue.y, key);
			value.z = ObscuredFloat.Decrypt(hiddenValue.z, key);
			value.w = ObscuredFloat.Decrypt(hiddenValue.w, key);

			if (Detectors.ObscuredCheatingDetector.isRunning && !fakeValue.Equals(initialFakeValue) && !CompareQuaternionsWithTolerance(value, fakeValue))
			{
				Detectors.ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}

			return value;
		}

		private bool CompareQuaternionsWithTolerance(Quaternion q1, Quaternion q2)
		{
			float epsilon = Detectors.ObscuredCheatingDetector.Instance.quaternionEpsilon;
			return Math.Abs(q1.x - q2.x) < epsilon &&
				   Math.Abs(q1.y - q2.y) < epsilon &&
				   Math.Abs(q1.z - q2.z) < epsilon &&
				   Math.Abs(q1.w - q2.w) < epsilon;
		}

		#region operators, overrides, interface implementations
		//! @cond
		public static implicit operator ObscuredQuaternion(Quaternion value)
		{
			ObscuredQuaternion obscured = new ObscuredQuaternion(Encrypt(value));
			if (Detectors.ObscuredCheatingDetector.isRunning)
			{
				obscured.fakeValue = value;
			}
			return obscured;
		}

		public static implicit operator Quaternion(ObscuredQuaternion value)
		{
			return value.InternalDecrypt();
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// 
		/// <returns>
		/// A 32-bit signed integer hash code.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override int GetHashCode()
		{
			return InternalDecrypt().GetHashCode();
		}

		/// <summary>
		/// Returns a nicely formatted string of the Quaternion.
		/// </summary>
		public override string ToString()
		{
			return InternalDecrypt().ToString();
		}

		/// <summary>
		/// Returns a nicely formatted string of the Quaternion.
		/// </summary>
		public string ToString(string format)
		{
			return InternalDecrypt().ToString(format);
		}

		//! @endcond
#endregion

#region deprecated
		/// <summary>
		/// This is a deprecated version of Encrypt(Quaternion). May lead to the cheat detection false positives.
		/// </summary>
		[Obsolete("This method may lead to the cheating detection false positives. Please use Encrypt() instead.", false)]
		public static Quaternion EncryptDeprecated(Quaternion value)
		{
			return EncryptDeprecated(value, 0);
		}

		/// <summary>
		/// This is a deprecated version of Encrypt(Quaternion, int). May lead to the cheat detection false positives.
		/// </summary>
		[Obsolete("This method may lead to the cheating detection false positives. Please use Encrypt() instead.", false)]
		public static Quaternion EncryptDeprecated(Quaternion value, int key)
		{
			if (key == 0)
			{
				key = cryptoKey;
			}

			value.x = ObscuredFloat.Encrypt(value.x, key);
			value.y = ObscuredFloat.Encrypt(value.y, key);
			value.z = ObscuredFloat.Encrypt(value.z, key);
			value.w = ObscuredFloat.Encrypt(value.w, key);

			return value;
		}

		/// <summary>
		/// This is a deprecated version of Decrypt(Quaternion). May lead to the cheat detection false positives.
		/// </summary>
		[Obsolete("This method may lead to the cheating detection false positives. Please use Decrypt() instead.", false)]
		public static Quaternion DecryptDeprecated(Quaternion value)
		{
			return DecryptDeprecated(value, 0);
		}

		/// <summary>
		/// This is a deprecated version of Decrypt(Quaternion, int). May lead to the cheat detection false positives.
		/// </summary>
		[Obsolete("This method may lead to the cheating detection false positives. Please use Decrypt() instead.", false)]
		public static Quaternion DecryptDeprecated(Quaternion value, int key)
		{
			if (key == 0)
			{
				key = cryptoKey;
			}

			value.x = ObscuredFloat.Decrypt((int)value.x, key);
			value.y = ObscuredFloat.Decrypt((int)value.y, key);
			value.z = ObscuredFloat.Decrypt((int)value.z, key);
			value.w = ObscuredFloat.Decrypt((int)value.w, key);

			return value;
		}

		/// <summary>
		/// This is a deprecated version of GetEncrypted(). May lead to the cheat detection false positives.
		/// </summary>
		[Obsolete("This method may lead to the cheating detection false positives. Please use GetEncrypted() instead.", false)]
		public Quaternion GetEncryptedDeprecated()
		{
			ApplyNewCryptoKey();
			return (Quaternion)hiddenValue;
		}

		/// <summary>
		/// This is a deprecated version of SetEncrypted(). May lead to the cheat detection false positives.
		/// </summary>
		[Obsolete("This method may lead to the cheating detection false positives. Please use SetEncrypted() instead.", false)]
		public void SetEncryptedDeprecated(Quaternion encrypted)
		{
			hiddenValue = (RawEncryptedQuaternion)encrypted;
			if (Detectors.ObscuredCheatingDetector.isRunning)
			{
				fakeValue = InternalDecrypt();
			}
		}
#endregion

		/// <summary>
		/// Used to store encrypted Quaternion.
		/// </summary>
		public struct RawEncryptedQuaternion
		{
			internal int x;
			internal int y;
			internal int z;
			internal int w;

			// destructive, value accuracy will be reduced, used for depecated methods
			private RawEncryptedQuaternion(float x, float y, float z, float w)
			{
				this.x = (int)x;
				this.y = (int)y;
				this.z = (int)z;
				this.w = (int)w;
			}

			// destructive, value accuracy will be reduced, used for depecated methods
			public static explicit operator Quaternion(RawEncryptedQuaternion value)
			{
				return new Quaternion(value.x, value.y, value.z, value.w);
			}

			// destructive, value accuracy will be reduced, used for depecated methods
			public static explicit operator RawEncryptedQuaternion(Quaternion value)
			{
				return new RawEncryptedQuaternion(value.x, value.y, value.z, value.w);
			}
		}
	}
}