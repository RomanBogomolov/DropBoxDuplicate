using System;
using System.Collections.Generic;

namespace DropBoxDuplicate.Model
{
    /// <summary>
    /// Необходим для сериализации в Json
    /// </summary>
    public class Share
    {
        public AccessType AccessAtribute { get; set; }
        public Guid UserId { get; set; }
        public Guid FileId { get; set; }

        public Share(Guid userId, Guid fileId)
        {
            UserId         = userId;
            FileId         = fileId;
            AccessAtribute = AccessType.Read;
        }
    }
    
}