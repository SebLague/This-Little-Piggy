using System;
using System.Runtime.InteropServices;

namespace CodeStage.AntiCheat.ObscuredTypes
{
#if !UNITY_FLASH
	/// <summary>
	/// Use it instead of regular <c>decimal</c> for any cheating-sensitive variables.
	/// </summary>
	/// <strong>\htmlonly<font color="FF4040">WARNING:</font>\endhtmlonly Flash Player is not supported! Consider using ObscuredFloat instead.</strong><br/>
	/// <strong><em>Regular type is faster and memory wiser comparing to the obscured one!</em></strong>
	[Serializable]
	public struct ObscuredDecimal : IEquatable<ObscuredDecimal>
	{
		private static long cryptoKey = 209208L;

		private long currentCryptoKey;
		private byte[] hiddenValue;
		private decimal fakeValue;
		private bool inited;

		private ObscuredDecimal(byte[] value)
		{
			currentCryptoKey = cryptoKey;
			hiddenValue = value;
			fakeValue = 0m;
			inited = true;
		}

		/// <summary>
		/// Allows to change default crypto key of this type instances. All new instances will use specified key.<br/>
		/// All current instances will use previous key unless you call ApplyNewCryptoKey() on them explicitly.
		/// </summary>
		public static void SetNewCryptoKey(long newKey)
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
		/// Use this simple encryption method to encrypt any decimal value, uses default crypto key.
		/// </summary>
		public static decimal Encrypt(decimal value)
		{
			return Encrypt(value, cryptoKey);
		}

		/// <summary>
		/// Use this simple encryption method to encrypt any decimal value, uses passed crypto key.
		/// </summary>
		public static decimal Encrypt(decimal value, long key)
		{
			var u = new DecimalLongBytesUnion();
			u.d = value;
			u.l1 = u.l1 ^ key;
			u.l2 = u.l2 ^ key;

			return u.d;
		}

		private static byte[] InternalEncrypt(decimal value)
		{
			return InternalEncrypt(value, 0L);
		}

		private static byte[] InternalEncrypt(decimal value, long key)
		{
			long currKey = key;
			if (currKey == 0L)
			{
				currKey = cryptoKey;
			}

			DecimalLongBytesUnion union = new DecimalLongBytesUnion();
			union.d = value;
			union.l1 = union.l1 ^ currKey;
			union.l2 = union.l2 ^ currKey;

			return new[]
			{
				union.b1, union.b2, union.b3, union.b4, union.b5, union.b6, union.b7, union.b8,
				union.b9, union.b10, union.b11, union.b12, union.b13, union.b14, union.b15, union.b16
			};
		}

		/// <summary>
		/// Use it to decrypt long you got from Encrypt(decimal) back to decimal, uses default crypto key.
		/// </summary>
		public static decimal Decrypt(decimal value)
		{
			return Decrypt(value, cryptoKey);
		}

		/// <summary>
		/// Use it to decrypt long you got from Encrypt(decimal) back to decimal, uses passed crypto key.
		/// </summary>
		public static decimal Decrypt(decimal value, long key)
		{
			DecimalLongBytesUnion u = new DecimalLongBytesUnion();
			u.d = value;
			u.l1 = u.l1 ^ key;
			u.l2 = u.l2 ^ key;
			return u.d;
		}

		/// <summary>
		/// Allows to pick current obscured value as is.
		/// </summary>
		/// Use it in conjunction with SetEncrypted().<br/>
		/// Useful for saving data in obscured state.
		public decimal GetEncrypted()
		{
			ApplyNewCryptoKey();

			DecimalLongBytesUnion union = new DecimalLongBytesUnion();
			union.b1 = hiddenValue[0];
			union.b2 = hiddenValue[1];
			union.b3 = hiddenValue[2];
			union.b4 = hiddenValue[3];
			union.b5 = hiddenValue[4];
			union.b6 = hiddenValue[5];
			union.b7 = hiddenValue[6];
			union.b8 = hiddenValue[7];
			union.b9 = hiddenValue[8];
			union.b10 = hiddenValue[9];
			union.b11 = hiddenValue[10];
			union.b12 = hiddenValue[11];
			union.b13 = hiddenValue[12];
			union.b14 = hiddenValue[13];
			union.b15 = hiddenValue[14];
			union.b16 = hiddenValue[15];

			return union.d;
		}

		/// <summary>
		/// Allows to explicitly set current obscured value.
		/// </summary>
		/// Use it in conjunction with GetEncrypted().<br/>
		/// Useful for loading data stored in obscured state.
		public void SetEncrypted(decimal encrypted)
		{
			DecimalLongBytesUnion union = new DecimalLongBytesUnion();
			union.d = encrypted;

			hiddenValue = new[]
			{
				union.b1, union.b2, union.b3, union.b4, union.b5, union.b6, union.b7, union.b8,
				union.b9, union.b10, union.b11, union.b12, union.b13, union.b14, union.b15, union.b16
			};

			if (Detectors.ObscuredCheatingDetector.isRunning)
			{
				fakeValue = InternalDecrypt();
			}
		}

		private decimal InternalDecrypt()
		{
			if (!inited)
			{
				currentCryptoKey = cryptoKey;
				hiddenValue = InternalEncrypt(0m);
				fakeValue = 0m;
				inited = true;
			}

			long key = cryptoKey;

			if (currentCryptoKey != cryptoKey)
			{
				key = currentCryptoKey;
			}

			DecimalLongBytesUnion union = new DecimalLongBytesUnion();
			union.b1 = hiddenValue[0];
			union.b2 = hiddenValue[1];
			union.b3 = hiddenValue[2];
			union.b4 = hiddenValue[3];
			union.b5 = hiddenValue[4];
			union.b6 = hiddenValue[5];
			union.b7 = hiddenValue[6];
			union.b8 = hiddenValue[7];
			union.b9 = hiddenValue[8];
			union.b10 = hiddenValue[9];
			union.b11 = hiddenValue[10];
			union.b12 = hiddenValue[11];
			union.b13 = hiddenValue[12];
			union.b14 = hiddenValue[13];
			union.b15 = hiddenValue[14];
			union.b16 = hiddenValue[15];

			union.l1 = union.l1 ^ key;
			union.l2 = union.l2 ^ key;

			decimal decrypted = union.d;

			if (Detectors.ObscuredCheatingDetector.isRunning && fakeValue != 0 && decrypted != fakeValue)
			{
				Detectors.ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}

			return decrypted;
		}

		[StructLayout(LayoutKind.Explicit)]
		private struct DecimalLongBytesUnion
		{
			[FieldOffset(0)]
			public decimal d;

			[FieldOffset(0)]
			public long l1;

			[FieldOffset(8)]
			public long l2;

			[FieldOffset(0)]
			public byte b1;

			[FieldOffset(1)]
			public byte b2;

			[FieldOffset(2)]
			public byte b3;

			[FieldOffset(3)]
			public byte b4;

			[FieldOffset(4)]
			public byte b5;

			[FieldOffset(5)]
			public byte b6;

			[FieldOffset(6)]
			public byte b7;

			[FieldOffset(7)]
			public byte b8;

			[FieldOffset(8)]
			public byte b9;

			[FieldOffset(9)]
			public byte b10;

			[FieldOffset(10)]
			public byte b11;

			[FieldOffset(11)]
			public byte b12;

			[FieldOffset(12)]
			public byte b13;

			[FieldOffset(13)]
			public byte b14;

			[FieldOffset(14)]
			public byte b15;

			[FieldOffset(15)]
			public byte b16;
		}

		#region operators, overrides, interface implementations
		//! @cond
		public static implicit operator ObscuredDecimal(decimal value)
		{
			ObscuredDecimal obscured = new ObscuredDecimal(InternalEncrypt(value));
			if (Detectors.ObscuredCheatingDetector.isRunning)
			{
				obscured.fakeValue = value;
			}
			return obscured;
		}

		public static implicit operator decimal(ObscuredDecimal value)
		{
			return value.InternalDecrypt();
		}

		public static ObscuredDecimal operator ++(ObscuredDecimal input)
		{
			decimal decrypted = input.InternalDecrypt() + 1m;
			input.hiddenValue = InternalEncrypt(decrypted, input.currentCryptoKey);

			if (Detectors.ObscuredCheatingDetector.isRunning)
			{
				input.fakeValue = decrypted;
			}

			return input;
		}

		public static ObscuredDecimal operator --(ObscuredDecimal input)
		{
			decimal decrypted = input.InternalDecrypt() - 1m;
			input.hiddenValue = InternalEncrypt(decrypted, input.currentCryptoKey);

			if (Detectors.ObscuredCheatingDetector.isRunning)
			{
				input.fakeValue = decrypted;
			}
			return input;
		}

		/// <summary>
		/// Converts the numeric value of this instance to its equivalent string representation.
		/// </summary>
		/// 
		/// <returns>
		/// The string representation of the value of this instance.
		/// </returns>
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

		/// <summary>
		/// Returns a value indicating whether this instance is equal to a specified object.
		/// </summary>
		/// 
		/// <returns>
		/// true if <paramref name="obj"/> is an instance of ObscuredDecimal and equals the value of this instance; otherwise, false.
		/// </returns>
		/// <param name="obj">An object to compare with this instance. </param><filterpriority>2</filterpriority>
		public override bool Equals(object obj)
		{
			if (!(obj is ObscuredDecimal))
				return false;
			ObscuredDecimal d = (ObscuredDecimal)obj;
			return d.InternalDecrypt().Equals(InternalDecrypt());
		}

		/// <summary>
		/// Returns a value indicating whether this instance and a specified <see cref="T:System.Decimal"/> object represent the same value.
		/// </summary>
		/// 
		/// <returns>
		/// true if <paramref name="obj"/> is equal to this instance; otherwise, false.
		/// </returns>
		/// <param name="obj">A <see cref="T:System.Decimal"/> object to compare to this instance.</param><filterpriority>2</filterpriority>
		public bool Equals(ObscuredDecimal obj)
		{
			return obj.InternalDecrypt().Equals(InternalDecrypt());
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
		//! @endcond
		#endregion
	}
#endif
}