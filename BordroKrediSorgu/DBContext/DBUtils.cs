using System;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace DeltaWebApi.DBContext
{
    public class DBUtils
    {
        public static byte[] CompressDataSet(DataSet ds)
        {
            try
            {
                byte[] inbyt = DataSetToByte(ds);
                System.IO.MemoryStream objStream = new MemoryStream();
                System.IO.Compression.DeflateStream objZS =
                new System.IO.Compression.DeflateStream(objStream,
                System.IO.Compression.CompressionMode.Compress);
                objZS.Write(inbyt, 0, inbyt.Length);
                objZS.Flush();
                objZS.Close();
                return objStream.ToArray();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static byte[] DataSetToByte(DataSet ds)
        {
            ds.RemotingFormat = SerializationFormat.Binary;
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, ds);
            byte[] inbyt = ms.ToArray();
            return inbyt;
        }

        public static DataSet DecompressDataSet(byte[] compressedBytes)
        {
            //try
            //{
            //    byte[] bytes = SevenZipHelper.Decompress(compressedBytes);

            //    DataSet ds = null;
            //    using (var memory = new MemoryStream(bytes))
            //    {
            //        memory.Seek(0, SeekOrigin.Begin);
            //        var formatter = new BinaryFormatter();
            //        ds = (DataSet)formatter.Deserialize(memory, null);
            //    }
            //    return ds;
            //}
            //catch (Exception)
            //{
            //    return null;
            //}

            //using (MemoryStream stream = new MemoryStream(compressedBytes))
            //{
            //    DataSet ds = new DataSet();
            //    using (GZipStream decompressStream = new GZipStream(stream, CompressionMode.Decompress))
            //    {
            //        ds.ReadXml(decompressStream, XmlReadMode.InferSchema);
            //        decompressStream.Close();
            //    }

            //    stream.Close();
            //    return ds;
            //}

            try
            {
                DataSet outDs = new DataSet();
                MemoryStream inMs = new MemoryStream(compressedBytes);
                inMs.Seek(0, 0);
                DeflateStream zipStream = new DeflateStream(inMs, CompressionMode.Decompress, true);
                byte[] outByt = ReadFullStream(zipStream);
                zipStream.Flush();
                zipStream.Close();
                MemoryStream outMs = new MemoryStream(outByt);
                outMs.Seek(0, 0);
                outDs.RemotingFormat = SerializationFormat.Binary;
                BinaryFormatter bf = new BinaryFormatter();
                outDs = (DataSet)bf.Deserialize(outMs, null);
                return outDs;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static byte[] ReadFullStream(Stream stream)
        {
            byte[] buffer = new byte[32768];
            using (MemoryStream ms = new MemoryStream())
            {
                while (true)
                {
                    int read = stream.Read(buffer, 0, buffer.Length);
                    if (read <= 0)
                        return ms.ToArray();
                    ms.Write(buffer, 0, read);
                }
            }
        }

        private static readonly byte[] encryptSalt = new byte[] { 0x35, 0x59, 0x27, 0x48 };

        public static byte[] Encrypt(byte[] input, string encryptKey)
        {
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(encryptKey, encryptSalt);
            MemoryStream ms = new MemoryStream();
            Aes aes = new AesManaged();
            aes.Key = pdb.GetBytes(aes.KeySize / 8);
            aes.IV = pdb.GetBytes(aes.BlockSize / 8);
            CryptoStream cs = new CryptoStream(ms,
              aes.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(input, 0, input.Length);
            cs.Close();
            return ms.ToArray();
        }

        public static byte[] Decrypt(byte[] input, string encryptKey)
        {
            //PasswordDeriveBytes pdb = new PasswordDeriveBytes("hjiweykaksd", new byte[] { 0x43, 0x87, 0x23, 0x72 })
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(encryptKey, encryptSalt);
            MemoryStream ms = new MemoryStream();
            Aes aes = new AesManaged();
            aes.Key = pdb.GetBytes(aes.KeySize / 8);
            aes.IV = pdb.GetBytes(aes.BlockSize / 8);
            CryptoStream cs = new CryptoStream(ms,
              aes.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(input, 0, input.Length);
            cs.Close();
            return ms.ToArray();
        }

        public static string Hash(string password)
        {
            var bytes = new UTF8Encoding().GetBytes(password);
            byte[] hashBytes;
            using (var algorithm = new System.Security.Cryptography.SHA512Managed())
            {
                hashBytes = algorithm.ComputeHash(bytes);
            }
            return Convert.ToBase64String(hashBytes);
        }

        public static DataTable enumTodatatablex(Type enumName)
        {
            MethodInfo[] mis = enumName.GetMethods();
            DataTable dt = new DataTable();
            //dt.Columns.Add("Id", System.Type.GetType("System.Int32"));
            dt.Columns.Add("Name", System.Type.GetType("System.String"));
            foreach (var item in enumName.GetEnumValues())
            {
                DataRow dr = dt.NewRow();
                int value = (int)item;
                dr["Name"] = value.ToString() + "::" + item.ToString();
                dt.Rows.Add(dr);
            }
            return dt;
        }

        public static bool ExistsRecord(DataSet ds)
        {
            try
            {
                return ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0;
            }
            catch
            {
                return false;
            }
        }

        //public static bool ExistsRecord(DataTable dt, bool msg = false)
        //{
        //    bool _ExistsRecord;
        //    try
        //    {
        //        _ExistsRecord = dt?.Rows.Count > 0;
        //        if (msg && !_ExistsRecord) UTL.MessageDialog.InfoMessage(LedgerDialogMessages.RecordNotFound);
        //        return _ExistsRecord;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        public static bool ExistsRecord(DataRow dr)
        {
            try
            {
                return !String.IsNullOrEmpty(dr.ToString());
            }
            catch
            {
                return false;
            }
        }

        public static DataTable EnumToDataTable(Type enumName)
        {
            MethodInfo[] mis = enumName.GetMethods();

            DataTable dt = new DataTable();
            dt.Columns.Add("Id", System.Type.GetType("System.Int32"));
            dt.Columns.Add("Description", System.Type.GetType("System.String"));

            int i = 0;
            foreach (string s in Enum.GetNames(enumName))
            {
                DataRow dr = dt.NewRow();
                dr["Id"] = i;
                dr["Description"] = s;
                dt.Rows.Add(dr);
                i++;
            }
            return dt;
        }

        //public static string GetXMLString(this object obj)
        //{
        //    try
        //    {
        //        if (obj is TextBox) return ((TextBox)obj).Text;
        //        return "<![CDATA[" + Convert.ToString(obj.ToString()) + "]]>";
        //    }
        //    catch
        //    {
        //        return "";
        //    }
        //}

        //public static int GetInt(this object obj)
        //{
        //    try
        //    {
        //        return Convert.ToInt32(obj.ToString());
        //    }
        //    catch
        //    {
        //        return 0;
        //    }
        //}

        //public static DateTime GetDateTime(this object obj)
        //{
        //    try
        //    {
        //        return Convert.ToDateTime(obj.ToString());
        //    }
        //    catch
        //    {
        //        return DateTime.MinValue.Date;
        //    }
        //}

        //public static string GetString(this object obj)
        //{
        //    try
        //    {
        //        if (obj is TextBox) return ((TextBox)obj).Text;
        //        return Convert.ToString(obj.ToString());
        //    }
        //    catch
        //    {
        //        return "";
        //    }
        //}

        //public static Double GetDouble(this object obj)
        //{
        //    try
        //    {
        //        return Convert.ToDouble(obj.ToString());
        //    }
        //    catch
        //    {
        //        return 0;
        //    }
        //}

        //public static float GetFloat(this object obj)
        //{
        //    try
        //    {
        //        return Convert.ToSingle(obj.ToString());
        //    }
        //    catch
        //    {
        //        return 0;
        //    }
        //}

        //public static decimal GetDecimal(this object obj)
        //{
        //    try
        //    {
        //        return Convert.ToDecimal(obj.ToString());
        //    }
        //    catch
        //    {
        //        return 0;
        //    }
        //}

        //public static bool In<T>(this T t, params T[] values)
        //{
        //    foreach (T value in values)
        //    {
        //        if (t.Equals(value))
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        //public static bool In<T>(this T t, params T[] values)
        //{
        //    return values.Contains(t);
        //}

        //if (1.In(1, 2))
        //try
        //{
        //    ds.RemotingFormat = SerializationFormat.Binary;
        //    byte[] bytes = null;
        //    using (var memory = new MemoryStream())
        //    {
        //        var formatter = new BinaryFormatter();
        //        formatter.Serialize(memory, ds);
        //        bytes = memory.ToArray();
        //    }
        //    return SevenZipHelper.Compress(bytes);
        //}
        //catch (Exception)
        //{
        //    return null;
        //}

        //using (MemoryStream stream = new MemoryStream())
        //{
        //    using (GZipStream compressStream = new GZipStream(stream, CompressionMode.Compress))
        //    {
        //        ds.WriteXml(compressStream,XmlWriteMode.IgnoreSchema);
        //    }
        //    return stream.ToArray();
        //}

    }
}