namespace DaisyControl_AI.Storage.DataAccessLayer
{
    public interface IDataItem
    {
        public string Id { get; set; }
        public DateTimeOffset LastModifiedAtUtc { get; set; }
        public DateTimeOffset CreatedAtUtc { get; set; }

        public long Revision { get; set; }
    }
}
