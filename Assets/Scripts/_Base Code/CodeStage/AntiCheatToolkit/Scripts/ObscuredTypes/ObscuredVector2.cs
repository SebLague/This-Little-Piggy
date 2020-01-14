using System;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	/// <summary>
	/// Use it instead of regular <c>Vector2</c> for any cheating-sensitive variables.
	/// </summary>
	/// <strong>\htmlonly<font color="FF4040">WARNING:</font>\endhtmlonly Doesn't mimic regular type API, thus should be used with extra caution.</strong> Cast it to regular, not obscured type to work with regular APIs.<br/>
	/// <strong><em>Regular type is faster and memory wiser comparing to the obscured one!</em></strong>
	[Serializable]
	public struct ObscuredVector2
	{
		private static int cryptoKey = 120206;
		private static readonly Vector2 initialFakeValue = Vector2.zero;

		private int currentCryptoKey;
		private RawEncryptedVector2 hiddenValue;
		private Vector2 fakeValue;
		private bool inited;

		private ObscuredVector2(RawEncryptedVector2 value)
		{
			currentCryptoKey = cryptoKey;
			hiddenValue = value;
			fakeValue = initialFakeValue;
			inited = true;
		}

		public float x
		{
			get
			{
				float decrypted = InternalDecryptField(hiddenValue.x);
				if (Detectors.ObscuredCheatingDetector.isRunning && !fakeValue.Equals(initialFakeValue) && Math.Abs(decrypted - fakeValue.x) > Detectors.ObscuredCheatingDetector.Instance.vector2Epsilon)
				{
					Detectors.ObscuredCheatingDetector.Instance.OnCheatingDetected();
				}
				return decrypted;
			}

			set
			{
				hiddenValue.x = InternalEncryptField(value);
				if (Detectors.ObscuredCheatingDetector.isRunning)
				{
					fakeValue.x = value;
				}
			}
		}

		public float y
		{
			get
			{
				float decrypted = InternalDecryptField(hiddenValue.y);
				if (Detectors.ObscuredCheatingDetector.isRunning && !fakeValue.Equals(initialFakeValue) && Math.Abs(decrypted - fakeValue.y) > Detectors.ObscuredCheatingDetector.Instance.vector2Epsilon)
				{
					Detectors.ObscuredCheatingDetector.Instance.OnCheatingDetected();
				}
				return decrypted;
			}

			set
			{
				hiddenValue.y = InternalEncryptField(value);
				if (Detectors.ObscuredCheatingDetector.isRunning)
				{
					fakeValue.y = value;
				}
			}
		}

		public float this[int index]
		{
			get
			{
				switch (index)
				{
					case 0:
						return x;
					case 1:
						return y;
					default:
						throw new IndexOutOfRangeException("Invalid ObscuredVector2 index!");
				}
			}
			set
			{
				switch (index)
				{
					case 0:
						x = value;
						break;
					case 1:
						y = value;
						break;
					default:
						throw new IndexOutOfRangeException("Invalid ObscuredVector2 index!");
				}
			}
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
		/// Use this simple encryption method to encrypt any Vector2 value, uses default crypto key.
		/// </summary>
		public static RawEncryptedVector2 Encrypt(Vector2 value)
		{
			return Encrypt(value, 0);
		}

		/// <summary>
		/// Use this simple encryption method to encrypt any Vector2 value, uses passed crypto key.
		/// </summary>
		public static RawEncryptedVector2 Encrypt(Vector2 value, int key)
		{
			if (key == 0)
			{
				key = cryptoKey;
			}

			RawEncryptedVector2 result;
			result.x = ObscuredFloat.Encrypt(value.x, key);
			result.y = ObscuredFloat.Encrypt(value.y, key);

			return result;
		}

		/// <summary>
		/// Use it to decrypt RawEncryptedVector2 you got from Encrypt(), uses default crypto key.
		/// </summary>
		public static Vector2 Decrypt(RawEncryptedVector2 value)
		{
			return Decrypt(value, 0);
		}

		/// <summary>
		/// Use it to decrypt RawEncryptedVector2 you got from Encrypt(), uses passed crypto key.
		/// </summary>
		public static Vector2 Decrypt(RawEncryptedVector2 value, int key)
		{
			if (key == 0)
			{
				key = cryptoKey;
			}

			Vector2 result;
			result.x = ObscuredFloat.Decrypt(value.x, key);
			result.y = ObscuredFloat.Decrypt(value.y, key);

			return result;
		}

		/// <summary>
		/// Allows to pick current obscured value as is.
		/// </summary>
		/// Use it in conjunction with SetEncrypted().<br/>
		/// Useful for saving data in obscured state.
		public RawEncryptedVector2 GetEncrypted()
		{
			ApplyNewCryptoKey();
			return hiddenValue;
		}

		/// <summary>
		/// Allows to explicitly set current obscured value.
		/// </summary>
		/// Use it in conjunction with GetEncrypted().<br/>
		/// Useful for loading data stored in obscured state.
		public void SetEncrypted(RawEncryptedVector2 encrypted)
		{
			hiddenValue = encrypted;
			if (Detectors.ObscuredCheatingDetector.isRunning)
			{
				fakeValue = InternalDecrypt();
			}
		}

		private Vector2 InternalDecrypt()
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

			Vector2 value;

			value.x = ObscuredFloat.Decrypt(hiddenValue.x, key);
			value.y = ObscuredFloat.Decrypt(hiddenValue.y, key);

			if (Detectors.ObscuredCheatingDetector.isRunning && !fakeValue.Equals(initialFakeValue) && !CompareVectorsWithTolerance(value, fakeValue))
			{
				Detectors.ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}

			return value;
		}

		private bool CompareVectorsWithTolerance(Vector2 vector1, Vector2 vector2)
		{
			float epsilon = Detectors.ObscuredCheatingDetector.Instance.vector2Epsilon;
			return Math.Abs(vector1.x - vector2.x) < epsilon &&
				   Math.Abs(vector1.y - vector2.y) < epsilon;
		}

		private float InternalDecryptField(int encrypted)
		{
			int key = cryptoKey;

			if (currentCryptoKey != cryptoKey)
			{
				key = currentCryptoKey;
			}

			float result = ObscuredFloat.Decrypt(encrypted, key);
			return result;
		}

		private int InternalEncryptField(float encrypted)
		{
			int result = ObscuredFloat.Encrypt(encrypted, cryptoKey);
			return result;
		}

#region operators, overrides, interface implementations
		//! @cond
		public static implicit operator ObscuredVector2(Vector2 value)
		{
			ObscuredVector2 obscured = new ObscuredVector2(Encrypt(value));
			if (Detectors.ObscuredCheatingDetector.isRunning)
			{
				obscured.fakeValue = value;
			}
			return obscured;
		}

		public static implicit operator Vector2(ObscuredVector2 value)
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
		/// Returns a nicely formatted string for this vector.
		/// </summary>
		public override string ToString()
		{
			return InternalDecrypt().ToString();
		}

		/// <summary>
		/// Returns a nicely formatted string for this vector.
		/// </summary>
		public string ToString(string format)
		{
			return InternalDecrypt().ToString(format);
		}

		//! @endcond
		#endregion

#region deprecated
		/// <summary>
		/// This is a deprecated version of Encrypt(Vector2). May lead to the cheat detection false positives.
		/// </summary>
		[Obsolete("This method may lead to the cheating detection false positives. Please use Encrypt() instead.", false)]
		public static Vector2 EncryptDeprecated(Vector2 value)
		{
			return EncryptDeprecated(value, 0);
		}

		/// <summary>
		/// This is a deprecated version of Encrypt(Vector2, int). May lead to the cheat detection false positives.
		/// </summary>
		[Obsolete("This method may lead to the cheating detection false positives. Please use Encrypt() instead.", false)]
		public static Vector2 EncryptDeprecated(Vector2 value, int key)
		{
			if (key == 0)
			{
				key = cryptoKey;
			}

			value.x = ObscuredFloat.Encrypt(value.x, key);
			value.y = ObscuredFloat.Encrypt(value.y, key);

			return value;
		}

		/// <summary>
		/// This is a deprecated version of Decrypt(Vector2). May lead to the cheat detection false positives.
		/// </summary>
		[Obsolete("This method may lead to the cheating detection false positives. Please use Decrypt() instead.", false)]
		public static Vector2 DecryptDeprecated(Vector2 value)
		{
			return DecryptDeprecated(value, 0);
		}

		/// <summary>
		/// This is a deprecated version of Decrypt(Vector2, int). May lead to the cheat detection false positives.
		/// </summary>
		[Obsolete("This method may lead to the cheating detection false positives. Please use Decrypt() instead.", false)]
		public static Vector2 DecryptDeprecated(Vector2 value, int key)
		{
			if (key == 0)
			{
				key = cryptoKey;
			}

			value.x = ObscuredFloat.Decrypt((int)value.x, key);

			return value;
		}

		/// <summary>
		/// This is a deprecated version of GetEncrypted(). May lead to the cheat detection false positives.
		/// </summary>
		[Obsolete("This method may lead to the cheating detection false positives. Please use GetEncrypted() instead.", false)]
		public Vector2 GetEncryptedDeprecated()
		{
			ApplyNewCryptoKey();
			return (Vector2)hiddenValue;
		}

		/// <summary>
		/// This is a deprecated version of SetEncrypted(). May lead to the cheat detection false positives.
		/// </summary>
		[Obsolete("This method may lead to the cheating detection false positives. Please use SetEncrypted() instead.", false)]
		public void SetEncryptedDeprecated(Vector2 encrypted)
		{
			hiddenValue = (RawEncryptedVector2)encrypted;
			if (Detectors.ObscuredCheatingDetector.isRunning)
			{
				fakeValue = InternalDecrypt();
			}
		}
#endregion

		/// <summary>
		/// Used to store encrypted Vector2.
		/// </summary>
		public struct RawEncryptedVector2
		{
			internal int x;
			internal int y;

			// destructive, value accuracy will be reduced, used for depecated methods
			private RawEncryptedVector2(float x, float y)
			{
				this.x = (int)x;
				this.y = (int)y;
			}

			// destructive, value accuracy will be reduced, used for depecated methods
			public static explicit operator Vector2(RawEncryptedVector2 value)
			{
				return new Vector2(value.x, value.y);
			}

			// destructive, value accuracy will be reduced, used for depecated methods
			public static explicit operator RawEncryptedVector2(Vector2 value)
			{
				return new RawEncryptedVector2(value.x, value.y);
			}
		}
	
	}
}