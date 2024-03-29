﻿namespace PlexMatchGenerator.Constants
{
    public class MessageConstants
    {
        public const string EnterPlexToken = "Please enter your Plex Token: ";
        public const string TokenInvalid = "Plex Server Token entered must be valid and set";
        public const string EnterPlexServerUrl = "Please enter your Plex Server URL: ";
        public const string UrlInvalid = "Plex Server URL must be set, and must start with either http:// or https://";
        public const string ArgumentsMissing = "Plex Server URL and Plex Token are required! Exiting...";
        public const string FolderMissingOrInvalid = "Missing or Invalid Folder: {path}";
        public const string CompletedMessage = "Operation Completed";
        public const string PlexMatchWritten = ".plexmatch file written successfully for: {mediaTitle}";
        public const string PlexMatchSeasonWritten = ".plexmatch file written successfully for: {mediaTitle} Season {seasonNumber} in path {seasonPath}";
        public const string NoMediaFound = "No media location found for: {mediaItemTitle}";
        public const string NoLocationInfoForItemFound = "Item with title {itemTitle} and ID {itemId} returned no location information";
        public const string LoggerAttachedMessage = "Logger attached. Startup complete. Running file generator...";
        public const string LibrariesNoResults = "No data or malformed data received from server when querying for libraries";
        public const string LibraryItemsNoResults = "Library {libraryName} of type {libraryType} with ID {libraryID} returned no items";
        public const string LibraryProcessedSuccess = "Processed results for {library}: {records} processed";
        public const string LibraryProcessedSuccessWithSkipped = "Processed results for {library}: {records} processed, {skipped} skipped";
        public const string NoWriteBecauseDisabled = ".plexmatch file not written because overwrite disabled for item: {mediaTitle}";
        public const string LibrarySkipped = "Library {library} skipped because it is not in the list of libraries to process";
        public const string ShowSkipped = "Show {show} skipped because it is not in the list of shows to process";

        //exception messages
        public const string ExceptionHeaderMessage = "An unhandeled exception occurred details below:";
        public const string ExceptionTypeMessage = "Exception Type: {exceptionType}";
        public const string ExceptionMessageMessage = "Exception Message: {exceptionMessage}";
        public const string ExceptionInnerExceptionTypeMessage = "Inner Exception Type: {innerType}";
        public const string ExceptionInnerExceptionMessageMessage = "Inner Exception Message: {innerMessage}";
        public const string ExceptionSourceMessage = "Exception Source: {exceptionSource}";
        public const string ExceptionStackTraceMessage = "Exception Stack Trace: {stackTrace}";
    }
}
