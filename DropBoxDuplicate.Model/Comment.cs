using System;

namespace DropBoxDuplicate.Model
{
    public class Comment
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public DateTimeOffset PostDate { get; set; }
        public IdentityUser User { get; set; }
        public Files File { get; set; }

    }
}