using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Infrastructure
{
    public class GetListResponse
    {
        public Array Data { get; set; }
        public int TotalSize { get; set; }
        public GetListResponse(Array data, int totalSize)
        {
            Data = data;
            TotalSize = totalSize;
        }
    }
}
