#if UNITY_FLASH
using System;

namespace CodeStage.AntiCheat.Crypto
{
	// credits: Timm [http://www.csharp411.com/convert-binary-to-base64-string/]
	// slightly modified by me (focus, *cough*) version
	internal class Base64Tools
	{
		private const char PLUS_SIGN = '+';
		private const char SLASH = '/';

		internal static string ToBase64String(byte[] data)
		{
			if (data == null)
			{
				return String.Empty;
			}

			int length = data.Length;
			int padding = length % 3;
			if (padding > 0)
			{
				padding = 3 - padding;
			}
			int blocks = (length - 1) / 3 + 1;

			var s = new char[blocks * 4];

			for (int i = 0; i < blocks; i++)
			{
				bool finalBlock = i == blocks - 1;
				bool pad2 = false;
				bool pad1 = false;
				if (finalBlock)
				{
					pad2 = padding == 2;
					pad1 = padding > 0;
				}

				int index = i * 3;
				byte b1 = data[index];
				byte b2 = pad2 ? (byte)0 : data[index + 1];
				byte b3 = pad1 ? (byte)0 : data[index + 2];

				var temp1 = (byte)((b1 & 0xFC) >> 2);

				var temp = (byte)((b1 & 0x03) << 4);
				var temp2 = (byte)((b2 & 0xF0) >> 4);
				temp2 += temp;

				temp = (byte)((b2 & 0x0F) << 2);
				var temp3 = (byte)((b3 & 0xC0) >> 6);
				temp3 += temp;

				var temp4 = (byte)(b3 & 0x3F);

				index = i * 4;
				s[index] = SixBitToChar(temp1);
				s[index + 1] = SixBitToChar(temp2);
				s[index + 2] = pad2 ? '=' : SixBitToChar(temp3);
				s[index + 3] = pad1 ? '=' : SixBitToChar(temp4);
			}

			return new string(s);
		}

		private static char SixBitToChar(byte b)
		{
			char c;
			if (b < 26)
			{
				c = (char)(b + 'A');
			}
			else if (b < 52)
			{
				c = (char)(b - 26 + 'a');
			}
			else if (b < 62)
			{
				c = (char)(b - 52 + '0');
			}
			else if (b == 62)
			{
				c = PLUS_SIGN;
			}
			else
			{
				c = SLASH;
			}
			return c;
		}

		internal static byte[] FromBase64String(string s)
		{
			if (s == null)
			{
				return new byte[0];
			}

			int length = s.Length;
			int padding = 0;

			if (length > 2 && s[length - 2] == '=')
			{
				padding = 2;
			}
			else if (length > 1 && s[length - 1] == '=')
			{
				padding = 1;
			}

			int blocks = (length - 1) / 4 + 1;
			int bytes = blocks * 3;

			var data = new byte[bytes - padding];

			for (int i = 0; i < blocks; i++)
			{
				bool finalBlock = i == blocks - 1;
				bool pad2 = false;
				bool pad1 = false;
				if (finalBlock)
				{
					pad2 = padding == 2;
					pad1 = padding > 0;
				}

				int index = i * 4;
				byte temp1 = CharToSixBit(s[index]);
				byte temp2 = CharToSixBit(s[index + 1]);
				byte temp3 = CharToSixBit(s[index + 2]);
				byte temp4 = CharToSixBit(s[index + 3]);

				var b = (byte)(temp1 << 2);
				var b1 = (byte)((temp2 & 0x30) >> 4);
				b1 += b;

				b = (byte)((temp2 & 0x0F) << 4);
				var b2 = (byte)((temp3 & 0x3C) >> 2);
				b2 += b;

				b = (byte)((temp3 & 0x03) << 6);
				byte b3 = temp4;
				b3 += b;

				index = i * 3;
				data[index] = b1;
				if (!pad2)
				{
					data[index + 1] = b2;
				}
				if (!pad1)
				{
					data[index + 2] = b3;
				}
			}

			return data;
		}

		private static byte CharToSixBit(char c)
		{
			byte b;
			if (c >= 'A' && c <= 'Z')
			{
				b = (byte)(c - 'A');
			}
			else if (c >= 'a' && c <= 'z')
			{
				b = (byte)(c - 'a' + 26);
			}
			else if (c >= '0' && c <= '9')
			{
				b = (byte)(c - '0' + 52);
			}
			else if (c == PLUS_SIGN)
			{
				b = 62;
			}
			else
			{
				b = 63;
			}
			return b;
		}
	}
}

#endif