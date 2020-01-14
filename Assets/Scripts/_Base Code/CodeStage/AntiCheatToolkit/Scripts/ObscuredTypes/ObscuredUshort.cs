using System;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	/// <summary>
	/// Use it instead of regular <c>ushort</c> for any cheating-sensitive variables.
	/// </summary>
	/// <strong><em>Regular type is faster and memory wiser comparing to the obscured one!</em></strong>
	[Serializable]
	public struct ObscuredUShort : IEquatable<ObscuredUShort>
	{
		private static ushort cryptoKey = 224;

		private ushort currentCryptoKey;
		private ushort hiddenValue;
		private ushort fakeValue;
		private bool inited;

		private ObscuredUShort(ushort value)
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
		public static void SetNewCryptoKey(ushort newKey)
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
				hiddenValue = EncryptDecrypt(InternalDecrypt(), cryptoKey);
				currentCryptoKey = cryptoKey;
			}
		}

		/// <summary>
		/// Simple symmetric encryption, uses default crypto key.
		/// </summary>
		/// <returns>Encrypted or decrypted <c>ushort</c> (depending on what <c>ushort</c> was passed to the function)</returns>
		public static ushort EncryptDecrypt(ushort value)
		{
			return EncryptDecrypt(value, 0);
		}

		/// <summary>
		/// Simple symmetric encryption, uses passed crypto key.
		/// </summary>
		/// <returns>Encrypted or decrypted <c>ushort</c> (depending on what <c>ushort</c> was passed to the function)</returns>
		public static ushort EncryptDecrypt(ushort value, ushort key)
		{
			if (key == 0)
			{
				return (ushort)(value ^ cryptoKey);
			}
			return (ushort)(value ^ key);
		}

		/// <summary>
		/// Allows to pick current obscured value as is.
		/// </summary>
		/// Use it in conjunction with SetEncrypted().<br/>
		/// Useful for saving data in obscured state.
		public ushort GetEncrypted()
		{
			ApplyNewCryptoKey();

			return hiddenValue;
		}

		/// <summary>
		/// Allows to explicitly set current obscured value.
		/// </summary>
		/// Use it in conjunction with GetEncrypted().<br/>
		/// Useful for loading data stored in obscured state.
		public void SetEncrypted(ushort encrypted)
		{
			hiddenValue = encrypted;
			if (Detectors.ObscuredCheatingDetector.isRunning)
			{
				fakeValue = InternalDecrypt();
			}
		}

		private ushort InternalDecrypt()
		{
			if (!inited)
			{
				currentCryptoKey = cryptoKey;
				hiddenValue = EncryptDecrypt(0);
				fakeValue = 0;
				inited = true;
			}

			ushort key = cryptoKey;

			if (currentCryptoKey != cryptoKey)
			{
				key = currentCryptoKey;
			}

			ushort decrypted = EncryptDecrypt(hiddenValue, key);

			if (Detectors.ObscuredCheatingDetector.isRunning && fakeValue != 0 && decrypted != fakeValue)
			{
				Detectors.ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}

			return decrypted;
		}

		#region operators, overrides, interface implementations
		//! @cond
		public static implicit operator ObscuredUShort(ushort value)
		{
			ObscuredUShort obscured = new ObscuredUShort(EncryptDecrypt(value));
			if (Detectors.ObscuredCheatingDetector.isRunning)
			{
				obscured.fakeValue = value;
			}
			return obscured;
		}

		public static implicit operator ushort(ObscuredUShort value)
		{
			return value.InternalDecrypt();
		}

		public static ObscuredUShort operator ++(ObscuredUShort input)
		{
			ushort decrypted = (ushort)(input.InternalDecrypt() + 1);
			input.hiddenValue = EncryptDecrypt(decrypted, input.currentCryptoKey);

			if (Detectors.ObscuredCheatingDetector.isRunning)
			{
				input.fakeValue = decrypted;
			}
			return input;
		}

		public static ObscuredUShort operator --(ObscuredUShort input)
		{
			ushort decrypted = (ushort)(input.InternalDecrypt() - 1);
			input.hiddenValue = EncryptDecrypt(decrypted, input.currentCryptoKey);

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
		/// true if <paramref name="obj"/> is an instance of ObscuredUShort and equals the value of this instance; otherwise, false.
		/// </returns>
		/// <param name="obj">An object to compare with this instance, or null. </param><filterpriority>2</filterpriority>
		public override bool Equals(object obj)
		{
			if (!(obj is ObscuredUShort))
				return false;
			
			ObscuredUShort ob = (ObscuredUShort)obj;
			return hiddenValue == ob.hiddenValue;
		}

		/// <summary>
		/// Returns a value indicating whether this instance and a specified ObscuredUShort object represent the same value.
		/// </summary>
		/// 
		/// <returns>
		/// true if <paramref name="obj"/> is equal to this instance; otherwise, false.
		/// </returns>
		/// <param name="obj">An ObscuredUShort object to compare to this instance.</param><filterpriority>2</filterpriority>
		public bool Equals(ObscuredUShort obj)
		{
			return hiddenValue == obj.hiddenValue;
		}

		/// <summary>
		/// Converts the numeric value of this instance to its equivalent string representation.
		/// </summary>
		/// 
		/// <returns>
		/// The string representation of the value of this instance, consisting of a sequence of digits ranging from 0 to 9, without a sign or leading zeroes.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public override string ToString()
		{
			return InternalDecrypt().ToString();
		}

		/// <summary>
		/// Converts the numeric value of this instance to its equivalent string representation using the specified format.
		/// </summary>
		/// 
		/// <returns>
		/// The string representation of the value of this instance as specified by <paramref name="format"/>.
		/// </returns>
		/// <param name="format">A numeric format string.</param><exception cref="T:System.FormatException">The <paramref name="format"/> parameter is invalid. </exception><filterpriority>1</filterpriority>
		public string ToString(string format)
		{
			return InternalDecrypt().ToString(format);
		}

#if !UNITY_FLASH
		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// 
		/// <returns>
		/// A hash code for the current ObscuredUShort.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override int GetHashCode()
		{
			return InternalDecrypt().GetHashCode();
		}

		/// <summary>
		/// Converts the numeric value of this instance to its equivalent string representation using the specified culture-specific format information.
		/// </summary>
		/// 
		/// <returns>
		/// The string representation of the value of this instance , which consists of a sequence of digits ranging from 0 to 9, without a sign or leading zeros.
		/// </returns>
		/// <param name="provider">An object that supplies culture-specific formatting information. </param><filterpriority>1</filterpriority>
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
		/// <param name="format">A numeric format string.</param><param name="provider">An object that supplies culture-specific formatting information about this instance. </param><exception cref="T:System.FormatException">The <paramref name="format"/> parameter is invalid. </exception><filterpriority>1</filterpriority>
		public string ToString(string format, IFormatProvider provider)
		{
			return InternalDecrypt().ToString(format, provider);
		}
#else
		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// 
		/// <returns>
		/// A hash code for the current ObscuredUShort.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override int GetHashCode()
		{
			ushort value = InternalDecrypt();
			return (int)value;
		}
#endif
		//! @endcond
		#endregion
	}
}
