//using SuperSocket.Common;
//using SuperSocket.SocketBase.Protocol;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace YzProject.SocketService.SuperSocketHelper
//{
//    /// <summary>
//    /// （2）、建立一个数据接收过滤器帮助 类，作为过滤器的继承的父类，主要的作用是用来接收处理客户端传类的二进制字符，返回有效的数据部分。
//    /// 处理获取的所有数据
//    /// </summary>
//    /// <typeparam name="TRequestInfo"></typeparam>
//    public abstract class ReceiveFilterHelper<TRequestInfo> : ReceiveFilterBase<TRequestInfo>
//         where TRequestInfo : IRequestInfo
//    {
//        private SearchMarkState<byte> m_BeginSearchState;
//        private SearchMarkState<byte> m_EndSearchState;
//        private bool m_FoundBegin = false;
//        protected TRequestInfo NullRequestInfo = default(TRequestInfo);
//        /// <summary>
//        /// 初始化实例
//        /// </summary>
//        protected ReceiveFilterHelper()
//        {

//        }
//        /// <summary>
//        /// 过滤指定的会话
//        /// </summary>
//        /// <param name="readBuffer">数据缓存</param>
//        /// <param name="offset">数据起始位置</param>
//        /// <param name="length">缓存长度</param>
//        /// <param name="toBeCopied"></param>
//        /// <param name="rest"></param>
//        /// <returns></returns>
//        public override TRequestInfo Filter(byte[] readBuffer, int offset, int length, bool toBeCopied, out int rest)
//        {
//            rest = 0;
//            int searchEndMarkOffset;
//            int searchEndMarkLength;
//            //在此处做了处理，将接收到的第一个字符作为起始过滤标志，到结束。返回指定长度的数据。
//            byte[] startMark = new byte[] { readBuffer[offset] };
//            byte[] endMark = new byte[] { 0xff };
//            m_BeginSearchState = new SearchMarkState<byte>(startMark);
//            m_EndSearchState = new SearchMarkState<byte>(endMark);
//            //上一个开始标记长度
//            int prevMatched = 0;
//            int totalParsed = 0;

//            if (!m_FoundBegin)
//            {
//                prevMatched = m_BeginSearchState.Matched;
//                int pos = readBuffer.SearchMark(offset, length, m_BeginSearchState, out totalParsed);

//                if (pos < 0)
//                {
//                    //不要缓存无效数据
//                    if (prevMatched > 0 || (m_BeginSearchState.Matched > 0 && length != m_BeginSearchState.Matched))
//                    {
//                        State = FilterState.Error;
//                        return NullRequestInfo;
//                    }

//                    return NullRequestInfo;
//                }
//                else //找到匹配的开始标记
//                {
//                    //But not at the beginning
//                    if (pos != offset)
//                    {
//                        State = FilterState.Error;
//                        return NullRequestInfo;
//                    }
//                }

//                //找到开始标记
//                m_FoundBegin = true;

//                searchEndMarkOffset = pos + m_BeginSearchState.Mark.Length - prevMatched;

//                //This block only contain (part of)begin mark
//                if (offset + length <= searchEndMarkOffset)
//                {
//                    AddArraySegment(m_BeginSearchState.Mark, 0, m_BeginSearchState.Mark.Length, false);
//                    return NullRequestInfo;
//                }

//                searchEndMarkLength = offset + length - searchEndMarkOffset;
//            }
//            else//Already found begin mark
//            {
//                searchEndMarkOffset = offset;
//                searchEndMarkLength = length;
//            }

//            while (true)
//            {
//                var prevEndMarkMatched = m_EndSearchState.Matched;
//                var parsedLen = 0;
//                var endPos = readBuffer.SearchMark(searchEndMarkOffset, searchEndMarkLength, m_EndSearchState, out parsedLen);

//                //没有找到结束标记
//                if (endPos < 0)
//                {
//                    rest = 0;
//                    if (prevMatched > 0)//还缓存先前匹配的开始标记
//                        AddArraySegment(m_BeginSearchState.Mark, 0, prevMatched, false);
//                    AddArraySegment(readBuffer, offset, length, toBeCopied);
//                }

//                //totalParsed += parsedLen;
//                //rest = length - totalParsed;
//                totalParsed = 0;
//                byte[] commandData = new byte[BufferSegments.Count + prevMatched + totalParsed];

//                if (BufferSegments.Count > 0)
//                    BufferSegments.CopyTo(commandData, 0, 0, BufferSegments.Count);

//                if (prevMatched > 0)
//                    Array.Copy(m_BeginSearchState.Mark, 0, commandData, BufferSegments.Count, prevMatched);

//                Array.Copy(readBuffer, offset, commandData, BufferSegments.Count + prevMatched, totalParsed);

//                var requestInfo = ProcessMatchedRequest(commandData, 0, commandData.Length);


//                Reset();
//                return requestInfo;
//                if (prevMatched > 0)//Also cache the prev matched begin mark
//                    AddArraySegment(m_BeginSearchState.Mark, 0, prevMatched, false);
//                AddArraySegment(readBuffer, offset, length, toBeCopied);
//                //return NullRequestInfo;
//            }
//        }
//        /// <summary>
//        /// Processes the matched request.
//        /// </summary>
//        /// <param name="readBuffer">The read buffer.</param>
//        /// <param name="offset">The offset.</param>
//        /// <param name="length">The length.</param>
//        /// <returns></returns>
//        protected abstract TRequestInfo ProcessMatchedRequest(byte[] readBuffer, int offset, int length);

//        /// <summary>
//        /// Resets this instance.
//        /// </summary>
//        public override void Reset()
//        {
//            m_BeginSearchState.Matched = 0;
//            m_EndSearchState.Matched = 0;
//            m_FoundBegin = false;
//            base.Reset();
//        }
//    }
//}
