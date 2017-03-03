using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphDemo.DataProvider
{
    public class MessageBuffer
    {
        private int _mStartIndex;
        private int _mCurrentDataLength;
        private byte[] _mBuffer;
        private Resolver _mResolver;
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="bufferLength">长度</param>
        public MessageBuffer(int bufferLength, Resolver mResolver)
        {
            if (bufferLength <= 0)
            {
            }
            _mBuffer = new byte[bufferLength];
            _mStartIndex = 0;
            _mCurrentDataLength = 0;
            _mResolver = mResolver;
        }

        public void PutData(byte[] data)
        {
            AddData(data);
            if (_mResolver == null)
            {

            }
            else
            {
                int length = _mResolver.Resolve(GetData());
                if (length != 0)
                {
                    Release(length);
                }
            }
        }

        /// <summary>
        ///  向缓冲区中添加数据
        /// </summary>
        /// <param name="data">数据</param>
        public void AddData(byte[] data)
        {
            if (data == null)
            {
                new Exception(null);
            }
            if ((_mBuffer.Length - _mCurrentDataLength) < data.Length)
            {
                new Exception("缓冲区溢出，抛出异常");
            }
            if ((_mBuffer.Length - _mStartIndex) >= _mCurrentDataLength)
            {
                if (((_mBuffer.Length - _mStartIndex) - _mCurrentDataLength) >= data.Length)
                {
                    Array.Copy(data, 0, _mBuffer, _mStartIndex
                              + _mCurrentDataLength, data.Length);
                    _mCurrentDataLength = _mCurrentDataLength + data.Length;

                    return;
                }
                else
                {
                    Array.Copy(data, 0, _mBuffer, _mStartIndex
                            + _mCurrentDataLength, (_mBuffer.Length - _mStartIndex)
                            - _mCurrentDataLength); // 填充缓冲区后半部分
                    Array.Copy(data, (_mBuffer.Length - _mStartIndex)
                            - _mCurrentDataLength, _mBuffer, 0, data.Length
                            - ((_mBuffer.Length - _mStartIndex) - _mCurrentDataLength)); // 填充缓冲区前半部分
                    _mCurrentDataLength = _mCurrentDataLength + data.Length;

                    return;

                }
            }
            else
            {
                Array.Copy(data, 0, _mBuffer, _mCurrentDataLength
                        - (_mBuffer.Length - _mStartIndex), data.Length);
                _mCurrentDataLength = _mCurrentDataLength + data.Length;
                return;
            }

        }
        /// <summary>
        ///  从缓冲区中取出有效数据
        /// </summary>
        /// <returns></returns>
        public virtual byte[] GetData()
        {
            byte[] data = null;
            if ((_mBuffer.Length - _mStartIndex) >= _mCurrentDataLength)
            {
                data = new byte[_mCurrentDataLength];
                Array.Copy(_mBuffer, _mStartIndex, data, 0, _mCurrentDataLength);
            }
            else
            {
                data = new byte[_mCurrentDataLength];
                Array.Copy(_mBuffer, _mStartIndex, data, 0, _mBuffer.Length
                        - _mStartIndex);
                Array.Copy(_mBuffer, 0, data, _mBuffer.Length - _mStartIndex,
                        data.Length - (_mBuffer.Length - _mStartIndex));
            }
            return data;
        }
        /// <summary>
        ///  从缓冲区中取出有效数据
        /// </summary>
        /// <param name="dataLength">数据长度</param>
        /// <returns></returns>
        public byte[] GetData(int dataLength)
        {
            if (dataLength <= 0)
            {

            }
            if (_mCurrentDataLength < dataLength)
            {

            }
            byte[] data = null;
            if ((_mBuffer.Length - _mStartIndex) >= _mCurrentDataLength)
            {
                data = new byte[dataLength];
                Array.Copy(_mBuffer, _mStartIndex, data, 0, dataLength);
            }
            else
            {
                if ((_mBuffer.Length - _mStartIndex) < dataLength)
                {
                    data = new byte[dataLength];
                    Array.Copy(_mBuffer, _mStartIndex, data, 0, _mBuffer.Length
                            - _mStartIndex);
                    Array.Copy(_mBuffer, 0, data, _mBuffer.Length - _mStartIndex,
                            data.Length - (_mBuffer.Length - _mStartIndex));
                }
                else
                {
                    data = new byte[dataLength];
                    Array.Copy(_mBuffer, _mStartIndex, data, 0, dataLength);
                }

            }
            return data;
        }
        /// <summary>
        ///  重置缓冲区，不释放缓冲区内存空间
        /// </summary>
        public void clear()
        {
            _mStartIndex = 0;
            _mCurrentDataLength = 0;
        }
        /// <summary>
        /// 回收缓冲区占用的内存空间
        /// </summary>
        public void recycle()
        {
            _mBuffer = null;
            _mStartIndex = 0;
            _mCurrentDataLength = 0;
        }
        /// <summary>
        /// 针对缓冲区中有效数据的空间释放
        /// </summary>
        /// <param name="length"></param>
        public void Release(int length)
        {

            if (length <= 0)
            {

            }
            if (_mCurrentDataLength < length)
            {

            }
            if ((_mBuffer.Length - _mStartIndex) > length)
            {
                _mStartIndex = _mStartIndex + length;
                _mCurrentDataLength = _mCurrentDataLength - length;

            }
            else if ((_mBuffer.Length - _mStartIndex) == length)
            { 
                _mStartIndex = 0;
                _mCurrentDataLength = _mCurrentDataLength - length;

            }
            else
            { 
                _mStartIndex = length - (_mBuffer.Length - _mStartIndex);
                _mCurrentDataLength = _mCurrentDataLength - length;

            }
        }
    }
}
