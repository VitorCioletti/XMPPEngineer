using System;
using System.Threading;
using XMPPEngineer;
using XMPPEngineer.Client;
using XMPPEngineer.Im;

namespace XMPPEngineerTest
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            const string _server = "crio";
            const string _hostname = "crio";
            const string _usuario = "sistema";
            const string _senha = "churros";

            Action<string> imprime = s => Console.WriteLine($"{DateTime.Now} {s}");

            imprime("Criando cliente...");

            var client = new XmppClient(
                _hostname,
                _usuario,
                _senha,
                _server,
                validate: (sender, certificate, chain, sslPolicyErrors) => true
            );

            imprime("Conectando...");

            client.Connect();

            imprime("Conectado");

            Jid telema = $"usuario@{_hostname}";

            client.SendMessage(telema, body: "Teste de abertura do chat", type: MessageType.Chat);

            Jid troll = $"nikolas@{_hostname}";
            client.SendMessage(troll, body: "<attention xmlns=\"urn: xmpp:attention: 0\"/><buzz xmlns = \"http://www.jivesoftware.com/spark\" /><attention xmlns =\"urn:xmpp:attention:0\"/><buzz xmlns = \"http://www.jivesoftware.com/spark\" />", type: MessageType.Chat);


            Action<MessageEventArgs, string> responde = (e, s) =>
            {
                client.SendMessage(e.Message.From, s, type: MessageType.Chat);
                imprime($"Respondido \"{s}\"");
            };

            client.Message += (sender, e) =>
            {
                if (e.Message.Type == MessageType.Error)
                    throw new Exception($"FUDEU. Motivo: {e.Message}");

                var sair = e.Message.Body.Trim().ToLower() == "sair";

                imprime($"Recebido \"{e.Message.Body}\"");

                if (e.Message.Body.Trim().ToLower() == "ping")
                    responde(e, "pong");

                if (e.Message.Body.Trim().ToLower() == "pong")
                    responde(e, "ping");

                if (sair)
                {
                    responde(e, "Nunca!");
                    //Environment.Exit(0);
                }
            };

            client.Error += (sender, e) =>
            {
                Console.Write($"Erro: {e.Exception}");
            };

            Thread.Sleep(-1);
        }
    }
}
