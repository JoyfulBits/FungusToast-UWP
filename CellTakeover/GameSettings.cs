﻿namespace FungusToast
{
    internal static class GameSettings
    {
#if DEV
        public const string BaseURL = "http://localhost:4000/api";
#else
        public const string BaseURL = "http://some production url";
#endif
    }
}
