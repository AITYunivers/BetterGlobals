using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SharpEdif.User
{
    public class EditData
    {
        public FusionProperties props;
    }
    public class RunData
    {
        public Dictionary<string, object> gV = new Dictionary<string, object>();
        public Dictionary<string, bool> gVSet = new Dictionary<string, bool>();
    }
    
}