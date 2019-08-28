using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace Delta.UTL.DBUtil
{
    public static class StringUtil
    {
        public static string ParameterListToStr(List<Parameter> list)
        {
            string result = "";
            foreach (Parameter p in list)
            {
                result += "@" + p.ParamName + "=" + p.ParamValue + ",";
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
        public static bool IsStringEmpty(string txt)
        {
            if (txt == null) return true;
            if (txt.Trim().Length == 0) return true;
            return false;
        }
        public static bool IsNumeric(string s)
        {
            int intOutput;
            return (Int32.TryParse(s, out intOutput));
        }

        public static string SubString(string txt,int len)
        {
            if (txt == null) return null;
            if (txt.Trim().Length > len)
                return txt.Substring(0,len);
            else
                return txt;
        }

        public static string FormatString(decimal dbl, int type,string languageCode)
        {
            switch (type)
            {
                case 1:
                    return dbl.ToString("N", new CultureInfo(CultureInfoStr(languageCode)));
                default:
                    break;
            }
            return dbl.ToString();
        }

        public static string CultureInfoStr(string languageCode)
        {
            switch (languageCode)
            {
                case "EN":
                    return "en-US";
                case "TR":
                    return "tr-TR";
                case "RU":
                    return "ru-RU";
                default:
                    break;
            }
            return languageCode;
        }

        public static string ConvertToBase64(object obj, bool encrypt = false, string encryptKey = null)
        {
            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    new BinaryFormatter().Serialize(memoryStream, obj);
                    string base64 = Convert.ToBase64String(memoryStream.ToArray());

                    if (encrypt)
                    {
                        var encrypted = EncryptionLibrary.Encrypt(Encoding.UTF8.GetBytes(base64), encryptKey);
                        base64 = Convert.ToBase64String(encrypted);
                    }
                    return base64;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public static object ConvertFromBase64(string base64, bool encrypt = false, string encryptKey = null)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(base64);

                if (encrypt)
                {
                    var encrypted = Encoding.UTF8.GetString(EncryptionLibrary.Decrypt(bytes, encryptKey));
                    bytes = Convert.FromBase64String(encrypted);
                }
                using (MemoryStream memoryStream = new MemoryStream(bytes, 0, bytes.Length))
                {
                    memoryStream.Write(bytes, 0, bytes.Length);
                    memoryStream.Position = 0;
                    return new BinaryFormatter().Deserialize(memoryStream);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static string GeneratePassword()
        {
            const string alphanumericCharacters =
             "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
             "abcdefghijklmnopqrstuvwxyz" +
             "0123456789";
            return GetRandomString(8, alphanumericCharacters);
        }

        private static string GetRandomString(int length, IEnumerable<char> characterSet)
        {
            if (length < 0)
                throw new ArgumentException("length must not be negative", "length");
            if (length > int.MaxValue / 8) // 250 million chars ought to be enough for anybody
                throw new ArgumentException("length is too big", "length");
            if (characterSet == null)
                throw new ArgumentNullException("characterSet");
            var characterArray = characterSet.Distinct().ToArray();
            if (characterArray.Length == 0)
                throw new ArgumentException("characterSet must not be empty", "characterSet");

            var bytes = new byte[length * 8];
            new RNGCryptoServiceProvider().GetBytes(bytes);
            var result = new char[length];
            for (int i = 0; i < length; i++)
            {
                ulong value = BitConverter.ToUInt64(bytes, i * 8);
                result[i] = characterArray[value % (uint)characterArray.Length];
            }
            return new string(result);
        }
        public static string SerializeObjectToXML(object obj)
        {

            var serializer = new XmlSerializer(obj.GetType());
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            var ms = new MemoryStream();
            //the following line omits the xml declaration
            var settings = new XmlWriterSettings { OmitXmlDeclaration = true, Encoding = new UnicodeEncoding(false, false) };
            var writer = XmlWriter.Create(ms, settings);
            serializer.Serialize(writer, obj, ns);
            return Encoding.Unicode.GetString(ms.ToArray());
        }
    }
}
