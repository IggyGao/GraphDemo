using System;
using System.Collections.Generic;
using GraphDemo.Data;

namespace GraphDemo.DataProvider
{
    public class Resolver
    {
        private ICallback _callback;
        private static byte[] head;

        public Resolver(ICallback callback)
        {
            _callback = callback;
            head = new byte[10] { 0xF0, 0xF1, 0xF2, 0xF3, 0xF4, 0xF5, 0xF6, 0xF7, 0xF8, 0xF9 };
        }

        public int Resolve(byte[] buffer)
        {
            _callback.test();
            var messages = new List<Message>();
            var offset = 0;
            Int16 dotsnumber = 640;//可手动更改此处，如需动态适应不同模式，请增加代码和入口
            while (offset < buffer.Length - 10)
            {
                Result res = FindHead(buffer, offset);
                if (res == Result.NotComplete)
                {
                    break;
                }
                if (res == Result.NoHeadFound)
                {
                    offset++;
                }
                if (res == Result.Success)
                {
                    offset += 22;
                    Message message;
                    if (ResolveMessage(buffer, ref offset, dotsnumber, out message))
                        messages.Add(message);
                }
            }
            if (messages.Count != 0)
            {
                _callback.AddPoints(messages);
            }
            return offset;
        }

        /// <summary>
        /// 寻找包头
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Result FindHead(byte[] buffer, int offset)
        {
            if (buffer.Length < offset + 22 + 640 * 16)
                return Result.NotComplete;
            byte[] temp = new byte[10];
            for (int i = 0; i < 10; i++)
            {
                ReadByte(buffer, ref offset, ref temp[i]);
                if (temp[i] != head[i]) return Result.NoHeadFound;
            }
            return Result.Success;
          
        }

        /// <summary>
        /// 解析缓冲区(暂时只读取第一条线的低能数据)
        /// 如需动态适应不同模式，请增加代码和入口
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool ResolveMessage(byte[] buffer, ref int offset, 
            Int16 dotsnumber, out Message message)
        {
            short[] temp1 = new short[dotsnumber];
            short[] temp2 = new short[dotsnumber];
            short[] temp3 = new short[dotsnumber];
            short[] temp4 = new short[dotsnumber];
            Int16 temp = new short();
            for (int i = 0; i < dotsnumber; i++)
            {
                ReadInt16(buffer, ref offset, ref temp);
                offset += 2;
                temp1[i] = temp;
            }
            for (int i = 0; i < dotsnumber; i++)
            {
                ReadInt16(buffer, ref offset, ref temp);
                offset += 2;
                temp2[i] = temp;
            }
            for (int i = 0; i < dotsnumber; i++)
            {
                ReadInt16(buffer, ref offset, ref temp);
                offset += 2;
                temp3[i] = temp;
            }
            for (int i = 0; i < dotsnumber; i++)
            {
                ReadInt16(buffer, ref offset, ref temp);
                offset += 2;
                temp4[i] = temp;
            }
            message = new Message(temp1,temp2,temp3,temp4);
            return true;

        }

        /// <summary>
        ///  读取一个字节
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ReadByte(byte[] buffer, ref int offset, ref byte value)
        {
            if (IsCanRead(buffer, offset + 1))
            {
                value = buffer[offset];
                offset += 1;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 读取16位有符号数
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ReadInt16(byte[] buffer, ref int offset, ref short value)
        {
            if (IsCanRead(buffer, offset + 2))
            {
                var b2 = new byte[2];
                b2[0] = buffer[offset + 1];
                b2[1] = buffer[offset];
                value = BitConverter.ToInt16(b2, 0);
                offset += 2;
                return true;
            }
            return false;
        }

        public static bool IsCanRead(byte[] buffer, int offset)
        {
            return buffer.Length >= offset;
        }
    }
}