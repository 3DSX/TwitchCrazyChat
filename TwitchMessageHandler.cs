using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Reflection;
using System.Net;
using Starksoft.Aspen.Proxy;

namespace Palumbeando;

public class TwitchMessageHandler
{
    bool isRunning;
    bool hasStarted;

    static string server = "irc.chat.twitch.tv";
    static int port = 6667;

    static List<string> frasesPalumberas = new()
    {
        "Tania esta muy gordilla",
        "Hapyyuki es una streamer loca",
        "Me gustan los osos panda y el bambu de caña fresca",
        "Sabes que cree codigo lyoko?",
        "estoy que reviento demasiado gorda",
        "no me puedo sostener en la cama, se parte en tres",
        "cuando me caigo de la cama, los bomberos vienen a rescatarme",
        "oye que si te pasas de liste te vienes conmigo a la fakin corte",
        "no voy a parar hasta que me coma mi bambu",
        "tengo un oso panda en el jardin, quieres verlo?",
        "Si no como bambu, me entra el sindrome de abstinencia debido a mi addicion al mismo",
        "Joder, que gorda estoy, no quepo ni en el coche",
        "De pequeña intente subirme a un coche telerigido y se hundia",
        "Soy una gordilla y abogada en Gomez & Palumbo LLC",
        "Hola Soy Tania Palumbo Martinez Y Hablo Español, I came to swetherlands office...",
        "Hoy toca ir al mcdonals que tengo mucha hambre",
        "Acabaremos con los panchos, todos unidos",
        "Icarly ha sacado segunda temporada",
        "Me entra cagalera si no tengo comida cerca",
        "Estoy que no paro reboto todos los ataques en mi tripa",
        "Cansada de ver que todos creeis que soy un bot",
        "No hay nada mejor que un pollo dandole vueltas a la cuerda para que furule",
        "Yo de verdad que tambien lo cuido pero que colegeo!!",
        "Misco es un gato perro y se lo ha llevado a la caja de zapatos rota",
        "Losgatiwisxd es el stream mas random del mundo",
        "Has hecho mago o algo, pregunta",
        "Es que soy una pro",
        "Que tienes gatos o que pasa",
        "Puente de belen caminito de beleeen",
        "opaaaa que pedorra soy",
        "me gusta la magdanela",
        "me gusta sam puket",
        "el viernes banearon a un tonto",
        "a dormir con amor",
        "te espera doraimon detras del armario"
    };

    public bool ManageChatLog { get; }

    string channelName; // in lowercase

    ProxyData proxyData;
    BotDetails botDetails;

    TcpClient stabilishedTcpClient;
    bool tcpClientSuccess;

    int chatLogCount;

    public TwitchMessageHandler(string channel, BotDetails botDetails, ProxyData proxyData, bool chatLog = false)
    {
        this.botDetails = botDetails;
        this.proxyData = proxyData;
        this.channelName = channel;

        this.ManageChatLog = chatLog;
    }

    public async Task SendMessage()
    {
        Console.WriteLine($"Estoy gorda: {botDetails.Nick}");

        await Task.Run(async () =>
        {
            Task proxyTask = CreateSocketClient(proxyData);
            await proxyTask;

            if (!tcpClientSuccess)
            {
                Console.WriteLine($"Failed to connect {botDetails.Nick} to proxy {proxyData.Host}:{proxyData.Port}");
                return;
            }

            else
                Console.WriteLine($"Successfuly connected{proxyData.Host}:{proxyData.Port}");
        });


        using (stabilishedTcpClient)
        {
            Console.WriteLine($"Connecting to {server}\n Account: {botDetails.Nick} using SOCKS5 @{((IPEndPoint)stabilishedTcpClient.Client.RemoteEndPoint).Address}:{((IPEndPoint)stabilishedTcpClient.Client.RemoteEndPoint).Port}");
            Console.WriteLine($"Connected: {stabilishedTcpClient.Connected}");

            var stream = stabilishedTcpClient.GetStream();
            var writer = new StreamWriter(stream);
            var reader = new StreamReader(stream);

            try
            {
                Console.WriteLine("begin Flus");

                writer.WriteLine("CAP REQ :twitch.tv/tags twitch.tv/commands");
                writer.WriteLine($"PASS oauth:{botDetails.BotToken}");
                writer.WriteLine($"NICK {botDetails.Nick}");
                writer.WriteLine($"USER {botDetails.Nick} 8 * :{botDetails.Nick}");

                writer.WriteLine($"JOIN #{channelName}");
                writer.Flush();


                Console.WriteLine("Flused");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace + ex.InnerException);
            }

            isRunning = true;

            Console.WriteLine("Stream LISTO:" + writer.BaseStream.CanRead);

            while (stabilishedTcpClient.Connected && isRunning)
            {
                string data = "";

                try
                {
                    if (chatLogCount > 13 && ManageChatLog)
                        data = reader.ReadLineSingleBreak();

                    else if (chatLogCount > 13 && !hasStarted)
                        data = reader.ReadLineSingleBreak();

                    else if (chatLogCount <= 13)
                        data = reader.ReadLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception ocurred: maybe the chat was cleared?\nTrace: {ex.StackTrace}");
                }

                if (!string.IsNullOrEmpty(data))
                {
                    if ((chatLogCount <= 13 && !hasStarted) || ManageChatLog)
                        Console.WriteLine(data); // global

                    chatLogCount++; // for each instance

                    if ((data.ToLower().Contains("a comer") || data.ToLower().Contains("que pancho")) && !hasStarted)
                    {
                        Console.WriteLine("Empezando Palumbeo");
                        await Task.Delay(500); // allow all bots to read the message

                        try
                        {
                            writer.WriteLine($"PRIVMSG #{channelName} :Hola Soy {botDetails.Nick} y te voy a palumbear haciendo el {new Random().Next(30, 70)}");
                            writer.Flush();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message + ex.StackTrace + ex.InnerException);
                        }

                        hasStarted = true;

                        await Task.Delay(3000);
                    }

                    /*
                    if (data.ToLower().Contains("tania muere") || data.ToLower().Contains("toma bambu") || data.ToLower().Contains("tania fuera"))
                    {
                        Console.WriteLine($"{botDetails.Nick} ha parado de palumbear.");
                        stream.Close();

                        isRunning = false;
                    }
                    */

                }

                // Palumbea
                if (hasStarted)
                {
                    Console.WriteLine($"Sent message. : {chatLogCount}");

                    try
                    {
                        writer.WriteLine($"PRIVMSG #{channelName} :{frasesPalumberas[new Random().Next(0, frasesPalumberas.Count - 1)]}");
                        writer.Flush();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message + ex.StackTrace + ex.InnerException);
                    }

                    await Task.Delay(2700);
                }

                await Task.Delay(100);
            }

            writer.Dispose();
            reader.Dispose();
        }
    }

    Task CreateSocketClient(ProxyData data)
    {
        Socks5ProxyClient proxyClient;

        if (string.IsNullOrEmpty(data.Username) || string.IsNullOrEmpty(data.Password))
            proxyClient = new Socks5ProxyClient(data.Host, data.Port);
        else
            proxyClient = new Socks5ProxyClient(data.Host, data.Port, data.Username, data.Password);

        try
        {
            stabilishedTcpClient = proxyClient.CreateConnection(server, port);
        }
        catch (Exception)
        {
            Console.WriteLine($"Timeout connecting to proxy {proxyData.Host}:{proxyData.Port}");
            return Task.FromResult(false);
        }

        tcpClientSuccess = true;
        return Task.FromResult(true);
    }

}

