using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalMapper.App.Libraries
{
    public class ImageHelper
    {
        public static string BuildDataUrl(string contentType, string base64)
        {
            return $"data:{contentType};base64,{base64}";
        }
    }
}
