using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvantestMessagingFoundation
{
    public interface ILogProvider
    {
        /// <summary>
        /// this can be implemented in inheriting class.
        /// </summary>
        /// <param name="message"></param>
        void Log(string message);
    }
}
