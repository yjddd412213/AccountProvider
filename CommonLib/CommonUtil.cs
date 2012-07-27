using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Globalization;
using System.Collections;
using System.Security.Cryptography;
using System.Net;

namespace CommonLib
{
    public enum StatusCode
    {
        Invalid,

        Success,
        Fail,
        CmdQueued,
        InvalidState,
        InvalidInput,
        SessionNotFound,
        NotSupported,
        UserNotFound,
        AuthenticationFailed,
        Denied,
        AlreadyExist,
        AlreadyActivated,
        Disconnected,
        Completed,
        InvalidData,
        NotFound,
        Offline,
        Aborted,
        Retry,
        NotActivated,
    }
    public class DateUtil
    {
        /// <summary>
        /// Finds the next date whose day of the week equals the specified day of the week.
        /// </summary>
        /// <param name="startDate">
        ///		The date to begin the search.
        /// </param>
        /// <param name="desiredDay">
        ///		The desired day of the week whose date will be returneed.
        /// </param>
        /// <returns>
        ///		The returned date occurs on the given date's week.
        ///		If the given day occurs before given date, the date for the
        ///		following week's desired day is returned.
        /// </returns>
        public static DateTime GetNextDateForDay(DateTime startDate, DayOfWeek desiredDay)
        {
            // Given a date and day of week,
            // find the next date whose day of the week equals the specified day of the week.
            return startDate.AddDays(DaysToAdd(startDate.DayOfWeek, desiredDay));
        }

        /// <summary>
        /// Calculates the number of days to add to the given day of
        /// the week in order to return the next occurrence of the
        /// desired day of the week.
        /// </summary>
        /// <param name="current">
        ///		The starting day of the week.
        /// </param>
        /// <param name="desired">
        ///		The desired day of the week.
        /// </param>
        /// <returns>
        ///		The number of days to add to <var>current</var> day of week
        ///		in order to achieve the next <var>desired</var> day of week.
        /// </returns>
        public static int DaysToAdd(DayOfWeek current, DayOfWeek desired)
        {
            // f( c, d ) = g( c, d ) mod 7, g( c, d ) > 7
            //           = g( c, d ), g( c, d ) < = 7
            //   where 0 <= c < 7 and 0 <= d < 7

            int c = (int)current;
            int d = (int)desired;
            int n = (7 - c + d);

            return (n > 7) ? n % 7 : n;
        }

        public static Nullable<DayOfWeek> FindNextDayOfWeek(DayOfWeek[] DayOfWeeks)
        {
            DateTime now = DateTime.Now;
            Nullable<DayOfWeek> nextDayOfWeek = null;
            if (DayOfWeeks != null && DayOfWeeks.Length > 0)
            {
                DayOfWeek smallest = DayOfWeeks[0];
                foreach (DayOfWeek dayOfWeek in DayOfWeeks)
                {
                    if (smallest > dayOfWeek)
                    {
                        smallest = dayOfWeek;
                    }

                    if (dayOfWeek > now.DayOfWeek)
                    {
                        nextDayOfWeek = dayOfWeek;
                        break;
                    }
                }

                if (nextDayOfWeek == null)
                {
                    nextDayOfWeek = smallest;
                }
            }

            return nextDayOfWeek;
        }
    }

    public class GZipUtil
    {
        public static MemoryStream Compress(MemoryStream input)
        {
            MemoryStream output = null;

            if (input != null)
            {
                GZipStream gzStream = null;
                output = new MemoryStream();

                try
                {
                    byte[] inputBytes = input.ToArray();
                    gzStream = new GZipStream(output, CompressionMode.Compress, true);
                    gzStream.Write(inputBytes, 0, inputBytes.Length);
                    gzStream.Flush();
                    gzStream.Close();

                    output.Position = 0;
                }
                catch (Exception e)
                {
                    // ignore for now.
                }
            }

            return output;
        }
        public static byte[] Compress(byte[] input)
        {
            byte[] output = null;

            if (input != null)
            {
                GZipStream compressedzipStream = null;
                MemoryStream memStream = new MemoryStream();

                try
                {
                    compressedzipStream = new GZipStream(memStream, CompressionMode.Compress, true);
                    compressedzipStream.Write(input, 0, input.Length);
                    compressedzipStream.Flush();
                }
                catch (Exception e)
                {
                    // ignore for now.
                }
                finally
                {
                    compressedzipStream.Close();
                }

                output = memStream.ToArray();
                memStream.Close();
            }

            

            return output;
        }

        public static string DecompressToString(MemoryStream inputStream)
        {
            string uncompressedString = null;

            MemoryStream uncompressedStream = DecompressToStream(inputStream);

            if (uncompressedStream != null)
            {
                byte[] buffer = uncompressedStream.ToArray();
                if(buffer != null)
                {
                    uncompressedString = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                }
            }

            return uncompressedString;
        }

        public static MemoryStream DecompressToStream(MemoryStream inputStream)
        {
            MemoryStream outputStream = null;

            if (inputStream != null)
            {
                inputStream.Position = 0;

                GZipStream gs = new GZipStream(inputStream, CompressionMode.Decompress);

                BinaryReader reader = new BinaryReader(gs);

                outputStream = new MemoryStream();

                BinaryWriter writer = new BinaryWriter(outputStream);

                int bytesRead;

                byte[] buffer = new byte[2];

                try
                {
                    do
                    {
                        bytesRead = reader.Read(buffer, 0, buffer.Length);

                        writer.Write(buffer, 0, bytesRead);

                    } while (bytesRead > 0);

                    writer.Flush();

                    outputStream.Position = 0;

                }
                catch(Exception e)
                {
                    // TODO: ignore for now.
                }
            }

            return outputStream;
        }
    }

    public class Base64Util
    {
        public static string EncodeToBase64(byte[] bytes, Encoding encoding)
        {
            return System.Convert.ToBase64String(bytes);
        }

        public static byte[] DecodeFromBase64(string b64String, Encoding encoding)
        {
            return System.Convert.FromBase64String(b64String);
        }

        public static string EncodeToBase64(string srcString)
        {
            string b64Str = null;
            if (srcString != null)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(srcString);

                b64Str = System.Convert.ToBase64String(bytes);
            }
            return b64Str;
        }

        public static string DecodeFromBase64(string b64String)
        {
            string clearStr = null;
            if (b64String != null)
            {
                byte[] bytes = System.Convert.FromBase64String(b64String);
                clearStr = Encoding.UTF8.GetString(bytes);
            }
            return clearStr;
        }
    }

    public class EnumConverter<T>
    {
        public static T StringToEnum(string enumStr, T initValue)
        {
            T result = initValue;

            if (enumStr != null)
            {
                try
                {
                    result = (T)Enum.Parse(typeof(T), enumStr);
                }
                catch (Exception e)
                {
                    // ignore.
                }
            }

            return result;
        }
    }

    public class MD5Util
    {
        public static byte[] GetDigest(string userID, string password)
        {
            MD5CryptoServiceProvider md5Provider = new MD5CryptoServiceProvider();
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(userID + ":" + password);
            bytes = md5Provider.ComputeHash(bytes);
            return bytes;
        }
    }

    public class AuthUtil
    {
        public static bool AuthenticateMD5(string hashB64, string userID, string password)
        {
            bool authenticated = false;

            byte[] clientHash = System.Convert.FromBase64String(hashB64);
            byte[] serverHash = MD5Util.GetDigest(userID, password);

            bool match = true;
            if (clientHash != null && serverHash != null && clientHash.Length == serverHash.Length)
            {
                for (int i = 0; i < clientHash.Length; i++)
                {
                    if (serverHash[i] != clientHash[i])
                    {
                        match = false;
                        break;
                    }
                }
            }
            else
            {
                match = false;
            }

            if (match)
            {
                authenticated = true;
            }

            return authenticated;
        }
    }


    public class DynamicArray<T>
    {
        public T[] RemoveFromArray(T[] src, T objToRemove, ref bool removed)
        {
            T[] result = null;
            removed = false;

            if (src != null && src.Length > 0 && objToRemove != null)
            {
                int len = src.Length;

                int index = Array.IndexOf<T>(src, objToRemove);

                if (index != -1)
                {
                    removed = true;

                    if (len > 1)
                    {
                        result = new T[len - 1];

                        int i = 0; // result index

                        for (int j = 0; j < src.Length; j++)
                        {
                            if (j != index)
                            {
                                // copy over other elements except the one to be removed.
                                result[i] = src[j];
                                i++;
                            }
                        }

                    }
                }
            }

            return result;
        }

        public T[] RemoveAt(T[] src, int index)
        {
            T[] result = null;

            if (src != null && src.Length > 0)
            {
                int len = src.Length;

                if (index >= 0)
                {
                    if (len > 1)
                    {
                        result = new T[len - 1];

                        int i = 0; // result index

                        for (int j = 0; j < src.Length; j++)
                        {
                            if (j != index)
                            {
                                // copy over other elements except the one to be removed.
                                result[i] = src[j];
                                i++;
                            }
                        }

                    }
                }
            }

            return result;
        }

        public T[] AddToArray(T[] src, T newObj)
        {
            T[] retArr = null;

            // if src is null, then just allocate a one element array to contain the new string.
            if (src == null)
            {
                if (newObj != null)
                {
                    retArr = new T[1];
                    retArr[0] = newObj;
                }
            }
            else
            {
                // If newObj is null, just use the source array.
                if (newObj == null)
                {
                    retArr = src;
                }
                else
                {
                    // allocate array length = src.Length + 1
                    retArr = new T[src.Length + 1];

                    // copy over source array elements

                    for (int i = 0; i < src.Length; i++)
                    {
                        retArr[i] = src[i];
                    }

                    // set the new element
                    retArr[src.Length] = newObj;
                }

            }

            return retArr;
        }


        public T[] AddToArrayFront(T[] src, T newObj)
        {
            T[] retArr = null;

            // if src is null, then just allocate a one element array to contain the new string.
            if (src == null)
            {
                if (newObj != null)
                {
                    retArr = new T[1];
                    retArr[0] = newObj;
                }
            }
            else
            {
                // If newObj is null, just use the source array.
                if (newObj == null)
                {
                    retArr = src;
                }
                else
                {
                    // allocate array length = src.Length + 1
                    retArr = new T[src.Length + 1];

                    // set the first element
                    retArr[0] = newObj;

                    if (src.Length > 0)
                    {
                        // copy over source array elements
                        for (int i = 0; i < src.Length; i++)
                        {
                            retArr[1+i] = src[i];
                        }
                    }

                }

            }

            return retArr;
        }
        public T[] RemoveFromArrayRange(T[] src, T[] toBeRemoved)
        {
            T[] result = src;
            if (src != null && toBeRemoved != null)
            {
                bool removed = false;
                foreach (T item in toBeRemoved)
                {
                    result = RemoveFromArray(result, item, ref removed);
                }
            }
            return result;
        }

        public T[] Insert(T[] src, T newObj, int index)
        {
            T[] retArr = null;

            if ((index > src.Length) || (index < 0))
            {
                throw new IndexOutOfRangeException();
            }
            else
            {
                // If newObj is null, just use the source array.
                if (newObj == null || src == null)
                {
                    throw new InvalidDataException();
                }
                else
                {
                    // allocate array length = src.Length + 1
                    retArr = new T[src.Length + 1];

                    // set new obj.
                    retArr[index] = newObj;

                    // copy over source array elements except the new obj.

                    for (int i = 0; i < retArr.Length; i++)
                    {
                        if (i < index)
                        {
                            retArr[i] = src[i];
                        }
                        else if (i > index)
                        {
                            retArr[i] = src[i-1];
                        }
                    }
                }

            }

            return retArr;
        }

        //public T[] AddToArrayRange(T[] src, T[] newObjs)
        //{
        //    T[] retArr = null;

        //    // if src is null, then just allocate a one element array to contain the new string.
        //    if (src == null)
        //    {
        //        if (newObjs != null && newObjs.Length > 0)
        //        {
        //            retArr = new T[newObjs.Length];

        //            for (int i=0; i<newObjs.Length; i++)
        //            {
        //                retArr[i] = newObjs[i];
        //            }
        //        }
        //    }
        //    else
        //    {
        //        // If newObj is null, just use the source array.
        //        if (newObjs == null)
        //        {
        //            retArr = src;
        //        }
        //        else if(newObjs.Length > 0)
        //        {
        //            // allocate array length = src.Length + newObjs.Length
        //            retArr = new T[src.Length + newObjs.Length];

        //            // copy over source array elements

        //            for (int i = 0; i < src.Length; i++)
        //            {
        //                retArr[i] = src[i];
        //            }

        //            for (int i = 0; i < newObjs.Length; i++)
        //            {
        //                // set the new elements
        //                retArr[i+src.Length] = newObjs[i];
        //            }
        //        }

        //    }

        //    return retArr;
        //}

        public T[] AddToArrayRange(T[] src, ICollection<T> newObjs)
        {
            T[] retArr = null;

            // if src is null, then just allocate a one element array to contain the new string.
            if (src == null)
            {
                if (newObjs != null && newObjs.Count > 0)
                {
                    retArr = new T[newObjs.Count];

                    for (int i = 0; i < newObjs.Count; i++)
                    {
                        retArr[i] = newObjs.ElementAt(i);
                    }
                }
            }
            else
            {
                // If newObj is null, just use the source array.
                if (newObjs == null)
                {
                    retArr = src;
                }
                else if (newObjs.Count > 0)
                {
                    // allocate array length = src.Length + newObjs.Length
                    retArr = new T[src.Length + newObjs.Count];

                    // copy over source array elements

                    for (int i = 0; i < src.Length; i++)
                    {
                        retArr[i] = src[i];
                    }

                    for (int i = 0; i < newObjs.Count; i++)
                    {
                        // set the new elements
                        retArr[i + src.Length] = newObjs.ElementAt(i);
                    }
                }

            }

            return retArr;
        }

        public T[] AddRange(T[] src, IList<T> newObjs)
        {
            T[] retArr = null;

            // if src is null, then just allocate a one element array to contain the new string.
            if (src == null)
            {
                if (newObjs != null && newObjs.Count > 0)
                {
                    retArr = new T[newObjs.Count];

                    for (int i = 0; i < newObjs.Count; i++)
                    {
                        retArr[i] = newObjs[i];
                    }
                }
            }
            else
            {
                // If newObj is null, just use the source array.
                if (newObjs == null)
                {
                    retArr = src;
                }
                else if (newObjs.Count > 0)
                {
                    // allocate array length = src.Length + newObjs.Length
                    retArr = new T[src.Length + newObjs.Count];

                    // copy over source array elements

                    for (int i = 0; i < src.Length; i++)
                    {
                        retArr[i] = src[i];
                    }

                    for (int i = 0; i < newObjs.Count; i++)
                    {
                        // set the new elements
                        retArr[i + src.Length] = newObjs[i];
                    }
                }

            }

            return retArr;
        }
    }
    public class DynamicArrayEx<T>
    {
        public T[] RemoveFromArray(ref T[] src, T objToRemove, ref bool removed)
        {
            T[] srcTemp = src;

            removed = false;

            if (src != null && src.Length > 0 && objToRemove != null)
            {
                int len = src.Length;

                int index = Array.IndexOf<T>(src, objToRemove);

                if (index != -1)
                {
                    removed = true;

                    if (len > 1)
                    {
                        src = new T[len - 1];

                        int i = 0; // result index

                        for (int j = 0; j < srcTemp.Length; j++)
                        {
                            if (j != index)
                            {
                                // copy over other elements except the one to be removed.
                                src[i] = srcTemp[j];
                                i++;
                            }
                        }

                    }
                }
            }

            return src;
        }


        public T[] RemoveAt(ref T[] src, int index)
        {
            T[] srcTemp = src;

            if (src != null && src.Length > 0)
            {
                int len = src.Length;

                if (index >= 0)
                {
                    if (len > 1)
                    {
                        src = new T[len - 1];

                        int i = 0; // result index

                        for (int j = 0; j < srcTemp.Length; j++)
                        {
                            if (j != index)
                            {
                                // copy over other elements except the one to be removed.
                                src[i] = srcTemp[j];
                                i++;
                            }
                        }

                    }
                }
            }

            return src;
        }
        public T[] AddToArray(ref T[] src, T newObj)
        {
            T[] srcTemp = src;

            // if src is null, then just allocate a one element array to contain the new string.
            if (src == null)
            {
                if (newObj != null)
                {
                    src = new T[1];
                    src[0] = newObj;
                }
            }
            else
            {
                // If newObj is null, just use the source array.
                if (newObj == null)
                {
                    //retArr = src;
                }
                else
                {
                    // allocate array length = src.Length + 1
                    src = new T[srcTemp.Length + 1];

                    // copy over source array elements

                    for (int i = 0; i < srcTemp.Length; i++)
                    {
                        src[i] = srcTemp[i];
                    }

                    // set the new element
                    src[srcTemp.Length] = newObj;
                }

            }

            return src;
        }

        public T[] Insert(ref T[] src, T newObj, int index)
        {
            T[] srcTemp = src;

            if ((index > src.Length) || (index < 0))
            {
                throw new IndexOutOfRangeException();
            }
            else
            {
                // If newObj is null, just use the source array.
                if (newObj == null || src == null)
                {
                    throw new InvalidDataException();
                }
                else
                {
                    // allocate array length = src.Length + 1
                    src = new T[src.Length + 1];

                    // set new obj.
                    src[index] = newObj;

                    // copy over source array elements except the new obj.

                    for (int i = 0; i < src.Length; i++)
                    {
                        if (i < index)
                        {
                            src[i] = srcTemp[i];
                        }
                        else if (i > index)
                        {
                            src[i] = srcTemp[i - 1];
                        }
                    }
                }

            }

            return src;
        }

        public T[] AddToArrayRange(ref T[] src, T[] newObjs)
        {
            T[] srcTemp = src;

            // if src is null, then just allocate a one element array to contain the new string.
            if (src == null)
            {
                if (newObjs != null && newObjs.Length > 0)
                {
                    src = new T[newObjs.Length];

                    for (int i = 0; i < newObjs.Length; i++)
                    {
                        src[i] = newObjs[i];
                    }
                }
            }
            else
            {
                // If newObj is null, just use the source array.
                if (newObjs == null)
                {
                    //retArr = src;
                }
                else if (newObjs.Length > 0)
                {
                    // allocate array length = src.Length + newObjs.Length
                    src = new T[src.Length + newObjs.Length];

                    // copy over source array elements

                    for (int i = 0; i < srcTemp.Length; i++)
                    {
                        src[i] = srcTemp[i];
                    }

                    for (int i = 0; i < newObjs.Length; i++)
                    {
                        // set the new elements
                        src[i + srcTemp.Length] = newObjs[i];
                    }
                }

            }

            return src;
        }
    }
}
