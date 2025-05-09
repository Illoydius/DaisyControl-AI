namespace DaisyControl_AI.Storage.Dtos.Errors
{
    public static class StorageResponseErrorCodes
    {
        /// DaisyControlAddMessageToBufferRequestExecutor Codes 10000-10199
        public const long DaisyControlAddMessageToBufferRequestExecutor_UserNotFound = 10000;
        public const long DaisyControlAddMessageToBufferRequestExecutor_InvalidDto = 10005;

        /// DaisyControlGetMessageFromBufferRequestExecutor Codes 10200-10399
        public const long DaisyControlGetMessageFromBufferRequestExecutor_InvalidDto = 10200;

        /// DaisyControlGetMessageFromBufferRequestExecutor Codes 10400-10599
        public const long DaisyControlUpdateMessageFromBufferRequestExecutor_InvalidDto = 10400;
        public const long DaisyControlUpdateMessageFromBufferRequestExecutor_MessageToUpdateNotFound = 10405;
    }
}
