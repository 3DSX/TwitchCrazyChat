namespace Palumbeando;

    public class ProxyData
    {
        public string Host { get; set; }
        public int Port { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }

        public ProxyData(string host, int port)
        {
            this.Host = host;
            this.Port = port;
        }

        public ProxyData(string host, int port, string username, string password) : this(host, port)
        {
            this.Username = username;
            this.Password = password;
        }

    }

