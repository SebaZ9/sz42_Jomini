using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Lidgren.Network;
using System.Net;
using System.IO;
using ProtoBuf;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using ProtoMessageClient;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Extends the ConcurrentQueue to fire an event whenever a new item is enqueued
/// </summary>
/// <typeparam name="T"></typeparam>
public class ConcurrentQueueWithEvent<T> : ConcurrentQueue<T>
{
        public EventWaitHandle eventWaiter { get; set; }

        public ConcurrentQueueWithEvent() : base()
        {
            eventWaiter = new EventWaitHandle(false, EventResetMode.AutoReset);
        }

        /// <summary>
        /// Add a new item to the queue and set the EventWaitHandle
        /// </summary>
        /// <param name="t">Item to be enqueued</param>
        public new void Enqueue(T t)
        {
            base.Enqueue(t);
            eventWaiter.Set();
        }
}

public partial class TextTestClient
{
    public ConcurrentQueueWithEvent<ProtoMessage> protobufMessageQueue;
    public ConcurrentQueueWithEvent<string> stringMessageQueue;
    public Network net;
    public string playerID;

    public TextTestClient()
    {
        protobufMessageQueue = new ConcurrentQueueWithEvent<ProtoMessage>();
        stringMessageQueue = new ConcurrentQueueWithEvent<string>();
    }

    /*************************************
           *** General Commands ***
     *************************************/

    /// <summary>
    /// Log in to the server
    /// </summary>
    /// <param name="user">Username</param>
    /// <param name="pass">Password</param>
    public void LogInAndConnect(string user, string pass, string ipAddress, byte[] key = null)
    {
        net = new Network(this, key);
        net.Connect(user, pass, ipAddress);
        playerID = user;
    }

    public bool IsConnectedAndLoggedIn()
    {
        if (net.GetConnectionStatusString().Equals("Connected") && net.loggedIn)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Clear both message queues- used in tests where we don't care about the login messages
    /// </summary>
    public void ClearMessageQueues()
    {
        // Note- With ConcurrentQueues, it is preferrable to discard the previous queue than to attempt to Dequeue all elements in case the queue is currently being written to
        protobufMessageQueue = new ConcurrentQueueWithEvent<ProtoMessage>();
        stringMessageQueue = new ConcurrentQueueWithEvent<string>();
    }
    public void LogOut()
    {
        ClearMessageQueues();
        net.Disconnect();
    }

    /// <summary>
    /// Gets the next message from the server by repeatedly polling the message queue. 
    /// </summary>
    /// <returns>Message from server</returns>
    /// <throws>TaskCanceledException if task is cancelled</throws>
    public ProtoMessage CheckForProtobufMessage() {
        ProtoMessage m = null;
        var waitHandles = new WaitHandle[] { protobufMessageQueue.eventWaiter, net.ctSource.Token.WaitHandle };
        while (!protobufMessageQueue.TryDequeue(out m)) {
            EventWaitHandle.WaitAny(waitHandles);
            net.ctSource.Token.ThrowIfCancellationRequested();
        }
        Console.WriteLine("Recieve from server: " + m.ActionType + " | " + m.ResponseType + " | " + m.MessageFields);
        return m;
    }

    /// <summary>
    /// Gets the next message from the server by repeatedly polling the message queue
    /// </summary>
    /// <returns>Message from server</returns>
    /// <throws>TaskCanceledException if task is cancelled</throws>
    private string CheckForStringMessage()
    {
        string s = null;
        var waitHandles = new WaitHandle[] { stringMessageQueue.eventWaiter, net.ctSource.Token.WaitHandle };
        while (!stringMessageQueue.TryDequeue(out s))
        {
            EventWaitHandle.WaitAny(waitHandles);
            net.ctSource.Token.ThrowIfCancellationRequested();
        }
        return s;
    }

    /// <summary>
    /// Gets the next message recieved from the server
    /// </summary>
    /// <returns>Task containing the reply as a result</returns>
    public string GetServerMessage()
    {
        string reply;
        reply = CheckForStringMessage();
        return reply;
    }        

    protected virtual void Dispose(bool dispose)
    {
        net.Dispose();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}


public partial class TextTestClient
{

    public class Network : IDisposable
    {
        private TextTestClient tClient;
        RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
        HashAlgorithm hash = new SHA256Managed();
        public NetClient client = null;
        private NetConnection connection;
        private string user;
        private string pass;
        private IPAddress ip = NetUtility.Resolve("192.168.0.16");
        private int port = 8000;
        private NetEncryption encryptionAlg = null;
        /// <summary>
        /// Optional- set encryption key manually for use in testing
        /// </summary>
        private byte[] key;
        public bool autoLogIn { get; set; }
        public bool loggedIn { get; set; }
        public CancellationTokenSource ctSource;

        public Network(TextTestClient tc, byte[] key = null)
        {
            tClient = tc;
            autoLogIn = true;
            this.key = key;
            ctSource = new CancellationTokenSource();
            InitializeClient();
        }


        public string GetConnectionStatusString()
        {
            return client.ConnectionStatus.ToString();
        }


        void InitializeClient()
        {
            NetPeerConfiguration config = new NetPeerConfiguration("test");
            config.SetMessageTypeEnabled(NetIncomingMessageType.ConnectionLatencyUpdated, false);
            config.ConnectionTimeout = 3000f;
            client = new NetClient(config);
        }

        public void Connect(string username, string pass, string ipAddress)
        {
            user = username;
            this.pass = pass;
            client.Start();
            string host = ip.ToString();
            // Encryption would occur somewhere here.
            if (username != null)
            {
                NetOutgoingMessage msg = client.CreateMessage(username);
                msg.Write("TestString");
                NetConnection c = client.Connect(host, port, msg);
            }
            else
            {
                connection = client.Connect(host, port);
            }
            // Start listening for responses
            Thread t_reader = new Thread(new ThreadStart(this.Listen));
            t_reader.Start();
        }
        

        public void Disconnect()
        {
            ctSource.Cancel();
            if (!(client.ConnectionStatus == NetConnectionStatus.None || client.ConnectionStatus == NetConnectionStatus.Disconnected))
            {
                client.Disconnect("Logging out.");
            }
            Dispose();
            client.Shutdown("Client logged out.");
        }
            
        /// <summary>
        /// Computes the hash of a salt appended to source byte array
        /// </summary>
        /// <param name="toHash">bytes to be hashed</param>
        /// <param name="salt">salt</param>
        /// <returns>computed hash</returns>
        public byte[] ComputeHash(byte[] toHash, byte[] salt)
        {
            byte[] fullHash = new byte[toHash.Length + salt.Length];
            toHash.CopyTo(fullHash, 0);
            salt.CopyTo(fullHash, toHash.Length);
            byte[] hashcode = hash.ComputeHash(fullHash);
            return hashcode;
        }

        /*        ------------------ OLD ---------------
        public void Send(ProtoMessage message, bool encrypt = true)
        {
            NetOutgoingMessage msg = client.CreateMessage();
            MemoryStream ms = new MemoryStream();
            try
            {
                Serializer.SerializeWithLengthPrefix<ProtoMessage>(ms, message, ProtoBuf.PrefixStyle.Fixed32);
                msg.Write(ms.GetBuffer());
                if (encryptionAlg != null && encrypt)
                {
                    msg.Encrypt(encryptionAlg);
                }
                var result = client.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
                client.FlushSendQueue();

            }
            catch (Exception e)
            {
                Debug.Log("Connected - remote hail message\n" + e.GetType()+ "\n" + e.Message + "\n" + e.StackTrace);
                Exception inner;
                if(e.InnerException != null) {
                    inner = e.InnerException;
                    Debug.Log("Inner Exception:\n" + inner.GetType()+ "\n" + inner.Message + "\n" + inner.StackTrace);
                }
            }
        }*/

        public void Send(ProtoMessage message, bool encrypt = true) {
            NetOutgoingMessage msg = client.CreateMessage();
            MemoryStream ms = new MemoryStream();
            try {
                Serializer.SerializeWithLengthPrefix<ProtoMessage>(ms, message, PrefixStyle.Fixed32);
                msg.Write(ms.GetBuffer());
                if (encryptionAlg != null && encrypt) {
                    msg.Encrypt(encryptionAlg);
                }
                var result = client.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
                client.FlushSendQueue();

            } catch (Exception e) {
                Debug.Log(e.Message);
            }

        }

        public void ComputeAndSendHashAndKey(ProtoLogIn salts, byte[] key)
        {

            string hashstring = "";
            foreach (byte b in salts.userSalt)
            {
                hashstring += b.ToString();
            }
            string sessSalt = "";
            foreach (byte b in salts.sessionSalt)
            {
                sessSalt += b.ToString();
            }
            byte[] passBytes = Encoding.UTF8.GetBytes(pass);
            byte[] hashPassword = ComputeHash(passBytes, salts.userSalt);
            string passHash = "";
            foreach (byte b in hashPassword)
            {
                passHash += b.ToString();
            }
            byte[] hashFull = ComputeHash(hashPassword, salts.sessionSalt);
            string fullHash = "";
            foreach (byte b in hashFull)
            {
                fullHash += b.ToString();
            }
            var response = new ProtoLogIn
            {
                userSalt = hashFull,
                ActionType = Actions.LogIn,
                Key = key
            };
            Send(response, false);
        }

        /// <summary>
        /// Validates the certificate supplied by the server, and also creates a symmetric encryption key
        /// </summary>
        /// <returns><c>true</c>, if certificate was validated, <c>false</c> otherwise.</returns>
        /// <param name="login">ProtoLogin containing certificate</param>
        public bool ValidateCertificateAndCreateKey(ProtoLogIn login, out byte[] key)
        {
            if (login == null || login.certificate == null)
            {
                key = null;
                return false;
            }
            else
            {
                try
                {
                    // Get certificate
                    X509Certificate2 cert = new X509Certificate2(login.certificate);
                    RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)cert.PublicKey.Key;
//#if DEBUG
                    if(this.key != null)
                    {
                        if (this.key.Length == 0)
                        {
                            encryptionAlg = new NetAESEncryption(client);
                        }
                        else
                        {
                            encryptionAlg = new NetAESEncryption(client,
                            this.key, 0, this.key.Length);
                        }
                        key = rsa.Encrypt(this.key, false);
                    }
                    else
                    {
                        // If no key, do not use an encryption algorithm
                        encryptionAlg = null;
                        key = null;
                    }
//#else 
                    //// Create a new symmetric key
                    //TripleDES des = TripleDESCryptoServiceProvider.Create();
                    //des.GenerateKey();
                    //// Encrypt key with server's public key
                    //this.key = des.Key;
                    //key = rsa.Encrypt(des.Key, false);
                    //// Initialise the algoitm
                    //encryptionAlg = new NetAESEncryption(client, des.Key, 0, des.Key.Length);
                    //Console.WriteLine("CLIENT: my unencrypted key:");
                    //foreach(var bite in des.Key) {
                    //    Console.Write(bite.ToString());
                    //}
//#endif
                    // Validate certificate
                    if(!cert.Verify())
                    {
                        X509Chain CertificateChain = new X509Chain();
                        //If you do not provide revokation information, use the following line.
                        CertificateChain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                        bool IsCertificateChainValid = CertificateChain.Build(cert);
                        if (!IsCertificateChainValid)
                        {
                            for (int i = 0; i < CertificateChain.ChainStatus.Length; i++)
                            {
                            }
                            // TODO change to false after testing
                            return true;
                        }

                    }
                    // temporary certificate validation fix
                    return true;
                    //return cert.Verify();
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("A problem occurred when parsing certificate from bytes: \n" + "type: " + e.GetType().FullName + "\n " + ", source: " + e.Source + "\n message: " + e.Message);
                    key = null;
                    return true;
                }
            }
        }

        public void Listen()
        {
            while (client.Status == NetPeerStatus.Running && !ctSource.Token.IsCancellationRequested)
            {
                WaitHandle.WaitAny(new WaitHandle[] { client.MessageReceivedEvent, ctSource.Token.WaitHandle });
                NetIncomingMessage im;
                while ((im = client.ReadMessage()) != null && !ctSource.IsCancellationRequested)
                {
                    switch (im.MessageType)
                    {
                        case NetIncomingMessageType.DebugMessage:
                            break;
                        case NetIncomingMessageType.ErrorMessage:
                            break;
                        case NetIncomingMessageType.WarningMessage:
                            break;
                        case NetIncomingMessageType.VerboseDebugMessage:
                            break;
                        case NetIncomingMessageType.Data:
                            try
                            {
                                if (encryptionAlg != null)
                                {
                                    im.Decrypt(encryptionAlg);
                                }
                                MemoryStream ms = new MemoryStream(im.Data);
                                ProtoMessage m = null;
                                try
                                {
                                    m = ProtoBuf.Serializer.DeserializeWithLengthPrefix<ProtoMessage>(ms, ProtoBuf.PrefixStyle.Fixed32);
                                }
                                catch (Exception)
                                {
                                    // Attempt to read string and add to message queue
                                    string s = im.ReadString();
                                    Debug.Log(s);
                                    if (!string.IsNullOrEmpty(s))
                                    {
                                        tClient.stringMessageQueue.Enqueue(s);
                                    }
                                }
                                if (m != null)
                                {
                                    if (m.ResponseType == DisplayMessages.LogInSuccess)
                                    {
                                        loggedIn = true;
                                        tClient.protobufMessageQueue.Enqueue(m);
                                    }
                                    else
                                    {
                                        if (m.ActionType == Actions.Update)
                                        {
                                            tClient.protobufMessageQueue.Enqueue(m);
                                            //try {
                                            //    ProtoClient updateMessage = (ProtoClient)m;

                                            //}
                                            //catch(Exception e) {
                                            //    Debug.Log("Non-ProtoClient update message recieved.");
                                            //}
                                        }
                                        else
                                        {
                                            tClient.protobufMessageQueue.Enqueue(m);
                                            if (m.ActionType == Actions.LogIn && m.ResponseType == DisplayMessages.None)
                                            {
                                                byte[] key = null;
                                                if (ValidateCertificateAndCreateKey(m as ProtoLogIn, out key))
                                                {
                                                    ComputeAndSendHashAndKey(m as ProtoLogIn, key);
                                                }
                                            }
                                            else
                                            {
                                                // Attempt to read string and add to message queue
                                                string s = im.ReadString();
                                                if (!string.IsNullOrEmpty(s))
                                                {
                                                    tClient.stringMessageQueue.Enqueue(s);
                                                }
                                            }
                                        }

                                    }


                                }
                            }
                            catch (Exception e)
                            {
                                Debug.Log("Error in reading data: " + e.GetType() + " :" + e.Message + "; Stack Trace: " + e.StackTrace);
                            }
                            break;

                        case NetIncomingMessageType.StatusChanged:
                            NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();
                            //MemoryStream ms2 = new MemoryStream(im.SenderConnection.RemoteHailMessage.Data);
                            if (status == NetConnectionStatus.Connected)
                            {
                                if (im.SenderConnection.RemoteHailMessage != null)
                                {
                                    try
                                    {
                                        MemoryStream ms2 = new MemoryStream(im.SenderConnection.RemoteHailMessage.Data);
                                        ProtoMessage m = ProtoBuf.Serializer.DeserializeWithLengthPrefix<ProtoMessage>(ms2, ProtoBuf.PrefixStyle.Fixed32);
                                        if (m != null)
                                        {
                                            
                                            tClient.protobufMessageQueue.Enqueue(m);
                                            if (m.ActionType == Actions.LogIn && m.ResponseType == DisplayMessages.None)
                                            {
                                                byte[] key = null;
                                                if (ValidateCertificateAndCreateKey(m as ProtoLogIn, out key))
                                                {
                                                    if (autoLogIn)
                                                    {
                                                        ComputeAndSendHashAndKey(m as ProtoLogIn, key);
                                                    }
                                                }
                                                else
                                                {
                                                    client.Disconnect("Invalid Certificate");
                                                }

                                            }

                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Debug.Log("Connected - remote hail message\n" + e.GetType()+ "\n" + e.Message + "\n" + e.StackTrace);
                                        //string innerE;
                                        Exception inner;
                                        if(e.InnerException != null) {
                                            inner = e.InnerException;
                                            Debug.Log("Inner Exception:\n" + inner.GetType()+ "\n" + inner.Message + "\n" + inner.StackTrace);
                                        }
                                    }
                                }
                                break;
                            }
                            else if (status == NetConnectionStatus.Disconnected)
                            {
                                string reason = im.ReadString();
                                if(reason == null) {
                                    reason = "Unknown";
                                }
                                Debug.Log("The server has disconnected you. Reason: " + reason);
                                //if (!string.IsNullOrEmpty(reason))
                                //{
                                //    tClient.stringMessageQueue.Enqueue(reason);
                                //}
                            }
                            //if (im.SenderConnection.RemoteHailMessage != null && status == NetConnectionStatus.Connected)
                            //{

                            //}
                            break;
                        case NetIncomingMessageType.ConnectionLatencyUpdated:
                            break;
                        default:
                            break;
                    }
                    client.Recycle(im);
                }
            }
            Debug.Log("Client listening thread ends");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool dispose)
        {
            //crypto.Dispose();
            ((IDisposable)hash).Dispose();
        }
    }
}

