using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace wan24.Linux
{
    /// <summary>
    /// Unix time
    /// 
    /// The structure represents an Unix timestamp as 64 bit integer.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 8)]
    public struct LongUnixTime : 
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
        IFormattable, 
        ISerializable
    {
        #region Fields
        /// <summary>
        /// Regular expression to match a 64 bit positive (or zero) integer in a string
        /// </summary>
        private static readonly Regex RX_NUMBER = new Regex(
            @"^(\d|[1-9]\d{1,14}|1000000000000000|10000000000000000|100000000000000000|[1-9]000000000000000000|9[0-2]00000000000000000|92[0-2]0000000000000000|922[0-3]000000000000000|9223[0-3]00000000000000|92233[0-7]0000000000000|922337[0-2]000000000000|92233720[0-3]0000000000|922337203[0-6]000000000|9223372036[0-8]00000000|92233720368[0-5]0000000|922337203685[0-4]000000|9223372036854[0-7]00000|92233720368547[0-7]0000|922337203685477[0-5]000|922337203685477[56]000)$",
            RegexOptions.Compiled
            );

        /// <summary>
        /// Min. value
        /// </summary>
        public static readonly LongUnixTime MinValue = new LongUnixTime();
        /// <summary>
        /// Max. value
        /// </summary>
        public static readonly LongUnixTime MaxValue = new LongUnixTime(long.MaxValue);

        /// <summary>
        /// UTC seconds since 1970-01-01 00:00.00
        /// </summary>
        [FieldOffset(0), JsonInclude]
        public readonly long Seconds;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="seconds">UTC seconds since 1970-01-01 00:00.00</param>
        public LongUnixTime(int seconds = 0)
        {
            if (seconds < 0) throw new ArgumentOutOfRangeException(nameof(seconds));
            Seconds = seconds;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="seconds">UTC seconds since 1970-01-01 00:00.00</param>
        public LongUnixTime(uint seconds = 0) => Seconds = seconds;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="seconds">UTC seconds since 1970-01-01 00:00.00</param>
        [JsonConstructor]
        public LongUnixTime(long seconds)
        {
            if (seconds < 0) throw new ArgumentOutOfRangeException(nameof(seconds));
            Seconds = seconds;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="seconds">UTC seconds since 1970-01-01 00:00.00</param>
        public LongUnixTime(ulong seconds = 0)
        {
            if (seconds > long.MaxValue) throw new ArgumentOutOfRangeException(nameof(seconds));
            Seconds = (long)seconds;
        }
        #endregion

        #region Static properties
        /// <summary>
        /// Now
        /// </summary>
        public static LongUnixTime Now => new LongUnixTime((long)(DateTime.UtcNow - UnixTime.UnixEpoch).TotalSeconds);

        /// <summary>
        /// Today
        /// </summary>
        public static LongUnixTime Today => (LongUnixTime)DateTime.Today.ToUniversalTime();
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
        public LongUnixTime AddSeconds(long seconds) => this + seconds;

        /// <summary>
        /// Add minutes
        /// </summary>
        /// <param name="minutes">Minutes</param>
        /// <returns>New Unix time</returns>
        public LongUnixTime AddMinutes(long minutes) => this + minutes * UnixTime.ONE_MINUTE;

        /// <summary>
        /// Add hours
        /// </summary>
        /// <param name="hours">Hours</param>
        /// <returns>New Unix time</returns>
        public LongUnixTime AddHours(long hours) => this + hours * UnixTime.ONE_HOUR;

        /// <summary>
        /// Add days
        /// </summary>
        /// <param name="days">Days</param>
        /// <returns>New Unix time</returns>
        public LongUnixTime AddDays(long days) => this + days * UnixTime.ONE_DAY;

        /// <summary>
        /// Add months
        /// </summary>
        /// <param name="months">Months</param>
        /// <returns>New Unix time</returns>
        public LongUnixTime AddMonths(int months) => (LongUnixTime)AsDateTimeUtc.AddMonths(months);

        /// <summary>
        /// Add years
        /// </summary>
        /// <param name="years">Years</param>
        /// <returns>New Unix time</returns>
        public LongUnixTime AddYears(int years) => (LongUnixTime)AsDateTimeUtc.AddYears(years);
        #endregion

        #region Overrides
        /// <inheritdoc/>
        public override int GetHashCode() => Seconds.GetHashCode();

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is IUInt64Value uivalue && uivalue.Value == (ulong)Seconds;

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
        bool IEquatable<DateTime>.Equals(DateTime other)
        {
            if (other.Kind == DateTimeKind.Local) other = other.ToUniversalTime();
            return Seconds.Equals((UnixTime.UnixEpoch - other).TotalSeconds);
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
            if (conversionType == typeof(UnixTime)) return new UnixTime((int)Seconds);
            if (conversionType == typeof(UnsignedUnixTime)) return new UnsignedUnixTime((uint)Seconds);
            if (conversionType == typeof(LongUnixTime)) return this;
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

        /// <inheritdoc/>
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) => info.AddValue(nameof(Seconds), (ulong)Seconds);
        #endregion

        #region Static methods
        /// <summary>
        /// Get from DateTime
        /// </summary>
        /// <param name="dateTime">DateTime</param>
        /// <returns>Unix time</returns>
        public static LongUnixTime FromDateTime(DateTime dateTime) => new LongUnixTime((long)(dateTime.ToUniversalTime() - UnixTime.UnixEpoch).TotalSeconds);

        /// <summary>
        /// Get from UTC DateTime
        /// </summary>
        /// <param name="dateTime">UTC DateTime</param>
        /// <returns>Unix time</returns>
        public static LongUnixTime FromDateTimeUtc(DateTime dateTime) => new LongUnixTime((long)(dateTime - UnixTime.UnixEpoch).TotalSeconds);

        /// <summary>
        /// Get from a string
        /// </summary>
        /// <param name="str">String</param>
        /// <returns>Unix time</returns>
        public static LongUnixTime FromString(string str)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));
            if (!RX_NUMBER.IsMatch(str)) throw new ArgumentException("Invalid number format", nameof(str));
            return new LongUnixTime(long.Parse(str));
        }

        /// <summary>
        /// Parse from a string
        /// </summary>
        /// <param name="str">String</param>
        /// <returns>Unix time</returns>
        public static LongUnixTime Parse(string str) => FromString(str);

        /// <summary>
        /// Try parsing a string
        /// </summary>
        /// <param name="str">String</param>
        /// <param name="unixTime">Unix time</param>
        /// <returns>Succeed?</returns>
        public static bool TryParse(string str, out LongUnixTime unixTime)
        {
            if (string.IsNullOrWhiteSpace(str) || !RX_NUMBER.IsMatch(str))
            {
                unixTime = default;
                return false;
            }
            unixTime = new LongUnixTime(long.Parse(str));
            return true;
        }
        #endregion

        #region Implicit operators
        /// <summary>
        /// Cast to string
        /// </summary>
        /// <param name="unixTime">Unix time</param>
        public static implicit operator string(LongUnixTime unixTime) => unixTime.ToString();

        /// <summary>
        /// Cast to DateTime
        /// </summary>
        /// <param name="unixTime">Unix time</param>
        public static implicit operator DateTime(LongUnixTime unixTime) => unixTime.AsDateTime;

        /// <summary>
        /// Cast to Unix time
        /// </summary>
        /// <param name="unixTime">Unix time</param>
        public static implicit operator UnixTime(LongUnixTime unixTime) => new UnixTime((int)unixTime.Seconds);

        /// <summary>
        /// Cast to Unix time
        /// </summary>
        /// <param name="unixTime">Unix time</param>
        public static implicit operator UnsignedUnixTime(LongUnixTime unixTime) => new UnsignedUnixTime((uint)unixTime.Seconds);

        /// <summary>
        /// Cast to Unix time
        /// </summary>
        /// <param name="unixTime">Unix time</param>
        public static implicit operator UnsignedLongUnixTime(LongUnixTime unixTime) => new UnsignedLongUnixTime((ulong)unixTime.Seconds);

        /// <summary>
        /// Cast as integer
        /// </summary>
        /// <param name="unixTime">Unix time</param>
        public static implicit operator int(LongUnixTime unixTime) => (int)unixTime.Seconds;

        /// <summary>
        /// Cast as integer
        /// </summary>
        /// <param name="unixTime">Unix time</param>
        public static implicit operator uint(LongUnixTime unixTime) => (uint)unixTime.Seconds;

        /// <summary>
        /// Cast as integer
        /// </summary>
        /// <param name="unixTime">Unix time</param>
        public static implicit operator long(LongUnixTime unixTime) => unixTime.Seconds;

        /// <summary>
        /// Cast as integer
        /// </summary>
        /// <param name="unixTime">Unix time</param>
        public static implicit operator ulong(LongUnixTime unixTime) => (ulong)unixTime.Seconds;

        /// <summary>
        /// Cast as integer
        /// </summary>
        /// <param name="unixTime">Unix time</param>
        public static implicit operator double(LongUnixTime unixTime) => unixTime.Seconds;
        #endregion

        #region Explicit operators
        /// <summary>
        /// Cast from DateTime
        /// </summary>
        /// <param name="dateTime">DateTime</param>
        public static explicit operator LongUnixTime(DateTime dateTime) => dateTime.Kind == DateTimeKind.Local ? FromDateTime(dateTime) : FromDateTimeUtc(dateTime);

        /// <summary>
        /// Cast from seconds
        /// </summary>
        /// <param name="seconds">Seconds</param>
        public static explicit operator LongUnixTime(int seconds) => new LongUnixTime(seconds);

        /// <summary>
        /// Cast from seconds
        /// </summary>
        /// <param name="seconds">Seconds</param>
        public static explicit operator LongUnixTime(uint seconds) => new LongUnixTime(seconds);

        /// <summary>
        /// Cast from seconds
        /// </summary>
        /// <param name="seconds">Seconds</param>
        public static explicit operator LongUnixTime(long seconds) => new LongUnixTime(seconds);

        /// <summary>
        /// Cast from seconds
        /// </summary>
        /// <param name="seconds">Seconds</param>
        public static explicit operator LongUnixTime(ulong seconds) => new LongUnixTime((long)seconds);

        /// <summary>
        /// Cast from seconds
        /// </summary>
        /// <param name="seconds">Seconds</param>
        public static explicit operator LongUnixTime(double seconds) => new LongUnixTime((long)seconds);

        /// <summary>
        /// Cast from string
        /// </summary>
        /// <param name="str">String</param>
        public static explicit operator LongUnixTime(string str) => FromString(str);
        #endregion

        #region Addition operators
        /// <summary>
        /// Summarize
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Timespan</param>
        /// <returns>New unix time</returns>
        public static LongUnixTime operator +(LongUnixTime a, TimeSpan b) => new LongUnixTime((long)(a.Seconds + b.TotalSeconds));

        /// <summary>
        /// Summarize
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>New unix time</returns>
        public static LongUnixTime operator +(LongUnixTime a, int b) => new LongUnixTime(a.Seconds + b);

        /// <summary>
        /// Summarize
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>New unix time</returns>
        public static LongUnixTime operator +(LongUnixTime a, uint b) => new LongUnixTime(a.Seconds + b);

        /// <summary>
        /// Summarize
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>New unix time</returns>
        public static LongUnixTime operator +(LongUnixTime a, long b) => new LongUnixTime(a.Seconds + b);

        /// <summary>
        /// Summarize
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>New unix time</returns>
        public static LongUnixTime operator +(LongUnixTime a, ulong b) => new LongUnixTime((long)((ulong)a.Seconds + b));

        /// <summary>
        /// Summarize
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>New unix time</returns>
        public static LongUnixTime operator +(LongUnixTime a, double b) => new LongUnixTime((long)(a.Seconds + b));
        #endregion

        #region Substraction operators
        /// <summary>
        /// Substract
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Timespan</param>
        /// <returns>Timespan</returns>
        public static LongUnixTime operator -(LongUnixTime a, TimeSpan b) => new LongUnixTime((long)(a.Seconds - b.TotalSeconds));

        /// <summary>
        /// Substract
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Timespan</returns>
        public static TimeSpan operator -(LongUnixTime a, UnixTime b) => TimeSpan.FromSeconds(a.Seconds - b.Seconds);

        /// <summary>
        /// Substract
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Timespan</returns>
        public static TimeSpan operator -(LongUnixTime a, UnsignedUnixTime b) => TimeSpan.FromSeconds(a.Seconds - b.Seconds);

        /// <summary>
        /// Substract
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Timespan</returns>
        public static TimeSpan operator -(LongUnixTime a, LongUnixTime b) => TimeSpan.FromSeconds(a.Seconds - b.Seconds);

        /// <summary>
        /// Substract
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Timespan</returns>
        public static TimeSpan operator -(LongUnixTime a, UnsignedLongUnixTime b) => TimeSpan.FromSeconds(a.Seconds - (double)b.Seconds);

        /// <summary>
        /// Substract
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Timespan</returns>
        public static LongUnixTime operator -(LongUnixTime a, int b) => new LongUnixTime(a.Seconds - b);

        /// <summary>
        /// Substract
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Timespan</returns>
        public static LongUnixTime operator -(LongUnixTime a, uint b) => new LongUnixTime(a.Seconds - b);

        /// <summary>
        /// Substract
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Timespan</returns>
        public static LongUnixTime operator -(LongUnixTime a, long b) => new LongUnixTime(a.Seconds - b);

        /// <summary>
        /// Substract
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Timespan</returns>
        public static LongUnixTime operator -(LongUnixTime a, ulong b) => new LongUnixTime((long)((ulong)a.Seconds - b));

        /// <summary>
        /// Substract
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Timespan</returns>
        public static LongUnixTime operator -(LongUnixTime a, double b) => new LongUnixTime((long)(a.Seconds - b));
        #endregion

        #region Comparsion operators
        /// <summary>
        /// Determine if greater than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Greater than?</returns>
        public static bool operator >(LongUnixTime a, UnixTime b) => a.Seconds > b.Seconds;

        /// <summary>
        /// Determine if lower than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Lower than?</returns>
        public static bool operator <(LongUnixTime a, UnixTime b) => a.Seconds < b.Seconds;

        /// <summary>
        /// Determine if greater than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Greater than?</returns>
        public static bool operator >=(LongUnixTime a, UnixTime b) => a.Seconds >= b.Seconds;

        /// <summary>
        /// Determine if lower than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Lower than?</returns>
        public static bool operator <=(LongUnixTime a, UnixTime b) => a.Seconds <= b.Seconds;

        /// <summary>
        /// Determine if greater than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Greater than?</returns>
        public static bool operator >(LongUnixTime a, UnsignedUnixTime b) => a.Seconds > b.Seconds;

        /// <summary>
        /// Determine if lower than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Lower than?</returns>
        public static bool operator <(LongUnixTime a, UnsignedUnixTime b) => a.Seconds < b.Seconds;

        /// <summary>
        /// Determine if greater than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Greater than?</returns>
        public static bool operator >=(LongUnixTime a, UnsignedUnixTime b) => a.Seconds >= b.Seconds;

        /// <summary>
        /// Determine if lower than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Lower than?</returns>
        public static bool operator <=(LongUnixTime a, UnsignedUnixTime b) => a.Seconds <= b.Seconds;

        /// <summary>
        /// Determine if greater than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Greater than?</returns>
        public static bool operator >(LongUnixTime a, LongUnixTime b) => a.Seconds > b.Seconds;

        /// <summary>
        /// Determine if lower than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Lower than?</returns>
        public static bool operator <(LongUnixTime a, LongUnixTime b) => a.Seconds < b.Seconds;

        /// <summary>
        /// Determine if greater than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Greater than?</returns>
        public static bool operator >=(LongUnixTime a, LongUnixTime b) => a.Seconds >= b.Seconds;

        /// <summary>
        /// Determine if lower than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Lower than?</returns>
        public static bool operator <=(LongUnixTime a, LongUnixTime b) => a.Seconds <= b.Seconds;

        /// <summary>
        /// Determine if greater than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Greater than?</returns>
        public static bool operator >(LongUnixTime a, UnsignedLongUnixTime b) => (ulong)a.Seconds > b.Seconds;

        /// <summary>
        /// Determine if lower than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Lower than?</returns>
        public static bool operator <(LongUnixTime a, UnsignedLongUnixTime b) => (ulong)a.Seconds < b.Seconds;

        /// <summary>
        /// Determine if greater than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Greater than?</returns>
        public static bool operator >=(LongUnixTime a, UnsignedLongUnixTime b) => (ulong)a.Seconds >= b.Seconds;

        /// <summary>
        /// Determine if lower than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Lower than?</returns>
        public static bool operator <=(LongUnixTime a, UnsignedLongUnixTime b) => (ulong)a.Seconds <= b.Seconds;

        /// <summary>
        /// Determine if greater than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Greater than?</returns>
        public static bool operator >(LongUnixTime a, int b) => a.Seconds > b;

        /// <summary>
        /// Determine if lower than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Lower than?</returns>
        public static bool operator <(LongUnixTime a, int b) => a.Seconds < b;

        /// <summary>
        /// Determine if greater than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Greater than?</returns>
        public static bool operator >=(LongUnixTime a, int b) => a.Seconds >= b;

        /// <summary>
        /// Determine if lower than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Lower than?</returns>
        public static bool operator <=(LongUnixTime a, int b) => a.Seconds <= b;

        /// <summary>
        /// Determine if greater than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Greater than?</returns>
        public static bool operator >(LongUnixTime a, uint b) => a.Seconds > b;

        /// <summary>
        /// Determine if lower than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Lower than?</returns>
        public static bool operator <(LongUnixTime a, uint b) => a.Seconds < b;

        /// <summary>
        /// Determine if greater than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Greater than?</returns>
        public static bool operator >=(LongUnixTime a, uint b) => a.Seconds >= b;

        /// <summary>
        /// Determine if lower than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Lower than?</returns>
        public static bool operator <=(LongUnixTime a, uint b) => a.Seconds <= b;

        /// <summary>
        /// Determine if greater than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Greater than?</returns>
        public static bool operator >(LongUnixTime a, long b) => a.Seconds > b;

        /// <summary>
        /// Determine if lower than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Lower than?</returns>
        public static bool operator <(LongUnixTime a, long b) => a.Seconds < b;

        /// <summary>
        /// Determine if greater than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Greater than?</returns>
        public static bool operator >=(LongUnixTime a, long b) => a.Seconds >= b;

        /// <summary>
        /// Determine if lower than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Lower than?</returns>
        public static bool operator <=(LongUnixTime a, long b) => a.Seconds <= b;

        /// <summary>
        /// Determine if greater than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Greater than?</returns>
        public static bool operator >(LongUnixTime a, ulong b) => (ulong)a.Seconds > b;

        /// <summary>
        /// Determine if lower than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Lower than?</returns>
        public static bool operator <(LongUnixTime a, ulong b) => (ulong)a.Seconds < b;

        /// <summary>
        /// Determine if greater than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Greater than?</returns>
        public static bool operator >(LongUnixTime a, double b) => (ulong)a.Seconds > b;

        /// <summary>
        /// Determine if lower than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Lower than?</returns>
        public static bool operator <(LongUnixTime a, double b) => (ulong)a.Seconds < b;

        /// <summary>
        /// Determine if greater than or equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Greater than?</returns>
        public static bool operator >=(LongUnixTime a, double b) => (ulong)a.Seconds >= b;

        /// <summary>
        /// Determine if lower than
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Lower than?</returns>
        public static bool operator <=(LongUnixTime a, double b) => (ulong)a.Seconds <= b;

        /// <summary>
        /// Determine if equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Is equal?</returns>
        public static bool operator ==(LongUnixTime a, UnixTime b) => a.Seconds == b.Seconds;

        /// <summary>
        /// Determine if not equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Is not equal?</returns>
        public static bool operator !=(LongUnixTime a, UnixTime b) => a.Seconds != b.Seconds;

        /// <summary>
        /// Determine if equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Is equal?</returns>
        public static bool operator ==(LongUnixTime a, UnsignedUnixTime b) => a.Seconds == b.Seconds;

        /// <summary>
        /// Determine if not equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Is not equal?</returns>
        public static bool operator !=(LongUnixTime a, UnsignedUnixTime b) => a.Seconds != b.Seconds;

        /// <summary>
        /// Determine if equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Is equal?</returns>
        public static bool operator ==(LongUnixTime a, LongUnixTime b) => a.Seconds == b.Seconds;

        /// <summary>
        /// Determine if not equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Is not equal?</returns>
        public static bool operator !=(LongUnixTime a, LongUnixTime b) => a.Seconds != b.Seconds;

        /// <summary>
        /// Determine if equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Is equal?</returns>
        public static bool operator ==(LongUnixTime a, UnsignedLongUnixTime b) => (ulong)a.Seconds == b.Seconds;

        /// <summary>
        /// Determine if not equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Unix time B</param>
        /// <returns>Is not equal?</returns>
        public static bool operator !=(LongUnixTime a, UnsignedLongUnixTime b) => (ulong)a.Seconds != b.Seconds;

        /// <summary>
        /// Determine if equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Is equal?</returns>
        public static bool operator ==(LongUnixTime a, int b) => a.Seconds == b;

        /// <summary>
        /// Determine if not equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Is not equal?</returns>
        public static bool operator !=(LongUnixTime a, int b) => a.Seconds != b;

        /// <summary>
        /// Determine if equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Is equal?</returns>
        public static bool operator ==(LongUnixTime a, uint b) => a.Seconds == b;

        /// <summary>
        /// Determine if not equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Is not equal?</returns>
        public static bool operator !=(LongUnixTime a, uint b) => a.Seconds != b;

        /// <summary>
        /// Determine if equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Is equal?</returns>
        public static bool operator ==(LongUnixTime a, long b) => a.Seconds == b;

        /// <summary>
        /// Determine if not equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Is not equal?</returns>
        public static bool operator !=(LongUnixTime a, long b) => a.Seconds != b;

        /// <summary>
        /// Determine if equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Is equal?</returns>
        public static bool operator ==(LongUnixTime a, ulong b) => (ulong)a.Seconds == b;

        /// <summary>
        /// Determine if not equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Is not equal?</returns>
        public static bool operator !=(LongUnixTime a, ulong b) => (ulong)a.Seconds != b;

        /// <summary>
        /// Determine if equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Is equal?</returns>
        public static bool operator ==(LongUnixTime a, double b) => a.Seconds == b;

        /// <summary>
        /// Determine if not equal
        /// </summary>
        /// <param name="a">Unix time A</param>
        /// <param name="b">Seconds</param>
        /// <returns>Is not equal?</returns>
        public static bool operator !=(LongUnixTime a, double b) => a.Seconds != b;
        #endregion

        #region Other operators
        /// <summary>
        /// Divide
        /// </summary>
        /// <param name="unixTime">Unix time</param>
        /// <param name="divisor">Divisor</param>
        /// <returns>New unix time</returns>
        public static LongUnixTime operator /(LongUnixTime unixTime, long divisor) => new LongUnixTime((long)Math.Round((double)unixTime.Seconds / divisor));

        /// <summary>
        /// Divide
        /// </summary>
        /// <param name="unixTime">Unix time</param>
        /// <param name="divisor">Divisor</param>
        /// <returns>New unix time</returns>
        public static LongUnixTime operator /(LongUnixTime unixTime, double divisor) => new LongUnixTime((long)Math.Round(unixTime.Seconds / divisor));

        /// <summary>
        /// Multiplicate
        /// </summary>
        /// <param name="unixTime">Unix time</param>
        /// <param name="multiplicator">Multiplicator</param>
        /// <returns>New unix time</returns>
        public static LongUnixTime operator *(LongUnixTime unixTime, long multiplicator) => new LongUnixTime(unixTime.Seconds * multiplicator);

        /// <summary>
        /// Multiplicate
        /// </summary>
        /// <param name="unixTime">Unix time</param>
        /// <param name="multiplicator">Multiplicator</param>
        /// <returns>New unix time</returns>
        public static LongUnixTime operator *(LongUnixTime unixTime, double multiplicator) => new LongUnixTime((long)Math.Round(unixTime.Seconds * multiplicator));

        /// <summary>
        /// Modulus
        /// </summary>
        /// <param name="unixTime">Unix time</param>
        /// <param name="modulus">Modulus</param>
        /// <returns>Rest</returns>
        public static long operator %(LongUnixTime unixTime, long modulus) => unixTime.Seconds % modulus;
        #endregion
    }
}
