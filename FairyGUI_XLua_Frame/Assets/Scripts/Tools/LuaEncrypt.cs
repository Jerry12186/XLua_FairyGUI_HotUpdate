using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PackageSys
{
    public class LuaEncrypt : Singleton<LuaEncrypt>
    {
       
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="content">要加密的内容</param>
        /// <returns></returns>
        public byte[] Encrypt(byte[] data)
        {
            System.Array.Reverse(data);
            for (int i = 0; i < data.Length; ++i)
            {
                data[i] = (byte)~data[i];
            }
            return data;
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="content">要解密的内容</param>
        /// <returns></returns>
        public byte[] Deencrypt(byte[] data)
        {
            for (int i = 0; i < data.Length; ++i)
            {
                data[i] = (byte)~data[i];
            }
            System.Array.Reverse(data);
            return data;
        }
    }
}