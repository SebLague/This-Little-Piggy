using System;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	/// <summary>
	/// Use it instead of regular <c>sbyte</c> for any cheating-sensitive variables.
	/// </summary>
	/// <strong><em>Regular type is faster and memory wiser comparing to the obscured one!</em></strong>
	[Serializable]
	public struct ObscuredSByte : IEquatable<ObscuredSByte>
	{
		private static sbyte cryptoKey = 112;

		private sbyte currentCryptoKey;
		private sbyte hiddenValue;
		private sbyte fakeValue;
		private bool inited;

		private ObscuredSByte(sbyte value)
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
		public static void SetNewCryptoKey(sbyte newKey)
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
		/// <returns>Encrypted or decrypted <c>sbyte</c> (depending on what <c>sbyte</c> was passed to the function)</returns>
		public static sbyte EncryptDecrypt(sbyte value)
		{
			return EncryptDecrypt(value, 0);
		}

		/// <summary>
		/// Simple symmetric encryption, uses passed crypto key.
		/// </summary>
		/// <returns>Encrypted or decrypted <c>sbyte</c> (depending on what <c>sbyte</c> was passed to the function)</returns>
		public static sbyte EncryptDecrypt(sbyte value, sbyte key)
		{
			if (key == 0)
			{
				return (sbyte)(value ^ cryptoKey);
			}
			return (sbyte)(value ^ key);
		}

		/// <summary>
		/// Allows to pick current obscured value as is.
		/// </summary>
		/// Use it in conjunction with SetEncrypted().<br/>
		/// Useful for saving data in obscured state.
		public sbyte GetEncrypted()
		{
			ApplyNewCryptoKey();

			return hiddenValue;
		}

		/// <summary>
		/// Allows to explicitly set current obscured value.
		/// </summary>
		/// Use it in conjunction with GetEncrypted().<br/>
		/// Useful for loading data stored in obscured state.
		public void SetEncrypted(sbyte encrypted)
		{
			hiddenValue = encrypted;
			if (Detectors.ObscuredCheatingDetector.isRunning)
			{
				fakeValue = InternalDecrypt();
			}
		}

		private sbyte InternalDecrypt()
		{
			if (!inited)
			{
				currentCryptoKey = cryptoKey;
				hiddenValue = EncryptDecrypt(0);
				fakeValue = 0;
				inited = true;
			}

			sbyte key = cryptoKey;

			if (currentCryptoKey != cryptoKey)
			{
				key = currentCryptoKey;
			}

			sbyte decrypted = EncryptDecrypt(hiddenValue, key);

			if (Detectors.ObscuredCheatingDetector.isRunning && fakeValue != 0 && decrypted != fakeValue)
			{
				Detectors.ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}

			return decrypted;
		}

		#region operators, overrides, interface implementations
		//! @cond
		public static implicit operator ObscuredSByte(sbyte value)
		{
			ObscuredSByte obscured = new ObscuredSByte(EncryptDecrypt(value));
			if (Detectors.ObscuredCheatingDetector.isRunning)
			{
				obscured.fakeValue = value;
			}
			return obscured;
		}

		public static implicit operator sbyte(ObscuredSByte value)
		{
			return value.InternalDecrypt();
		}

		public static ObscuredSByte operator ++(ObscuredSByte input)
		{
			sbyte decrypted = (sbyte)(input.InternalDecrypt() + 1);
			input.hiddenValue = EncryptDecrypt(decrypted, input.currentCryptoKey);

			if (Detectors.ObscuredCheatingDetector.isRunning)
			{
				input.fakeValue = decrypted;
			}
			return input;
		}

		public static ObscuredSByte operator --(ObscuredSByte input)
		{
			sbyte decrypted = (sbyte)(input.InternalDecrypt() - 1);
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
		/// true if <paramref name="obj"/> is an instance of ObscuredSByte and equals the value of this instance; otherwise, false.
		/// </returns>
		/// <param name="obj">An object to compare with this instance, or null. </param><filterpriority>2</filterpriority>
		public override bool Equals(object obj)
		{
			if (!(obj is ObscuredSByte))
				return false;

			ObscuredSByte ob = (ObscuredSByte)obj;
			return hiddenValue == ob.hiddenValue;
		}

		/// <summary>
		/// Returns a value indicating whether this instance and a specified ObscuredSByte object represent the same value.
		/// </summary>
		/// 
		/// <returns>
		/// true if <paramref name="obj"/> is equal to this instance; otherwise, false.
		/// </returns>
		/// <param name="obj">An ObscuredSByte object to compare to this instance.</param><filterpriority>2</filterpriority>
		public bool Equals(ObscuredSByte obj)
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
		/// A hash code for the current ObscuredSByte.
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
		/// A hash code for the current ObscuredSByte.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override int GetHashCode()
		{
			return (int)InternalDecrypt();
		}
#endif
		//! @endcond
		#endregion
	}
}
