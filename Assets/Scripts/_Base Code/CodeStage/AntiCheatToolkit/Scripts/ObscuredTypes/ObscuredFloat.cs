using System;
using UnityEngine;
#if !UNITY_FLASH
using System.Runtime.InteropServices;
#endif

namespace CodeStage.AntiCheat.ObscuredTypes
{
	/// <summary>
	/// Use it instead of regular <c>float</c> for any cheating-sensitive variables.
	/// </summary>
	/// <strong>\htmlonly<font color="7030A0">IMPORTANT:</font>\endhtmlonly ObscuredFloat in Flash Player works significanlty slower comparing to other platforms.</strong><br/>
	/// <strong><em>Regular type is faster and memory wiser comparing to the obscured one!</em></strong>
	[Serializable]
	public struct ObscuredFloat : IEquatable<ObscuredFloat>
	{
		private static int cryptoKey = 230887;

		[SerializeField]
		private int currentCryptoKey;

		[SerializeField]
		private byte[] hiddenValue;

		[SerializeField]
		private float fakeValue;

		[SerializeField]
		private bool inited;

#if !UNITY_FLASH
		private ObscuredFloat(byte[] value)
		{
			currentCryptoKey = cryptoKey;
			hiddenValue = value;
			fakeValue = 0;
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
				hiddenValue = InternalEncrypt(InternalDecrypt(), cryptoKey);
				currentCryptoKey = cryptoKey;
			}
		}

		/// <summary>
		/// Use this simple encryption method to encrypt any float value, uses default crypto key.
		/// </summary>
		public static int Encrypt(float value)
		{
			return Encrypt(value, cryptoKey);
		}

		/// <summary>
		/// Use this simple encryption method to encrypt any float value, uses passed crypto key.
		/// </summary>
		public static int Encrypt(float value, int key)
		{
			var u = new FloatIntBytesUnion();
			u.f = value;
			u.i = u.i ^ key;

			return u.i;
		}

		private static byte[] InternalEncrypt(float value)
		{
			return InternalEncrypt(value, 0);
		}

		private static byte[] InternalEncrypt(float value, int key)
		{
			int currKey = key;
			if (currKey == 0)
			{
				currKey = cryptoKey;
			}

			var u = new FloatIntBytesUnion();
			u.f = value;
			u.i = u.i ^ currKey;

			return new[] { u.b1, u.b2, u.b3, u.b4 };
		}

		/// <summary>
		/// Use it to decrypt int you got from Encrypt(float) back to float, uses default crypto key.
		/// </summary>
		public static float Decrypt(int value)
		{
			return Decrypt(value, cryptoKey);
		}

		/// <summary>
		/// Use it to decrypt int you got from Encrypt(float) back to float, uses passed crypto key.
		/// </summary>
		public static float Decrypt(int value, int key)
		{
			var u = new FloatIntBytesUnion();
			u.i = value ^ key;
			return u.f;
		}

		/// <summary>
		/// Allows to pick current obscured value as is.
		/// </summary>
		/// Use it in conjunction with SetEncrypted().<br/>
		/// Useful for saving data in obscured state.
		public int GetEncrypted()
		{
			ApplyNewCryptoKey();

			var union = new FloatIntBytesUnion();
			union.b1 = hiddenValue[0];
			union.b2 = hiddenValue[1];
			union.b3 = hiddenValue[2];
			union.b4 = hiddenValue[3];

			return union.i;
		}

		/// <summary>
		/// Allows to explicitly set current obscured value.
		/// </summary>
		/// Use it in conjunction with GetEncrypted().<br/>
		/// Useful for loading data stored in obscured state.
		public void SetEncrypted(int encrypted)
		{
			FloatIntBytesUnion union = new FloatIntBytesUnion();
			union.i = encrypted;

			hiddenValue = new[] { union.b1, union.b2, union.b3, union.b4 };

			if (Detectors.ObscuredCheatingDetector.isRunning)
			{
				fakeValue = InternalDecrypt();
			}
		}

		private float InternalDecrypt()
		{
			if (!inited)
			{
				currentCryptoKey = cryptoKey;
				hiddenValue = InternalEncrypt(0);
				fakeValue = 0;
				inited = true;
			}

			int key = cryptoKey;

			if (currentCryptoKey != cryptoKey)
			{
				key = currentCryptoKey;
			}

			var union = new FloatIntBytesUnion();
			union.b1 = hiddenValue[0];
			union.b2 = hiddenValue[1];
			union.b3 = hiddenValue[2];
			union.b4 = hiddenValue[3];

			union.i = union.i ^ key;

			float decrypted = union.f;

			if (Detectors.ObscuredCheatingDetector.isRunning && fakeValue != 0 && Math.Abs(decrypted - fakeValue) > Detectors.ObscuredCheatingDetector.Instance.floatEpsilon)
			{
				Detectors.ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}

			return decrypted;
		}

		[StructLayout(LayoutKind.Explicit)]
		private struct FloatIntBytesUnion
		{
			[FieldOffset(0)]
			public float f;

			[FieldOffset(0)]
			public int i;

			[FieldOffset(0)]
			public byte b1;

			[FieldOffset(1)]
			public byte b2;

			[FieldOffset(2)]
			public byte b3;

			[FieldOffset(3)]
			public byte b4;
		}
		//! @cond
#else

		private ObscuredFloat(byte[] value)
		{
			currentCryptoKey = cryptoKey;
			hiddenValue = value;
			fakeValue = 0;
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
		/// Use this simple encryption method to encrypt any float value, uses default crypto key.
		/// </summary>
		public static int Encrypt(float value)
		{
			return Encrypt(value, cryptoKey);
		}

		/// <summary>
		/// Use this simple encryption method to encrypt any float value, uses passed crypto key.
		/// </summary>
		public static int Encrypt(float value, int key)
		{
			return BitConverter.ToInt32(BitConverter.GetBytes(value), 0) ^ key;
		}

		private static byte[] InternalEncrypt(float value)
		{
			return InternalEncrypt(value, 0);
		}

		private static byte[] InternalEncrypt(float value, int key)
		{
			int currKey = key;
			if (currKey == 0)
			{
				currKey = cryptoKey;
			}

			int num = BitConverter.ToInt32(BitConverter.GetBytes(value), 0) ^ currKey;

			return BitConverter.GetBytes(num);
		}

		/// <summary>
		/// Use it to decrypt int you got from Encrypt(float) back to float, uses default crypto key.
		/// </summary>
		public static float Decrypt(int value)
		{
			return Decrypt(value, cryptoKey);
		}

		/// <summary>
		/// Use it to decrypt int you got from Encrypt(float) back to float, uses passed crypto key.
		/// </summary>
		public static float Decrypt(int value, int key)
		{
			return BitConverter.ToSingle(BitConverter.GetBytes(value ^ key), 0);
		}

		/// <summary>
		/// Use it after SetNewCryptoKey() to re-encrypt current instance using new crypto key.
		/// </summary>
		public void ApplyNewCryptoKey()
		{
			if (currentCryptoKey != cryptoKey)
			{
				hiddenValue = InternalEncrypt(InternalDecrypt(), cryptoKey);
				currentCryptoKey = cryptoKey;
			}
		}

		/// <summary>
		/// Allows to pick current obscured value as is.
		/// </summary>
		/// Use it in conjunction with SetEncrypted().<br/>
		/// Useful for saving data in obscured state.
		public int GetEncrypted()
		{
			ApplyNewCryptoKey();

			return BitConverter.ToInt32(hiddenValue, 0);
		}

		/// <summary>
		/// Allows to explicitly set current obscured value.
		/// </summary>
		/// Use it in conjunction with GetEncrypted().<br/>
		/// Useful for loading data stored in obscured state.
		public void SetEncrypted(int encrypted)
		{
			hiddenValue = BitConverter.GetBytes(encrypted);
			if (Detectors.ObscuredCheatingDetector.isRunning)
			{
				fakeValue = InternalDecrypt();
			}
		}

		private float InternalDecrypt()
		{
			if (!inited)
			{
				currentCryptoKey = cryptoKey;
				hiddenValue = InternalEncrypt(0);
				fakeValue = 0;
				inited = true;
			}
			
			int key = cryptoKey;

			if (currentCryptoKey != cryptoKey)
			{
				key = currentCryptoKey;
			}

			float decrypted = BitConverter.ToSingle(BitConverter.GetBytes(BitConverter.ToInt32(hiddenValue, 0) ^ key), 0);

			if (Detectors.ObscuredCheatingDetector.isRunning && fakeValue != 0 && Math.Abs(decrypted - fakeValue) > 0.01f)
			{
				Detectors.ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}

			return decrypted;
		}
#endif

		#region operators, overrides, interface implementations
		public static implicit operator ObscuredFloat(float value)
		{
			ObscuredFloat obscured = new ObscuredFloat(InternalEncrypt(value));
			if (Detectors.ObscuredCheatingDetector.isRunning)
			{
				obscured.fakeValue = value;
			}
			return obscured;
		}

		public static implicit operator float(ObscuredFloat value)
		{
			return value.InternalDecrypt();
		}

		public static ObscuredFloat operator ++(ObscuredFloat input)
		{
			float decrypted = input.InternalDecrypt() + 1f;
			input.hiddenValue = InternalEncrypt(decrypted, input.currentCryptoKey);

			if (Detectors.ObscuredCheatingDetector.isRunning)
			{
				input.fakeValue = decrypted;
			}

			return input;
		}

		public static ObscuredFloat operator --(ObscuredFloat input)
		{
			float decrypted = input.InternalDecrypt() - 1f;
			input.hiddenValue = InternalEncrypt(decrypted, input.currentCryptoKey);

			if (Detectors.ObscuredCheatingDetector.isRunning)
			{
				input.fakeValue = decrypted;
			}

			return input;
		}

		/// <summary>
		/// Returns a value indicating whether this instance is equal to a specified object.
		/// </summary>
		/// 
		/// <returns>
		/// true if <paramref name="obj"/> is an instance of ObscuredFloat and equals the value of this instance; otherwise, false.
		/// </returns>
		/// <param name="obj">An object to compare with this instance. </param><filterpriority>2</filterpriority>
		public override bool Equals(object obj)
		{
			if (!(obj is ObscuredFloat))
				return false;
			ObscuredFloat d = (ObscuredFloat)obj;
			float dParam = d.InternalDecrypt();
			float dThis = InternalDecrypt();
			if ((double)dParam == (double)dThis)
				return true;
			return float.IsNaN(dParam) && float.IsNaN(dThis);
		}

		/// <summary>
		/// Returns a value indicating whether this instance and a specified ObscuredFloat object represent the same value.
		/// </summary>
		/// 
		/// <returns>
		/// true if <paramref name="obj"/> is equal to this instance; otherwise, false.
		/// </returns>
		/// <param name="obj">An ObscuredFloat object to compare to this instance.</param><filterpriority>2</filterpriority>
		public bool Equals(ObscuredFloat obj)
		{
			float dParam = obj.InternalDecrypt();
			float dThis = InternalDecrypt();


			if ((double)dParam == (double)dThis)
				return true;
			return float.IsNaN(dParam) && float.IsNaN(dThis);
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
		/// Converts the numeric value of this instance to its equivalent string representation.
		/// </summary>
		/// 
		/// <returns>
		/// The string representation of the value of this instance.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public override string ToString()
		{
			return InternalDecrypt().ToString();
		}

		/// <summary>
		/// Converts the numeric value of this instance to its equivalent string representation, using the specified format.
		/// </summary>
		/// 
		/// <returns>
		/// The string representation of the value of this instance as specified by <paramref name="format"/>.
		/// </returns>
		/// <param name="format">A numeric format string (see Remarks).</param><exception cref="T:System.FormatException"><paramref name="format"/> is invalid. </exception><filterpriority>1</filterpriority>
		public string ToString(string format)
		{
			return InternalDecrypt().ToString(format);
		}
#if !UNITY_FLASH
		/// <summary>
		/// Converts the numeric value of this instance to its equivalent string representation using the specified culture-specific format information.
		/// </summary>
		/// 
		/// <returns>
		/// The string representation of the value of this instance as specified by <paramref name="provider"/>.
		/// </returns>
		/// <param name="provider">An <see cref="T:System.IFormatProvider"/> that supplies culture-specific formatting information. </param><filterpriority>1</filterpriority>
		public string ToString(IFormatProvider provider)
		{
			return InternalDecrypt().ToString(provider);
		}

		/// <summary>
		/// Converts the numeric value of this instance to its equivalent string representation using the specified format and culture-specific format information.
		/// </summary>
		/// 
		/// <returns>
		/// The string representation of the value of this instance as specified by <paramref name="format"/> and <paramref name="provider"/>.
		/// </returns>
		/// <param name="format">A numeric format string (see Remarks).</param><param name="provider">An <see cref="T:System.IFormatProvider"/> that supplies culture-specific formatting information. </param><filterpriority>1</filterpriority>
		public string ToString(string format, IFormatProvider provider)
		{
			return InternalDecrypt().ToString(format, provider);
		}
#endif
		//! @endcond
		#endregion

	}
}