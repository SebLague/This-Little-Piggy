using System;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	/// <summary>
	/// Use it instead of regular <c>Vector3</c> for any cheating-sensitive variables.
	/// </summary>
	/// <strong>\htmlonly<font color="FF4040">WARNING:</font>\endhtmlonly Doesn't mimic regular type API, thus should be used with extra caution.</strong> Cast it to regular, not obscured type to work with regular APIs.<br/>
	/// <strong><em>Regular type is faster and memory wiser comparing to the obscured one!</em></strong>
	[Serializable]
	public struct ObscuredVector3
	{
		private static int cryptoKey = 120207;
		private static readonly Vector3 initialFakeValue = Vector3.zero;

		private int currentCryptoKey;
		private RawEncryptedVector3 hiddenValue;
		private Vector3 fakeValue;
		private bool inited;

		private ObscuredVector3(RawEncryptedVector3 encrypted)
		{
			currentCryptoKey = cryptoKey;
			hiddenValue = encrypted;
			fakeValue = initialFakeValue;
			inited = true;
		}

		public float x
		{
			get
			{
				float decrypted = InternalDecryptField(hiddenValue.x);
				if (Detectors.ObscuredCheatingDetector.isRunning && !fakeValue.Equals(initialFakeValue) && Math.Abs(decrypted - fakeValue.x) > Detectors.ObscuredCheatingDetector.Instance.vector3Epsilon)
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
				if (Detectors.ObscuredCheatingDetector.isRunning && !fakeValue.Equals(initialFakeValue) && Math.Abs(decrypted - fakeValue.y) > Detectors.ObscuredCheatingDetector.Instance.vector3Epsilon)
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

		public float z
		{
			get
			{
				float decrypted = InternalDecryptField(hiddenValue.z);
				if (Detectors.ObscuredCheatingDetector.isRunning && !fakeValue.Equals(initialFakeValue) && Math.Abs(decrypted - fakeValue.z) > Detectors.ObscuredCheatingDetector.Instance.vector3Epsilon)
				{
					Detectors.ObscuredCheatingDetector.Instance.OnCheatingDetected();
				}
				return decrypted;
			}

			set
			{
				hiddenValue.z = InternalEncryptField(value);
				if (Detectors.ObscuredCheatingDetector.isRunning)
				{
					fakeValue.z = value;
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
					case 2:
						return z;
					default:
						throw new IndexOutOfRangeException("Invalid ObscuredVector3 index!");
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
					case 2:
						z = value;
						break;
					default:
						throw new IndexOutOfRangeException("Invalid ObscuredVector3 index!");
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
		/// Use this simple encryption method to encrypt any Vector3 value, uses default crypto key.
		/// </summary>
		public static RawEncryptedVector3 Encrypt(Vector3 value)
		{
			return Encrypt(value, 0);
		}

		/// <summary>
		/// Use this simple encryption method to encrypt any Vector3 value, uses passed crypto key.
		/// </summary>
		public static RawEncryptedVector3 Encrypt(Vector3 value, int key)
		{
			if (key == 0)
			{
				key = cryptoKey;
			}

			RawEncryptedVector3 result;
			result.x = ObscuredFloat.Encrypt(value.x, key);
			result.y = ObscuredFloat.Encrypt(value.y, key);
			result.z = ObscuredFloat.Encrypt(value.z, key);

			return result;
		}

		/// <summary>
		/// Use it to decrypt RawEncryptedVector3 you got from Encrypt(), uses default crypto key.
		/// </summary>
		public static Vector3 Decrypt(RawEncryptedVector3 value)
		{
			return Decrypt(value, 0);
		}

		/// <summary>
		/// Use it to decrypt RawEncryptedVector3 you got from Encrypt(), uses passed crypto key.
		/// </summary>
		public static Vector3 Decrypt(RawEncryptedVector3 value, int key)
		{
			if (key == 0)
			{
				key = cryptoKey;
			}

			Vector3 result;
			result.x = ObscuredFloat.Decrypt(value.x, key);
			result.y = ObscuredFloat.Decrypt(value.y, key);
			result.z = ObscuredFloat.Decrypt(value.z, key);

			return result;
		}

		/// <summary>
		/// Allows to pick current obscured value as is.
		/// </summary>
		/// Use it in conjunction with SetEncrypted().<br/>
		/// Useful for saving data in obscured state.
		public RawEncryptedVector3 GetEncrypted()
		{
			ApplyNewCryptoKey();
			return hiddenValue;
		}

		/// <summary>
		/// Allows to explicitly set current obscured value.
		/// </summary>
		/// Use it in conjunction with GetEncrypted().<br/>
		/// Useful for loading data stored in obscured state.
		public void SetEncrypted(RawEncryptedVector3 encrypted)
		{
			hiddenValue = encrypted;
			if (Detectors.ObscuredCheatingDetector.isRunning)
			{
				fakeValue = InternalDecrypt();
			}
		}

		private Vector3 InternalDecrypt()
		{
			if (!inited)
			{
				currentCryptoKey = cryptoKey;
				hiddenValue = Encrypt(initialFakeValue, cryptoKey);
				fakeValue = initialFakeValue;
				inited = true;
			}

			int key = cryptoKey;

			if (currentCryptoKey != cryptoKey)
			{
				key = currentCryptoKey;
			}

			Vector3 value;

			value.x = ObscuredFloat.Decrypt(hiddenValue.x, key);
			value.y = ObscuredFloat.Decrypt(hiddenValue.y, key);
			value.z = ObscuredFloat.Decrypt(hiddenValue.z, key);

			if (Detectors.ObscuredCheatingDetector.isRunning && !fakeValue.Equals(Vector3.zero) && !CompareVectorsWithTolerance(value, fakeValue))
			{
				Detectors.ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}

			return value;
		}

		private bool CompareVectorsWithTolerance(Vector3 vector1, Vector3 vector2)
		{
			float epsilon = Detectors.ObscuredCheatingDetector.Instance.vector3Epsilon;
			return Math.Abs(vector1.x - vector2.x) < epsilon &&
				   Math.Abs(vector1.y - vector2.y) < epsilon &&
				   Math.Abs(vector1.z - vector2.z) < epsilon;
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

#region operators, overrides, etc.
		//! @cond
		public static implicit operator ObscuredVector3(Vector3 value)
		{
			ObscuredVector3 obscured = new ObscuredVector3(Encrypt(value, cryptoKey));
			if (Detectors.ObscuredCheatingDetector.isRunning)
			{
				obscured.fakeValue = value;
			}
			return obscured;
		}

		public static implicit operator Vector3(ObscuredVector3 value)
		{
			return value.InternalDecrypt();
		}

		public static ObscuredVector3 operator +(ObscuredVector3 a, ObscuredVector3 b)
		{
			return a.InternalDecrypt() + b.InternalDecrypt();
		}

		public static ObscuredVector3 operator +(Vector3 a, ObscuredVector3 b)
		{
			return a + b.InternalDecrypt();
		}

		public static ObscuredVector3 operator +(ObscuredVector3 a, Vector3 b)
		{
			return a.InternalDecrypt() + b;
		}

		public static ObscuredVector3 operator -(ObscuredVector3 a, ObscuredVector3 b)
		{
			return a.InternalDecrypt() - b.InternalDecrypt();
		}

		public static ObscuredVector3 operator -(Vector3 a, ObscuredVector3 b)
		{
			return a - b.InternalDecrypt();
		}

		public static ObscuredVector3 operator -(ObscuredVector3 a, Vector3 b)
		{
			return a.InternalDecrypt() - b;
		}

		public static ObscuredVector3 operator -(ObscuredVector3 a)
		{
			return -a.InternalDecrypt();
		}

		public static ObscuredVector3 operator *(ObscuredVector3 a, float d)
		{
			return a.InternalDecrypt() * d;
		}

		public static ObscuredVector3 operator *(float d, ObscuredVector3 a)
		{
			return d * a.InternalDecrypt();
		}

		public static ObscuredVector3 operator /(ObscuredVector3 a, float d)
		{
			return a.InternalDecrypt() / d;
		}

		public static bool operator ==(ObscuredVector3 lhs, ObscuredVector3 rhs)
		{
			return lhs.InternalDecrypt() == rhs.InternalDecrypt();
		}

		public static bool operator ==(Vector3 lhs, ObscuredVector3 rhs)
		{
			return lhs == rhs.InternalDecrypt();
		}

		public static bool operator ==(ObscuredVector3 lhs, Vector3 rhs)
		{
			return lhs.InternalDecrypt() == rhs;
		}

		public static bool operator !=(ObscuredVector3 lhs, ObscuredVector3 rhs)
		{
			return lhs.InternalDecrypt() != rhs.InternalDecrypt();
		}

		public static bool operator !=(Vector3 lhs, ObscuredVector3 rhs)
		{
			return lhs != rhs.InternalDecrypt();
		}

		public static bool operator !=(ObscuredVector3 lhs, Vector3 rhs)
		{
			return lhs.InternalDecrypt() != rhs;
		}

		public override bool Equals(object other)
		{
			return InternalDecrypt().Equals(other);
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
		/// This is a deprecated version of Encrypt(Vector3). May lead to the cheat detection false positives.
		/// </summary>
		[Obsolete("This method may lead to the cheating detection false positives. Please use Encrypt() instead.", false)]
		public static Vector3 EncryptDeprecated(Vector3 value)
		{
			return EncryptDeprecated(value, 0);
		}

		/// <summary>
		/// This is a deprecated version of Encrypt(Vector3, int). May lead to the cheat detection false positives.
		/// </summary>
		[Obsolete("This method may lead to the cheating detection false positives. Please use Encrypt() instead.", false)]
		public static Vector3 EncryptDeprecated(Vector3 value, int key)
		{
			if (key == 0)
			{
				key = cryptoKey;
			}

			value.x = ObscuredFloat.Encrypt(value.x, key);
			value.y = ObscuredFloat.Encrypt(value.y, key);
			value.z = ObscuredFloat.Encrypt(value.z, key);

			return value;
		}

		/// <summary>
		/// This is a deprecated version of Decrypt(Vector3). May lead to the cheat detection false positives.
		/// </summary>
		[Obsolete("This method may lead to the cheating detection false positives. Please use Decrypt() instead.", false)]
		public static Vector3 DecryptDeprecated(Vector3 value)
		{
			return DecryptDeprecated(value, 0);
		}

		/// <summary>
		/// This is a deprecated version of Decrypt(Vector3, int). May lead to the cheat detection false positives.
		/// </summary>
		[Obsolete("This method may lead to the cheating detection false positives. Please use Decrypt() instead.", false)]
		public static Vector3 DecryptDeprecated(Vector3 value, int key)
		{
			if (key == 0)
			{
				key = cryptoKey;
			}

			value.x = ObscuredFloat.Decrypt((int)value.x, key);
			value.y = ObscuredFloat.Decrypt((int)value.y, key);
			value.z = ObscuredFloat.Decrypt((int)value.z, key);

			return value;
		}

		/// <summary>
		/// This is a deprecated version of GetEncrypted(). May lead to the cheat detection false positives.
		/// </summary>
		[Obsolete("This method may lead to the cheating detection false positives. Please use GetEncrypted() instead.", false)]
		public Vector3 GetEncryptedDeprecated()
		{
			ApplyNewCryptoKey();
			return (Vector3)hiddenValue;
		}

		/// <summary>
		/// This is a deprecated version of SetEncrypted(). May lead to the cheat detection false positives.
		/// </summary>
		[Obsolete("This method may lead to the cheating detection false positives. Please use SetEncrypted() instead.", false)]
		public void SetEncryptedDeprecated(Vector3 encrypted)
		{
			hiddenValue = (RawEncryptedVector3)encrypted;
			if (Detectors.ObscuredCheatingDetector.isRunning)
			{
				fakeValue = InternalDecrypt();
			}
		}
#endregion

		/// <summary>
		/// Used to store encrypted Vector3.
		/// </summary>
		public struct RawEncryptedVector3
		{
			internal int x;
			internal int y;
			internal int z;

			// destructive, value accuracy will be reduced, used for depecated methods
			private RawEncryptedVector3(float x, float y, float z)
			{
				this.x = (int)x;
				this.y = (int)y;
				this.z = (int)z;
			}

			// destructive, value accuracy will be reduced, used for depecated methods
			public static explicit operator Vector3(RawEncryptedVector3 value)
			{
				return new Vector3(value.x, value.y, value.z);
			}

			// destructive, value accuracy will be reduced, used for depecated methods
			public static explicit operator RawEncryptedVector3(Vector3 value)
			{
				return new RawEncryptedVector3(value.x, value.y, value.z);
			}
		}
	}
}