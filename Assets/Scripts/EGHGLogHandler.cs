using System;
using UnityEngine;

namespace KevinSkyba
{
    namespace EGHG
    {
        public class EGHGLogHandler : ILogHandler
        {
            public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
            {
                Debug.unityLogger.logHandler.LogFormat(logType, context, format, args);
            }

            public void LogException(Exception exception, UnityEngine.Object context)
            {
                Debug.unityLogger.LogException(exception, context);
            }
        }
    }
}
