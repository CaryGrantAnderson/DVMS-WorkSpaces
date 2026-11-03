using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

/*
    Conversions.cs contains rutines to convert numbers eg. bin to hex etc.

*/

namespace LnkFileExplorer
{
    public partial class Form1 : Form
    {
        // These pointer conversions do NOT check up on the length of the field!
        public UInt32 ptrToInt32(Byte[] ptr)
        { // probably the fastest conversion, little endian
            return (UInt32)(ptr[0] | (ptr[1] << 8) | (ptr[2] << 16) | (ptr[3] << 24));
        }
        public UInt16 ptrToInt16(Byte[] ptr)
        { // probably the fastest conversion, little endian
            return (UInt16)(ptr[0] | (ptr[1] << 8));
        }
        public UInt32 ptrToInt32BigEndian(Byte[] ptr)
        { // probably the fastest conversion, big endian
            return (UInt32)(ptr[3] | (ptr[2] << 8) | (ptr[1] << 16) | (ptr[0] << 24));
        }
        public UInt16 ptrToInt16BigEndian(Byte[] ptr)
        { // probably the fastest conversion, big endian
            return (UInt16)(ptr[1] | (ptr[0] << 8));
        }

        public string bytesToUnicode(Byte[] bytes)
        {
            return System.Text.Encoding.Unicode.GetString(bytes);
        }

        public Byte[] unicodeToBytes(string shlp)
        {
            Byte[] dataB = System.Text.Encoding.Unicode.GetBytes(shlp);
            return dataB;
        }

        public string makeLen8String(UInt32 val, int zeroPos)
        {
            string shlp = DecimalToAny((long)val, 16);
            while (shlp.Length < 8)
                shlp = "0" + shlp;
            if (zeroPos == 4)
            {
                shlp = shlp.Insert(4, " ");
                return shlp;
            }
            shlp = shlp.Insert(6, " ");
            shlp = shlp.Insert(4, " ");
            shlp = shlp.Insert(2, " ");
            return shlp;
        }
        public string makeLen4String(UInt16 val)
        {
            string shlp = DecimalToAny((long)val, 16);
            while (shlp.Length < 4)
                shlp = "0" + shlp;
            return shlp;
        }

        // Fast Numeric conversions Bin, Oct, Hex, Dec
        const string Digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public static string DecimalToAny(long decimalNumber, int radix)
        {
            const int BitsInLong = 64;
            //const string Digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            if (radix < 2 || radix > Digits.Length)
                throw new ArgumentException("The radix must be >= 2 and <= " + Digits.Length);
            if (decimalNumber == 0)
                return "0";
            int index = BitsInLong - 1;
            long currentNumber = Math.Abs(decimalNumber);
            char[] charArray = new char[BitsInLong];
            while (currentNumber != 0)
            {
                int remainder = (int)(currentNumber % radix);
                charArray[index--] = Digits[remainder];
                currentNumber = currentNumber / radix;
            }
            string result = new String(charArray, index + 1, BitsInLong - index - 1);
            if (decimalNumber < 0)
            {
                result = "-" + result;
            }
            return result;
        }
        private void DtoH(string tb)
        {
            tb = DecimalToAny(int.Parse(tb), 16);
        }
        private void DtoB(string tb)
        {
            tb = DecimalToAny(int.Parse(tb), 2);
        }
        private void DtoO(string tb)
        {
            tb = DecimalToAny(int.Parse(tb), 8);
        }
        // This is supposed to be for a ToolTip!
        private void BtoH(string tb)
        {
            long l = Convert.ToInt64(tb, 2);
            tb = DecimalToAny(l, 16);
        }
        private void BtoD(string tb)
        {
            long l = Convert.ToInt64(tb, 2);
            tb = DecimalToAny(l, 10);
        }
        private void BtoO(string tb)
        {
            long l = Convert.ToInt64(tb, 2);
            tb = DecimalToAny(l, 8);
        }
        private void HtoD(string tb)
        {
            long l = Convert.ToInt64(tb, 16);
            tb = DecimalToAny(l, 10);
        }
        private void HtoB(string tb)
        {
            long l = Convert.ToInt64(tb, 16);
            tb = DecimalToAny(l, 2);
        }
        private void HtoO(string tb)
        {
            long l = Convert.ToInt64(tb, 16);
            tb = DecimalToAny(l, 8);
        }
        private void OtoD(string tb)
        {
            long l = Convert.ToInt64(tb, 8);
            tb = DecimalToAny(l, 10);
        }
        private void OtoH(string tb)
        {
            long l = Convert.ToInt64(tb, 8);
            tb = DecimalToAny(l, 16);
        }
        private void OtoB(string tb)
        {
            long l = Convert.ToInt64(tb, 8);
            tb = DecimalToAny(l, 2);
        }
    }
}
