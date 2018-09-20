﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GoferEx.Core
{
    public class SchemeObject
    {
        public string ProviderName { get; set; }
        public string LoginAuthUri { get; set; }
        public string DisplayName { get; set; }

        public SchemeObject(string provider, string displayName, string loginAuthUri)
        {
            ProviderName = provider;
            DisplayName = displayName;
            LoginAuthUri = loginAuthUri;
        }
    }
}