﻿using Microsoft.AspNetCore.Mvc;

namespace PxWeb.Code.Api2.Serialization
{
    public interface ISerializeManager
    { 
        IDataSerializer GetSerializer(string outputFormat, List<string> outputFormatParams);
    }
}
