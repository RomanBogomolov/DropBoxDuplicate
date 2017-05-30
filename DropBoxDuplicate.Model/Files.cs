using System;

namespace DropBoxDuplicate.Model
{
    public sealed class Files
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public double? FileSize { get; set; }
        public string FileExtension { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset? LastModifyDate { get; set; }
        public IdentityUser User { get; set; }
    }

    public enum AccessType : byte
    {
        Owner,
        Write,
        Read
    }
}