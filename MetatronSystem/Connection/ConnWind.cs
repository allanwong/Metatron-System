using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WAPIWrapperCSharp;

namespace Connection
{
    public class ConnWind
    {
        public static WindAPI w = new WindAPI();

        /// <summary>
        /// Open the Wind connection 
        /// </summary>
        public static void windEnsureStart()
        {
            if (!ConnWind.w.isconnected())
            {
                if (ConnWind.w.start() != 0)
                {
                    throw new Exception("windEnsureStart: wind fails to start/connect");
                }
            }
        }

        /// <summary>
        /// Check the data returned by Wind is correct
        /// </summary>
        /// <param name="wd">Wind Data Format</param>
        public static void windEnsureNoErr(WindData wd)
        {
            if (wd.errorCode != 0)
            {
                throw new Exception("windEnsureConnect: " + wd.GetErrorMsg());
            }
        }
    }
}
