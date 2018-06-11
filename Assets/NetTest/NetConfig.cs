using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Net
{
    /// <summary>
    /// 协议名称枚举
    /// 添加新协议请同步NetProtocolEnumConveror中的switch/case
    /// 经试验，没1次enum.ToString产生GC大小为147B 
    /// </summary>
    public enum NetProtocolEnum
    {
        Q3RDAuthWebGL,
        Q3RDWSEnter,
        QDataUpdate,
        QCustomsDataPut,
        QPing
    }

    public static class NetProtocolEnumConveror
    {
        public static string GetNetProtocolName(this NetProtocolEnum InNetProtocolEnum)
        {
            switch (InNetProtocolEnum)
            {
                case NetProtocolEnum.Q3RDAuthWebGL:
                    return "Q3RDAuthWebGL";
                case NetProtocolEnum.Q3RDWSEnter:
                    return "Q3RDWSEnter";
                case NetProtocolEnum.QDataUpdate:
                    return "QDataUpdate";
                case NetProtocolEnum.QCustomsDataPut:
                    return "QCustomsDataPut";
                case NetProtocolEnum.QPing:
                    return "QPing";
                default:
                    Debug.LogWarning("为了避免GC，请先完善转换器。");
                    return InNetProtocolEnum.ToString();
            }
        }
    }


    public class NetEventEnumCpr : IEqualityComparer<NetProtocolEnum>
    {
        public bool Equals(NetProtocolEnum InX, NetProtocolEnum InY)
        {
            return (int) InX == (int) InY;
        }

        public int GetHashCode(NetProtocolEnum InNetProtocolEnumEle)
        {
            return (int) InNetProtocolEnumEle;
        }
    }

    public enum NetResultEnum
    {
        Done, //正常
        Error, //服务器返回错误
        Offline, //离线状态
        Max
    }

    public class NetResultEnumCpr : IEqualityComparer<NetResultEnum>
    {
        public bool Equals(NetResultEnum InX, NetResultEnum InY)
        {
            return (int) InX == (int) InY;
        }

        public int GetHashCode(NetResultEnum InNetResultEnumEle)
        {
            return (int) InNetResultEnumEle;
        }
    }

    public class NetConfig
    {
        public const int MAX_RETRY_TIMES_USHORT = 5;
        public const int MAX_CACHE_NET_MSG_COUNT_USHORT = 30;

        public const int HEART_BEAT_MSG_CODE_I = -1;
        public const int HEART_BEAT_SPACE_TIME_I = 1 * 45;

        public const string NET_BACK_SUCCEED_CODE_S = "0";
    }

    public class NetUtility
    {
        private static MD5CryptoServiceProvider md5;

        public static string GetStringMd5(string InString)
        {
            return GetBytesMd5(Encoding.UTF8.GetBytes(InString));
        }

        public static string GetFileMd5(string InFilePath)
        {
            if (!string.IsNullOrEmpty(InFilePath) && File.Exists(InFilePath))
            {
                return GetBytesMd5(File.ReadAllBytes(InFilePath));
            }

            return string.Empty;
        }

        public static string GetBytesMd5(byte[] InBytes)
        {
            if (null == md5)
            {
                md5 = new MD5CryptoServiceProvider();
            }

            byte[] hashBytes = md5.ComputeHash(InBytes);
            int hashBytesLength = hashBytes.Length;
            StringBuilder sb = new StringBuilder(32);
            for (int i = 0; i < hashBytesLength; i++)
            {
                sb.Append(hashBytes[i].ToString("x2"));
            }

            return sb.ToString();
        }
    }
}

