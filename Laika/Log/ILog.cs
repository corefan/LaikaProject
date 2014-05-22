using System;

namespace Laika.Log
{
    /// <summary>
    /// 파일 로그 인터페이스. FileLogFactory로 생성하며 스레드로 부터 안전함.
    /// </summary>
    public interface ILog : IDisposable
    {
        /// <summary>
        /// 일반 정보 기록
        /// </summary>
        /// <param name="format">복합형식 문자열 입니다.</param>
        /// <param name="args">서식을 지정할 개체 입니다.</param>
        void INFO_LOG(string format, params object[] args);
        
        /// <summary>
        /// 디버그 정보 기록
        /// </summary>
        /// <param name="format">복합형식 문자열 입니다.</param>
        /// <param name="args">서식을 지정할 개체 입니다.</param>
        void DEBUG_LOG(string format, params object[] args);
        
        /// <summary>
        /// 경고 정보 기록
        /// </summary>
        /// <param name="format">복합형식 문자열 입니다.</param>
        /// <param name="args">서식을 지정할 개체 입니다.</param>
        void WARNING_LOG(string format, params object[] args);
        
        /// <summary>
        /// 치명적인 정보 기록
        /// </summary>
        /// <param name="format">복합형식 문자열 입니다.</param>
        /// <param name="args">서식을 지정할 개체 입니다.</param>
        void FATAL_LOG(string format, params object[] args);
        
        /// <summary>
        /// 오류 정보 기록
        /// </summary>
        /// <param name="format">복합형식 문자열 입니다.</param>
        /// <param name="args">서식을 지정할 개체 입니다.</param>
        void ERROR_LOG(string format, params object[] args);
    }
}
