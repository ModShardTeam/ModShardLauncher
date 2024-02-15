using ModShardLauncher.Controls;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace ModShardLauncher.Mods
{
    public class ModInterfaceEngine : EngineObject
    {
        public static dynamic Instance = new ModInterfaceEngine();
        public string ModPath;
        public int CurrentCallbackID = 0;
        public bool IsLoadHooks = false;

        public ModInterfaceEngine()
        {
            Variables = new Dictionary<string, EngineObject>();
            HookDelegates = new Dictionary<string, ModDelegate>();
            Callbacks = new Dictionary<int, Action<string>>();
            Initalize();
        }

        public void Initalize()
        {
            HookDelegates.Add("OnGameStart", new ModDelegate());
        }

        public Dictionary<string, EngineObject> Variables;

        public Dictionary<string, ModDelegate> HookDelegates;

        public Dictionary<int, Action<string>> Callbacks;

        public void Execute(string script, params object[] objects)
        {
            if (!ModInterfaceServer.Client.Connected) return;
            var code = script;
            objects.ToList().ForEach(o => code += " " + o.ToString());
            ModInterfaceServer.SendScript(code);
        }
        public void DoHook(string hook)
        {
            var data = hook.Split(" ");
            var parameters = data.Length > 1 ? data.Skip(1).ToArray() : new object[] { };
            if (HookDelegates.ContainsKey(data[0])) HookDelegates[data[0]].Invoke(parameters);
        }
        public void AddHook(string hook, Action<object[]> action)
        {
            if (HookDelegates.ContainsKey(hook)) HookDelegates[hook] += action;
        }
    }

    public class ModDelegate : EngineObject
    {
        private List<Action<object[]>> _actions = new List<Action<object[]>>();
        public static ModDelegate operator +(ModDelegate @delegate, Action<object[]> action)
        {
            @delegate._actions.Add(action);
            return @delegate;
        }
        public void Invoke(object[] obj)
        {
            foreach (var i in _actions)
                i.Invoke(obj);
        }
    }

    public class EngineObject : DynamicObject
    {
        private Dictionary<string, object> _values;
        public EngineObject() => _values = new Dictionary<string, object>();

        public object GetPropertyValue(string propertyName)
        {
            if (_values.ContainsKey(propertyName)) return _values[propertyName];
            return null;
        }
        public void SetPropertyValue(string propertyName, object value)
        {
            if (_values.ContainsKey(propertyName)) _values[propertyName] = value;
            else _values.Add(propertyName, value);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = GetPropertyValue(binder.Name);
            return result == null ? false : true;
        }
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            SetPropertyValue(binder.Name, value);
            return true;
        }
        public override bool TryInvokeMember(InvokeMemberBinder binder, object?[]? args, out object? result)
        {
            result = null;
            var member = GetPropertyValue(binder.Name);
            if (member == null) return false;
            if (member.GetType() == typeof(Action<object[]>))
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i].GetType() == typeof(string))
                        args[i] = "\"" + args[i].ToString().Replace(" ", "_") + "\"";

                }
                (member as Action<object[]>)(args);
                return true;
            }
            else return false;
        }
    }

    public class ModInterfaceServer
    {
        public static Socket Server { get; private set; }

        public static Socket Client { get; private set; }

        internal static ScriptEnginePage Window;

        public static void StartServer(int port)
        {
            string host = "127.0.0.1";
            IPAddress ip = IPAddress.Parse(host);
            IPEndPoint ipe = new IPEndPoint(ip, port);

            Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Server.Bind(ipe);
            MessageBox.Show(string.Format(Application.Current.FindResource("ServerStart").ToString(), port.ToString()));

            Window = new ScriptEnginePage();
            Window.Show();

            Thread t = new Thread(() =>
            {
                Server.Listen(0);

                Server.BeginAccept(new AsyncCallback((IAsyncResult ar) =>
                {
                    if (ar.AsyncState as Socket == null || !(ar.AsyncState as Socket).Connected) return;

                    Client = Server.EndAccept(ar);

                    while (Client.Connected)
                    {
                        if (Client.Available == 0) continue;
                        string message = "";
                        byte[] recvData = new byte[1024 * 64];
                        int bytes;
                        bytes = Client.Receive(recvData, recvData.Length, 0);
                        var ID = BitConverter.ToInt32(recvData.Take(4).ToArray());
                        message = Encoding.UTF8.GetString(recvData, 4, bytes - 4).Replace("\0", "");
                        Main.Instance.Dispatcher.Invoke(() =>
                        {
                            if (message.StartsWith("[MSG]"))
                                Window.MsgBox.AppendText("[Client]: " + message.Replace("[MSG]", "") + "\n");
                            else if (message.StartsWith("[CLB]"))
                            {
                                var data = message.Replace("[CLB]", "");
                                ModInterfaceEngine.Instance.Callbacks[ID].Invoke(data);
                                ModInterfaceEngine.Instance.Callbacks.Remove(ID);
                            }
                            else if (message.StartsWith("[HOK]"))
                            {
                                var data = message.Replace("[HOK]", "").Split("<EXTRAMSG>");
                                var extra = data[1].Split("<BUILTIN>");
                                var param = data.Length > 1 ? data.Skip(1).ToArray() : new string[] { };
                                if (extra[1] == "true") ModInterfaceEngine.Instance.DoHook(data[0], param);
                            }
                        });
                    }
                }), Server);
            });

            t.Start();
        }
        public static void SendScript(string script, Action<string> callback)
        {
            var data = BitConverter.GetBytes(script.Length)
                .Concat(BitConverter.GetBytes((int)ModInterfaceEngine.Instance.CurrentCallbackID))
                .Concat(Encoding.Unicode.GetBytes(script));
            Client.Send(data.ToArray());
            ModInterfaceEngine.Instance.Callbacks.Add(ModInterfaceEngine.Instance.CurrentCallbackID, callback);
            ModInterfaceEngine.Instance.CurrentCallbackID++;
            if (ModInterfaceEngine.Instance.CurrentCallbackID >= int.MaxValue / 4)
                ModInterfaceEngine.Instance.CurrentCallbackID = 0;
        }
        public static void SendScript(string script)
        {
            var name = script.Split(' ')[0];
            SendScript(script, ModLoader.ScriptCallbacks.ContainsKey(name)
                ? ModLoader.ScriptCallbacks[name]
                : (str) => {

                });
        }
    }
}