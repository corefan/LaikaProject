﻿using System;

namespace Laika.Log
{
	/// <summary>
	/// 파일로그 인스턴스 팩토리 클래스
	/// </summary>
	public class LogFactory
	{
		/// <summary>
		/// 파일로그 인스턴스를 생성하여 리턴합니다.
		/// </summary>
		/// <param name="param">원하는 옵션을 파라미터로 지정합니다.</param>
		/// <returns>로그 인스턴스를 리턴합니다.</returns>
		public static ILog CreateLogObject(LogParameter param)
		{
			if (param.OnlyTraceDebugMode == true)
			{
				return new TraceLog(param);
			}
			else if (param.Type == PartitionType.NONE)
			{
				return new NormalLog(param);
			}
			else if (param.Type == PartitionType.FILE_SIZE)
			{
				return new SizeLog(param);
			}
			else if (param.Type == PartitionType.TIME)
			{
				return new TimeLog(param);
			}
			else if (param.Type == PartitionType.WINDOWS_EVENT)
			{
				return new EventLog(param);
			}
			else
				throw new ArgumentException("Invalid partition type.");
		}
	}
}
