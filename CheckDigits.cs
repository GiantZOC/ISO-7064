using System;
using System.Collections.Generic;
using System.Text;

namespace ISO7064
{
	public class CheckDigits
	{
		public const string NumericCharSet = "0123456789";
		public const string Mod112CharSet = "0123456789X";
		public const string HexCharSet = "0123456789ABCDEF";
		public const string AlphaCharSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		public const string AlphanumericCharSet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		public const string Mod372CharSet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ*";

		/// <summary>
		/// Calculates a check digit based on ISO 7064.
		/// </summary>
		/// <param name="value">The string to check.</param>
		/// <param name="charSet">A character set containing all possible characters used in the supplied value.</param>
		/// <param name="doubleDigit">Specifies whether or not to use a double digit check digit.</param>
		/// <returns>Returns the value with the check digit(s) appended.</returns>
		public static string CalculateCheckDigit(string value, string charSet, bool doubleDigit)
		{
			int radix, modulus;
			GetCheckDigitRadixAndModulus(charSet, doubleDigit, out radix, out modulus);

			if (modulus != radix + 1)
				return CalculatePureSystemCheckDigit(value, radix, modulus, charSet, doubleDigit);
			else
				return CalculateHybridSystemCheckDigit(value, charSet);
		}

		//Algorithm from: https://github.com/danieltwagner/iso7064.
		public static string CalculatePureSystemCheckDigit(string value, int radix, int modulus, string charSet, bool doubleDigit)
		{
			if (String.IsNullOrEmpty(value))
				return null;

			value = value.ToUpper();
			int p = 0;

			foreach (char c in value)
			{
				int i = charSet.IndexOf(c);

				if (i == -1)
					return null;

				p = ((p + i) * radix) % modulus;
			}

			if (doubleDigit)
				p = (p * radix) % modulus;

			int checkDigit = (modulus - p + 1) % modulus;

			if (doubleDigit)
			{
				int second = checkDigit % radix;
				int first = (checkDigit - second) / radix;
				return value + charSet[first] + charSet[second];
			}
			else
				return value + charSet[checkDigit];
		}

		//Algorithm from: http://www.codeproject.com/Articles/16540/Error-Detection-Based-on-Check-Digit-Schemes
		public static string CalculateHybridSystemCheckDigit(string value, string charSet)
		{
			if (String.IsNullOrEmpty(value))
				return null;

			value = value.ToUpper();
			int radix = charSet.Length;
			int pos = radix;

			foreach (char c in value)
			{
				int i = charSet.IndexOf(c);

				if (i == -1)
					return null;

				pos += i;

				if (pos > radix)
					pos -= radix;

				pos *= 2;

				if (pos >= radix + 1)
					pos -= radix + 1;
			}

			pos = radix + 1 - pos;

			if (pos == radix)
				pos = 0;

			return value + charSet[pos];
		}

		/// <summary>
		/// Returns the correct ISO 7064 radix and modulus for the given character set and digit count.
		/// </summary>
		private static void GetCheckDigitRadixAndModulus(string charSet, bool doubleDigit, out int radix, out int modulus)
		{
			radix = charSet.Length;
			modulus = radix + 1;

			if (doubleDigit)
			{
				//The modulus numbers below for double digit calculations are defined by ISO 7064.
				switch (radix)
				{
					case 10:
						modulus = 97;
						break;
					case 16: //Mod 251,16 isn't defined in ISO 7064, but it could be useful so I added it anyway.
						modulus = 251;
						break;
					case 26:
						modulus = 661;
						break;
					case 36:
						modulus = 1271;
						break;
				}
			}
			else if (radix == 11)
			{
				//MOD 11,2 - Single digit 0-9 check with an added 'X' check digit.
				modulus = 11;
				radix = 2;
			}
			else if (radix == 37)
			{
				//MOD 37,2 - Single digit 0-9,A-Z check with an added '*' check digit.
				modulus = 37;
				radix = 2;
			}

			if (radix != 2 && radix != 10 && radix != 16 && radix != 26 && radix != 36)
				throw new ArgumentException("Invalid character set.", "charSet");
		}

		/// <summary>
		/// Verifies that the last character(s) of the supplied value are valid check digit(s).
		/// </summary>
		public static bool VerifyCheckDigit(string value, string charSet, bool doubleDigit)
		{
			int radix, modulus;
			GetCheckDigitRadixAndModulus(charSet, doubleDigit, out radix, out modulus);
			return VerifyCheckDigit(value, radix, modulus, charSet, doubleDigit);
		}

		public static bool VerifyCheckDigit(string value, int radix, int modulus, string charSet, bool doubleDigit)
		{
			int numDigits = (doubleDigit ? 2 : 1);

			if (value == null || value.Length <= numDigits)
				return false;

			value = value.ToUpper();
			string origValue = value.Substring(0, value.Length - numDigits);

			if (modulus != radix + 1)
				return (value == CalculatePureSystemCheckDigit(origValue, radix, modulus, charSet, doubleDigit));
			else
				return (value == CalculateHybridSystemCheckDigit(origValue, charSet));
		}

		/// <summary>
		/// Calculates ISO 7064 MOD 11,10 in single digit mode and MOD 97,10 in double digit mode.
		/// </summary>
		public static string CalculateNumericCheckDigit(string value, bool doubleDigit)
		{
			return CalculateCheckDigit(value, NumericCharSet, doubleDigit);
		}

		/// <summary>
		/// Verifies ISO 7064 MOD 11,10 in single digit mode and MOD 97,10 in double digit mode.
		/// </summary>
		public static bool VerifyNumericCheckDigit(string value, bool doubleDigit)
		{
			return VerifyCheckDigit(value, NumericCharSet, doubleDigit);
		}

		/// <summary>
		/// Calculates ISO 7064 MOD 17,16 in single digit mode and MOD 251,16 in double digit mode.
		/// </summary>
		public static string CalculateHexCheckDigit(string value, bool doubleDigit)
		{
			return CalculateCheckDigit(value, HexCharSet, doubleDigit);
		}

		/// <summary>
		/// Verifies ISO 7064 MOD 11,10 in single digit mode and MOD 97,10 in double digit mode.
		/// </summary>
		public static bool VerifyHexCheckDigit(string value, bool doubleDigit)
		{
			return VerifyCheckDigit(value, HexCharSet, doubleDigit);
		}

		/// <summary>
		/// Calculates ISO 7064 MOD 27,26 in single digit mode and MOD 661,26 in double digit mode.
		/// </summary>
		public static string CalculateAlphaCheckDigit(string value, bool doubleDigit)
		{
			return CalculateCheckDigit(value, AlphaCharSet, doubleDigit);
		}

		/// <summary>
		/// Verifies ISO 7064 MOD 27,26 in single digit mode and MOD 661,26 in double digit mode.
		/// </summary>
		public static bool VerifyAlphaCheckDigit(string value, bool doubleDigit)
		{
			return VerifyCheckDigit(value, AlphaCharSet, doubleDigit);
		}

		/// <summary>
		/// Calculates ISO 7064 MOD 37,36 in single digit mode and MOD 1271,36 in double digit mode.
		/// </summary>
		public static string CalculateAlphanumericCheckDigit(string value, bool doubleDigit)
		{
			return CalculateCheckDigit(value, AlphanumericCharSet, doubleDigit);
		}

		/// <summary>
		/// Verifies ISO 7064 MOD 37,36 in single digit mode and MOD 1271,36 in double digit mode.
		/// </summary>
		public static bool VerifyAlphanumericCheckDigit(string value, bool doubleDigit)
		{
			return VerifyCheckDigit(value, AlphanumericCharSet, doubleDigit);
		}
	}
}