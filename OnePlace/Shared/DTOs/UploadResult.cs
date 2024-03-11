﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Shared.DTOs
{
    public class UploadResult
    {
        public bool Upload { get; set; }
        public string? FileName { get; set; }
        public string? StoredFileName { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; } = false;
    }
}
