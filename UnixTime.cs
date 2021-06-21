using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace wan24.Linux
{
    /// <summary>
    /// Unix time
    /// 
    /// The structure represents an Unix timestamp as 32 bit integer.
    /// 
    /// Casting from/to several types and most calculation and comparsion operators is/are implemented:
    /// 
    /// <list type="bullet">
    /// <item><c>int</c> type</item>
    /// <item><c>uint</c> type</item>
    /// <item><c>long</c> type</item>
    /// <item><c>ulong</c> type</item>
    /// <item><c>double</c> type</item>
    /// <item><c>DateTime</c> type</item>
    /// <item><c>*UnixTime</c> types</item>
    /// <item>Addition</item>
    /// <item>Substraction</item>
    /// <item>Division</item>
    /// <item>Multiplication</item>
    /// <item>Modulus</item>
    /// </list>
    /// </summary>
    /// <example>
    /// These examples show some of the most important possibilities:
    /// <code>
    /// // Output the numeric Unix timestamp as string
    /// UnixTime today = UnixTime.Today;
    /// Console.WriteLine(today);
    /// 
    /// // Output the DateTime representation
    /// UnixTime tomorrow = today.AddDays(1);
    /// Console.WriteLine(tomorrow.AsDateTime);
    /// 
    /// // Calculate a simple substraction
    /// TimeSpan diff = tomorrow - today;
    /// Console.WriteLine($"From today to tomorrow the time difference is {diff.TotalSeconds} seconds.");
    /// 
    /// // Convert to a DateTime object
    /// DateTime todayPlus24h = UnixTime.Now.AddHours(24).AsDateTime;
    /// Console.WriteLine($"Today plus 24h is {todayPlus24h}.");
    /// </code>
    /// </example>
    [StructLayout(LayoutKind.Explicit, Size = 4)]
    public struct UnixTime :
        IInt32Value,
        IUInt32Value,
        IInt64Value,
        IUInt64Value,
        IComparable,
        IComparable<DateTime>,
        IComparable<IUInt64Value>,
        IComparable<int>,
        IComparable<uint>,
        IComparable<long>,
        IComparable<ulong>,
        IComparable<double>,
        IConvertible,
        IEquatable<DateTime>,
        IEquatable<IUInt64Value>,
        IEquatable<int>,
        IEquatable<uint>,
        IEquatable<long>,
        IEquatable<ulong>,
        IEquatable<double>,
        IFormattable
    {
        #region Constants
        /// <summary>
        /// Seconds of one minute
        /// </summary>
        public const uint ONE_MINUTE = 60;
        /// <summary>
        /// Seconds of one hour
        /// </summary>
        public const uint ONE_HOUR = 3600;
        /// <summary>
        /// Seconds of one day
        /// </summary>
        public const uint ONE_DAY = 86400;
        #endregion

        #region Fields
        /// <summary>
        /// Regular expression to match a 32 bit positive (or zero) integer in a string
        /// </summary>
        private static readonly Regex RX_NUMBER = new Regex(
            @"^(\d|[1-9]\d{1,8}|1\d{9}|20\d{8}|21[0-3]\d{7}|214[0-6]\d{6}|2147[0-3]\d{5}|21474[0-7]\d{4}|214748[0-2]\d{3}|2147483[0-5]\d{2}|21474836[0-3]\d|214748364[0-7])$",
            RegexOptions.Compiled
            );

        /// <summary>
        /// Unix epoch (1970-01-01 00:00.00)
        /// </summary>
        public static DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        /// <summary>
        /// Min. value
        /// </summary>
        public static readonly UnixTime MinValue = new UnixTime();
        /// <summary>
        /// Max. value
        /// </summary>
        public static readonly UnixTime MaxValue = new UnixTime(int.MaxValue);

        /// <summary>
        /// UTC seconds since 1970-01-01 00:00.00
        /// </summary>
        [FieldOffset(0), JsonInclude]
        public readonly int Seconds;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="seconds">UTC seconds since 1970-01-01 00:00.00</param>
        [JsonConstructor]
        public UnixTime(int seconds = 0)
        {
            if (seconds < 0) throw new ArgumentOutOfRangeException(nameof(seconds));
            Seconds = seconds;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="seconds">UTC seconds since 1970-01-01 00:00.00</param>
        public UnixTime(uint seconds)
        {
            if (seconds > int.MaxValue) throw new ArgumentOutOfRangeException(nameof(seconds));
            Seconds = (int)seconds;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="seconds">UTC seconds since 1970-01-01 00:00.00</param>
        public UnixTime(long seconds)
        {
            if (seconds < 0 || seconds > int.MaxValue) throw new ArgumentOutOfRangeException(nameof(seconds));
            Seconds = (int)seconds;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="seconds">UTC seconds since 1970-01-01 00:00.00</param>
        public UnixTime(ulong seconds)
        {
            if (seconds > int.MaxValue) throw new ArgumentOutOfRangeException(nameof(seconds));
            Seconds = (int)seconds;
        }
        #endregion

        #region Static properties
        /// <summary>
        /// Now
        /// </summary>
        public static UnixTime Now => new UnixTime((int)(DateTime.UtcNow - UnixEpoch).TotalSeconds);

        /// <summary>
        /// Today
        /// </summary>
        public static UnixTime Today => (UnixTime)DateTime.Today.ToUniversalTime();
        #endregion

        #region Value properties
        /// <summary>
        /// Get as DateTime
        /// </summary>
        [JsonIgnore]
        public DateTime AsDateTime => AsDateTimeUtc.ToLocalTime();

        /// <summary>
        /// Get as UTC DateTime
        /// </summary>
        [JsonIgnore]
        public DateTime AsDateTimeUtc => UnixEpoch.AddSeconds(Seconds);
        #endregion

        #region Interface properties
        /// <inheritdoc/>
        int IInt32Value.Value => Seconds;

        /// <inheritdoc/>
        uint IUInt32Value.Value => (uint)Seconds;

        /// <inheritdoc/>
        long IInt64Value.Value => Seconds;

        /// <inheritdoc/>
        ulong IUInt64Value.Value => (ulong)Seconds;
        #endregion

        #region Value methods
        /// <summary>
        /// Add seconds
        /// </summary>
        /// <param name="seconds">Seconds</param>
        /// <returns>New Unix time</returns>
        public UnixTime AddSeconds(int seconds) => this + seconds;

        /// <summary>
        /// Add minutes
        /// </summary>
        /// <param name="minutes">Minutes</param>
        /// <returns>New Unix time</returns>
        public UnixTime AddMinutes(int minutes) => this + minutes * ONE_MINUTE;

        /// <summary>
        /// Add hours
        /// </summary>
        /// <param name="hours">Hours</param>
        /// <returns>New Unix time</returns>
        public UnixTime AddHours(int hours) => this + hours * ONE_HOUR;

        /// <summary>
        /// Add days
        /// </summary>
        /// <param name="days">Days</param>
        /// <returns>New Unix time</returns>
        public UnixTime AddDays(int days) => this + days * ONE_DAY;

        /// <summary>
        /// Add months
        /// </summary>
        /// <param name="months">Months</param>
        /// <returns>New Unix time</returns>
        public UnixTime AddMonths(int months) => (UnixTime)AsDateTimeUtc.AddMonths(months);

        /// <summary>
        /// Add years
        /// </summary>
        /// <param name="years">Years</param>
        /// <returns>New Unix time</returns>
        public UnixTime AddYears(int years) => (UnixTime)AsDateTimeUtc.AddYears(years);
        #endregion

        #region Overrides
        /// <inheritdoc/>
        public override int GetHashCode() => Seconds.GetHashCode();

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is IUInt64Value uivalue && uivalue.Value == (uint)Seconds;

        /// <inheritdoc/>
        public override string ToString() => Seconds.ToString(CultureInfo.InvariantCulture);
        #endregion

        #region Interface methods
        /// <inheritdoc/>
        int IComparable.CompareTo(object obj)
        {
            if (obj == null) return 1;
            Type type;
            if (obj is IUInt64Value uivalue)
            {
                return Seconds.CompareTo(uivalue.Value);
            }
            else if ((type = obj.GetType()) == typeof(DateTime))
            {
                DateTime dateTime = (DateTime)obj;
                if (dateTime.Kind == DateTimeKind.Local) dateTime = dateTime.ToUniversalTime();
                return Seconds.CompareTo((UnixEpoch - dateTime).TotalSeconds);
            }
            else if (type == typeof(int) || type == typeof(uint) || type == typeof(long) || type == typeof(ulong) || type == typeof(double))
            {
                return Seconds.CompareTo(obj);
            }
            return -1;
        }

        /// <inheritdoc/>
        int IComparable<DateTime>.CompareTo(DateTime other)
        {
            if (other.Kind == DateTimeKind.Local) other = other.ToUniversalTime();
            return Seconds.CompareTo((UnixEpoch - other).TotalSeconds);
        }

        /// <inheritdoc/>
        int IComparable<IUInt64Value>.CompareTo(IUInt64Value other) => Seconds.CompareTo(other.Value);

        /// <inheritdoc/>
        int IComparable<int>.CompareTo(int other) => Seconds.CompareTo(other);

        /// <inheritdoc/>
        int IComparable<uint>.CompareTo(uint other) => Seconds.CompareTo(other);

        /// <inheritdoc/>
        int IComparable<long>.CompareTo(long other) => Seconds.CompareTo(other);

        /// <inheritdoc/>
        int IComparable<ulong>.CompareTo(ulong other) => Seconds.CompareTo(other);

        /// <inheritdoc/>
        int IComparable<double>.CompareTo(double other) => Seconds.CompareTo(other);

        /// <inheritdoc/>
        bool IEquatable<DateTime>.Equals(DateTime other)
        {
            if (other.Kind == DateTimeKind.Local) other = other.ToUniversalTime();
            return Seconds.Equals((UnixEpoch - other).TotalSeconds);
        }

        /// <inheritdoc/>
        bool IEquatable<IUInt64Value>.Equals(IUInt64Value other) => Seconds.Equals(other.Value);

        /// <inheritdoc/>
        bool IEquatable<int>.Equals(int other) => Seconds.Equals(other);

        /// <inheritdoc/>
        bool IEquatable<uint>.Equals(uint other) => Seconds.Equals(other);

        /// <inheritdoc/>
        bool IEquatable<long>.Equals(long other) => Seconds.Equals(other);

        /// <inheritdoc/>
        bool IEquatable<ulong>.Equals(ulong other) => Seconds.Equals(other);

        /// <inheritdoc/>
        bool IEquatable<double>.Equals(double other) => Seconds.Equals(other);

        /// <inheritdoc/>
        TypeCode IConvertible.GetTypeCode() => Seconds.GetTypeCode();

        /// <inheritdoc/>
        bool IConvertible.ToBoolean(IFormatProvider provider) => throw new InvalidCastException();

        /// <inheritdoc/>
        byte IConvertible.ToByte(IFormatProvider provider) => (byte)Seconds;

        /// <inheritdoc/>
        char IConvertible.ToChar(IFormatProvider provider) => throw new InvalidCastException();

        /// <inheritdoc/>
        DateTime IConvertible.ToDateTime(IFormatProvider provider) => AsDateTime;

        /// <inheritdoc/>
        decimal IConvertible.ToDecimal(IFormatProvider provider) => Seconds;

        /// <inheritdoc/>
        double IConvertible.ToDouble(IFormatProvider provider) => Seconds;

        /// <inheritdoc/>
        short IConvertible.ToInt16(IFormatProvider provider) => (short)Seconds;

        /// <inheritdoc/>
        int IConvertible.ToInt32(IFormatProvider provider) => Seconds;

        /// <inheritdoc/>
        long IConvertible.ToInt64(IFormatProvider provider) => Seconds;

        /// <inheritdoc/>
        sbyte IConvertible.ToSByte(IFormatProvider provider) => (sbyte)Seconds;

        /// <inheritdoc/>
        float IConvertible.ToSingle(IFormatProvider provider) => Seconds;

        /// <inheritdoc/>
        string IConvertible.ToString(IFormatProvider provider) => ToString();

        /// <inheritdoc/>
        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            IConvertible instance = this;
            if (conversionType == typeof(int)) return instance.ToInt32(provider);
            if (conversionType == typeof(uint)) return instance.ToUInt32(provider);
            if (conversionType == typeof(byte)) return instance.ToByte(provider);
            if (conversionType == typeof(sbyte)) return instance.ToSByte(provider);
            if (conversionType == typeof(short)) return instance.ToInt16(provider);
            if (conversionType == typeof(ushort)) return instance.ToUInt16(provider);
            if (conversionType == typeof(long)) return instance.ToInt64(provider);
            if (conversionType == typeof(ulong)) return instance.ToUInt64(provider);
            if (conversionType == typeof(float)) return instance.ToSingle(provider);
            if (conversionType == typeof(double)) return instance.ToDouble(provider);
            if (conversionType == typeof(decimal)) return instance.ToDecimal(provider);
            if (conversionType == typeof(DateTime)) return instance.ToDateTime(provider);
            if (conversionType == typeof(string)) return instance.ToString(provider);
            if (conversionType == typeof(UnixTime)) return this;
            if (conversionType == typeof(UnsignedUnixTime)) return new UnsignedUnixTime((uint)Seconds);
            if (conversionType == typeof(LongUnixTime)) return new LongUnixTime(Seconds);
            if (conversionType == typeof(UnsignedLongUnixTime)) return new UnsignedLongUnixTime((ulong)Seconds);
            throw new InvalidCastException();
        }

        /// <inheritdoc/>
        ushort IConvertible.ToUInt16(IFormatProvider provider) => (ushort)Seconds;

        /// <inheritdoc/>
        uint IConvertible.ToUInt32(IFormatProvider provider) => (uint)Seconds;

        /// <inheritdoc/>
        ulong IConvertible.ToUInt64(IFormatProvider provider) => (ulong)Seconds;

        /// <inheritdoc/>
        string IFormattable.ToString(string format, IFormatProvider formatProvider) => AsDateTime.ToString(format, formatProvider);
        #endregion

        #region Static methods
        /// <summary>
        /// Get from DateTime
        /// </summary>
        /// <param name="dateTime">DateTime</param>
        /// <returns>Unix time</returns>
        public static UnixTime FromDateTime(DateTime dateTime) => new UnixTime((int)(dateTime.ToUniversalTime() - UnixEpoch).TotalSeconds);

        /// <summary>
        /// Get from UTC DateTime
        /// </summary>
        /// <param name="dateTime">UTC DateTime</param>
        /// <returns>Unix time</returns>
        public static UnixTime FromDateTimeUtc(DateTime dateTime) => new UnixTime((int)(dateTime - UnixEpoch).TotalSeconds);

        /// <summary>
        /// Get from a string
        /// </summary>
        /// <param name="str">String</param>
        /// <returns>Unix time</returns>
        public static UnixTime FromString(string str)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));
            if (!RX_NUMBER.IsMatch(str)) throw new ArgumentException("Invalid number format", nameof(str));
            return new UnixTime(int.Parse(str));
        }

        /// <summary>
        /// Parse from a string
        /// </summary>
        /// <param name="str">String</param>
        /// <returns>Unix time</returns>
        public static UnixTime Parse(string str) => FromString(str);

        /// <summary>
        /// Try parsing a string
        /// </summary>
        /// <param name="str">String</param>
        /// <param name="unixTime">Unix time</param>
        /// <returns>Succeed?</returns>
        public static bool TryParse(string str, out UnixTime unixTime)
        {
            if (string.IsNullOrWhiteSpace(str) || !RX_NUMBER.IsMatch(str))
            {
                unixTime = default;
                return false;
            }
            unixTime = new UnixTime(int.Parse(str));
            return true;
        }
        #endregion

        #region Implicit operators
        /// <summary>
        /// Cast to string
        /// </summary>
        /// <param name="unixTime">Unix time</param>
        public static implicit operator string(UnixTime unixTime) => unixTime.ToString();

        /// <summary>
        /// Cast to DateTime
        /// </summary>
        /// <param name="unixTime">Unix time</param>
        public static implicit operator DateTime(UnixTime unixTime) => unixTime.AsDateTime;

        /// <summary>
        /// Cast as integer
        /// </summary>
        /// <param name="unixTime">Unix time</param>
        public static implicit operator int(UnixTime unixTime) => unixTime.Seconds;

        /// <summary>
        /// Cast as integer
        /// </summary>
        /// <param name="unixTime">Unix time</param>
        public static implicit operator uint(UnixTime unixTime) => (uint)unixTime.Seconds;

        /// <summary>
        /// Cast as integer
        /// </summary>
        /// <param name="unixTime">Unix time</param>
        public static implicit operator long(UnixTime unixTime) => unixTime.Seconds;

        /// <summary>
        /// Cast as integer
        /// </summary>
        /// <param name="unixTime">Unix time</param>
        public static implicit operator ulong(UnixTime unixTime) => (ulong)unixTime.Seconds;

        /// <summary>
        /// Cast as integer
        /// </summary>
        /// <param name="unixTime">Unix time</param>
        public static implicit operator double(UnixTime unixTime) => (ulong)unixTime.Seconds;

        /// <summary>
        /// Cast as Unix time
        /// </summary>
        /// <param name="unixTime">Unix time</param>
        public static implicit operator UnsignedUnixTime(UnixTime unixTime) => new UnsignedUnixTime(unixTime.Seconds);

        /// <summary>
        /// Cast as Unix time
        /// </summary>
        /// <param name="unixTime">Unix time</param>
        public static implicit operator LongUnixTime(UnixTime unixTime) => new LongUnixTime(unixTime.Seconds);

        /// <summary>
        /// Cast as Unix time
        /// </summary>
        /// <param name="unixTime">Unix time</param>
        public static implicit operator UnsignedLongUnixTime(UnixTime unixTime) => new UnsignedLongUnixTime(unixTime.Seconds);
        #endregion

        #region Explicit operators
        /// <summary>
        /// Cast from DateTime
        /// </summary>
        /// <param name="dateTime">DateTime</param>
        public static explicit operator UnixTime(DateTime dateTime) => dateTime.Kind == DateTimeKind.Local ? FromDateTime(dateTime) : FromDateTimeUtc(dateTime);

        /// <summary>
        /// Cast from seconds
        /// </summary>
        /// <param name="seconds">Seconds</param>
        public static explicit operator UnixTime(int seconds) => new UnixTime(seconds);

        /// <summary>
        /// Cast from seconds
        /// </summary>
        /// <param name="seconds">Seconds</param>
        public static explicit operator UnixTime(uint seconds) => new UnixTime(seconds);

        /// <summary>
        /// Cast from seconds
        /// </summary>
        /// <param name="seconds">Seconds</param>
        public static explicit operator UnixTime(long seconds) => new UnixTime(seconds);

        /// <summary>
        /// Cast from seconds
        /// </summary>
        /// <param name="seconds">Seconds</param>
        public static explicit operator UnixTime(ulong seconds) => new UnixTime(seconds);

        /// <summary>
        /// Cast from seconds
        /// </summary>
        /// <param name="seconds">Seconds</param>
        public static explicit operator UnixTime(double seconds) => new UnixTime((int)seconds);

        /// <summary>
        /// Cast from string
        /// </summary>
        /// <param name="str">String</param>
        public static explicit operator UnixTime(string str) => FromString(str);
        #endregion

        #region Addition operators
        /// <summary>
        /// Summarize
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Timespan</param>
        /// <returns>New unix time</returns>
        public static UnixTime operator +(UnixTime a, TimeSpan b) => new UnixTime((int)(a.Seconds + b.TotalSeconds));

        /// <summary>
        /// Summarize
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>New unix time</returns>
        public static UnixTime operator +(UnixTime a, int b) => new UnixTime(a.Seconds + b);

        /// <summary>
        /// Summarize
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>New unix time</returns>
        public static UnixTime operator +(UnixTime a, uint b) => new UnixTime((int)(a.Seconds + b));

        /// <summary>
        /// Summarize
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>New unix time</returns>
        public static UnixTime operator +(UnixTime a, long b) => new UnixTime((int)(a.Seconds + b));

        /// <summary>
        /// Summarize
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>New unix time</returns>
        public static UnixTime operator +(UnixTime a, ulong b) => new UnixTime((int)((ulong)a.Seconds + b));

        /// <summary>
        /// Summarize
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>New unix time</returns>
        public static UnixTime operator +(UnixTime a, double b) => new UnixTime((int)(a.Seconds + b));
        #endregion

        #region Substraction operators
        /// <summary>
        /// Substract
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Timespan</returns>
        public static TimeSpan operator -(UnixTime a, UnixTime b) => TimeSpan.FromSeconds(a.Seconds - b.Seconds);

        /// <summary>
        /// Substract
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Timespan</returns>
        public static TimeSpan operator -(UnixTime a, UnsignedUnixTime b) => TimeSpan.FromSeconds(a.Seconds - b.Seconds);

        /// <summary>
        /// Substract
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Timespan</returns>
        public static TimeSpan operator -(UnixTime a, LongUnixTime b) => TimeSpan.FromSeconds(a.Seconds - b.Seconds);

        /// <summary>
        /// Substract
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Timespan</returns>
        public static TimeSpan operator -(UnixTime a, UnsignedLongUnixTime b) => TimeSpan.FromSeconds(a.Seconds - (double)b.Seconds);

        /// <summary>
        /// Substract
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Timespan</param>
        /// <returns>Timespan</returns>
        public static UnixTime operator -(UnixTime a, TimeSpan b) => new UnixTime((int)(a.Seconds - b.TotalSeconds));

        /// <summary>
        /// Substract
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Timespan</returns>
        public static UnixTime operator -(UnixTime a, int b) => new UnixTime(a.Seconds - b);

        /// <summary>
        /// Substract
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Timespan</returns>
        public static UnixTime operator -(UnixTime a, uint b) => new UnixTime((int)(a.Seconds - b));

        /// <summary>
        /// Substract
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Timespan</returns>
        public static UnixTime operator -(UnixTime a, long b) => new UnixTime((int)(a.Seconds - b));

        /// <summary>
        /// Substract
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Timespan</returns>
        public static UnixTime operator -(UnixTime a, ulong b) => new UnixTime((int)((ulong)a.Seconds - b));

        /// <summary>
        /// Substract
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Timespan</returns>
        public static UnixTime operator -(UnixTime a, double b) => new UnixTime((int)(a.Seconds - b));
        #endregion

        #region Comparsion operators
        /// <summary>
        /// Determine if greater than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Greater than?</returns>
        public static bool operator >(UnixTime a, UnixTime b) => a.Seconds > b.Seconds;

        /// <summary>
        /// Determine if lower than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Lower than?</returns>
        public static bool operator <(UnixTime a, UnixTime b) => a.Seconds < b.Seconds;

        /// <summary>
        /// Determine if greater than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Greater than?</returns>
        public static bool operator >=(UnixTime a, UnixTime b) => a.Seconds >= b.Seconds;

        /// <summary>
        /// Determine if lower than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Lower than?</returns>
        public static bool operator <=(UnixTime a, UnixTime b) => a.Seconds <= b.Seconds;

        /// <summary>
        /// Determine if greater than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Greater than?</returns>
        public static bool operator >(UnixTime a, UnsignedUnixTime b) => a.Seconds > b.Seconds;

        /// <summary>
        /// Determine if lower than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Lower than?</returns>
        public static bool operator <(UnixTime a, UnsignedUnixTime b) => a.Seconds < b.Seconds;

        /// <summary>
        /// Determine if greater than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Greater than?</returns>
        public static bool operator >=(UnixTime a, UnsignedUnixTime b) => a.Seconds >= b.Seconds;

        /// <summary>
        /// Determine if lower than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Lower than?</returns>
        public static bool operator <=(UnixTime a, UnsignedUnixTime b) => a.Seconds <= b.Seconds;
        /// <summary>
        /// Determine if greater than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Greater than?</returns>
        public static bool operator >(UnixTime a, LongUnixTime b) => a.Seconds > b.Seconds;

        /// <summary>
        /// Determine if lower than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Lower than?</returns>
        public static bool operator <(UnixTime a, LongUnixTime b) => a.Seconds < b.Seconds;

        /// <summary>
        /// Determine if greater than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Greater than?</returns>
        public static bool operator >=(UnixTime a, LongUnixTime b) => a.Seconds >= b.Seconds;

        /// <summary>
        /// Determine if lower than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Lower than?</returns>
        public static bool operator <=(UnixTime a, LongUnixTime b) => a.Seconds <= b.Seconds;
        /// <summary>
        /// Determine if greater than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Greater than?</returns>
        public static bool operator >(UnixTime a, UnsignedLongUnixTime b) => (uint)a.Seconds > b.Seconds;

        /// <summary>
        /// Determine if lower than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Lower than?</returns>
        public static bool operator <(UnixTime a, UnsignedLongUnixTime b) => (uint)a.Seconds < b.Seconds;

        /// <summary>
        /// Determine if greater than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Greater than?</returns>
        public static bool operator >=(UnixTime a, UnsignedLongUnixTime b) => (uint)a.Seconds >= b.Seconds;

        /// <summary>
        /// Determine if lower than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Lower than?</returns>
        public static bool operator <=(UnixTime a, UnsignedLongUnixTime b) => (uint)a.Seconds <= b.Seconds;

        /// <summary>
        /// Determine if greater than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Greater than?</returns>
        public static bool operator >(UnixTime a, int b) => a.Seconds > b;

        /// <summary>
        /// Determine if lower than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Lower than?</returns>
        public static bool operator <(UnixTime a, int b) => a.Seconds < b;

        /// <summary>
        /// Determine if greater than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Greater than?</returns>
        public static bool operator >=(UnixTime a, int b) => a.Seconds >= b;

        /// <summary>
        /// Determine if lower than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Lower than?</returns>
        public static bool operator <=(UnixTime a, int b) => a.Seconds <= b;

        /// <summary>
        /// Determine if greater than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Greater than?</returns>
        public static bool operator >(UnixTime a, uint b) => a.Seconds > b;

        /// <summary>
        /// Determine if lower than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Lower than?</returns>
        public static bool operator <(UnixTime a, uint b) => a.Seconds < b;

        /// <summary>
        /// Determine if greater than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Greater than?</returns>
        public static bool operator >=(UnixTime a, uint b) => a.Seconds >= b;

        /// <summary>
        /// Determine if lower than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Lower than?</returns>
        public static bool operator <=(UnixTime a, uint b) => a.Seconds <= b;

        /// <summary>
        /// Determine if greater than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Greater than?</returns>
        public static bool operator >(UnixTime a, long b) => a.Seconds > b;

        /// <summary>
        /// Determine if lower than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Lower than?</returns>
        public static bool operator <(UnixTime a, long b) => a.Seconds < b;

        /// <summary>
        /// Determine if greater than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Greater than?</returns>
        public static bool operator >=(UnixTime a, long b) => a.Seconds >= b;

        /// <summary>
        /// Determine if lower than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Lower than?</returns>
        public static bool operator <=(UnixTime a, long b) => a.Seconds <= b;

        /// <summary>
        /// Determine if greater than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Greater than?</returns>
        public static bool operator >(UnixTime a, ulong b) => (uint)a.Seconds > b;

        /// <summary>
        /// Determine if lower than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Lower than?</returns>
        public static bool operator <(UnixTime a, ulong b) => (uint)a.Seconds < b;

        /// <summary>
        /// Determine if greater than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Greater than?</returns>
        public static bool operator >=(UnixTime a, ulong b) => (uint)a.Seconds >= b;

        /// <summary>
        /// Determine if lower than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Lower than?</returns>
        public static bool operator <=(UnixTime a, ulong b) => (uint)a.Seconds <= b;

        /// <summary>
        /// Determine if greater than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Greater than?</returns>
        public static bool operator >(UnixTime a, double b) => a.Seconds > b;

        /// <summary>
        /// Determine if lower than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Lower than?</returns>
        public static bool operator <(UnixTime a, double b) => a.Seconds < b;

        /// <summary>
        /// Determine if greater than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Greater than?</returns>
        public static bool operator >=(UnixTime a, double b) => a.Seconds >= b;

        /// <summary>
        /// Determine if lower than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Lower than?</returns>
        public static bool operator <=(UnixTime a, double b) => a.Seconds <= b;

        /// <summary>
        /// Determine if equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Is equal?</returns>
        public static bool operator ==(UnixTime a, UnixTime b) => a.Seconds == b.Seconds;

        /// <summary>
        /// Determine if not equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Is not equal?</returns>
        public static bool operator !=(UnixTime a, UnixTime b) => a.Seconds != b.Seconds;

        /// <summary>
        /// Determine if equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Is equal?</returns>
        public static bool operator ==(UnixTime a, UnsignedUnixTime b) => a.Seconds == b.Seconds;

        /// <summary>
        /// Determine if not equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Is not equal?</returns>
        public static bool operator !=(UnixTime a, UnsignedUnixTime b) => a.Seconds != b.Seconds;

        /// <summary>
        /// Determine if equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Is equal?</returns>
        public static bool operator ==(UnixTime a, LongUnixTime b) => a.Seconds == b.Seconds;

        /// <summary>
        /// Determine if not equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Is not equal?</returns>
        public static bool operator !=(UnixTime a, LongUnixTime b) => a.Seconds != b.Seconds;

        /// <summary>
        /// Determine if equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Is equal?</returns>
        public static bool operator ==(UnixTime a, UnsignedLongUnixTime b) => (uint)a.Seconds == b.Seconds;

        /// <summary>
        /// Determine if not equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Is not equal?</returns>
        public static bool operator !=(UnixTime a, UnsignedLongUnixTime b) => (uint)a.Seconds != b.Seconds;

        /// <summary>
        /// Determine if equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Is equal?</returns>
        public static bool operator ==(UnixTime a, int b) => a.Seconds == b;

        /// <summary>
        /// Determine if not equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Is not equal?</returns>
        public static bool operator !=(UnixTime a, int b) => a.Seconds != b;

        /// <summary>
        /// Determine if equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Is equal?</returns>
        public static bool operator ==(UnixTime a, uint b) => a.Seconds == b;

        /// <summary>
        /// Determine if not equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Is not equal?</returns>
        public static bool operator !=(UnixTime a, uint b) => a.Seconds != b;

        /// <summary>
        /// Determine if equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Is equal?</returns>
        public static bool operator ==(UnixTime a, long b) => a.Seconds == b;

        /// <summary>
        /// Determine if not equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Is not equal?</returns>
        public static bool operator !=(UnixTime a, long b) => a.Seconds != b;

        /// <summary>
        /// Determine if equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Is equal?</returns>
        public static bool operator ==(UnixTime a, ulong b) => (uint)a.Seconds == b;

        /// <summary>
        /// Determine if not equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Is not equal?</returns>
        public static bool operator !=(UnixTime a, ulong b) => (uint)a.Seconds != b;

        /// <summary>
        /// Determine if equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Is equal?</returns>
        public static bool operator ==(UnixTime a, double b) => a.Seconds == b;

        /// <summary>
        /// Determine if not equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Is not equal?</returns>
        public static bool operator !=(UnixTime a, double b) => a.Seconds != b;
        #endregion

        #region Other operators
        /// <summary>
        /// Divide
        /// </summary>
        /// <param name="unixTime">Unix time</param>
        /// <param name="divider">Divider</param>
        /// <returns>New unix time</returns>
        public static UnixTime operator /(UnixTime unixTime, int divider) => new UnixTime((int)Math.Round((double)unixTime.Seconds / divider));

        /// <summary>
        /// Divide
        /// </summary>
        /// <param name="unixTime">Unix time</param>
        /// <param name="divider">Divider</param>
        /// <returns>New unix time</returns>
        public static UnixTime operator /(UnixTime unixTime, float divider) => new UnixTime((int)Math.Round((double)unixTime.Seconds / divider));

        /// <summary>
        /// Multiplicate
        /// </summary>
        /// <param name="unixTime">Unix time</param>
        /// <param name="multiplicator">Multiplicator</param>
        /// <returns>New unix time</returns>
        public static UnixTime operator *(UnixTime unixTime, int multiplicator) => new UnixTime(unixTime.Seconds * multiplicator);

        /// <summary>
        /// Multiplicate
        /// </summary>
        /// <param name="unixTime">Unix time</param>
        /// <param name="multiplicator">Multiplicator</param>
        /// <returns>New unix time</returns>
        public static UnixTime operator *(UnixTime unixTime, float multiplicator) => new UnixTime((int)Math.Round(unixTime.Seconds * multiplicator));

        /// <summary>
        /// Modulus
        /// </summary>
        /// <param name="unixTime">Unix time</param>
        /// <param name="modulus">Modulus</param>
        /// <returns>Rest</returns>
        public static int operator %(UnixTime unixTime, int modulus) => unixTime.Seconds % modulus;
        #endregion
    }
}
