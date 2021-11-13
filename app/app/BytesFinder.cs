using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Linq.Expressions;

#pragma warning disable CA1303
namespace byteapp
{
        public class BytesFinder
        {


            private readonly PatternInfo _mPattInfo;

            public BytesFinder(byte[] pattern)
            {
                if (pattern == null || pattern.Length == 0) throw new ArgumentException("pattern is null or empty.", nameof(pattern));
                _mPattInfo = new PatternInfo(pattern);
            }


            public BytesFinder(string pattern)
            {
                if (pattern == null) throw new ArgumentNullException(nameof(pattern), "pattern is null.");
                _mPattInfo = new PatternInfo(pattern.Replace(" ", string.Empty)); //remove placeholder
            }

            #region instance methods

            /// <summary>
            /// Reports the zero-based index of the first occurrence of the pattern in the specified bytes.
            /// </summary>
            /// <param name="source">The bytes to search for an occurrence.</param>
            /// <returns>The zero-based index position of the occurrence if the pattern is found, otherwise, -1.</returns>
            /// <exception cref="ArgumentException"><paramref name="source"/> is null or empty.</exception>
            public int FindIndexIn(byte[] source)
            {
                if (source == null || source.Length == 0) throw new ArgumentException("source is null or empty.", nameof(source));
                return InnerFindIndex(source, in _mPattInfo, 0, source.Length);
            }

            /// <summary>
            /// Reports the zero-based index of the first occurrence of the pattern in the specified bytes.
            /// The search starts at the specified position.
            /// </summary>
            /// <param name="source">The bytes to search for an occurrence.</param>
            /// <param name="startIndex">The search starting position.</param>
            /// <returns>The zero-based index position of the occurrence if the pattern is found, otherwise, -1.</returns>
            /// <exception cref="ArgumentException"><paramref name="source"/> is null or empty.</exception>
            /// <exception cref="ArgumentOutOfRangeException">
            /// <paramref name="startIndex"/> is less than 0.
            /// <para>- Or -</para>
            /// <paramref name="startIndex"/> is greater than or equal to the length of <paramref name="source"/>.
            /// </exception>
            public int FindIndexIn(byte[] source, int startIndex)
            {
                Ensure_source_startIndex(source, startIndex);
                return InnerFindIndex(source, in _mPattInfo, startIndex, source.Length);
            }

            /// <summary>
            /// Reports the zero-based index of the first occurrence of the pattern in the specified bytes.
            /// The search starts at the specified position and examines a specified number of <see cref="byte"/> positions.
            /// </summary>
            /// <param name="source">The bytes to search for an occurrence.</param>
            /// <param name="startIndex">The search starting position.</param>
            /// <param name="count">The number of <see cref="byte"/> positions to examine.</param>
            /// <returns>The zero-based index position of the occurrence if the pattern is found, otherwise, -1.</returns>
            /// <exception cref="ArgumentException"><paramref name="source"/> is null or empty.</exception>
            /// <exception cref="ArgumentOutOfRangeException">
            /// <paramref name="startIndex"/> is less than 0.
            /// <para>- Or -</para>
            /// <paramref name="startIndex"/> is greater than or equal to the length of <paramref name="source"/>.
            /// <para>- Or -</para>
            /// <paramref name="count"/> is less than or equal to 0.
            /// <para>- Or -</para>
            /// <paramref name="count"/> is greater than the length of source minus <paramref name="startIndex"/>.
            /// </exception>
            public int FindIndexIn(byte[] source, int startIndex, int count)
            {
                Ensure_source_startIndex_count(source, startIndex, count);
                return InnerFindIndex(source, in _mPattInfo, startIndex, count);
            }

            #endregion

            #region static methods

            /// <summary>
            /// Reports the zero-based index of the first occurrence of the specified pattern in the specified bytes source.
            /// </summary>
            /// <param name="source">The bytes to search for an occurrence.</param>
            /// <param name="pattern">The bytes pattern to seek.</param>
            /// <returns>The zero-based index position of the occurrence if the <paramref name="pattern"/> is found, otherwise, -1.</returns>
            /// <exception cref="ArgumentException">
            /// <paramref name="source"/> is null or empty.
            /// <para>- Or -</para>
            /// <paramref name="pattern"/> is null or empty.
            /// </exception>
            public static int FindIndex(byte[] source, byte[] pattern)
            {
                if (source == null || source.Length == 0) throw new ArgumentException("source is null or empty.", nameof(source));
                if (pattern == null || pattern.Length == 0) throw new ArgumentException("pattern is null or empty.", nameof(pattern));
                using (var pattInfo = new PatternInfo(pattern))
                {
                    return InnerFindIndex(source, in pattInfo, 0, source.Length);
                }

            }

            /// <summary>
            /// Reports the zero-based index of the first occurrence of the specified pattern in the specified bytes source.
            /// The search starts at the specified position.
            /// </summary>
            /// <param name="source">The bytes to search for an occurrence.</param>
            /// <param name="pattern">The bytes pattern to seek.</param>
            /// <param name="startIndex">The search starting position.</param>
            /// <returns>The zero-based index position of the occurrence if the <paramref name="pattern"/> is found, otherwise, -1.</returns>
            /// <exception cref="ArgumentException">
            /// <paramref name="source"/> is null or empty.
            /// <para>- Or -</para>
            /// <paramref name="pattern"/> is null or empty.
            /// </exception>
            /// <exception cref="ArgumentOutOfRangeException">
            /// <paramref name="startIndex"/> is less than 0.
            /// <para>- Or -</para>
            /// <paramref name="startIndex"/> is greater than or equal to the length of <paramref name="source"/>.
            /// </exception>
            public static int FindIndex(byte[] source, byte[] pattern, int startIndex)
            {
                if (pattern == null || pattern.Length == 0) throw new ArgumentException("pattern is null or empty.", nameof(pattern));
                Ensure_source_startIndex(source, startIndex);
                using (var pattInfo = new PatternInfo(pattern))
                {
                    return InnerFindIndex(source, in pattInfo, startIndex, source.Length);
                }
            }

            /// <summary>
            /// Reports the zero-based index of the first occurrence of the specified pattern in the specified bytes source.
            /// The search starts at the specified position and examines a specified number of <see cref="byte"/> positions.
            /// </summary>
            /// <param name="source">The bytes to search for an occurrence.</param>
            /// <param name="pattern">The bytes pattern to seek.</param>
            /// <param name="startIndex">The search starting position.</param>
            /// <param name="count">The number of <see cref="byte"/> positions to examine.</param>
            /// <returns>The zero-based index position of the occurrence if the <paramref name="pattern"/> is found, otherwise, -1.</returns>
            /// <exception cref="ArgumentException">
            /// <paramref name="source"/> is null or empty.
            /// <para>- Or -</para>
            /// <paramref name="pattern"/> is null or empty.
            /// </exception>
            /// <exception cref="ArgumentOutOfRangeException">
            /// <paramref name="startIndex"/> is less than 0.
            /// <para>- Or -</para>
            /// <paramref name="startIndex"/> is greater than or equal to the length of <paramref name="source"/>.
            /// <para>- Or -</para>
            /// <paramref name="count"/> is less than or equal to 0.
            /// <para>- Or -</para>
            /// <paramref name="count"/> is greater than the length of source minus <paramref name="startIndex"/>.
            /// </exception>
            public static int FindIndex(byte[] source, byte[] pattern, int startIndex, int count)
            {
                if (pattern == null || pattern.Length == 0) throw new ArgumentException("pattern is null or empty.", nameof(pattern));
                Ensure_source_startIndex_count(source, startIndex, count);
                using (var pattInfo = new PatternInfo(pattern))
                {
                    return InnerFindIndex(source, in pattInfo, startIndex, count);
                }
            }


            /// <summary>
            /// Reports the zero-based index of the first occurrence of the specified pattern in the specified bytes source.
            /// </summary>
            /// <param name="source">The bytes to search for an occurrence.</param>
            /// <param name="pattern">The <see cref="string"/> pattern to seek.</param>
            /// <returns>The zero-based index position of the occurrence if the <paramref name="pattern"/> is found, otherwise, -1.</returns>
            /// <exception cref="ArgumentException"><paramref name="source"/> is null or empty.</exception>
            /// <exception cref="ArgumentNullException"><paramref name="pattern"/> is null.</exception>
            /// <exception cref="FormatException">
            /// The length of <paramref name="pattern"/> is 0 or not equal to this value division by 2.
            /// <para>- Or -</para>
            /// Unexpected char in <paramref name="pattern"/>.
            /// </exception>
            public static int FindIndex(byte[] source, string pattern)
            {
                if (source == null || source.Length == 0) throw new ArgumentException("source is null or empty.", nameof(source));
                return (new BytesFinder(pattern)).FindIndexIn(source);
            }

            /// <summary>
            /// Reports the zero-based index of the first occurrence of the specified pattern in the specified bytes source.
            /// The search starts at the specified position.
            /// </summary>
            /// <param name="source">The bytes to search for an occurrence.</param>
            /// <param name="pattern">The <see cref="string"/> pattern to seek.</param>
            /// <param name="startIndex">The search starting position.</param>
            /// <returns>The zero-based index position of the occurrence if the <paramref name="pattern"/> is found, otherwise, -1.</returns>
            /// <exception cref="ArgumentException"><paramref name="source"/> is null or empty.</exception>
            /// <exception cref="ArgumentNullException"><paramref name="pattern"/> is null.</exception>
            /// <exception cref="FormatException">
            /// The length of <paramref name="pattern"/> is 0 or not equal to this value division by 2.
            /// <para>- Or -</para>
            /// Unexpected char in <paramref name="pattern"/>.
            /// </exception>
            /// <exception cref="ArgumentOutOfRangeException">
            /// <paramref name="startIndex"/> is less than 0.
            /// <para>- Or -</para>
            /// <paramref name="startIndex"/> is greater than or equal to the length of <paramref name="source"/>.
            /// </exception>
            public static int FindIndex(byte[] source, string pattern, int startIndex)
            {
                Ensure_source_startIndex(source, startIndex);
                return (new BytesFinder(pattern)).FindIndexIn(source, startIndex);
            }

            /// <summary>
            /// Reports the zero-based index of the first occurrence of the specified pattern in the specified bytes source.
            /// The search starts at the specified position and examines a specified number of <see cref="byte"/> positions.
            /// </summary>
            /// <param name="source">The bytes to search for an occurrence.</param>
            /// <param name="pattern">The <see cref="string"/> pattern to seek.</param>
            /// <param name="startIndex">The search starting position.</param>
            /// <param name="count">The number of <see cref="byte"/> positions to examine.</param>
            /// <returns>The zero-based index position of the occurrence if the <paramref name="pattern"/> is found, otherwise, -1.</returns>
            /// <exception cref="ArgumentException"><paramref name="source"/> is null or empty.</exception>
            /// <exception cref="ArgumentNullException"><paramref name="pattern"/> is null.</exception>
            /// <exception cref="FormatException">
            /// The length of <paramref name="pattern"/> is 0 or not equal to this value division by 2.
            /// <para>- Or -</para>
            /// Unexpected char in <paramref name="pattern"/>.
            /// </exception>
            /// <exception cref="ArgumentOutOfRangeException">
            /// <paramref name="startIndex"/> is less than 0.
            /// <para>- Or -</para>
            /// <paramref name="startIndex"/> is greater than or equal to the length of <paramref name="source"/>.
            /// <para>- Or -</para>
            /// <paramref name="count"/> is less than or equal to 0.
            /// <para>- Or -</para>
            /// <paramref name="count"/> is greater than the length of source minus <paramref name="startIndex"/>.
            /// </exception>
            public static int FindIndex(byte[] source, string pattern, int startIndex, int count)
            {
                Ensure_source_startIndex_count(source, startIndex, count);
                return (new BytesFinder(pattern)).FindIndexIn(source, startIndex, count);
            }


            #endregion

            private static int InnerFindIndex(byte[] source, in PatternInfo pattern, int startIndex, int count)
            {
                var patternLength = pattern.PatternLength;
                var pattMaxIdx = patternLength - 1;
                var maxLen = count - patternLength + 1;
                unsafe
                {
                    fixed (int* next = pattern.MoveTable)
                    {
                        fixed (byte* src = source)
                        {
                            while (startIndex < maxLen)
                            {
                                var mov = next[src[startIndex + pattMaxIdx]];
                                if (mov < patternLength)
                                {
                                    startIndex += mov;
                                    if (pattern.CheckWithPattern(source, startIndex)) return startIndex;
                                    ++startIndex;
                                }
                                else
                                {
                                    startIndex += patternLength;
                                }
                            }
                            return -1;
                        }
                    }
                }
            }

            private static void Ensure_source_startIndex(byte[] source, int startIndex)
            {
                if (source == null || source.Length == 0) throw new ArgumentException("source is null or empty.", nameof(source));
                if (startIndex < 0) throw new ArgumentOutOfRangeException(nameof(startIndex), "startIndex is less than 0.");
                if (startIndex >= source.Length) throw new ArgumentOutOfRangeException(nameof(startIndex), "startIndex is greater than or equal to the length of source.");
            }

            private static void Ensure_source_startIndex_count(byte[] source, int startIndex, int count)
            {
                if (source == null || source.Length == 0) throw new ArgumentException("source is null or empty.", nameof(source));
                if (startIndex < 0) throw new ArgumentOutOfRangeException(nameof(startIndex), "startIndex is less than 0.");
                if (startIndex >= source.Length) throw new ArgumentOutOfRangeException(nameof(startIndex), "startIndex is greater than or equal to the length of source.");
                if (count <= 0) throw new ArgumentOutOfRangeException(nameof(startIndex), "count is less than or equal to 0.");
                if (count > source.Length - startIndex) throw new ArgumentOutOfRangeException(nameof(count), "count is greater than the length of source minus startIndex.");
            }

            private readonly struct PatternInfo : IDisposable
            {

                private static readonly ConcurrentBag<int[]> _MoveTablePool = new ConcurrentBag<int[]>();

                private delegate bool ComparePatternFunc(byte[] source, byte[] pattern, int index);

                #region expressions
                private static readonly ParameterExpression _ExpParamSource = Expression.Parameter(typeof(byte[]), "source");
                private static readonly ParameterExpression _ExpParamIndex = Expression.Parameter(typeof(int), "index");
                private static readonly Expression _ExpSourceMaxCount = Expression.Subtract(Expression.ArrayLength(_ExpParamSource), _ExpParamIndex);

                private static readonly ParameterExpression _ExpUnusedParamBytesPattern = Expression.Parameter(typeof(byte[]), "unusedBytesPattern");
                private static readonly BinaryExpression _ExpArrayItemIterator = Expression.ArrayIndex(_ExpParamSource, Expression.PostIncrementAssign(_ExpParamIndex));
                private static readonly BlockExpression _ExpUnconditionalTrue = Expression.Block(Expression.PreIncrementAssign(_ExpParamIndex), Expression.Constant(true, typeof(bool)));
                #endregion

                private readonly ComparePatternFunc Comparer;

                public readonly int PatternLength;

                public readonly byte[] BytesPattern;

                public readonly int[] MoveTable;

                public PatternInfo(byte[] pattern)
                {
                    PatternLength = pattern.Length;
                    BytesPattern = pattern;
                    MoveTable = GetTableFormBag(PatternLength);
                    var pattMaxIdx = PatternLength - 1;

                    unsafe
                    {
                        fixed (int* next = MoveTable)
                        {
                            fixed (byte* patt = pattern)
                            {
                                for (int i = 0; i < PatternLength; i++)
                                {
                                    next[patt[i]] = pattMaxIdx - i;
                                }
                            }
                        }
                    }

                    Comparer = new ComparePatternFunc(CompareCore);
                }

                public PatternInfo(string pattern)
                {
                    var strLen = pattern.Length;
                    if (strLen == 0 || (strLen & 1) == 1) throw new FormatException("The length of pattern is 0 or not equal to this value division by 2.");
                    var patternLength = strLen >> 1;
                    var maxMove = patternLength - 1;
                    var moveTable = GetTableFormBag(patternLength);

                    Expression exp = Expression.LessThanOrEqual(
                        Expression.Constant(patternLength, typeof(int)),
                        _ExpSourceMaxCount);

                    #region  generates move table and comparison expression
                    unsafe
                    {
                        fixed (int* next = moveTable)
                        {
                            fixed (char* patt = pattern)
                            {
                                var iPatt = (int*)patt;

                                var idx = 0;
                                while (idx < strLen)
                                {
                                    var badMove = maxMove - (idx >> 1);
                                    var currentChar = patt[idx++];
                                    var nextChar = patt[idx++];
                                    int nextDigit;
                                    if (currentChar == '?')
                                    {
                                        if (nextChar == '?') //??
                                        {
                                            SetMultiBadMove(next, badMove, 0, 1); //update move table
                                                                                  //update expression
                                            exp = Expression.AndAlso(exp, _ExpUnconditionalTrue);
                                        }
                                        else //?a
                                        {
                                            nextDigit = GetHexDigit(nextChar);
                                            SetMultiBadMove(next, badMove, nextDigit, 0x10); //update move table
                                            exp = MakeExpCmpDigit(exp, nextDigit, 0x0F); //update expression
                                        }
                                    }
                                    else
                                    {
                                        var firstDigit = GetHexDigit(currentChar) << 4;

                                        if (nextChar == '?') //a?
                                        {
                                            SetMultiBadMove(next, badMove, firstDigit, 1); //update move table
                                            exp = MakeExpCmpDigit(exp, firstDigit, 0xF0); //update expression
                                        }
                                        else //ab
                                        {
                                            nextDigit = GetHexDigit(nextChar);
                                            var hexNum = (byte)(firstDigit | nextDigit);
                                            next[hexNum] = badMove; //update move table
                                                                    //update expression
                                            exp = Expression.AndAlso(
                                                    exp,
                                                    Expression.Equal(
                                                        _ExpArrayItemIterator,
                                                        Expression.Constant(hexNum, typeof(byte))));
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    Comparer = Expression.Lambda<ComparePatternFunc>(
                        exp, _ExpParamSource, _ExpUnusedParamBytesPattern, _ExpParamIndex)
                        .Compile();

                    PatternLength = patternLength;
                    MoveTable = moveTable;
                    BytesPattern = null;
                }

                public bool CheckWithPattern(byte[] source, int index) => Comparer(source, BytesPattern, index);

                private static Expression MakeExpCmpDigit(Expression exp, int digit, int mask) => Expression.AndAlso(
                    exp,
                    Expression.Equal(
                        Expression.And(
                            _ExpArrayItemIterator,
                            Expression.Constant((byte)mask, typeof(byte))),
                        Expression.Constant((byte)digit, typeof(byte))));

                private static bool CompareCore(byte[] source, byte[] pattern, int index)
                {
                    var length = pattern.Length;
                    unsafe
                    {
                        fixed (byte* src = source, patt = pattern)
                        {
                            for (var i = 0; i < length; i++)
                            {
                                if (src[index + i] != patt[i])
                                {
                                    return false;
                                }
                            }
                            return true;
                        }
                    }
                }

                private unsafe static void SetMultiBadMove(int* moveTable, int badMove, int start, int step)
                {
                    for (int i = start; i < 256; i += step)
                    {
                        moveTable[i] = badMove;
                    }
                }

                private static int GetHexDigit(char number)
                {
                    if (number >= '0' && number <= '9')
                    {
                        return number - '0';
                    }
                    else if ((number >= 'a' && number <= 'f') ||
                             (number >= 'A' && number <= 'F'))
                    {
                        return (number & 7) + 9;     //  'a'=0x61, 'A'=0x41
                    }
                    throw new FormatException("Unexpected char in pattern.");
                }

                private static int[] GetTableFormBag(int patternLength)
                {
                    var result = _MoveTablePool.TryTake(out var item) ? item : new int[256];
                    unsafe
                    {
                        fixed (int* buffer = result)
                        {
                            for (int i = 0; i < 256; i++)
                            {
                                buffer[i] = patternLength;
                            }
                        }
                    }
                    return result;
                }
                public void Dispose()
                {
                    _MoveTablePool.Add(MoveTable);
                }
            }
        }
    }

    /*
     * 
     * 
     * 
    
    "A102C3"　　√

    "A1 02 C3"　√

    "A102 C3"　  √

    "A1 ?? C3"　√

    "A1 ?2 C3"　√

    "A1 0? C3"　√

    "A1 2 C3"　　×

    "A1 02 195"　×

    "A1 ? C3"　　×



    var source = new byte[] { 0, 1, 2, 3, 4, 5, 6, 0x17, };
    int result;

    //Found, result is 1.
    result = BytesFinder.FindIndex(source, new byte[] { 1, 2, 3 });
    result = new BytesFinder(new byte[] { 1, 2, 3 }).FindIndexIn(source);

    //Not found, result is -1.
    result = BytesFinder.FindIndex(source, new byte[] { 8, 9, 10 });
    result = new BytesFinder(new byte[] { 8, 9, 10 }).FindIndexIn(source);

    //Found, result is 1.
    result = BytesFinder.FindIndex(source, "01 02 ?? 04");
    result = new BytesFinder("01 02 ?? 04").FindIndexIn(source);

    //Not found, result is -1.
    result = BytesFinder.FindIndex(source, "01 02 ?? 03");
    result = new BytesFinder("01 02 ?? 03").FindIndexIn(source);

    //Found, result is 5.
    result = BytesFinder.FindIndex(source, "05 06 ?7");
    result = new BytesFinder("05 06 ?7").FindIndexIn(source);

    //Found, result is 5.
    result = BytesFinder.FindIndex(source, "05 06 1?");
    result = new BytesFinder("05 06 1?").FindIndexIn(source);

    //Not found, result is -1.
    result = BytesFinder.FindIndex(source, "05 06 ?8");
    result = BytesFinder.FindIndex(source, "05 06 2?");

    //Found, result is 0.
    result = BytesFinder.FindIndex(source, "?? ?? ?? ?? ?? ?? ?? ??");

    //Not found, result is -1.
    result = BytesFinder.FindIndex(source, "?? ?? ?? ?? ?? ?? ?? ?? ??");



     */
