using System;
using System.Runtime.InteropServices;

namespace CodeStage.AntiCheat.ObscuredTypes
{
#if !UNITY_FLASH
	/// <summary>
	/// Use it instead of regular <c>double</c> for any cheating-sensitive variables.
	/// </summary>
	/// <strong>\htmlonly<font color="FF4040">WARNING:</font>\endhtmlonly Flash Player is not supported! Consider using ObscuredFloat instead.</strong><br/>
	/// <strong><em>Regular type is faster and memory wiser comparing to the obscured one!</em></strong>
	[Serializable]
	public struct ObscuredDouble : IEquatable<ObscuredDouble>
	{
		private static long cryptoKey = 210987L;

		private long currentCryptoKey;
		private byte[] hiddenValue;
		private double fakeValue;
		private bool inited;

		private ObscuredDouble(byte[] value)
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
		/// Use this simple encryption method to encrypt any double value, uses default crypto key.
		/// </summary>
		public static long Encrypt(double value)
		{
			return Encrypt(value, cryptoKey);
		}

		/// <summary>
		/// Use this simple encryption method to encrypt any double value, uses passed crypto key.
		/// </summary>
		public static long Encrypt(double value, long key)
		{
			var u = new DoubleLongBytesUnion();
			u.d = value;
			u.l = u.l ^ key;

			return u.l;
		}

		private static byte[] InternalEncrypt(double value)
		{
			return InternalEncrypt(value, 0L);
		}

		private static byte[] InternalEncrypt(double value, long key)
		{
			long currKey = key;
			if (currKey == 0L)
			{
				currKey = cryptoKey;
			}

			var u = new DoubleLongBytesUnion();
			u.d = value;
			u.l = u.l ^ currKey;

			return new[] { u.b1, u.b2, u.b3, u.b4, u.b5, u.b6, u.b7, u.b8};
		}

		/// <summary>
		/// Use it to decrypt long you got from Encrypt(double) back to double, uses default crypto key.
		/// </summary>
		public static double Decrypt(long value)
		{
			return Decrypt(value, cryptoKey);
		}

		/// <summary>
		/// Use it to decrypt long you got from Encrypt(double) back to double, uses passed crypto key.
		/// </summary>
		public static double Decrypt(long value, long key)
		{
			var u = new DoubleLongBytesUnion();
			u.l = value ^ key;
			return u.d;
		}

		/// <summary>
		/// Allows to pick current obscured value as is.
		/// </summary>
		/// Use it in conjunction with SetEncrypted().<br/>
		/// Useful for saving data in obscured state.
		public long GetEncrypted()
		{
			ApplyNewCryptoKey();

			var union = new DoubleLongBytesUnion();
			union.b1 = hiddenValue[0];
			union.b2 = hiddenValue[1];
			union.b3 = hiddenValue[2];
			union.b4 = hiddenValue[3];
			union.b5 = hiddenValue[4];
			union.b6 = hiddenValue[5];
			union.b7 = hiddenValue[6];
			union.b8 = hiddenValue[7];

			return union.l;
		}

		/// <summary>
		/// Allows to explicitly set current obscured value.
		/// </summary>
		/// Use it in conjunction with GetEncrypted().<br/>
		/// Useful for loading data stored in obscured state.
		public void SetEncrypted(long encrypted)
		{
			var union = new DoubleLongBytesUnion();
			union.l = encrypted;

			hiddenValue = new[] { union.b1, union.b2, union.b3, union.b4, union.b5, union.b6, union.b7, union.b8 };

			if (Detectors.ObscuredCheatingDetector.isRunning)
			{
				fakeValue = InternalDecrypt();
			}
		}

		private double InternalDecrypt()
		{
			if (!inited)
			{
				currentCryptoKey = cryptoKey;
				hiddenValue = InternalEncrypt(0);
				fakeValue = 0;
				inited = true;
			}

			long key = cryptoKey;

			if (currentCryptoKey != cryptoKey)
			{
				key = currentCryptoKey;
			}

			var union = new DoubleLongBytesUnion();
			union.b1 = hiddenValue[0];
			union.b2 = hiddenValue[1];
			union.b3 = hiddenValue[2];
			union.b4 = hiddenValue[3];
			union.b5 = hiddenValue[4];
			union.b6 = hiddenValue[5];
			union.b7 = hiddenValue[6];
			union.b8 = hiddenValue[7];

			union.l = union.l ^ key;

			double decrypted = union.d;

			if (Detectors.ObscuredCheatingDetector.isRunning && fakeValue != 0 && Math.Abs(decrypted - fakeValue) > 0.000001d)
			{
				Detectors.ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}

			return decrypted;
		}

		[StructLayout(LayoutKind.Explicit)]
		private struct DoubleLongBytesUnion
		{
			[FieldOffset(0)]
			public double d;

			[FieldOffset(0)]
			public long l;

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
		}

		#region operators, overrides, interface implementations
		//! @cond
		public static implicit operator ObscuredDouble(double value)
		{
			ObscuredDouble obscured = new ObscuredDouble(InternalEncrypt(value));
			if (Detectors.ObscuredCheatingDetector.isRunning)
			{
				obscured.fakeValue = value;
			}
			return obscured;
		}

		public static implicit operator double(ObscuredDouble value)
		{
			return value.InternalDecrypt();
		}

		public static ObscuredDouble operator ++(ObscuredDouble input)
		{
			double decrypted = input.InternalDecrypt() + 1d;
			input.hiddenValue = InternalEncrypt(decrypted, input.currentCryptoKey);

			if (Detectors.ObscuredCheatingDetector.isRunning)
			{
				input.fakeValue = decrypted;
			}

			return input;
		}

		public static ObscuredDouble operator --(ObscuredDouble input)
		{
			double decrypted = input.InternalDecrypt() - 1d;
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
		/// true if <paramref name="obj"/> is an instance of ObscuredDouble and equals the value of this instance; otherwise, false.
		/// </returns>
		/// <param name="obj">An object to compare with this instance. </param><filterpriority>2</filterpriority>
		public override bool Equals(object obj)
		{
			if (!(obj is ObscuredDouble))
				return false;
			ObscuredDouble d = (ObscuredDouble)obj;
			double dParam = d.InternalDecrypt();
			double dThis = InternalDecrypt();
			
			if (dParam == dThis)
				return true;
			return double.IsNaN(dParam) && double.IsNaN(dThis);
		}

		/// <summary>
		/// Returns a value indicating whether this instance and a specified <see cref="T:System.Double"/> object represent the same value.
		/// </summary>
		/// 
		/// <returns>
		/// true if <paramref name="obj"/> is equal to this instance; otherwise, false.
		/// </returns>
		/// <param name="obj">A <see cref="T:System.Double"/> object to compare to this instance.</param><filterpriority>2</filterpriority>
		public bool Equals(ObscuredDouble obj)
		{
			double dParam = obj.InternalDecrypt();
			double dThis = InternalDecrypt();

			if (dParam == dThis)
				return true;
			return double.IsNaN(dParam) && double.IsNaN(dThis);
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