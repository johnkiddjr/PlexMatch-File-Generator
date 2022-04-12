﻿namespace PlexMatchGenerator.Constants
{
    public class RegexConstants
    {
        //public const string RootPathMatchPattern = @"([a-zA-Z\\\/]:[a-zA-Z0-9\\\/]*|\\{2}[a-zA-Z0-9]*:?[0-9]{0,5}[\\\/a-zA-Z0-9]*|[\\\/a-zA-Z0-9]*)";
        public const string RootPathMatchPattern = "([a-zA-Z\\\\\\/]:[^:?*\"><|\\r\\n\\t\\f\\v]*|\\\\{2}[a-zA-Z0-9]*:?[0-9]{0,5}[^:?*\"><|\\r\\n\\t\\f\\v]*|[^:?*\"><|\\r\\n\\t\\f\\v]+)";
    }
}
