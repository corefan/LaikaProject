using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Laika.Log
{
	internal class TraceLog : IFileLog
	{
		private FileLogParameter Param;
		internal TraceLog(FileLogParameter param)
		{
			Param = param;
		}
		/// <summary>
		/// 일반 정보 기록
		/// </summary>
		/// <param name="format">복합형식 문자열 입니다.</param>
		/// <param name="args">서식을 지정할 개체 입니다.</param>
		public void INFO_LOG(string format, params object[] args)
		{ }

		/// <summary>
		/// 디버그 정보 기록
		/// </summary>
		/// <param name="format">복합형식 문자열 입니다.</param>
		/// <param name="args">서식을 지정할 개체 입니다.</param>
		public void DEBUG_LOG(string format, params object[] args)
		{ }

		/// <summary>
		/// 경고 정보 기록
		/// </summary>
		/// <param name="format">복합형식 문자열 입니다.</param>
		/// <param name="args">서식을 지정할 개체 입니다.</param>
		public void WARNING_LOG(string format, params object[] args)
		{ }

		/// <summary>
		/// 치명적인 정보 기록
		/// </summary>
		/// <param name="format">복합형식 문자열 입니다.</param>
		/// <param name="args">서식을 지정할 개체 입니다.</param>
		public void FATAL_LOG(string format, params object[] args)
		{ }

		/// <summary>
		/// 오류 정보 기록
		/// </summary>
		/// <param name="format">복합형식 문자열 입니다.</param>
		/// <param name="args">서식을 지정할 개체 입니다.</param>
		public void ERROR_LOG(string format, params object[] args)
		{ }
	}
}
