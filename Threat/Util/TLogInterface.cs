﻿namespace MESHelper.Threat.Util
{
    public interface TLogInterface
    {
        void Debug(string message);
        void Info(string message);
        void Warn(string message);
        void Error(string message);
    }
}
