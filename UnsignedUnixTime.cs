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
    /// The structure represents an Unix timestamp as unsigned 32 bit integer.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 4)]
    public struct UnsignedUnixTime :
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
        #region Fields
        /// <summary>
        /// Regular expression to match a 32 bit unsigned integer in a string
        /// </summary>
        private static readonly Regex RX_NUMBER = new Regex(
            @"^(\d|[1-9]\d{1,8}|[1-3]\d{9}|4[01]\d{8}|42[0-8]\d{7}|429[0-3]\d{6}|4294[0-8]\d{5}|42949[0-5]\d{4}|429496[0-6]\d{3}|4294967[01]\d{2}|42949672[0-8]\d|429496729[0-5])$",
            RegexOptions.Compiled
            );

        /// <summary>
        /// Min. value
        /// </summary>
        public static readonly UnsignedUnixTime MinValue = new UnsignedUnixTime();
        /// <summary>
        /// Max. value
        /// </summary>
        public static readonly UnsignedUnixTime MaxValue = new UnsignedUnixTime(uint.MaxValue);

        /// <summary>
        /// UTC seconds since 1970-01-01 00:00.00
        /// </summary>
        [FieldOffset(0), JsonInclude]
        public readonly uint Seconds;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="seconds">UTC seconds since 1970-01-01 00:00.00</param>
        public UnsignedUnixTime(int seconds)
        {
            if (seconds < 0) throw new ArgumentOutOfRangeException(nameof(seconds));
            Seconds = (uint)seconds;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="seconds">UTC seconds since 1970-01-01 00:00.00</param>
        [JsonConstructor]
        public UnsignedUnixTime(uint seconds = 0) => Seconds = seconds;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="seconds">UTC seconds since 1970-01-01 00:00.00</param>
        public UnsignedUnixTime(long seconds)
        {
            if (seconds < 0 || seconds > uint.MaxValue) throw new ArgumentOutOfRangeException(nameof(seconds));
            Seconds = (uint)seconds;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="seconds">UTC seconds since 1970-01-01 00:00.00</param>
        public UnsignedUnixTime(ulong seconds)
        {
            if (seconds > uint.MaxValue) throw new ArgumentOutOfRangeException(nameof(seconds));
            Seconds = (uint)seconds;
        }
        #endregion

        #region Static properties
        /// <summary>
        /// Now
        /// </summary>
        public static UnsignedUnixTime Now => new UnsignedUnixTime((uint)(DateTime.UtcNow - UnixTime.UnixEpoch).TotalSeconds);

        /// <summary>
        /// Today
        /// </summary>
        public static UnsignedUnixTime Today => (UnsignedUnixTime)DateTime.Today.ToUniversalTime();
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
        public DateTime AsDateTimeUtc => UnixTime.UnixEpoch.AddSeconds(Seconds);
        #endregion

        #region Interface properties
        /// <inheritdoc/>
        uint IUInt32Value.Value => Seconds;

        /// <inheritdoc/>
        long IInt64Value.Value => Seconds;

        /// <inheritdoc/>
        ulong IUInt64Value.Value => Seconds;
        #endregion

        #region Value methods
        /// <summary>
        /// Add seconds
        /// </summary>
        /// <param name="seconds">Seconds</param>
        /// <returns>New Unix time</returns>
        public UnsignedUnixTime AddSeconds(uint seconds) => this + seconds;

        /// <summary>
        /// Add minutes
        /// </summary>
        /// <param name="minutes">Minutes</param>
        /// <returns>New Unix time</returns>
        public UnsignedUnixTime AddMinutes(uint minutes) => this + minutes * UnixTime.ONE_MINUTE;

        /// <summary>
        /// Add hours
        /// </summary>
        /// <param name="hours">Hours</param>
        /// <returns>New Unix time</returns>
        public UnsignedUnixTime AddHours(uint hours) => this + hours * UnixTime.ONE_HOUR;

        /// <summary>
        /// Add days
        /// </summary>
        /// <param name="days">Days</param>
        /// <returns>New Unix time</returns>
        public UnsignedUnixTime AddDays(uint days) => this + days * UnixTime.ONE_DAY;

        /// <summary>
        /// Add months
        /// </summary>
        /// <param name="months">Months</param>
        /// <returns>New Unix time</returns>
        public UnsignedUnixTime AddMonths(int months) => (UnsignedUnixTime)AsDateTimeUtc.AddMonths(months);

        /// <summary>
        /// Add years
        /// </summary>
        /// <param name="years">Years</param>
        /// <returns>New Unix time</returns>
        public UnsignedUnixTime AddYears(int years) => (UnsignedUnixTime)AsDateTimeUtc.AddYears(years);
        #endregion

        #region Overrides
        /// <inheritdoc/>
        public override int GetHashCode() => Seconds.GetHashCode();

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is IUInt64Value uivalue && uivalue.Value == Seconds;

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
                return Seconds.CompareTo((UnixTime.UnixEpoch - dateTime).TotalSeconds);
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
            return Seconds.CompareTo((UnixTime.UnixEpoch - other).TotalSeconds);
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
        bool IEquatable<IUInt64Value>.Equals(IUInt64Value other) => Seconds.Equals(other.Value);

        /// <inheritdoc/>
        bool IEquatable<DateTime>.Equals(DateTime other)
        {
            if (other.Kind == DateTimeKind.Local) other = other.ToUniversalTime();
            return Seconds.Equals((UnixTime.UnixEpoch - other).TotalSeconds);
        }

        /// <inheritdoc/>
        bool IEquatable<int>.Equals(int other) => Seconds == other;

        /// <inheritdoc/>
        bool IEquatable<uint>.Equals(uint other) => Seconds == other;

        /// <inheritdoc/>
        bool IEquatable<long>.Equals(long other) => Seconds == other;

        /// <inheritdoc/>
        bool IEquatable<ulong>.Equals(ulong other) => Seconds == other;

        /// <inheritdoc/>
        bool IEquatable<double>.Equals(double other) => Seconds == other;

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
        int IConvertible.ToInt32(IFormatProvider provider) => (int)Seconds;

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
            if (conversionType == typeof(UnsignedUnixTime)) return this;
            if (conversionType == typeof(UnixTime)) return new UnixTime((int)Seconds);
            if (conversionType == typeof(LongUnixTime)) return new LongUnixTime(Seconds);
            if (conversionType == typeof(UnsignedLongUnixTime)) return new UnsignedLongUnixTime(Seconds);
            throw new InvalidCastException();
        }

        /// <inheritdoc/>
        ushort IConvertible.ToUInt16(IFormatProvider provider) => (ushort)Seconds;

        /// <inheritdoc/>
        uint IConvertible.ToUInt32(IFormatProvider provider) => Seconds;

        /// <inheritdoc/>
        ulong IConvertible.ToUInt64(IFormatProvider provider) => Seconds;

        /// <inheritdoc/>
        string IFormattable.ToString(string format, IFormatProvider formatProvider) => AsDateTime.ToString(format, formatProvider);
        #endregion

        #region Static methods
        /// <summary>
        /// Get from DateTime
        /// </summary>
        /// <param name="dateTime">DateTime</param>
        /// <returns>Unix time</returns>
        public static UnsignedUnixTime FromDateTime(DateTime dateTime) => new UnsignedUnixTime((uint)(dateTime.ToUniversalTime() - UnixTime.UnixEpoch).TotalSeconds);

        /// <summary>
        /// Get from UTC DateTime
        /// </summary>
        /// <param name="dateTime">UTC DateTime</param>
        /// <returns>Unix time</returns>
        public static UnsignedUnixTime FromDateTimeUtc(DateTime dateTime) => new UnsignedUnixTime((uint)(dateTime - UnixTime.UnixEpoch).TotalSeconds);

        /// <summary>
        /// Get from a string
        /// </summary>
        /// <param name="str">String</param>
        /// <returns>Unix time</returns>
        public static UnsignedUnixTime FromString(string str)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));
            if (!RX_NUMBER.IsMatch(str)) throw new ArgumentException("Invalid number format", nameof(str));
            return new UnsignedUnixTime(uint.Parse(str));
        }

        /// <summary>
        /// Parse from a string
        /// </summary>
        /// <param name="str">String</param>
        /// <returns>Unix time</returns>
        public static UnsignedUnixTime Parse(string str) => FromString(str);

        /// <summary>
        /// Try parsing a string
        /// </summary>
        /// <param name="str">String</param>
        /// <param name="unixTime">Unix time</param>
        /// <returns>Succeed?</returns>
        public static bool TryParse(string str, out UnsignedUnixTime unixTime)
        {
            if (string.IsNullOrWhiteSpace(str) || !RX_NUMBER.IsMatch(str))
            {
                unixTime = default;
                return false;
            }
            unixTime = new UnsignedUnixTime(uint.Parse(str));
            return true;
        }
        #endregion

        #region Implicit operators
        /// <summary>
        /// Cast to string
        /// </summary>
        /// <param name="unixTime">Unix time</param>
        public static implicit operator string(UnsignedUnixTime unixTime) => unixTime.ToString();

        /// <summary>
        /// Cast to DateTime
        /// </summary>
        /// <param name="unixTime">Unix time</param>
        public static implicit operator DateTime(UnsignedUnixTime unixTime) => unixTime.AsDateTime;

        /// <summary>
        /// Cast as integer
        /// </summary>
        /// <param name="unixTime">Unix time</param>
        public static implicit operator int(UnsignedUnixTime unixTime) => (int)unixTime.Seconds;

        /// <summary>
        /// Cast as integer
        /// </summary>
        /// <param name="unixTime">Unix time</param>
        public static implicit operator uint(UnsignedUnixTime unixTime) => unixTime.Seconds;

        /// <summary>
        /// Cast as integer
        /// </summary>
        /// <param name="unixTime">Unix time</param>
        public static implicit operator long(UnsignedUnixTime unixTime) => unixTime.Seconds;

        /// <summary>
        /// Cast as integer
        /// </summary>
        /// <param name="unixTime">Unix time</param>
        public static implicit operator ulong(UnsignedUnixTime unixTime) => unixTime.Seconds;

        /// <summary>
        /// Cast as integer
        /// </summary>
        /// <param name="unixTime">Unix time</param>
        public static implicit operator double(UnsignedUnixTime unixTime) => unixTime.Seconds;

        /// <summary>
        /// Cast as Unix time
        /// </summary>
        /// <param name="unixTime">Unix time</param>
        public static implicit operator UnixTime(UnsignedUnixTime unixTime) => new UnixTime((int)unixTime.Seconds);

        /// <summary>
        /// Cast as Unix time
        /// </summary>
        /// <param name="unixTime">Unix time</param>
        public static implicit operator LongUnixTime(UnsignedUnixTime unixTime) => new LongUnixTime(unixTime.Seconds);

        /// <summary>
        /// Cast as Unix time
        /// </summary>
        /// <param name="unixTime">Unix time</param>
        public static implicit operator UnsignedLongUnixTime(UnsignedUnixTime unixTime) => new UnsignedLongUnixTime(unixTime.Seconds);
        #endregion

        #region Explicit operators
        /// <summary>
        /// Cast from DateTime
        /// </summary>
        /// <param name="dateTime">DateTime</param>
        public static explicit operator UnsignedUnixTime(DateTime dateTime) => dateTime.Kind == DateTimeKind.Local ? FromDateTime(dateTime) : FromDateTimeUtc(dateTime);

        /// <summary>
        /// Cast from seconds
        /// </summary>
        /// <param name="seconds">Seconds</param>
        public static explicit operator UnsignedUnixTime(int seconds) => new UnsignedUnixTime((uint)seconds);

        /// <summary>
        /// Cast from seconds
        /// </summary>
        /// <param name="seconds">Seconds</param>
        public static explicit operator UnsignedUnixTime(uint seconds) => new UnsignedUnixTime(seconds);

        /// <summary>
        /// Cast from seconds
        /// </summary>
        /// <param name="seconds">Seconds</param>
        public static explicit operator UnsignedUnixTime(long seconds) => new UnsignedUnixTime((uint)seconds);

        /// <summary>
        /// Cast from seconds
        /// </summary>
        /// <param name="seconds">Seconds</param>
        public static explicit operator UnsignedUnixTime(ulong seconds) => new UnsignedUnixTime((uint)seconds);

        /// <summary>
        /// Cast from seconds
        /// </summary>
        /// <param name="seconds">Seconds</param>
        public static explicit operator UnsignedUnixTime(double seconds) => new UnsignedUnixTime((uint)seconds);

        /// <summary>
        /// Cast from string
        /// </summary>
        /// <param name="str">String</param>
        public static explicit operator UnsignedUnixTime(string str) => FromString(str);
        #endregion

        #region Addition operators
        /// <summary>
        /// Summarize
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Timespan</param>
        /// <returns>New unix time</returns>
        public static UnsignedUnixTime operator +(UnsignedUnixTime a, TimeSpan b) => new UnsignedUnixTime((uint)(a.Seconds + b.TotalSeconds));

        /// <summary>
        /// Summarize
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>New unix time</returns>
        public static UnsignedUnixTime operator +(UnsignedUnixTime a, int b) => new UnsignedUnixTime((uint)(a.Seconds + b));

        /// <summary>
        /// Summarize
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>New unix time</returns>
        public static UnsignedUnixTime operator +(UnsignedUnixTime a, uint b) => new UnsignedUnixTime(a.Seconds + b);

        /// <summary>
        /// Summarize
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>New unix time</returns>
        public static UnsignedUnixTime operator +(UnsignedUnixTime a, long b) => new UnsignedUnixTime((uint)(a.Seconds + b));

        /// <summary>
        /// Summarize
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>New unix time</returns>
        public static UnsignedUnixTime operator +(UnsignedUnixTime a, ulong b) => new UnsignedUnixTime((uint)(a.Seconds + b));

        /// <summary>
        /// Summarize
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>New unix time</returns>
        public static UnsignedUnixTime operator +(UnsignedUnixTime a, double b) => new UnsignedUnixTime((uint)(a.Seconds + b));
        #endregion

        #region Substraction operators
        /// <summary>
        /// Substract
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Timespan</returns>
        public static TimeSpan operator -(UnsignedUnixTime a, UnixTime b) => TimeSpan.FromSeconds(a.Seconds - b.Seconds);

        /// <summary>
        /// Substract
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Timespan</returns>
        public static TimeSpan operator -(UnsignedUnixTime a, UnsignedUnixTime b) => TimeSpan.FromSeconds(a.Seconds - b.Seconds);

        /// <summary>
        /// Substract
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Timespan</returns>
        public static TimeSpan operator -(UnsignedUnixTime a, LongUnixTime b) => TimeSpan.FromSeconds(a.Seconds - b.Seconds);

        /// <summary>
        /// Substract
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Timespan</returns>
        public static TimeSpan operator -(UnsignedUnixTime a, UnsignedLongUnixTime b) => TimeSpan.FromSeconds(a.Seconds - b.Seconds);

        /// <summary>
        /// Substract
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Timespan</param>
        /// <returns>New unix time</returns>
        public static UnsignedUnixTime operator -(UnsignedUnixTime a, TimeSpan b) => new UnsignedUnixTime((uint)(a.Seconds - b.TotalSeconds));

        /// <summary>
        /// Substract
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>New unix time</returns>
        public static UnsignedUnixTime operator -(UnsignedUnixTime a, int b) => new UnsignedUnixTime((uint)(a.Seconds - b));

        /// <summary>
        /// Substract
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>New unix time</returns>
        public static UnsignedUnixTime operator -(UnsignedUnixTime a, uint b) => new UnsignedUnixTime(a.Seconds - b);

        /// <summary>
        /// Substract
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>New unix time</returns>
        public static UnsignedUnixTime operator -(UnsignedUnixTime a, long b) => new UnsignedUnixTime((uint)(a.Seconds - b));

        /// <summary>
        /// Substract
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>New unix time</returns>
        public static UnsignedUnixTime operator -(UnsignedUnixTime a, ulong b) => new UnsignedUnixTime((uint)(a.Seconds - b));

        /// <summary>
        /// Substract
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>New unix time</returns>
        public static UnsignedUnixTime operator -(UnsignedUnixTime a, double b) => new UnsignedUnixTime((uint)(a.Seconds - b));
        #endregion

        #region Comparsion operators
        /// <summary>
        /// Determine if greater than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Greater than?</returns>
        public static bool operator >(UnsignedUnixTime a, UnixTime b) => a.Seconds > b.Seconds;

        /// <summary>
        /// Determine if lower than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Lower than?</returns>
        public static bool operator <(UnsignedUnixTime a, UnixTime b) => a.Seconds < b.Seconds;

        /// <summary>
        /// Determine if greater than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Greater than?</returns>
        public static bool operator >=(UnsignedUnixTime a, UnixTime b) => a.Seconds >= b.Seconds;

        /// <summary>
        /// Determine if lower than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Lower than?</returns>
        public static bool operator <=(UnsignedUnixTime a, UnixTime b) => a.Seconds <= b.Seconds;

        /// <summary>
        /// Determine if greater than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Greater than?</returns>
        public static bool operator >(UnsignedUnixTime a, UnsignedUnixTime b) => a.Seconds > b.Seconds;

        /// <summary>
        /// Determine if lower than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Lower than?</returns>
        public static bool operator <(UnsignedUnixTime a, UnsignedUnixTime b) => a.Seconds < b.Seconds;

        /// <summary>
        /// Determine if greater than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Greater than?</returns>
        public static bool operator >=(UnsignedUnixTime a, UnsignedUnixTime b) => a.Seconds >= b.Seconds;

        /// <summary>
        /// Determine if lower than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Lower than?</returns>
        public static bool operator <=(UnsignedUnixTime a, UnsignedUnixTime b) => a.Seconds <= b.Seconds;

        /// <summary>
        /// Determine if greater than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Greater than?</returns>
        public static bool operator >(UnsignedUnixTime a, LongUnixTime b) => a.Seconds > b.Seconds;

        /// <summary>
        /// Determine if lower than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Lower than?</returns>
        public static bool operator <(UnsignedUnixTime a, LongUnixTime b) => a.Seconds < b.Seconds;

        /// <summary>
        /// Determine if greater than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Greater than?</returns>
        public static bool operator >=(UnsignedUnixTime a, LongUnixTime b) => a.Seconds >= b.Seconds;

        /// <summary>
        /// Determine if lower than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Lower than?</returns>
        public static bool operator <=(UnsignedUnixTime a, LongUnixTime b) => a.Seconds <= b.Seconds;

        /// <summary>
        /// Determine if greater than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Greater than?</returns>
        public static bool operator >(UnsignedUnixTime a, UnsignedLongUnixTime b) => a.Seconds > b.Seconds;

        /// <summary>
        /// Determine if lower than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Lower than?</returns>
        public static bool operator <(UnsignedUnixTime a, UnsignedLongUnixTime b) => a.Seconds < b.Seconds;

        /// <summary>
        /// Determine if greater than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Greater than?</returns>
        public static bool operator >=(UnsignedUnixTime a, UnsignedLongUnixTime b) => a.Seconds >= b.Seconds;

        /// <summary>
        /// Determine if lower than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Lower than?</returns>
        public static bool operator <=(UnsignedUnixTime a, UnsignedLongUnixTime b) => a.Seconds <= b.Seconds;

        /// <summary>
        /// Determine if greater than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Greater than?</returns>
        public static bool operator >(UnsignedUnixTime a, int b) => a.Seconds > b;

        /// <summary>
        /// Determine if lower than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Lower than?</returns>
        public static bool operator <(UnsignedUnixTime a, int b) => a.Seconds < b;

        /// <summary>
        /// Determine if greater than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Greater than?</returns>
        public static bool operator >=(UnsignedUnixTime a, int b) => a.Seconds >= b;

        /// <summary>
        /// Determine if lower than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Lower than?</returns>
        public static bool operator <=(UnsignedUnixTime a, int b) => a.Seconds <= b;

        /// <summary>
        /// Determine if greater than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Greater than?</returns>
        public static bool operator >(UnsignedUnixTime a, uint b) => a.Seconds > b;

        /// <summary>
        /// Determine if lower than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Lower than?</returns>
        public static bool operator <(UnsignedUnixTime a, uint b) => a.Seconds < b;

        /// <summary>
        /// Determine if greater than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Greater than?</returns>
        public static bool operator >=(UnsignedUnixTime a, uint b) => a.Seconds >= b;

        /// <summary>
        /// Determine if lower than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Lower than?</returns>
        public static bool operator <=(UnsignedUnixTime a, uint b) => a.Seconds <= b;

        /// <summary>
        /// Determine if greater than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Greater than?</returns>
        public static bool operator >(UnsignedUnixTime a, long b) => a.Seconds > b;

        /// <summary>
        /// Determine if lower than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Lower than?</returns>
        public static bool operator <(UnsignedUnixTime a, long b) => a.Seconds < b;

        /// <summary>
        /// Determine if greater than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Greater than?</returns>
        public static bool operator >=(UnsignedUnixTime a, long b) => a.Seconds >= b;

        /// <summary>
        /// Determine if lower than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Lower than?</returns>
        public static bool operator <=(UnsignedUnixTime a, long b) => a.Seconds <= b;

        /// <summary>
        /// Determine if greater than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Greater than?</returns>
        public static bool operator >(UnsignedUnixTime a, ulong b) => a.Seconds > b;

        /// <summary>
        /// Determine if lower than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Lower than?</returns>
        public static bool operator <(UnsignedUnixTime a, ulong b) => a.Seconds < b;

        /// <summary>
        /// Determine if greater than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Greater than?</returns>
        public static bool operator >=(UnsignedUnixTime a, ulong b) => a.Seconds >= b;

        /// <summary>
        /// Determine if lower than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Lower than?</returns>
        public static bool operator <=(UnsignedUnixTime a, ulong b) => a.Seconds <= b;

        /// <summary>
        /// Determine if greater than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Greater than?</returns>
        public static bool operator >(UnsignedUnixTime a, double b) => a.Seconds > b;

        /// <summary>
        /// Determine if lower than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Lower than?</returns>
        public static bool operator <(UnsignedUnixTime a, double b) => a.Seconds < b;

        /// <summary>
        /// Determine if greater than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Greater than?</returns>
        public static bool operator >=(UnsignedUnixTime a, double b) => a.Seconds >= b;

        /// <summary>
        /// Determine if lower than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Lower than?</returns>
        public static bool operator <=(UnsignedUnixTime a, double b) => a.Seconds <= b;

        /// <summary>
        /// Determine if equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Is equal?</returns>
        public static bool operator ==(UnsignedUnixTime a, UnixTime b) => a.Seconds == b.Seconds;

        /// <summary>
        /// Determine if not equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Is not equal?</returns>
        public static bool operator !=(UnsignedUnixTime a, UnixTime b) => a.Seconds != b.Seconds;

        /// <summary>
        /// Determine if equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Is equal?</returns>
        public static bool operator ==(UnsignedUnixTime a, UnsignedUnixTime b) => a.Seconds == b.Seconds;

        /// <summary>
        /// Determine if not equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Is not equal?</returns>
        public static bool operator !=(UnsignedUnixTime a, UnsignedUnixTime b) => a.Seconds != b.Seconds;

        /// <summary>
        /// Determine if equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Is equal?</returns>
        public static bool operator ==(UnsignedUnixTime a, LongUnixTime b) => a.Seconds == b.Seconds;

        /// <summary>
        /// Determine if not equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Is not equal?</returns>
        public static bool operator !=(UnsignedUnixTime a, LongUnixTime b) => a.Seconds != b.Seconds;

        /// <summary>
        /// Determine if equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Is equal?</returns>
        public static bool operator ==(UnsignedUnixTime a, UnsignedLongUnixTime b) => a.Seconds == b.Seconds;

        /// <summary>
        /// Determine if not equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Is not equal?</returns>
        public static bool operator !=(UnsignedUnixTime a, UnsignedLongUnixTime b) => a.Seconds != b.Seconds;

        /// <summary>
        /// Determine if equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Is equal?</returns>
        public static bool operator ==(UnsignedUnixTime a, int b) => a.Seconds == b;

        /// <summary>
        /// Determine if not equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Is not equal?</returns>
        public static bool operator !=(UnsignedUnixTime a, int b) => a.Seconds != b;

        /// <summary>
        /// Determine if equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Is equal?</returns>
        public static bool operator ==(UnsignedUnixTime a, uint b) => a.Seconds == b;

        /// <summary>
        /// Determine if not equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Is not equal?</returns>
        public static bool operator !=(UnsignedUnixTime a, uint b) => a.Seconds != b;

        /// <summary>
        /// Determine if equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Is equal?</returns>
        public static bool operator ==(UnsignedUnixTime a, long b) => a.Seconds == b;

        /// <summary>
        /// Determine if not equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Is not equal?</returns>
        public static bool operator !=(UnsignedUnixTime a, long b) => a.Seconds != b;

        /// <summary>
        /// Determine if equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Is equal?</returns>
        public static bool operator ==(UnsignedUnixTime a, ulong b) => a.Seconds == b;

        /// <summary>
        /// Determine if not equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Is not equal?</returns>
        public static bool operator !=(UnsignedUnixTime a, ulong b) => a.Seconds != b;

        /// <summary>
        /// Determine if equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Is equal?</returns>
        public static bool operator ==(UnsignedUnixTime a, double b) => a.Seconds == b;

        /// <summary>
        /// Determine if not equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Is not equal?</returns>
        public static bool operator !=(UnsignedUnixTime a, double b) => a.Seconds != b;
        #endregion

        #region Other operators
        /// <summary>
        /// Divide
        /// </summary>
        /// <param name="unixTime">Unix time</param>
        /// <param name="divider">Divider</param>
        /// <returns>New unix time</returns>
        public static UnsignedUnixTime operator /(UnsignedUnixTime unixTime, uint divider) => new UnsignedUnixTime((uint)Math.Round((double)unixTime.Seconds / divider));

        /// <summary>
        /// Divide
        /// </summary>
        /// <param name="unixTime">Unix time</param>
        /// <param name="divider">Divider</param>
        /// <returns>New unix time</returns>
        public static UnsignedUnixTime operator /(UnsignedUnixTime unixTime, float divider) => new UnsignedUnixTime((uint)Math.Round((double)unixTime.Seconds / divider));

        /// <summary>
        /// Multiplicate
        /// </summary>
        /// <param name="unixTime">Unix time</param>
        /// <param name="multiplicator">Multiplicator</param>
        /// <returns>New unix time</returns>
        public static UnsignedUnixTime operator *(UnsignedUnixTime unixTime, uint multiplicator) => new UnsignedUnixTime(unixTime.Seconds * multiplicator);

        /// <summary>
        /// Multiplicate
        /// </summary>
        /// <param name="unixTime">Unix time</param>
        /// <param name="multiplicator">Multiplicator</param>
        /// <returns>New unix time</returns>
        public static UnsignedUnixTime operator *(UnsignedUnixTime unixTime, float multiplicator) => new UnsignedUnixTime((uint)Math.Round(unixTime.Seconds * multiplicator));

        /// <summary>
        /// Modulus
        /// </summary>
        /// <param name="unixTime">Unix time</param>
        /// <param name="modulus">Modulus</param>
        /// <returns>Rest</returns>
        public static uint operator %(UnsignedUnixTime unixTime, uint modulus) => unixTime.Seconds % modulus;
        #endregion
    }
}
