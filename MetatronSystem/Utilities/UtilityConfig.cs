using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Collections.Specialized;

namespace Utilities
{
    class UtilityConfig
    {
        /// <summary>
        /// return the key value in certain section
        /// </summary>
        /// <param name="strKey">Key name</param>
        /// <param name="strSname">Section name</param>
        /// <returns></returns>
        public static string GetAppConfig(string strKey, string strSname)
        {
            System.Collections.Specialized.NameValueCollection NCSection = (System.Collections.Specialized.NameValueCollection)ConfigurationManager.GetSection(strSname);
            return NCSection[strKey].ToString();
        }
    }
}
