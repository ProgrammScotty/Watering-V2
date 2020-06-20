public enum Bmx280PowerMode : byte
{
    /// <summary>
    /// No operations, all registers accessible, lowest power mode, selected after startup.
    /// </summary>
    Sleep = 0b00,

    /// <summary>
    /// Perform one measurement, store results, and return to sleep mode.
    /// </summary>
    Forced = 0b10,

    /// <summary>
    /// Perpetual cycling of measurements and inactive periods.
    /// This interval is determined by the combination of IIR filter and standby time options.
    /// </summary>
    Normal = 0b11
}

internal enum Bme280Register : byte
{
    CTRL_HUM = 0xF2,

    DIG_H1 = 0xA1,
    DIG_H2 = 0xE1,
    DIG_H3 = 0xE3,
    DIG_H4 = 0xE4,
    DIG_H5 = 0xE5,
    DIG_H6 = 0xE7,

    HUMIDDATA = 0xFD
}

internal enum Bmx280Register : byte
{
    CTRL_MEAS = 0xF4,

    DIG_T1 = 0x88,
    DIG_T2 = 0x8A,
    DIG_T3 = 0x8C,

    DIG_P1 = 0x8E,
    DIG_P2 = 0x90,
    DIG_P3 = 0x92,
    DIG_P4 = 0x94,
    DIG_P5 = 0x96,
    DIG_P6 = 0x98,
    DIG_P7 = 0x9A,
    DIG_P8 = 0x9C,
    DIG_P9 = 0x9E,

    STATUS = 0xF3,
    CONFIG = 0xF5,

    PRESSUREDATA = 0xF7,
    TEMPDATA_MSB = 0xFA
}

internal enum Bmxx80Register : byte
{
    CHIPID = 0xD0,
    RESET = 0xE0
}

public enum Bmx280FilteringMode : byte
{
    /// <summary>
    /// Filter off.
    /// </summary>
    Off = 0b000,

    /// <summary>
    /// Coefficient x2.
    /// </summary>
    X2 = 0b001,

    /// <summary>
    /// Coefficient x4.
    /// </summary>
    X4 = 0b010,

    /// <summary>
    /// Coefficient x8.
    /// </summary>
    X8 = 0b011,

    /// <summary>
    /// Coefficient x16.
    /// </summary>
    X16 = 0b100
}

public enum StandbyTime : byte
{
    /// <summary>
    /// 0.5 ms.
    /// </summary>
    Ms0_5 = 0b000,

    /// <summary>
    /// 62.5 ms.
    /// </summary>
    Ms62_5 = 0b001,

    /// <summary>
    /// 125 ms.
    /// </summary>
    Ms125 = 0b010,

    /// <summary>
    /// 250 ms.
    /// </summary>
    Ms250 = 0b011,

    /// <summary>
    /// 500 ms.
    /// </summary>
    Ms500 = 0b100,

    /// <summary>
    /// 1,000 ms.
    /// </summary>
    Ms1000 = 0b101,

    /// <summary>
    /// 10 ms.
    /// </summary>
    Ms10 = 0b110,

    /// <summary>
    /// 20 ms.
    /// </summary>
    Ms20 = 0b111,
}

public enum Sampling : byte
{
    /// <summary>
    /// Skipped (output set to 0x80000).
    /// </summary>
    Skipped = 0b000,

    /// <summary>
    /// Oversampling x1.
    /// </summary>
    UltraLowPower = 0b001,

    /// <summary>
    /// Oversampling x2.
    /// </summary>
    LowPower = 0b010,

    /// <summary>
    /// Oversampling x4.
    /// </summary>
    Standard = 0b011,

    /// <summary>
    /// Oversampling x8.
    /// </summary>
    HighResolution = 0b100,

    /// <summary>
    /// Oversampling x16.
    /// </summary>
    UltraHighResolution = 0b101,
}