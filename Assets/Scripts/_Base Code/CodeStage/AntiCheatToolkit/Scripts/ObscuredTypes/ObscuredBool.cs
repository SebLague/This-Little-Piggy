using System;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	/// <summary>
	/// Use it instead of regular <c>bool</c> for any cheating-sensitive variables.
	/// </summary>
	/// <strong><em>Regular type is faster and memory wiser comparing to the obscured one!</em></strong>
	[Serializable]
	public struct ObscuredBool : IEquatable<ObscuredBool>
	{
		private static byte cryptoKey = 215;

		[SerializeField]
		private byte currentCryptoKey;

		[SerializeField]
		private int hiddenValue;

		[SerializeField]
		private bool fakeValue;

		[SerializeField]
		private bool fakeValueChanged;

		[SerializeField]
		private bool inited;

		private ObscuredBool(int value)
		{
			currentCryptoKey = cryptoKey;
			hiddenValue = value;
			fakeValue = false;
			fakeValueChanged = false;
			inited = true;
		}

		/// <summary>
		/// Allows to change default crypto key of this type instances. All new instances will use specified key.<br/>
		/// All current instances will use previous key unless you call ApplyNewCryptoKey() on them explicitly.
		/// </summary>
		public static void SetNewCryptoKey(byte newKey)
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
		/// Use this simple encryption method to encrypt any bool value, uses default crypto key.
		/// </summary>
		public static int Encrypt(bool value)
		{
			return Encrypt(value, 0);
		}

		/// <summary>
		/// Use this simple encryption method to encrypt any bool value, uses passed crypto key.
		/// </summary>
		public static int Encrypt(bool value, byte key)
		{
			if (key == 0)
			{
				key = cryptoKey;
			}

			int encryptedValue = value ? 213 : 181;

			encryptedValue ^= key;

			return encryptedValue;
		}

		/// <summary>
		/// Use it to decrypt int you got from Encrypt(bool), uses default crypto key.
		/// </summary>
		public static bool Decrypt(int value)
		{
			return Decrypt(value, 0);
		}

		/// <summary>
		/// Use it to decrypt int you got from Encrypt(bool), uses passed crypto key.
		/// </summary>
		public static bool Decrypt(int value, byte key)
		{
			if (key == 0)
			{
				key = cryptoKey;
			}

			value ^= key;

			return value != 181;
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
				fakeValueChanged = true;
			}
		}

		private bool InternalDecrypt()
		{
			if (!inited)
			{
				currentCryptoKey = cryptoKey;
				hiddenValue = Encrypt(false);
				fakeValue = false;
				fakeValueChanged = true;
				inited = true;
			}

			byte key = cryptoKey;

			if (currentCryptoKey != cryptoKey)
			{
				key = currentCryptoKey;
			}

			int value = hiddenValue;
			value ^= key;

			bool decrypted = value != 181;

			if (Detectors.ObscuredCheatingDetector.isRunning && fakeValueChanged && decrypted != fakeValue)
			{
				Detectors.ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}

			return decrypted;
		}

		#region operators, overrides, interface implementations
		//! @cond
		public static implicit operator ObscuredBool(bool value)
		{
			ObscuredBool obscured = new ObscuredBool(Encrypt(value));

			if (Detectors.ObscuredCheatingDetector.isRunning)
			{
				obscured.fakeValue = value;
				obscured.fakeValueChanged = true;
			}

			return obscured;
		}

		public static implicit operator bool(ObscuredBool value)
		{
			return value.InternalDecrypt();
		}

		/// <summary>
		/// Returns a value indicating whether this instance is equal to a specified object.
		/// </summary>
		/// 
		/// <returns>
		/// true if <paramref name="obj"/> is an instance of ObscuredBool and equals the value of this instance; otherwise, false.
		/// </returns>
		/// <param name="obj">An object to compare with this instance. </param><filterpriority>2</filterpriority>
		public override bool Equals(object obj)
		{
			if (!(obj is ObscuredBool))
				return false;

			ObscuredBool oi = (ObscuredBool)obj;
			return (hiddenValue == oi.hiddenValue);
		}

		/// <summary>
		/// Returns a value indicating whether this instance is equal to a specified ObscuredBool value.
		/// </summary>
		/// 
		/// <returns>
		/// true if <paramref name="obj"/> has the same value as this instance; otherwise, false.
		/// </returns>
		/// <param name="obj">An ObscuredVector3 value to compare to this instance.</param><filterpriority>2</filterpriority>
		public bool Equals(ObscuredBool obj)
		{
			return hiddenValue == obj.hiddenValue;
		}

#if !UNITY_FLASH || UNITY_EDITOR
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
#else
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
			return !InternalDecrypt() ? 0 : 1;
		}
#endif

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

		//! @endcond
		#endregion

	}
}