using System;
using System.Collections.Generic;

namespace Example.Models
{
    public partial class MessageBoard
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Content { get; set; }
        public long CreateTime { get; set; }
        public long? UpdateTime { get; set; }
    }
}
