using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreThree.Models
{
    public class FileModel
    {
        public string FileName { get; set; }
        public string FileType { get; set; }
        public double FileSize { get; set; }
        public DateTime CreateTime { get; set; }
        public string FilePath { get; set; }
        public Unit Unit { get; set; }
    }

    public enum Unit
    {
        B,KB,MB,G
    }
}
