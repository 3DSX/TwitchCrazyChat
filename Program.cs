using Palumbeando;

// See https://aka.ms/new-console-template for more information

string channel = "viithi";

/* Load chat files */

List<string> messagesProxyFile = File.ReadAllLines("../../../chatproxys.txt").ToList();
List<string> messagesNicksFile = File.ReadAllLines("../../../chatnicks.txt").ToList();
List<string> messagesBotsFile = File.ReadAllLines("../../../chatbots.txt").ToList();

/* Convert to objects */

List<ProxyData> messagesProxyList = new ();
messagesProxyFile.ForEach(proxy => messagesProxyList.Add(new ProxyData(proxy, 45786, "user", "password")));

List<BotDetails> messagesBotsList = new ();
messagesBotsFile.ForEach(botToken => messagesBotsList.Add(new BotDetails(nick: messagesNicksFile[messagesBotsList.Count], botToken: botToken)));

List<TwitchMessageHandler> runningHandlers = new ();

await Task.Run(async () =>
{
    bool botChatLog = true;
    // Launch each bot
    foreach (BotDetails bot in messagesBotsList)
    {
        int proxyListIndex = new Random().Next(0, messagesProxyList.Count - 1);

        var handler = new TwitchMessageHandler(channel, bot, messagesProxyList[proxyListIndex], botChatLog);
        Task.Run(() => handler.SendMessage());

        runningHandlers.Add(handler);
        messagesProxyList.RemoveAt(proxyListIndex); // dont use this proxy again in runtime
        botChatLog = false;

        await Task.Delay(2000); // global join delay between
    }
});

Console.ReadLine();