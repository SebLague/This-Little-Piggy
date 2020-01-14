using System;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	/// <summary>
	/// Use it instead of regular <c>int</c> for any cheating-sensitive variables.
	/// </summary>
	/// <strong><em>Regular type is faster and memory wiser comparing to the obscured one!</em></strong>
	[Serializable]
	public struct ObscuredInt : IEquatable<ObscuredInt>
	{
		private static int cryptoKey = 444444;

		[SerializeField]
		private int currentCryptoKey;

		[SerializeField]
		private int hiddenValue;

		[SerializeField]
		private int fakeValue;

		[SerializeField]
		private bool inited;

		private ObscuredInt(int value)
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
				hiddenValue = Encrypt(InternalDecrypt(), cryptoKey);
				currentCryptoKey = cryptoKey;
			}
		}

		/// <summary>
		/// Simple symmetric encryption, uses default crypto key.
		/// </summary>
		/// <returns>Encrypted <c>int</c>.</returns>
		public static int Encrypt(int value)
		{
			return Encrypt(value, 0);
		}

		/// <summary>
		/// Simple symmetric encryption, uses passed crypto key.
		/// </summary>
		/// <returns>Encrypted <c>int</c>.</returns>
		public static int Encrypt(int value, int key)
		{
			if (key == 0)
			{
				return value ^ cryptoKey;
			}
			return value ^ key;
		}

		/// <summary>
		/// Simple symmetric encryption, uses default crypto key.
		/// </summary>
		/// <returns>Decrypted <c>int</c>.</returns>
		public static int Decrypt(int value)
		{
			return Decrypt(value, 0);
		}

		/// <summary>
		/// Simple symmetric encryption, uses passed crypto key.
		/// </summary>
		/// <returns>Decrypted <c>int</c>.</returns>
		public static int Decrypt(int value, int key)
		{
			if (key == 0)
			{
				return value ^ cryptoKey;
			}
			return value ^ key;
		}

		/// <summary>
		/// Allows to pick current obscured value as is.
		/// </summary>
		/// Use it in conjunction with SetEncrypted().<br/>
		/// Useful for saving data in obscured state.
		public int GetEncrypted()
		{
			ApplyNewCryptoKey();
			return hiddenValue;
		}

		/// <summary>
		/// Allows to explicitly set current obscured value.
		/// </summary>
		/// Use it in conjunction with GetEncrypted().<br/>
		/// Useful for loading data stored in obscured state.
		public void SetEncrypted(int encrypted)
		{
			hiddenValue = encrypted;
			if (Detectors.ObscuredCheatingDetector.isRunning)
			{
				fakeValue = InternalDecrypt();
			}
		}

		private int InternalDecrypt()
		{
			if (!inited)
			{
				currentCryptoKey = cryptoKey;
				hiddenValue = Encrypt(0);
				fakeValue = 0;
				inited = true;
			}

			int key = cryptoKey;

			if (currentCryptoKey != cryptoKey)
			{
				key = currentCryptoKey;
			}

			int decrypted = Decrypt(hiddenValue, key);

			if (Detectors.ObscuredCheatingDetector.isRunning && fakeValue != 0 && decrypted != fakeValue)
			{
				Debug.LogWarning("decrypted = " + decrypted);
				Debug.LogWarning("fakeValue = " + fakeValue);
				Detectors.ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}

			return decrypted;
		}

		#region operators, overrides, interface implementations
		//! @cond
		public static implicit operator ObscuredInt(int value)
		{
			ObscuredInt obscured = new ObscuredInt(Encrypt(value));
			if (Detectors.ObscuredCheatingDetector.isRunning)
			{
				obscured.fakeValue = value;
			}
			return obscured;
		}

		public static implicit operator int(ObscuredInt value)
		{
			return value.InternalDecrypt();
		}

		public static ObscuredInt operator ++(ObscuredInt input)
		{
			int decrypted = input.InternalDecrypt() + 1;
			input.hiddenValue = Encrypt(decrypted, input.currentCryptoKey);

			if (Detectors.ObscuredCheatingDetector.isRunning)
			{
				input.fakeValue = decrypted;
			}
			return input;
		}

		public static ObscuredInt operator --(ObscuredInt input)
		{
			int decrypted = input.InternalDecrypt() - 1;
			input.hiddenValue = Encrypt(decrypted, input.currentCryptoKey);

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
		/// true if <paramref name="obj"/> is an instance of ObscuredInt and equals the value of this instance; otherwise, false.
		/// </returns>
		/// <param name="obj">An object to compare with this instance. </param><filterpriority>2</filterpriority>
		public override bool Equals(object obj)
		{
			if (!(obj is ObscuredInt))
				return false;

			ObscuredInt oi = (ObscuredInt)obj;
			return (hiddenValue == oi.hiddenValue);
		}

		/// <summary>
		/// Returns a value indicating whether this instance is equal to a specified ObscuredInt value.
		/// </summary>
		/// 
		/// <returns>
		/// true if <paramref name="obj"/> has the same value as this instance; otherwise, false.
		/// </returns>
		/// <param name="obj">An ObscuredInt value to compare to this instance.</param><filterpriority>2</filterpriority>
		public bool Equals(ObscuredInt obj)
		{
			return hiddenValue == obj.hiddenValue;
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
		/// The string representation of the value of this instance, consisting of a negative sign if the value is negative, and a sequence of digits ranging from 0 to 9 with no leading zeroes.
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
		/// <param name="format">A numeric format string (see Remarks).</param><exception cref="T:System.FormatException"><paramref name="format"/> is invalid or not supported. </exception><filterpriority>1</filterpriority>

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
		/// <param name="format">A numeric format string (see Remarks).</param><param name="provider">An <see cref="T:System.IFormatProvider"/> that supplies culture-specific formatting information. </param><exception cref="T:System.FormatException"><paramref name="format"/> is invalid or not supported.</exception><filterpriority>1</filterpriority>
		public string ToString(string format, IFormatProvider provider)
		{
			return InternalDecrypt().ToString(format, provider);
		}
#endif
		//! @endcond
		#endregion
	}
}