using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace DeltaWebApi.Util
{
    public static class ObjectExtention
    {
        //public static T Convert<T>(this object obj)
        //{
        //    return (T)obj;    
        //}

        public static Guid ToGuid(this object obj)
        {
            try
            {
                return new Guid(obj.ToString());
            }
            catch (Exception)
            {
                return Guid.Empty;
            }
        }

        public static int GetInt(this object obj)
        {
            //try
            //{
            //    return Convert.ToInt32(obj.ToString());
            //}
            //catch
            //{
            //    return 0;
            //}

            int result = 0;
            try
            {
                if (obj != null && obj != DBNull.Value)
                    result = Convert.ToInt32(obj);
            }
            catch (Exception)
            {

            }
            return result;
        }

        public static long GetLong(this object obj)
        {
            //try
            //{
            //    return Convert.ToInt64(obj.ToString());
            //}
            //catch
            //{
            //    return 0;
            //}
            long result = 0;
            try
            {

                if (obj != null && obj != DBNull.Value)
                    result = Convert.ToInt64(obj);
            }
            catch (Exception)
            {

            }
            return result;
        }

        public static bool GetBoolean(this object obj)
        {
            //bool boolval;
            //try
            //{
            //    boolval = (bool)obj;
            //}
            //catch (Exception)
            //{
            //    return false;
            //}
            //return boolval;
            bool boolval = false;
            try
            {
                switch (Type.GetTypeCode(obj.GetType()))
                {
                    case TypeCode.Byte:
                    case TypeCode.Decimal:
                    case TypeCode.Double:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.SByte:
                    case TypeCode.Single:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                        boolval = (int)obj == 1;
                        break;
                    case TypeCode.Boolean:
                        boolval = (bool)obj;
                        break;
                    case TypeCode.String:
                        boolval = (string)obj == "True" || (string)obj == "true";
                        break;
                    case TypeCode.Char:
                        boolval = (string)obj == "1";
                        break;
                    case TypeCode.Object:
                    case TypeCode.Empty:
                    case TypeCode.DateTime:
                    case TypeCode.DBNull:
                        boolval = false;
                        break;
                }
                //boolval = (bool)obj;
            }
            catch (Exception)
            {
                boolval = false;
            }
            return boolval;
        }

        public static DateTime GetDateTime(this object obj)
        {
            DateTime result = new DateTime();
            try
            {
                if (obj != null && obj != DBNull.Value)
                    result = Convert.ToDateTime(obj.ToString());
            }
            catch
            {
                result = DateTime.MinValue.Date;
            }
            return result;
        }

        public static DateTime? GetDateTimeNullable(this object obj)
        {
            DateTime date;
            if (obj != null && obj != DBNull.Value && DateTime.TryParse(obj.ToString(), out date))
            {
                return date;
            }
            else
            {
                return null;
            }
        }

        public static string GetString(this object obj)
        {
            string result = "";

            try
            {
                if (obj != null && obj != DBNull.Value)
                    return Convert.ToString(obj.ToString());
            }
            catch
            {
            }
            return result;
        }

        public static Double GetDouble(this object obj)
        {
            try
            {
                return Convert.ToDouble(obj.ToString());
            }
            catch
            {
                return 0;
            }
        }

        public static float GetFloat(this object obj)
        {
            try
            {
                return Convert.ToSingle(obj.ToString());
            }
            catch
            {
                return 0;
            }
        }

        public static decimal GetDecimal(this object obj)
        {
            //try
            //{
            //    return Convert.ToDecimal(obj.ToString());
            //}
            //catch
            //{
            //    return 0;
            //}

            decimal result = 0;
            try
            {
                if (obj != null && obj != DBNull.Value)
                {
                    result = Convert.ToDecimal(obj);
                }

            }
            catch (Exception)
            {

            }


            return result;
        }

        public static decimal GetDecimal2Digit(this object obj)
        {
            //try
            //{
            //    return Convert.ToDecimal(obj.ToString());
            //}
            //catch
            //{
            //    return 0;
            //}

            decimal result = 0;
            try
            {
                if (obj != null && obj != DBNull.Value)
                {
                    result = Convert.ToDecimal(obj);
                    result = Convert.ToDecimal(result.ToString("F"));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return result;
        }

        public static bool IsNull(this object obj)
        {
            try
            {
                return String.IsNullOrEmpty(obj.ToString());
            }
            catch
            {
                return true;
            }
        }

        public static bool IsNullOrZero(this object obj)
        {
            try
            {
                if (obj == null)
                {
                    return true;
                }
                if (obj is int || obj is long || obj is double || obj is float || obj is decimal)
                {
                    return obj.GetDecimal() == 0;
                }
                else if (obj is string)
                {
                    if (!obj.IsNull())
                    {
                        return obj.GetString() == "0";
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return true;
            }
        }

        public static string GetBooleanStr(this object obj)
        {
            bool boolval = false;
            try
            {
                switch (Type.GetTypeCode(obj.GetType()))
                {
                    case TypeCode.Byte:
                    case TypeCode.Decimal:
                    case TypeCode.Double:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.SByte:
                    case TypeCode.Single:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                        boolval = (int)obj == 1;
                        break;
                    case TypeCode.Boolean:
                        boolval = (bool)obj;
                        break;
                    case TypeCode.String:
                    case TypeCode.Char:
                        boolval = (string)obj == "1";
                        break;
                    case TypeCode.Object:
                    case TypeCode.Empty:
                    case TypeCode.DateTime:
                    case TypeCode.DBNull:
                        boolval = false;
                        break;
                }
                //boolval = (bool)obj;
            }
            catch (Exception)
            {
                boolval = false;
            }
            return boolval ? "1" : "0";
        }

        public static bool GetExists<T>(this T t, params T[] values)
        {
            return values.Contains(t);
        }

        public static IEnumerable Cartesian(this IEnumerable<IEnumerable> items)
        {
            var slots = items
               // initialize enumerators
               .Select(x => x.GetEnumerator())
               // get only those that could start in case there is an empty collection
               .Where(x => x.MoveNext())
               .ToArray();

            while (true)
            {
                // yield current values
                yield return slots.Select(x => x.Current);

                // increase enumerators
                foreach (var slot in slots)
                {
                    // reset the slot if it couldn't move next
                    if (!slot.MoveNext())
                    {
                        // stop when the last enumerator resets
                        if (slot == slots.Last()) { yield break; }
                        slot.Reset();
                        slot.MoveNext();
                        // move to the next enumerator if this reseted
                        continue;
                    }
                    // we could increase the current enumerator without reset so stop here
                    break;
                }
            }
        }

    }
    public static class StoredProcedureParameterExtensions
    {
        private static Dictionary<string, Type> Mappings;

        static StoredProcedureParameterExtensions()
        {
            Mappings = new Dictionary<string, Type>();
            Mappings.Add("bigint", typeof(Int64));
            Mappings.Add("binary", typeof(Byte[]));
            Mappings.Add("bit", typeof(Boolean));
            Mappings.Add("bool", typeof(Boolean));
            Mappings.Add("char", typeof(String));
            Mappings.Add("string", typeof(String));
            Mappings.Add("date", typeof(DateTime));
            Mappings.Add("datetime", typeof(DateTime));
            Mappings.Add("datetime2", typeof(DateTime));
            Mappings.Add("datetimeoffset", typeof(DateTimeOffset));
            Mappings.Add("decimal", typeof(Decimal));
            Mappings.Add("float", typeof(Double));
            Mappings.Add("image", typeof(Byte[]));
            Mappings.Add("int", typeof(Int32));
            Mappings.Add("money", typeof(Decimal));
            Mappings.Add("nchar", typeof(String));
            Mappings.Add("ntext", typeof(String));
            Mappings.Add("numeric", typeof(Decimal));
            Mappings.Add("nvarchar", typeof(String));
            Mappings.Add("real", typeof(Single));
            Mappings.Add("rowversion", typeof(Byte[]));
            Mappings.Add("smalldatetime", typeof(DateTime));
            Mappings.Add("smallint", typeof(Int16));
            Mappings.Add("smallmoney", typeof(Decimal));
            Mappings.Add("text", typeof(String));
            Mappings.Add("time", typeof(TimeSpan));
            Mappings.Add("timestamp", typeof(Byte[]));
            Mappings.Add("tinyint", typeof(Byte));
            Mappings.Add("uniqueidentifier", typeof(Guid));
            Mappings.Add("varbinary", typeof(Byte[]));
            Mappings.Add("varchar", typeof(String));

        }

        public static Type ToClrType(this string sqlType)
        {
            Type datatype = null;
            if (Mappings.TryGetValue(sqlType, out datatype))
                return datatype;
            throw new TypeLoadException(string.Format("Can not load CLR Type from {0}", sqlType));
        }

    }
}