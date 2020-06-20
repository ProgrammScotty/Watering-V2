using System;
using System.Collections.Generic;
using System.Text;

namespace Bmxx80
{
    internal class Bmp280CalibrationData : Bmxx80CalibrationData
    {
        /// <summary>
        /// Read coefficient data from device.
        /// </summary>
        /// <param name="bmxx80Base">The <see cref="Bmxx80Base"/> to read coefficient data from.</param>
        protected internal override void ReadFromDevice(Bmxx80Base bmxx80Base)
        {
            // Read temperature calibration data
            DigT1 = bmxx80Base.Read16BitsFromRegister((byte)Bmx280Register.DIG_T1);
            DigT2 = (short)bmxx80Base.Read16BitsFromRegister((byte)Bmx280Register.DIG_T2);
            DigT3 = (short)bmxx80Base.Read16BitsFromRegister((byte)Bmx280Register.DIG_T3);

            // Read pressure calibration data
            DigP1 = bmxx80Base.Read16BitsFromRegister((byte)Bmx280Register.DIG_P1);
            DigP2 = (short)bmxx80Base.Read16BitsFromRegister((byte)Bmx280Register.DIG_P2);
            DigP3 = (short)bmxx80Base.Read16BitsFromRegister((byte)Bmx280Register.DIG_P3);
            DigP4 = (short)bmxx80Base.Read16BitsFromRegister((byte)Bmx280Register.DIG_P4);
            DigP5 = (short)bmxx80Base.Read16BitsFromRegister((byte)Bmx280Register.DIG_P5);
            DigP6 = (short)bmxx80Base.Read16BitsFromRegister((byte)Bmx280Register.DIG_P6);
            DigP7 = (short)bmxx80Base.Read16BitsFromRegister((byte)Bmx280Register.DIG_P7);
            DigP8 = (short)bmxx80Base.Read16BitsFromRegister((byte)Bmx280Register.DIG_P8);
            DigP9 = (short)bmxx80Base.Read16BitsFromRegister((byte)Bmx280Register.DIG_P9);
        }
    }
}
