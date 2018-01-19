# ISO-7064

Code taken from CodePlex project that will be shut down: https://iso7064.codeplex.com

This article makes ISO7064 look like a great scheme for preventing data entry errors: https://www.codeproject.com/Articles/16540/Error-Detection-Based-on-Check-Digit-Schemes

SO 7064 Mod N/N+1
This section presents an overview of a family of check digits. Implemetations are presented in Downloads.

## The ISO specifications are designed for the following alphabets:

* numeric (10 digits: 0 to 9)
* alphabetic (26 letters: A to Z)
* alphanumeric (letters and digits)

## The ISO specifications are designed to detect the following (taking from the ISO website):
* all single substitution errors (the substitution of a single character for another, for example 4234 for 1234);
* all or nearly all single (local) transposition errors (the transposition of two single characters, either adjacent or with one character between them, for example 12354 or 12543 for 12345);
* all or nearly all shift errors (shifts of the whole string to the left or right);
* a high proportion of double substitution errors (two separate single substitution errors in the same string, for example 7234587 for 1234567);
* a high proportion of all other errors.
