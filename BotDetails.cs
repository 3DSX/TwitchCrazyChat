using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Palumbeando
{
    public class BotDetails
    {
        public BotDetails(string nick, string botToken)
        {
            this.Nick = nick;
            this.BotToken = botToken;
        }

        public string Nick {get; set;}
        public string BotToken { get; set; }

    }
}
