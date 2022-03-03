using System;
using System.Collections.Generic;
using System.IO;
using Lidgren.Network;
using ProtoBuf;
using System.Threading;
using System.Diagnostics.Contracts;
using System.Globalization;
using ProtoMessageClient;
using JominiGame;
using System.Text;

namespace JominiServer
{
    public class Server
    {

        /// <summary>
        /// Contains the connection and Client object of all connected, but not necessarily logged in, clients
        /// </summary>
        private Dictionary<NetConnection, Client> ClientConnections;
        private NetServer NetworkServer;
        private int Port;
        private string HostName;
        private int MaxConnections;
        private string AppIdentifier;

        private Game ActiveGame;

        /// <summary>
        /// Cancellation token- used to abort listening thread
        /// </summary>
        private CancellationTokenSource CTSource;
        /// <summary>
        /// Lock used to ensure list of connected clients is consistent
        /// </summary>
        private readonly object ServerLock = new object();

        private DateTime LastServerUpdate;
        /* Possible server configs
        private bool UseAutoUpdate = false;
        private int UpdateSeasonTime = 30; // Season will auto update ever (UpdateSeasonTime) Seconds
        */

        public Server(Game game)
        {
            ActiveGame = game;
            Client cl = new("Seba", (PlayerCharacter)ActiveGame.GetCharacter("Char_47"));
            ProtoMessage protoMessage = ViewCharacter("Char_47", cl);
            byte[] protoByte = SerializeMessage(protoMessage);
            PrintByteArray(protoByte);
            /*
            LastServerUpdate = DateTime.Now;
            LogInManager.StoreNewUser("acc1", "pass1");
            LogInManager.StoreNewUser("acc2", "pass2");
            NetPeerConfiguration config = new NetPeerConfiguration(app_identifier);
            config.LocalAddress = NetUtility.Resolve(host_name);
            config.MaximumConnections = max_connections;
            config.Port = port;
            config.SetMessageTypeEnabled(NetIncomingMessageType.ConnectionApproval, true);
            config.SetMessageTypeEnabled(NetIncomingMessageType.ConnectionLatencyUpdated, false);
            config.PingInterval = 10f;
            config.ConnectionTimeout = 100f;

            config.EnableUPnP = true; //#########

            server = new NetServer(config);
            ctSource = new CancellationTokenSource();
            server.Start();

            try
            {
                for (int i = 0; i < 3; i++)
                {
                    if (server.UPnP.ForwardPort(port, "Designated Listening Port"))
                    {
                        Globals_Server.logEvent("Port forwarded successfully.");
                        break;
                    }
                    else
                    {
                        Globals_Server.logEvent("Port forward FAILED " + i.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                Globals_Server.logEvent(e.ToString());
            }

            Globals_Server.server = server;
            Globals_Server.logEvent("Server started- host: " + host_name + ", port: " + port + ", appID: " +
                                    app_identifier + ", max connections: " + max_connections);
            Client client = new Client("acc1", "Char_47");
            Globals_Server.Clients.Add("acc1", client);
            Client client2 = new Client("acc2", "Char_126");
            Globals_Server.Clients.Add("acc2", client2);
            String dir = Directory.GetCurrentDirectory();
            //dir = dir.Remove(dir.IndexOf("RepairHist_mmo"));
            String path;
            if (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                path = Path.Combine(dir, "Certificates");
            }
            else
            {
                dir = Directory.GetParent(dir).FullName;
                dir = Directory.GetParent(dir).FullName;
                path = Path.Combine(dir, "Certificates");
            }
            LogInManager.InitialiseCertificateAndRSA(path);
            if (createAccountForAllPCs)
                createUsersForEachNotUsedPC();


            Thread listenThread = new Thread(new ThreadStart(Listen));
            listenThread.Start();*/
        }

        /// <summary>
        /// Server listening thread- accepts connections, receives messages, deserializes them and hands them to ProcessMessage
        /// </summary>
        [ContractVerification(true)]
        public void Listen()
        {
            /*
            while (server.Status == NetPeerStatus.Running && !ctSource.Token.IsCancellationRequested)
            {
                Globals_Server.logEvent("Waiting for event");
                UpdateSeason();
                NetIncomingMessage im;
                WaitHandle.WaitAny(new WaitHandle[] { server.MessageReceivedEvent, ctSource.Token.WaitHandle }, 5000);
                while ((im = server.ReadMessage()) != null && !ctSource.Token.IsCancellationRequested)
                {
                    if (im.SenderConnection != null)
                    {
                        Globals_Server.logEvent("Recieved: " + im.MessageType.ToString() + " | " + im.SenderConnection.RemoteEndPoint.ToString());
                    }
                    else
                    {
                        Globals_Server.logEvent("Recieved: " + im.MessageType.ToString() + " | NULL");
                    }

                    switch (im.MessageType)
                    {
                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.ErrorMessage:
                        case NetIncomingMessageType.WarningMessage:
                            Globals_Server.logError("Recieved warning message: " + im.ReadString());
                            break;
                        case NetIncomingMessageType.VerboseDebugMessage:
                        case NetIncomingMessageType.Data:
                            {
                                if (!clientConnections.ContainsKey(im.SenderConnection))
                                {
                                    //error
                                    im.SenderConnection.Disconnect("Not recognised");
                                    return;
                                }
                                Client c = clientConnections[im.SenderConnection];
                                if (c.alg != null)
                                {
                                    im.Decrypt(c.alg);
                                }
                                ProtoMessage m = null;
                                using (MemoryStream ms = new MemoryStream(im.Data))
                                {
                                    try
                                    {
                                        m = Serializer.DeserializeWithLengthPrefix<ProtoMessage>(ms, PrefixStyle.Fixed32);
                                    }
                                    catch (Exception e)
                                    {
                                        NetOutgoingMessage errorMessage = server.CreateMessage(
                                            "Failed to deserialise message. The message may be incorrect, or the decryption may have failed.");
                                        if (c.alg != null)
                                        {
                                            errorMessage.Encrypt(c.alg);
                                        }
                                        server.SendMessage(errorMessage, im.SenderConnection,
                                            NetDeliveryMethod.ReliableOrdered);
                                        Globals_Server.logError("Failed to deserialize message for client: " + c.username);
                                    }
                                }
                                if (m == null)
                                {
                                    string error = "Recieved null message from " + im.SenderEndPoint.ToString();
                                    if (clientConnections.ContainsKey(im.SenderConnection))
                                    {
                                        error += ", recognised client " + clientConnections[im.SenderConnection];
                                    }
                                    else
                                    {
                                        error += ", unrecognised client (possible ping)";
                                    }
                                    error += ". Data: " + im.ReadString();
                                    Globals_Server.logError(error);
                                    break;
                                }

                                if (m.ActionType == Actions.LogIn)
                                {
                                    ProtoLogIn login = m as ProtoLogIn;
                                    if (login == null)
                                    {
                                        im.SenderConnection.Disconnect("Received blank login message.");
                                        return;
                                    }
                                    lock (ServerLock)
                                    {
                                        if (LogInManager.VerifyUser(c.username, login.userSalt))
                                        {
                                            if (LogInManager.ProcessLogIn(login, c))
                                            {
                                                string log = c.username + " logs in from " + im.SenderEndPoint.ToString();
                                                Globals_Server.logEvent(log);
                                            }
                                        }
                                        else
                                        {
                                            ProtoMessage reply = new ProtoMessage
                                            {
                                                ActionType = Actions.LogIn,
                                                ResponseType = DisplayMessages.LogInFail
                                            };
                                            Server.SendViaProto(reply, c.conn, c.alg);
                                            //reply = new ProtoMessage {
                                            //    ActionType = Actions.Update,
                                            //    ResponseType = DisplayMessages.Error
                                            //};
                                            //Server.SendViaProto(reply, c.conn, c.alg);
                                            im.SenderConnection.Disconnect("Authentication Fail");
                                            Globals_Server.logEvent("Wrong Password, disconnecting user.");
                                        }
                                    }
                                }
                                // temp for testing, should validate connection first
                                else if (clientConnections.ContainsKey(im.SenderConnection))
                                {
                                    if (Globals_Game.IsObserver(c))
                                    {
                                        ProcessMessage(m, im.SenderConnection);
                                        ProtoClient clientDetails = new ProtoClient(c);
                                        clientDetails.ActionType = Actions.Update;
                                        clientDetails.ResponseType = DisplayMessages.Success;
                                        SendViaProto(clientDetails, im.SenderConnection, c.alg);
                                    }
                                    else
                                    {
                                        im.SenderConnection.Disconnect("Not logged in- Disconnecting");
                                    }
                                }
                            }
                            break;
                        case NetIncomingMessageType.StatusChanged:
                            byte stat = im.ReadByte();
                            NetConnectionStatus status = NetConnectionStatus.None;
                            if (Enum.IsDefined(typeof(NetConnectionStatus), Convert.ToInt32(stat)))
                            {
                                status = (NetConnectionStatus)stat;
                            }
                            else
                            {
                                Globals_Server.logError("Failure to parse byte " + stat + " to NetConnectionStatus for endpoint " + im.ReadIPEndPoint());
                            }
                            Globals_Server.logEvent("\tStatus is now: " + status);
                            if (status == NetConnectionStatus.Disconnected)
                            {
                                string reason = im.ReadString();
                                if (reason == null)
                                {
                                    reason = "Unknown";
                                }
                                Globals_Server.logEvent(im.SenderConnection.RemoteEndPoint.ToString() + " has disconnected. Reason: " + reason);
                                if (clientConnections.ContainsKey(im.SenderConnection))
                                {
                                    Disconnect(im.SenderConnection);
                                }
                            }
                            break;
                        case NetIncomingMessageType.ConnectionApproval:
                            {
                                string senderID = im.ReadString();
                                string text = im.ReadString();
                                Client client;
                                Globals_Server.Clients.TryGetValue(senderID, out client);
                                if (client != null)
                                {
                                    ProtoLogIn logIn;
                                    //ProtoMessage logIn;
                                    if (!LogInManager.AcceptConnection(client, text, out logIn))
                                    {
                                        im.SenderConnection.Deny("User not recognised.");
                                    }
                                    else
                                    {
                                        ProtoMessage temp = logIn;

                                        NetOutgoingMessage msg = server.CreateMessage();
                                        MemoryStream ms = new MemoryStream();
                                        // Include X509 certificate as bytes for client to validate
                                        //Serializer.SerializeWithLengthPrefix<ProtoLogIn>(ms, logIn, PrefixStyle.Fixed32);
                                        Serializer.SerializeWithLengthPrefix<ProtoMessage>(ms, temp, PrefixStyle.Fixed32);
                                        msg.Write(ms.GetBuffer());

                                        clientConnections.Add(im.SenderConnection, client);
                                        client.conn = im.SenderConnection;
                                        im.SenderConnection.Approve(msg);
                                        //server.FlushSendQueue();
                                        Globals_Server.logEvent("Accepted connection from " + client.username + " | " + senderID + " | " + text);
                                    }
                                }
                                else
                                {
                                    im.SenderConnection.Deny("Username unrecognised.");
                                }
                                server.FlushSendQueue();
                            }

                            break;
                        case NetIncomingMessageType.ConnectionLatencyUpdated:
                            Globals_Server.logEvent("LATENCY: Still getting these.");
                            break;
                        default:
                            Globals_Server.logError("Received unrecognised incoming message type: " + im.MessageType);
                            break;
                    }
                    server.Recycle(im);
                }
            }
            Globals_Server.logEvent("Server listening thread exits.");*/
        }

        /// <summary>
        ///     Processes a client request to switch to controlling a different Character
        /// </summary>
        /// <param name="ID">ID of character to control</param>
        /// <param name="client">Client who wishes to use this character</param>
        /// <returns>
        ///     ProtoCharacter message with response type "Success" on success, ProtoMessage with ErrorGenericCharacterUnidentified
        ///     for invalid character ID
        ///     ProtoMessage with ErrorGenericUnauthorised if do not own character, CharacterIsDead if trying to use a dead
        ///     character,
        /// </returns>
        /// <remarks>
        ///     On success the client's active character is changed. Empty or null charIDs default to the client's
        ///     PlayerCharacter
        /// </remarks>
        private ProtoMessage UseChar(string ID, Client client)
        {
            Character character =  ActiveGame.GetCharacter(ID);
            ProtoMessage protoMessage = new()
            {
                ActionType = Actions.UseChar,
                Message = "",
                MessageFields = Array.Empty<string>(),
                ResponseType = DisplayMessages.None
            };

            if (character == null)
            {
                protoMessage.ResponseType = DisplayMessages.ErrorGenericCharacterUnidentified;
                return protoMessage;
            }

            if (!PermissionManager.isAuthorized(PermissionManager.ownsCharNotCapturedOrAdmin, client.MyPlayerCharacter, character))
            {
                protoMessage.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                return protoMessage;
            }
            // Cannot use a character that is dead
            if (!character.IsAlive)
            {
                protoMessage.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                return protoMessage;
            }
            client.ActiveCharacter = character;
            protoMessage = new ProtoCharacter(character);
            protoMessage.ActionType = Actions.UseChar;
            protoMessage.ResponseType = DisplayMessages.Success;
            return protoMessage;
        }

        /// <summary>
        ///     Processes a client request to get a list of all players, including usernames, nationality and PlayerCharacter names
        /// </summary>
        /// <param name="client">Client who is requesting list of players</param>
        /// <returns>Collection of other players wrapped in a ProtoMessage</returns>
        public ProtoMessage GetPlayers(Client client)
        {
            ProtoGenericArray<ProtoPlayer> players = new();
            List<ProtoPlayer> playerList = new();
            foreach (Client cl in ClientConnections.Values)
            {
                // Do not include dead characters or self
                if (!cl.MyPlayerCharacter.IsAlive || cl.Username.Equals(client.Username))
                    continue;
                ProtoPlayer player = new()
                {
                    PlayerID = cl.Username,
                    PCID = cl.MyPlayerCharacter.ID,
                    PCName = cl.MyPlayerCharacter.FullName(),
                    NatID = cl.MyPlayerCharacter.Nationality.NatID
                };
                playerList.Add(player);
            }
            players.fields = playerList.ToArray();
            players.ResponseType = DisplayMessages.Success;
            return players;
        }

        /// <summary>
        ///     Processes a client request to view a character. The level of detail varies based on who is viewing which character
        /// </summary>
        /// <param name="charID">ID of character to view</param>
        /// <param name="client">Client who wishes to view Character</param>
        /// <returns>
        ///     Details of character if successful (hides information if do not own character, hides location if character is
        ///     captured), CharacterUnidentified if invalid, MessageInvalid if null
        /// </returns>
        public ProtoMessage ViewCharacter(string ID, Client client)
        {
            Character character = ActiveGame.GetCharacter(ID);
            ProtoMessage protoMessage = new()
            {
                ActionType = Actions.ViewChar,
                Message = "",
                MessageFields = Array.Empty<string>(),
                ResponseType = DisplayMessages.None
            };

            if (character == null)
            {
                protoMessage.ResponseType = DisplayMessages.ErrorGenericCharacterUnidentified;
                return protoMessage;
            }

            // Check whether player owns character, include additional information if so
            bool viewAll = PermissionManager.isAuthorized(PermissionManager.ownsCharNotCapturedOrAdmin, client.MyPlayerCharacter, character);
            bool seeLocation = PermissionManager.isAuthorized(PermissionManager.canSeeFiefOrAdmin, client.MyPlayerCharacter, character.Location);
            if (character is NonPlayerCharacter npc)
            {
                ProtoNonPlayerCharacter characterDetails = new(npc, ActiveGame, client);
                characterDetails.ResponseType = DisplayMessages.Success;
                // If unemployed include hire details
                // If captured, hide location
                Console.WriteLine("Currently sending all data about character, even if its supposed to be hidden");
                return characterDetails;
            }
            else
            {
                ProtoPlayerCharacter characterDetails = new((PlayerCharacter)character);
                characterDetails.ResponseType = DisplayMessages.Success;
                Console.WriteLine("Currently sending all data about character, even if its supposed to be hidden");
                return characterDetails;
            }
        }

        /// <summary>
        ///     Processes a client request to hire an NPC by bidding
        /// </summary>
        /// <param name="charID">ID of character to hire</param>
        /// <param name="bid">Amount client wishes to bid</param>
        /// <param name="client">Client to hire NPC</param>
        /// <returns>
        ///     ProtoNPC containing updated hire details and hire status- may have bidded successfully but not bid high enough;
        ///     MessageInvalid if null character id; CharacterUnidentified if invalid character ID, ErrorGenericTooFarFromFief if
        ///     not in same fief as character to hire, CharacterHeldCaptive if your character is held captive,
        ///     CharacterHireNotEmployable if character cannot be employed. Within the ActionController will return PositiveInteger
        ///     if a bid is not included or is invalid.
        /// </returns>
        public ProtoMessage HireNPC(NonPlayerCharacter NPC, uint bid, Game game, Client client)
        {

            // Ensure player is near character
            if (!client.MyPlayerCharacter.Location.ID.Equals(NPC.Location.ID))
            {
                return new ProtoMessage()
                {
                    ActionType = Actions.HireNPC,
                    Message = "",
                    MessageFields = Array.Empty<string>(),
                    ResponseType = DisplayMessages.ErrorGenericTooFarFromFief
                };
            }
            if (client.MyPlayerCharacter.Captor != null)
            {
                return new ProtoMessage()
                {
                    ActionType = Actions.HireNPC,
                    Message = "",
                    MessageFields = Array.Empty<string>(),
                    ResponseType = DisplayMessages.CharacterHeldCaptive
                };
            }
            if (!NPC.CheckCanHire(client.MyPlayerCharacter))
            {
                return new ProtoMessage()
                {
                    ActionType = Actions.HireNPC,
                    Message = "",
                    MessageFields = Array.Empty<string>(),
                    ResponseType = DisplayMessages.CharacterHireNotEmployable
                };
            }

            // Send result and updated character detail
            // Result contains character details, the result of hiring (response type) and any necessary fields for filling in response
            client.MyPlayerCharacter.ProcessEmployOffer(NPC, bid);
            ProtoNonPlayerCharacter viewCharacter = new(NPC, game, client);
            viewCharacter.ResponseType = DisplayMessages.NotImplementedYet;
            viewCharacter.Message = "Not implemented";
            viewCharacter.MessageFields = Array.Empty<string>();
            return viewCharacter;
        }

        /// <summary>
        ///     Processes a client request to fire an NPC
        /// </summary>
        /// <param name="charID">Character ID of character to be fired</param>
        /// <param name="client">Client who wishes to fire NPC</param>
        /// <returns>
        ///     MessageInvalid if not a valid character ID; CharacterUnidentified if not a valid character;
        ///     CharacterFireNotEmployee if trying to fire someone who is not an employee; Success otherwise
        /// </returns>
        public ProtoMessage FireNPC(NonPlayerCharacter NPC, Client client)
        {
            // if is not npc, or is not employed by player, error
            if (NPC == null || NPC.Employer != client.MyPlayerCharacter)
            {
                return new ProtoMessage()
                {
                    ActionType = Actions.FireNPC,
                    Message = "",
                    MessageFields = Array.Empty<string>(),
                    ResponseType = DisplayMessages.CharacterFireNotEmployee
                };
            }
            client.MyPlayerCharacter.FireNPC(NPC);
            return new ProtoMessage()
            {
                ActionType = Actions.FireNPC,
                Message = NPC.ID,
                MessageFields = Array.Empty<string>(),
                ResponseType = DisplayMessages.Success
            };
        }

        /// <summary>
        ///     Processes a client request to view an army. Details will vary based on whether army is owned by client or not
        /// </summary>
        /// <param name="armyID">Army ID of army to view</param>
        /// <param name="client">Client who wishes to view army</param>
        /// <returns>
        ///     ProtoArmy with all details if successful; MessageInvalid if no or invalid army id; ArmyUnidentified if not in
        ///     master army list; Unauthorised if too far from army
        /// </returns>
        public ProtoMessage ViewArmy(Army army, Client client)
        {
            // Check whether client can see this army
            if (!PermissionManager.isAuthorized(PermissionManager.canSeeArmyOrAdmin, client.MyPlayerCharacter, army))
            {
                return new ProtoMessage()
                {
                    ActionType = Actions.ViewArmy,
                    Message = "",
                    MessageFields = Array.Empty<string>(),
                    ResponseType = DisplayMessages.ErrorGenericUnauthorised
                };
            }
            // Determine how much information is displayed by checking if owns army
            ProtoArmy armyDetails = new(army, client.MyPlayerCharacter);
            armyDetails.ResponseType = DisplayMessages.Success;
            if (PermissionManager.isAuthorized(PermissionManager.ownsArmyOrAdmin, client.MyPlayerCharacter, army))
            {
                armyDetails.IncludeAll(army);
            }
            return armyDetails;
        }

        /// <summary>
        ///     Processes a client request to disband an army
        /// </summary>
        /// <param name="armyID">Army ID of army to be disbanded</param>
        /// <param name="client">Client who wishes to disband the army</param>
        /// <returns>
        ///     Success if completed without error; MessageInvalid if no or invalid army ID; ArmyUnidentified if army not in
        ///     army master list; Unauthorised if do not own army
        /// </returns>
        public ProtoMessage DisbandArmy(Army army, Client client)
        {
            //Check is a valid Army
            if (!PermissionManager.isAuthorized(PermissionManager.ownsArmyOrAdmin, client.MyPlayerCharacter, army))
            {
                return new ProtoMessage()
                {
                    ActionType = Actions.DisbandArmy,
                    Message = "",
                    MessageFields = Array.Empty<string>(),
                    ResponseType = DisplayMessages.ErrorGenericUnauthorised
                };
            }
            army.DisbandArmy();
            return new ProtoMessage()
            {
                ActionType = Actions.DisbandArmy,
                Message = "",
                MessageFields = Array.Empty<string>(),
                ResponseType = DisplayMessages.Success
            };
        }

        /// <summary>
        ///     Processes a client request to get a list of all NPCs that meet the given type and role
        /// </summary>
        /// <param name="type">
        ///     Type of NPC grouping: can be Entourage, Grant, Family, Employ or a combination of Family and Employ
        ///     (for whole household)
        /// </param>
        /// <param name="role">
        ///     For use with "Grant" type, the role checks what role is to be granted. Presently only "leader" is
        ///     supported
        /// </param>
        /// <param name="item">
        ///     Item that is to be granted control of- in this case, as only army leadership is supported, the item
        ///     will be an army ID
        /// </param>
        /// <param name="client">Client who is performing this action</param>
        /// <returns>Result of request</returns>
        public ProtoMessage GetNPCList(string type, string role, Army army, Game game, Client client)
        {
            Console.WriteLine("Not sure on whole use, might need to split function");
            var listOfChars = new List<ProtoCharacterOverview>();
            switch (type)
            {
                case "Entourage":
                    {
                        Console.WriteLine("Request Entourage");
                        foreach (var entourageChar in client.MyPlayerCharacter.MyEntourage)
                        {
                            //  listOfChars.Add(entourageChar.firstName);
                            listOfChars.Add(new ProtoCharacterOverview(entourageChar, game));
                        }
                        break;
                    }
                case "Grant":
                    {
                        Console.WriteLine("Request Grant");
                        switch (role)
                        {
                            case "leader":
                                {
                                    if (!PermissionManager.isAuthorized(PermissionManager.ownsArmyOrAdmin,client.MyPlayerCharacter, army))
                                    {
                                        return new ProtoMessage()
                                        {
                                            ActionType = Actions.GetNPCList,
                                            Message = "",
                                            MessageFields = Array.Empty<string>(),
                                            ResponseType = DisplayMessages.ErrorGenericUnauthorised
                                        };
                                    }
                                    foreach (var npc in client.MyPlayerCharacter.MyNPCs)
                                    {
                                        if (npc.ChecksBeforeGranting())
                                        {
                                            listOfChars.Add(new ProtoCharacterOverview(npc, game));
                                        }
                                        else
                                        {
                                            if (npc.ChecksBeforeGranting())
                                            {
                                                listOfChars.Add(new ProtoCharacterOverview(npc, game));
                                            }
                                        }
                                    }
                                    break;
                                }
                            default:
                                {
                                    break;
                                }
                        }
                        break;
                    }
                default:
                    {
                        if (type.Contains("Family"))
                        {
                            listOfChars.Add(new ProtoCharacterOverview(client.MyPlayerCharacter, game));
                            foreach (var family in client.MyPlayerCharacter.MyNPCs)
                            {
                                // ensure character is employee
                                if (!string.IsNullOrWhiteSpace(family.FamilyID))
                                {
                                    listOfChars.Add(new ProtoCharacterOverview(family, game));
                                }
                            }
                        }
                        if (type.Contains("Employ"))
                        {
                            Console.WriteLine("Request Employ");
                            foreach (var employee in client.MyPlayerCharacter.MyNPCs)
                            {
                                // ensure character is employee
                                if (employee.Employer != null)
                                {
                                    // listOfChars.Add(employee.firstName);
                                    listOfChars.Add(new ProtoCharacterOverview(employee, game));
                                }
                            }
                        }
                    }
                    break;
            }
            return new ProtoGenericArray<ProtoCharacterOverview>()
            {
                ActionType = Actions.GetNPCList,
                Message = type,
                MessageFields = new[] { role, army.ID },
                ResponseType = DisplayMessages.Success,
                fields = listOfChars.ToArray()
            };
        }

        /// <summary>
        ///     Processes a client request to travel to another fief
        /// </summary>
        /// <param name="charID"></param>
        /// <param name="fiefID"></param>
        /// <param name="travelInstructions"></param>
        /// <param name="client"></param>
        /// <returns>Result of request</returns>
        public ProtoMessage TravelTo(Character character, Fief fief, Client client)
        {
            // Check permissions
            if (PermissionManager.isAuthorized(PermissionManager.ownsCharNotCapturedOrAdmin, client.MyPlayerCharacter, character) && character.IsAlive)
            {
                // If there's a targeted fief
                if (fief != null)
                {
                    if (character.MoveTo(fief))
                    {
                        return new ProtoMessage()
                        {
                            ActionType = Actions.TravelTo,
                            Message = "",
                            MessageFields = Array.Empty<string>(),
                            ResponseType = DisplayMessages.Success
                        };
                    }
                    else
                    {
                        return new ProtoMessage()
                        {
                            ActionType = Actions.TravelTo,
                            Message = "",
                            MessageFields = Array.Empty<string>(),
                            ResponseType = DisplayMessages.ErrorGenericFiefUnidentified
                        };
                    }
                }
            }
            // If unauthorised return error
            return new ProtoMessage()
            {
                ActionType = Actions.TravelTo,
                Message = "",
                MessageFields = Array.Empty<string>(),
                ResponseType = DisplayMessages.ErrorGenericUnauthorised
            };
        }

        /// <summary>
        /// </summary>
        /// <param name="charID">Char who would travel</param>
        /// <param name="fiefID">Destination</param>
        /// <param name="direction">direction, replace fiefID if != null</param>
        /// <param name="client"></param>
        /// <returns></returns>
        public ProtoMessage GetTravelCost(Character character, Fief fief, HexMapGraph mapGraph, Client client, string direction = null)
        {
            // Check permissions
            if (!PermissionManager.isAuthorized(PermissionManager.ownsCharNotCapturedOrAdmin, client.MyPlayerCharacter, character) && character.IsAlive)
            { 
                return new ProtoMessage()
                {
                    ActionType = Actions.GetTravelCost,
                    Message = "",
                    MessageFields = Array.Empty<string>(),
                    ResponseType = DisplayMessages.ErrorGenericFiefUnidentified
                };
            }
            if (direction == null)
            {
                double pathCost = character.GetPathCost(fief, mapGraph);
                if (pathCost < 0)
                    Console.WriteLine("Move cost is negative ? ");
                return new ProtoMessage()
                {
                    ActionType = Actions.GetTravelCost,
                    Message = pathCost.ToString(),
                    MessageFields = Array.Empty<string>(),
                    ResponseType = DisplayMessages.Success
                };
            }
            else
            {
                Fief fiefDir = mapGraph.GetFief(character.Location, direction.ToUpper());
                if (fiefDir == null)
                {
                    return new ProtoMessage()
                    {
                        ActionType = Actions.GetTravelCost,
                        Message = "",
                        MessageFields = Array.Empty<string>(),
                        ResponseType = DisplayMessages.ErrorGenericFiefUnidentified
                    };
                }
                double pathCost = character.GetPathCost(fiefDir, mapGraph);
                if (pathCost < 0)
                    Console.WriteLine("Move cost is negative ? ");
                return new ProtoMessage()
                {
                    ActionType = Actions.GetTravelCost,
                    Message = pathCost.ToString(),
                    MessageFields = Array.Empty<string>(),
                    ResponseType = DisplayMessages.Success
                };
            }            
        }

        /// <summary>
        /// </summary>
        /// <param name="charID">Char who would travel</param>
        /// <param name="client"></param>
        /// <returns></returns>
        public ProtoMessage GetAvailableTravelDirections(Character character, HexMapGraph mapGraph, Client client)
        {
            // Check permissions
            if (!PermissionManager.isAuthorized(PermissionManager.ownsCharNotCapturedOrAdmin, client.MyPlayerCharacter, character) && character.IsAlive)
            {
                return new ProtoMessage()
                {
                    ActionType = Actions.GetAvailableTravelDirections,
                    Message = "",
                    MessageFields = Array.Empty<string>(),
                    ResponseType = DisplayMessages.ErrorGenericUnauthorised
                };
            }
            return new ProtoMessage()
            {
                ActionType = Actions.GetAvailableTravelDirections,
                Message = "",
                MessageFields = character.FindAvailableTravelDirections(mapGraph),
                ResponseType = DisplayMessages.Success
            };
        }

        /// <summary>
        ///     Processes a client request to view details of another fief. Amount of information returned depends on whether this
        ///     fief is owned by the client
        /// </summary>
        /// <param name="fiefID">ID of fief to be viewed</param>
        /// <param name="client">Client who sent the request to view the fief</param>
        /// <returns>Result of request</returns>
        public ProtoMessage ViewFief(Fief fief, Game game, Client client)
        {
            // A null fief (or sending "home") gets the home fief
            if (fief == null)
            {
                fief = client.MyPlayerCharacter.HomeFief;
            }

            // If the client owns the fief, include all information
            ProtoFief fiefToView = new(fief, game)
            {
                ActionType = Actions.ViewFief,
                Message = "",
                MessageFields = Array.Empty<string>()
            };
            if (client.MyPlayerCharacter.OwnedFiefs.Contains(fief))
            {
                fiefToView.includeAll(fief);
                fiefToView.ResponseType = DisplayMessages.Success;
                return fiefToView;
            }
            // Can only see fief details if have a character in the fief
            var hasCharInFief = PermissionManager.isAuthorized(PermissionManager.canSeeFiefOrAdmin, client.MyPlayerCharacter, fief);
            if (hasCharInFief)
            {
                fiefToView.ResponseType = DisplayMessages.Success;
                return fiefToView;
            }
            return new ProtoMessage()
            {
                ActionType = Actions.ViewFief,
                Message = "",
                MessageFields = Array.Empty<string>(),
                ResponseType = DisplayMessages.ErrorGenericTooFarFromFief
            };
        }

        /// <summary>
        ///     Processes a client request to view all fiefs they own
        /// </summary>
        /// <param name="client">Client who sent the request</param>
        /// <returns>Result of request</returns>
        public ProtoMessage ViewMyFiefs(Game game, Client client)
        {
            Console.WriteLine("View My Fiefs");
            ProtoGenericArray<ProtoFief> fiefList = new();
            fiefList.fields = new ProtoFief[client.MyPlayerCharacter.OwnedFiefs.Count];
            var i = 0;
            Console.WriteLine($"I have {client.MyPlayerCharacter.OwnedFiefs.Count} fiefs");
            foreach (Fief f in client.MyPlayerCharacter.OwnedFiefs)
            {
                var fief = new ProtoFief(f, game);
                fief.includeAll(f);
                fiefList.fields[i] = fief;
                i++;
            }
            fiefList.ResponseType = DisplayMessages.Success;
            return fiefList;
        }

        /// <summary>
        ///     Processes a client request to appoint a new bailiff to a fief
        /// </summary>
        /// <param name="fiefID">The ID of the fief to appoint a bailiff to</param>
        /// <param name="charID">The ID of the character which will become the new bailiff</param>
        /// <param name="client">The client who sent the request</param>
        /// <returns>Result of request</returns>
        public ProtoMessage AppointBailiff(Fief fief, Character character, Game game, Client client)
        {

            // Ensure character owns fief and character, or is admin
            if (
                PermissionManager.isAuthorized(PermissionManager.ownsCharNotCapturedOrAdmin, client.MyPlayerCharacter, character) &&
                PermissionManager.isAuthorized(PermissionManager.ownsFiefOrAdmin, client.MyPlayerCharacter, fief))
            {
                // Check character can become bailiff
                if (character.ChecksBeforeGranting())
                {
                    // set bailiff, return fief
                    fief.Bailiff = character;
                    fief.BailiffDaysInFief = 0;
                    ProtoFief fiefToView = new(fief, game);
                    fiefToView.ResponseType = DisplayMessages.Success;
                    fiefToView.includeAll(fief);
                    return fiefToView;
                }
                else
                {
                    return new ProtoMessage()
                    {
                        ActionType = Actions.AppointBailiff,
                        Message = "",
                        MessageFields = Array.Empty<string>(),
                        ResponseType = DisplayMessages.Error
                    };
                }
            }
            // User unauthorised
            return new ProtoMessage()
            {
                ActionType = Actions.AppointBailiff,
                Message = "",
                MessageFields = Array.Empty<string>(),
                ResponseType = DisplayMessages.ErrorGenericUnauthorised
            };
        }

        /// <summary>
        ///     Process a client request to remove the current bailiff from a fief
        /// </summary>
        /// <param name="fiefID">ID of fief from which to remove the bailiff</param>
        /// <param name="client">Client who sent the request</param>
        /// <returns>Result of request</returns>
        public ProtoMessage RemoveBailiff(Fief fief, Game game, Client client)
        {
            // Fief
            // Ensure player is authorized
            if (PermissionManager.isAuthorized(PermissionManager.ownsFiefOrAdmin, client.MyPlayerCharacter, fief))
            {
                // Remove bailiff and return fief details
                if (fief.Bailiff != null)
                {
                    fief.Bailiff = null;
                    fief.BailiffDaysInFief = 0;
                    ProtoFief fiefToView = new(fief, game);
                    fiefToView.ResponseType = DisplayMessages.Success;
                    fiefToView.includeAll(fief);
                    return fiefToView;
                }
                // Error- no bailiff
                return new ProtoMessage()
                {
                    ActionType = Actions.RemoveBailiff,
                    Message = "",
                    MessageFields = Array.Empty<string>(),
                    ResponseType = DisplayMessages.FiefNoBailiff
                };
            }
            // Unauthorised
            return new ProtoMessage()
            {
                ActionType = Actions.RemoveBailiff,
                Message = "",
                MessageFields = Array.Empty<string>(),
                ResponseType = DisplayMessages.ErrorGenericUnauthorised
            };
        }

        /// <summary>
        ///     Process a client request to bar a number of characters from the fief
        /// </summary>
        /// <param name="fiefID">ID of fief</param>
        /// <param name="charIDs">Array of all IDs of characters to be banned</param>
        /// <param name="client">Client who sent the request</param>
        /// <returns>Result of request</returns>
        public ProtoMessage BarCharacters(Fief fief, Character[] characters, Game game,  Client client)
        {
            // Check player owns fief
            if (!PermissionManager.isAuthorized(PermissionManager.ownsFiefOrAdmin, client.MyPlayerCharacter, fief))
            {
                return new ProtoMessage()
                {
                    ActionType = Actions.BarCharacters,
                    Message = "",
                    MessageFields = Array.Empty<string>(),
                    ResponseType = DisplayMessages.ErrorGenericUnauthorised
                };
            }
            // List of characters that for whatever reason could not be barred
            var couldNotBar = new List<string>();
            // Bar characters
            foreach (Character character in characters)
            {
                // Try to bar, if fail add to list of unbarrables
                if (!fief.BarCharacter(character))
                {
                    couldNotBar.Add(character.FullName());
                }
            }
            // return fief, along with details of characters that could not be barred
            ProtoFief fiefToReturn = new(fief, game)
            {
                ActionType = Actions.BarCharacters,
                Message = "",
                MessageFields = Array.Empty<string>(),
                ResponseType = DisplayMessages.Success
            };
            // If failed to bar some characters, return the IDs of the characters that could not be barred
            if (couldNotBar.Count > 0)
            {
                fiefToReturn.ResponseType = DisplayMessages.FiefCouldNotBar;
                fiefToReturn.MessageFields = couldNotBar.ToArray();
            }
            fiefToReturn.includeAll(fief);
            return fiefToReturn;
        }

        /// <summary>
        ///     Processes a client request to unbar a number of characters from a fief
        /// </summary>
        /// <param name="fiefID">ID of fief to unbar the characters from</param>
        /// <param name="charIDs">List of character IDs of characters to be unbarred</param>
        /// <param name="client">Client who submitted the request</param>
        /// <returns>Result of request</returns>
        public ProtoMessage UnbarCharacters(Fief fief, Character[] characters, Game game, Client client)
        {
            // Check player owns fief
            if (!PermissionManager.isAuthorized(PermissionManager.ownsFiefOrAdmin, client.MyPlayerCharacter, fief))
            {
                return new ProtoMessage()
                {
                    ActionType = Actions.UnbarCharacters,
                    Message = "",
                    MessageFields = Array.Empty<string>(),
                    ResponseType = DisplayMessages.ErrorGenericUnauthorised
                };
            }
            // List of characters that for whatever reason could not be barred
            var couldNotUnbar = new List<string>();
            // Bar characters
            foreach (Character character in characters)
            {
                if (!fief.UnbarCharacter(character))
                {
                    couldNotUnbar.Add(character.FullName());
                }
            }
            // Return fief along with characters that could not be unbarred
            ProtoFief returnFiefState = new(fief, game)
            {
                ActionType = Actions.UnbarCharacters,
                Message = "",
                MessageFields = Array.Empty<string>(),
                ResponseType = DisplayMessages.Success
            };
            if (couldNotUnbar.Count > 0)
            {
                returnFiefState.ResponseType = DisplayMessages.FiefCouldNotUnbar;
                returnFiefState.MessageFields = couldNotUnbar.ToArray();
            }
            returnFiefState.includeAll(fief);
            return returnFiefState;
        }

        /// <summary>
        ///     Bar a number of nationalities from the fief
        /// </summary>
        /// <param name="fiefID">ID of fief to bar nationalities from</param>
        /// <param name="natIDs">List of nationality IDs of nationalities to be banned</param>
        /// <param name="client">Client who submitted the request</param>
        /// <returns>Result of request</returns>
        public ProtoMessage BarNationalities(Fief fief, Nationality[] nationalities, Game game, Client client)
        {
            // Check player owns fief
            if (!PermissionManager.isAuthorized(PermissionManager.ownsFiefOrAdmin, client.MyPlayerCharacter, fief))
            {
                return new ProtoMessage()
                {
                    ActionType = Actions.BarNationalities,
                    Message = "",
                    MessageFields = Array.Empty<string>(),
                    ResponseType = DisplayMessages.ErrorGenericUnauthorised
                };
            }
            // Attempt to bar nationalities
            var failedToBar = new List<string>();
            foreach (Nationality nationality in nationalities)
            {
                // Cannot ban self
                if (nationality == fief.Owner.Nationality)
                {
                    failedToBar.Add(nationality.Name);
                    continue;
                }
                fief.BarNationality(nationality);
            }
            // Return fief + nationalities failed to bar
            var fiefToReturn = new ProtoFief(fief, game)
            {
                ActionType = Actions.BarNationalities,
                Message = "",
                MessageFields = Array.Empty<string>(),
                ResponseType = DisplayMessages.Success
            };
            if (failedToBar.Count > 0)
            {
                fiefToReturn.ResponseType = DisplayMessages.FiefCouldNotBar;
                fiefToReturn.MessageFields = failedToBar.ToArray();
            }
            fiefToReturn.includeAll(fief);
            return fiefToReturn;
        }

        /// <summary>
        ///     Processes a client request to unbar a number of nationalities from a fief
        /// </summary>
        /// <param name="fiefID">ID of fief from which to unbar nationalities</param>
        /// <param name="natIDs">List of nationality IDs to unbar</param>
        /// <param name="client">Client who submitted request</param>
        /// <returns>Result of request</returns>
        public ProtoMessage UnbarNationalities(Fief fief, Nationality[] nationalities, Game game, Client client)
        {
            // Check player owns fief
            if (!PermissionManager.isAuthorized(PermissionManager.ownsFiefOrAdmin, client.MyPlayerCharacter, fief))
            {
                return new ProtoMessage()
                {
                    ActionType = Actions.UnbarCharacters,
                    Message = "",
                    MessageFields = Array.Empty<string>(),
                    ResponseType = DisplayMessages.ErrorGenericUnauthorised
                };
            }
            // Attempt to bar nationalities
            var failedToUnbar = new List<string>();
            foreach (Nationality nationality in nationalities)
            {
                // Cannot ban self
                if (nationality == fief.Owner.Nationality)
                {
                    failedToUnbar.Add(nationality.NatID);
                    continue;
                }
                // Attempt to bar nationality
                if (!fief.UnbarNationality(nationality.NatID))
                {
                    failedToUnbar.Add(nationality.NatID);
                }
            }
            // return fief along with nationalities that could not be unbarred
            var fiefToReturn = new ProtoFief(fief, game)
            {
                ActionType = Actions.UnbarCharacters,
                Message = "",
                MessageFields = Array.Empty<string>(),
                ResponseType = DisplayMessages.Success
            };
            if (failedToUnbar.Count > 0)
            {
                fiefToReturn.ResponseType = DisplayMessages.FiefCouldNotUnbar;
                fiefToReturn.MessageFields = failedToUnbar.ToArray();
            }
            fiefToReturn.includeAll(fief);
            return fiefToReturn;
        }

        /// <summary>
        ///     Processes a client request to grant the title of a fief to another character
        /// </summary>
        /// <param name="fiefID">ID of fief to grant the title of</param>
        /// <param name="charID">ID of character who will become the new title holder</param>
        /// <param name="client">Client who submitted the request</param>
        /// <returns>Result of request</returns>
        public ProtoMessage GrantFiefTitle(Fief fief, Character character, Game game, Client client)
        {

            // Ensure player has permission to grant title
            if (!PermissionManager.isAuthorized(PermissionManager.ownsFiefOrAdmin, client.MyPlayerCharacter, fief))
            {
                return new ProtoMessage()
                {
                    ActionType = Actions.GrantFiefTitle,
                    Message = "",
                    MessageFields = Array.Empty<string>(),
                    ResponseType = DisplayMessages.ErrorGenericUnauthorised
                };
            }

            if (character.ChecksBeforeGranting())
            {
                if (client.MyPlayerCharacter.GrantTitle(character, fief))
                {
                    ProtoFief f = new(fief, game)
                    {
                        ActionType = Actions.GrantFiefTitle,
                        Message = "grantedTitle",
                        MessageFields = Array.Empty<string>(),
                        ResponseType = DisplayMessages.Success
                    };
                    f.includeAll(fief);
                    return f;
                }
            }
            //Permission denied
            return new ProtoMessage()
            {
                ActionType = Actions.GrantFiefTitle,
                Message = "",
                MessageFields = Array.Empty<string>(),
                ResponseType = DisplayMessages.ErrorGenericUnauthorised
            };
        }

        /// <summary>
        ///     Processes a client request to adjust the expenditure in a fief
        /// </summary>
        /// <param name="fiefID"></param>
        /// <param name="adjustedValues"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public ProtoMessage AdjustExpenditure(Fief fief, double[] adjustedValues, Game game, Client client)
        {
            // Check permissions
            if (!PermissionManager.isAuthorized(PermissionManager.ownsFiefOrAdmin, client.MyPlayerCharacter, fief))
            {
                return new ProtoMessage()
                {
                    ActionType = Actions.AdjustExpenditure,
                    Message = "",
                    MessageFields = Array.Empty<string>(),
                    ResponseType = DisplayMessages.ErrorGenericUnauthorised
                };
            }
            if (adjustedValues == null)
            {
                var overspend = fief.GetAvailableTreasury(true);
                if (overspend < 0)
                {
                    fief.AutoAdjustExpenditure(Convert.ToUInt32(Math.Abs(overspend)));
                    var f = new ProtoFief(fief, game)
                    {
                        ActionType = Actions.AdjustExpenditure,
                        Message = "auto-adjusted",
                        MessageFields = Array.Empty<string>(),
                        ResponseType = DisplayMessages.FiefExpenditureAdjusted,
                    };
                    f.includeAll(fief);
                    return f;
                }
                return new ProtoMessage()
                {
                    ActionType = Actions.AdjustExpenditure,
                    Message = "",
                    MessageFields = Array.Empty<string>(),
                    ResponseType = DisplayMessages.ErrorGenericMessageInvalid
                };
            }
            if (adjustedValues.Length != 5)
            {
                return new ProtoMessage()
                {
                    ActionType = Actions.AdjustExpenditure,
                    Message = "Expected array:5",
                    MessageFields = Array.Empty<string>(),
                    ResponseType = DisplayMessages.ErrorGenericMessageInvalid
                };
            }
            // Perform conversion
            try
            {
                bool adjust = fief.AdjustExpenditures(
                    adjustedValues[0],
                    Convert.ToUInt32(adjustedValues[1]),
                    Convert.ToUInt32(adjustedValues[2]),
                    Convert.ToUInt32(adjustedValues[3]),
                    Convert.ToUInt32(adjustedValues[4]));
                if (adjust)
                {
                    return new ProtoMessage()
                    {
                        ActionType = Actions.AdjustExpenditure,
                        Message = "",
                        MessageFields = Array.Empty<string>(),
                        ResponseType = DisplayMessages.FiefExpenditureAdjusted
                    };
                }
                else
                {
                    return new ProtoMessage()
                    {
                        ActionType = Actions.AdjustExpenditure,
                        Message = "",
                        MessageFields = Array.Empty<string>(),
                        ResponseType = DisplayMessages.Error
                    };
                }
            }
            catch (OverflowException e)
            {
                return new ProtoMessage()
                {
                    ActionType = Actions.AdjustExpenditure,
                    Message = "Invalid values",
                    MessageFields = Array.Empty<string>(),
                    ResponseType = DisplayMessages.ErrorGenericMessageInvalid
                };
            }
        }

        public ProtoMessage TransferFunds(Fief fromFief, Fief toFief, int amount, Game game, Client client)
        {
            if (amount < 0)
            {
                return new ProtoMessage()
                {
                    ActionType = Actions.TransferFunds,
                    Message = "Invalid value",
                    MessageFields = Array.Empty<string>(),
                    ResponseType = DisplayMessages.ErrorGenericPositiveInteger
                };
            }
            // Ensure fief has sufficient funds
            if (amount > fromFief.GetAvailableTreasury(true))
            {
                return new ProtoMessage()
                {
                    ActionType = Actions.TransferFunds,
                    Message = "",
                    MessageFields = Array.Empty<string>(),
                    ResponseType = DisplayMessages.ErrorGenericInsufficientFunds
                };
            }
            else
            {
                // return success
                ProtoMessage error;
                if (fromFief.TreasuryTransfer(toFief, amount))
                {
                    ProtoFief success = new(fromFief, game)
                    {
                        ActionType = Actions.TransferFunds,
                        Message = "transferfunds",
                        MessageFields = Array.Empty<string>(),
                        ResponseType = DisplayMessages.Success
                    };
                    success.includeAll(fromFief);
                    return success;
                }
                else
                {
                    return new ProtoMessage()
                    {
                        ActionType = Actions.TransferFunds,
                        Message = "",
                        MessageFields = Array.Empty<string>(),
                        ResponseType = DisplayMessages.ErrorGenericInsufficientFunds
                    };
                }
            }
        }

        public ProtoMessage TransferFundsToPlayer(PlayerCharacter playerCharacter, int amount, Client client)
        {
            // Confirm both players have a home fief
            if (client.MyPlayerCharacter.HomeFief == null || playerCharacter.HomeFief == null)
            {
                return new ProtoMessage()
                {
                    ActionType = Actions.TransferFundsToPlayer,
                    Message = "",
                    MessageFields = Array.Empty<string>(),
                    ResponseType = DisplayMessages.ErrorGenericNoHomeFief
                };
            }
            // Perform treasury transfer, update
            if (client.MyPlayerCharacter.HomeFief.TreasuryTransfer(playerCharacter.HomeFief, amount))
            {
                return new ProtoMessage()
                {
                    ActionType = Actions.TransferFundsToPlayer,
                    Message = "transferfundsplayer",
                    MessageFields = Array.Empty<string>(),
                    ResponseType = DisplayMessages.Success
                };
            }
            else
            {
                return new ProtoMessage()
                {
                    ActionType = Actions.TransferFundsToPlayer,
                    Message = "Unknown Error",
                    MessageFields = Array.Empty<string>(),
                    ResponseType = DisplayMessages.Error
                };
            }
        }

        public ProtoMessage EnterExitKeep(Character character, Client client)
        {
            // check authorization
            if (!PermissionManager.isAuthorized(PermissionManager.ownsCharNotCapturedOrAdmin, client.MyPlayerCharacter, character))
            {
                return new ProtoMessage()
                {
                    ActionType = Actions.EnterExitKeep,
                    Message = "",
                    MessageFields = Array.Empty<string>(),
                    ResponseType = DisplayMessages.ErrorGenericUnauthorised
                };
            }
            if (character.ExitEnterKeep())
            {
                return new ProtoMessage()
                {
                    ActionType = Actions.EnterExitKeep,
                    Message = character.InKeep.ToString(),
                    MessageFields = Array.Empty<string>(),
                    ResponseType = DisplayMessages.Success
                };
            }
            else
            {
                return new ProtoMessage()
                {
                    ActionType = Actions.EnterExitKeep,
                    Message = "Unknown error",
                    MessageFields = Array.Empty<string>(),
                    ResponseType = DisplayMessages.Error
                };
            }
        }

        public ProtoMessage ListCharsInMeetingPlace(string placeType, Character character, Game game, Client client)
        {
            if (!PermissionManager.isAuthorized(PermissionManager.ownsCharNotCapturedOrAdmin, client.MyPlayerCharacter, character))
            {
                return new ProtoMessage()
                {
                    ActionType = Actions.ListCharsInMeetingPlace,
                    Message = "",
                    MessageFields = Array.Empty<string>(),
                    ResponseType = DisplayMessages.ErrorGenericUnauthorised
                };
            }
            if (string.IsNullOrWhiteSpace(placeType))
            {
                return new ProtoMessage()
                {
                    ActionType = Actions.ListCharsInMeetingPlace,
                    Message = "",
                    MessageFields = Array.Empty<string>(),
                    ResponseType = DisplayMessages.ErrorGenericUnauthorised
                };
            }
            // Enter/exit keep as appropriate depending on whether viewing court
            if (placeType.Equals("court"))
            {
                if (!character.InKeep)
                {
                    if (!character.EnterKeep())
                    {
                        return new ProtoMessage()
                        {
                            ActionType = Actions.ListCharsInMeetingPlace,
                            Message = "Couldnt enter keep ?",
                            MessageFields = Array.Empty<string>(),
                            ResponseType = DisplayMessages.Error
                        };
                    }
                }
            }
            else
            {
                if (character.InKeep)
                {
                    character.ExitKeep();
                }
            }
            // Get and return charcters in meeting place
            List<Character> characters = character.Location.ListCharsInMeetingPlace(placeType, client.MyPlayerCharacter);
            ProtoCharacterOverview[] charactersinMettingPlace = new ProtoCharacterOverview[characters.Count];
            for (int i = 0; i < characters.Count; i++)
            {
                charactersinMettingPlace[i] = new ProtoCharacterOverview(characters[i], game);
            }
            ProtoGenericArray<ProtoCharacterOverview> charsInPlace = new(charactersinMettingPlace)
            {
                Message = placeType,
                ResponseType = DisplayMessages.Success
            };
            return charsInPlace;
        }

        public ProtoMessage Camp(Character character, byte days, Client client)
        {
            // Perform authorization
            if (PermissionManager.isAuthorized(PermissionManager.ownsCharNotCapturedOrAdmin, client.MyPlayerCharacter, character))
            {
                if (character.CampWaitHere())
                {
                    return new ProtoMessage()
                    {
                        ActionType = Actions.Camp,
                        Message = "",
                        MessageFields = Array.Empty<string>(),
                        ResponseType = DisplayMessages.Success
                    };
                }
                else
                {
                    return new ProtoMessage()
                    {
                        ActionType = Actions.Camp,
                        Message = "",
                        MessageFields = Array.Empty<string>(),
                        ResponseType = DisplayMessages.Error
                    };
                }                
            }
            return new ProtoMessage()
            {
                ActionType = Actions.Camp,
                Message = "",
                MessageFields = Array.Empty<string>(),
                ResponseType = DisplayMessages.ErrorGenericUnauthorised
            };
        }

        public ProtoMessage AddRemoveEntourage(NonPlayerCharacter NPC, Game game, Client client)
        {
            // ensure player is authorized to add to entourage
            if (!PermissionManager.isAuthorized(PermissionManager.ownsCharNotCapturedOrAdmin, client.MyPlayerCharacter, NPC))
            {
                return new ProtoMessage()
                {
                    ActionType = Actions.AddRemoveEntourage,
                    Message = "",
                    MessageFields = Array.Empty<string>(),
                    ResponseType = DisplayMessages.ErrorGenericUnauthorised
                };
            }
            // Ensure playercharacter is not captured
            if (client.MyPlayerCharacter.Captor != null)
            {
                return new ProtoMessage()
                {
                    ActionType = Actions.AddRemoveEntourage,
                    Message = "",
                    MessageFields = Array.Empty<string>(),
                    ResponseType = DisplayMessages.CharacterHeldCaptive
                };
            }
            // If in entourage, remove- otherwise, add
            if (NPC.InEntourage)
            {
                client.MyPlayerCharacter.RemoveFromEntourage(NPC);
            }
            else
            {
                // Player character must be in same location as npc to add
                if (client.MyPlayerCharacter.Location != NPC.Location)
                {
                    return new ProtoMessage()
                    {
                        ActionType = Actions.AddRemoveEntourage,
                        Message = "",
                        MessageFields = Array.Empty<string>(),
                        ResponseType = DisplayMessages.ErrorGenericNotInSameFief
                    };
                }
                client.MyPlayerCharacter.AddToEntourage(NPC);
            }
            // return entourage
            var newEntourage = new List<ProtoCharacterOverview>();
            foreach (var entourageChar in client.MyPlayerCharacter.MyEntourage)
            {
                newEntourage.Add(new ProtoCharacterOverview(entourageChar, game));
            }
            var result = new ProtoGenericArray<ProtoCharacterOverview>(newEntourage.ToArray());
            result.ResponseType = DisplayMessages.Success;
            return result;
        }

        public ProtoMessage ProposeMarriage(Character groom, Character bride, Client client)
        {
            if (!PermissionManager.isAuthorized(PermissionManager.ownsCharOrAdmin, client.MyPlayerCharacter, groom))
            {
                return new ProtoMessage()
                {
                    ActionType = Actions.ProposeMarriage,
                    Message = "",
                    MessageFields = Array.Empty<string>(),
                    ResponseType = DisplayMessages.ErrorGenericUnauthorised
                };
            }
            if (groom.Captor != null || bride.Captor != null)
            {
                return new ProtoMessage()
                {
                    ActionType = Actions.ProposeMarriage,
                    Message = "",
                    MessageFields = Array.Empty<string>(),
                    ResponseType = DisplayMessages.CharacterHeldCaptive
                };
            }
            if (groom.ChecksBeforeProposal(bride))
            {
                if (groom.ProposeMarriage(bride))
                {
                    return new ProtoMessage()
                    {
                        ActionType = Actions.ProposeMarriage,
                        Message = "Proposal success",
                        MessageFields = Array.Empty<string>(),
                        ResponseType = DisplayMessages.Success
                    };
                }
            }
            return new ProtoMessage()
            {
                ActionType = Actions.ProposeMarriage,
                Message = "Unknown",
                MessageFields = Array.Empty<string>(),
                ResponseType = DisplayMessages.Error
            };
        }

        public ProtoMessage AcceptRejectProposal(uint jEntryID, bool accept, Client client)
        {
            throw new NotImplementedException("No journal entries implemented");
        }

        public ProtoMessage AppointHeir(NonPlayerCharacter NPC, Client client)
        {
            // Cannot appioint an heir if captured
            if (client.MyPlayerCharacter.Captor != null)
                return GenericMessage(Actions.AppointHeir, DisplayMessages.CharacterHeldCaptive);
            if (!PermissionManager.isAuthorized(PermissionManager.ownsCharOrAdmin, client.MyPlayerCharacter, NPC))
                return GenericMessage(Actions.AppointHeir, DisplayMessages.ErrorGenericUnauthorised);
            if (!NPC.ChecksForHeir(client.MyPlayerCharacter))
                return GenericMessage(Actions.AppointHeir, DisplayMessages.CharacterHeir);

            // check for an existing heir and remove
            foreach (var npc in client.MyPlayerCharacter.MyNPCs)
            {
                if (npc.IsHeir)
                {
                    npc.IsHeir = false;
                }
            }
            NPC.IsHeir = true;
            return new ProtoCharacter(NPC)
            {
                ResponseType = DisplayMessages.Success
            };
        }

        public ProtoMessage TryForChild(Character father, Client client)
        {
            // Authorize
            if (!PermissionManager.isAuthorized(PermissionManager.ownsCharNotCapturedOrAdmin, client.MyPlayerCharacter, father))
                return GenericMessage(Actions.TryForChild, DisplayMessages.ErrorGenericUnauthorised);
            // Confirm can get pregnant
            if (father.Spouse == null)
                return GenericMessage(Actions.TryForChild, DisplayMessages.BirthNotMarried);
            if (father.Spouse.IsPregnant)
                return GenericMessage(Actions.TryForChild, DisplayMessages.BirthAlreadyPregnant);
            if(father.Spouse.Location != father.Location)
                return GenericMessage(Actions.TryForChild, DisplayMessages.ErrorGenericNotInSameFief);

            if (father.GetSpousePregnant())
            {
                return GenericMessage(Actions.TryForChild, DisplayMessages.Success);
            }
            else
            {
                return GenericMessage(Actions.TryForChild, DisplayMessages.Error);
            }
        }

        public ProtoMessage RecruitTroops(Army army, uint amount, bool isConfirm, Client client)
        {
            if (army == null)
            {
                if(client.MyPlayerCharacter.RecruitTroops(amount, null, isConfirm))
                {
                    return GenericMessage(Actions.RecruitTroops, DisplayMessages.Success);
                } else
                {
                    return GenericMessage(Actions.RecruitTroops, DisplayMessages.RecruitCancelled);
                }
            }
            else
            {
                if (!client.MyPlayerCharacter.MyArmies.Contains(army))
                    return GenericMessage(Actions.RecruitTroops, DisplayMessages.ErrorGenericArmyUnidentified);
                if(army.Owner != client.MyPlayerCharacter)
                    return GenericMessage(Actions.RecruitTroops, DisplayMessages.ErrorGenericUnauthorised);

                if (client.MyPlayerCharacter.RecruitTroops(amount, army, isConfirm))
                {
                    return GenericMessage(Actions.RecruitTroops, DisplayMessages.Success);
                }
                else
                {
                    return GenericMessage(Actions.RecruitTroops, DisplayMessages.RecruitCancelled);
                }

            }
        }

        public ProtoMessage MaintainArmy(Army army, Client client)
        {
            if (army.Owner != client.MyPlayerCharacter)
                return GenericMessage(Actions.MaintainArmy, DisplayMessages.ErrorGenericUnauthorised);

            if (army.MaintainArmy())
            {
                return GenericMessage(Actions.MaintainArmy, DisplayMessages.ArmyMaintainConfirm);
            }
            else
            {
                return GenericMessage(Actions.MaintainArmy, DisplayMessages.ArmyMaintainInsufficientFunds);
            }
        }

        public ProtoMessage ListArmies(Client client)
        {
            ProtoGenericArray<ProtoArmyOverview> armies = new();
            armies.fields = new ProtoArmyOverview[client.MyPlayerCharacter.MyArmies.Count];
            var i = 0;
            foreach (var army in client.MyPlayerCharacter.MyArmies)
            {
                var armyDetails = new ProtoArmyOverview(army);
                armyDetails.IncludeAll(army);
                armies.fields[i] = armyDetails;
                i++;
            }
            armies.ResponseType = DisplayMessages.Success;
            return armies;
        }

        public ProtoMessage AppointLeader(Army army, Character character, Client client)
        {
            // Authorize
            if(client.MyPlayerCharacter != army.Owner || client.MyPlayerCharacter != ((NonPlayerCharacter)character).GetHeadOfFamily() || client.MyPlayerCharacter != ((NonPlayerCharacter)character).Employer)
                return GenericMessage(Actions.AppointLeader, DisplayMessages.ErrorGenericUnauthorised);

            if (character.ChecksBeforeGranting())
            {
                army.AssignNewLeader(character);
                return new ProtoArmy(army, client.MyPlayerCharacter);
            }
            else
            {
                return GenericMessage(Actions.AppointLeader, DisplayMessages.Error);
            }
        }

        public ProtoMessage DropOffTroops(Army army, uint[] troops, Character? character, Client client)
        {
            // Authorize
            if(army.Owner == client.MyPlayerCharacter)
                return GenericMessage(Actions.DropOffTroops, DisplayMessages.ErrorGenericUnauthorised);

            // Create a detachment
            if (army.CreateDetachment(troops, character))
            {
                var success = new ProtoArmy(army, client.MyPlayerCharacter)
                {
                    ResponseType = DisplayMessages.Success
                };
                success.IncludeAll(army);
                return success;
            }
            else
            {
                return GenericMessage(Actions.DropOffTroops, DisplayMessages.Error);
            }
        }

        public ProtoMessage ListDetachments(Army army, Client client)
        {
            if(client.MyPlayerCharacter == army.Owner)
                return GenericMessage(Actions.ListDetachments, DisplayMessages.ErrorGenericUnauthorised);

            List<ProtoDetachment> myAvailableTransfers = new();
            foreach (var transferDetails in army.Location.TroopTransfers.Values)
            {
                if (transferDetails.LeftFor.Equals(client.MyPlayerCharacter.ID))
                {
                    myAvailableTransfers.Add(new ProtoDetachment(transferDetails));
                }
            }

            ProtoGenericArray<ProtoDetachment> detachmentList = new(myAvailableTransfers.ToArray());
            detachmentList.ResponseType = DisplayMessages.Success;
            return detachmentList;
        }

        public ProtoMessage PickUpTroops(Army army, Detachment detachment, Client client)
        {
            if (client.MyPlayerCharacter != army.Owner)
                return GenericMessage(Actions.PickUpTroops, DisplayMessages.ErrorGenericUnauthorised);

            if (army.ProcessPickups(detachment))
            {
                ProtoArmy protoArmy = new(army, client.MyPlayerCharacter)
                {
                    ResponseType = DisplayMessages.Success
                };
                protoArmy.IncludeAll(army);
                return protoArmy;
            }
            else
            {
                ProtoArmy protoArmy = new(army, client.MyPlayerCharacter)
                {
                    ResponseType = DisplayMessages.ArmyPickupsNotEnoughDays
                };
                return protoArmy;
            }
        }

        public ProtoMessage PillageFief(Army army, Client client)
        {
            if (client.MyPlayerCharacter != army.Owner)
                return GenericMessage(Actions.PillageFief, DisplayMessages.ErrorGenericUnauthorised);

            Console.WriteLine("Need to do checks before pillaging");
            // var canPillage = Pillage_Siege.ChecksBeforePillageSiege(army, army.GetLocation(), out pillageError);
            ProtoPillageResult protoPillageResult = new(PillageSiege.ProcessPillage(army.Location, army))
            {
                ResponseType = DisplayMessages.Success,
                Message = "pillage"
            };
            return protoPillageResult;
        }

        public static ProtoMessage BesiegeFief(string armyID, Client client)
        {
            // Get army
            if (string.IsNullOrEmpty(armyID))
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
            }
            Army army = null;
            Globals_Game.armyMasterList.TryGetValue(armyID, out army);
            if (army == null)
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericArmyUnidentified);
            }
            if (!PermissionManager.isAuthorized(PermissionManager.ownsArmyOrAdmin, client.myPlayerCharacter, army))
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericUnauthorised);
            }
            ProtoMessage pillageError;
            var canSiege = Pillage_Siege.ChecksBeforePillageSiege(army, army.GetLocation(), out pillageError, "siege");
            if (canSiege)
            {
                var newSiege = Pillage_Siege.SiegeStart(army, army.GetLocation());
                var result = new ProtoSiegeDisplay(newSiege);
                result.ResponseType = DisplayMessages.Success;
                result.Message = "siege";
                return result;
            }
            return pillageError;
        }

        public static ProtoMessage SiegeRoundNegotiate(string siegeID, Client client)
        {
            if (string.IsNullOrWhiteSpace(siegeID))
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
            }

            // Get siege
            var siege = client.myPlayerCharacter.GetSiege(siegeID);
            if (siege == null)
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.ErrorGenericSiegeUnidentified;
                return error;
            }
            // Check besieger is pc
            if (siege.besiegingPlayer != client.myPlayerCharacter.charID)
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.SiegeNotBesieger;
                return error;
            }
            ProtoMessage siegeError = null;
            if (!siege.ChecksBeforeSiegeOperation(out siegeError))
            {
                return siegeError;
            }
            siege.SiegeReductionRound("negotiation");
            return new ProtoSiegeDisplay(siege);
        }

        public static ProtoMessage SiegeRoundReduction(string siegeID, Client client)
        {
            if (string.IsNullOrWhiteSpace(siegeID))
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
            }

            // get siege
            var siege = client.myPlayerCharacter.GetSiege(siegeID);
            if (siege == null)
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.ErrorGenericSiegeUnidentified;
                return error;
            }
            // check player is besieger
            if (siege.besiegingPlayer != client.myPlayerCharacter.charID)
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.SiegeNotBesieger;
                return error;
            }
            ProtoMessage siegeError = null;
            if (!siege.ChecksBeforeSiegeOperation(out siegeError))
            {
                return siegeError;
            }
            siege.SiegeReductionRound();
            return new ProtoSiegeDisplay(siege);
        }

        public static ProtoMessage SiegeRoundStorm(string siegeID, Client client)
        {
            if (string.IsNullOrWhiteSpace(siegeID))
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
            }

            // Get siege
            var siege = client.myPlayerCharacter.GetSiege(siegeID);
            if (siege == null)
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.ErrorGenericSiegeUnidentified;
                return error;
            }
            // check player is besieger
            if (siege.besiegingPlayer != client.myPlayerCharacter.charID)
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.SiegeNotBesieger;
                return error;
            }
            ProtoMessage siegeError = null;
            if (!siege.ChecksBeforeSiegeOperation(out siegeError))
            {
                return siegeError;
            }
            Console.WriteLine($"--------------------- STORM SIEGE 1 ------------------");
            Console.WriteLine(siege.siegeID);
            Console.WriteLine(siege.startYear);
            Console.WriteLine(siege.startSeason);
            Console.WriteLine(siege.besiegingPlayer);
            Console.WriteLine(siege.defendingPlayer);
            Console.WriteLine(siege.besiegerArmy);
            Console.WriteLine(siege.defenderGarrison);
            Console.WriteLine(siege.besiegedFief);
            Console.WriteLine(siege.days);
            Console.WriteLine(siege.totalDays);
            Console.WriteLine(siege.startKeepLevel);
            Console.WriteLine(siege.GetFief().keepLevel);
            Console.WriteLine(siege.totalCasualtiesAttacker);
            Console.WriteLine(siege.totalCasualtiesDefender);
            Console.WriteLine(siege.defenderAdditional);
            Console.WriteLine(siege.endDate);
            Console.WriteLine($"------------------- END STORM SIEG -----------------");
            siege.SiegeReductionRound("storm");
            Console.WriteLine($"--------------------- STORM SIEGE 2 ------------------");
            Console.WriteLine(siege.siegeID);
            Console.WriteLine(siege.startYear);
            Console.WriteLine(siege.startSeason);
            Console.WriteLine(siege.besiegingPlayer);
            Console.WriteLine(siege.defendingPlayer);
            Console.WriteLine(siege.besiegerArmy);
            Console.WriteLine(siege.defenderGarrison);
            Console.WriteLine(siege.besiegedFief);
            Console.WriteLine(siege.days);
            Console.WriteLine(siege.totalDays);
            Console.WriteLine(siege.startKeepLevel);
            Console.WriteLine(siege.GetFief().keepLevel);
            Console.WriteLine(siege.totalCasualtiesAttacker);
            Console.WriteLine(siege.totalCasualtiesDefender);
            Console.WriteLine(siege.defenderAdditional);
            Console.WriteLine(siege.endDate);
            Console.WriteLine($"------------------- END STORM SIEG -----------------");
            return new ProtoSiegeDisplay(siege);
        }

        public static ProtoMessage EndSiege(string siegeID, Client client)
        {
            if (string.IsNullOrWhiteSpace(siegeID))
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
            }

            // Get siege
            var siege = client.myPlayerCharacter.GetSiege(siegeID);
            if (siege == null)
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.ErrorGenericSiegeUnidentified;
                return error;
            }
            // check player is besieger
            if (siege.besiegingPlayer != client.myPlayerCharacter.charID)
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.SiegeNotBesieger;
                return error;
            }
            ProtoMessage siegeError = null;
            if (!siege.ChecksBeforeSiegeOperation(out siegeError, "end"))
            {
                return siegeError;
            }
            siege.SiegeEnd(false);
            var reply = new ProtoMessage();
            reply.ResponseType = DisplayMessages.Success;
            reply.Message = siege.siegeID;
            return reply;
        }

        public static ProtoMessage ViewSiege(string siegeID, Client client)
        {
            if (string.IsNullOrWhiteSpace(siegeID))
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
            }
            if (!client.myPlayerCharacter.mySieges.Contains(siegeID))
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                return error;
            }
            var siegeDetails = new ProtoSiegeDisplay(client.myPlayerCharacter.GetSiege(siegeID));
            siegeDetails.ResponseType = DisplayMessages.Success;
            return siegeDetails;
        }

        public static ProtoMessage AdjustCombatValues(string armyID, byte aggression, byte odds, Client client)
        {
            if (string.IsNullOrWhiteSpace(armyID))
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
            }
            // Get army
            Army army = null;
            Globals_Game.armyMasterList.TryGetValue(armyID, out army);
            if (army == null)
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericArmyUnidentified);
            }
            if (!PermissionManager.isAuthorized(PermissionManager.ownsArmyOrAdmin, client.myPlayerCharacter, army))
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericUnauthorised);
            }
            // Attempt to adjust standing orders
            var success = army.AdjustStandingOrders(aggression, odds);
            if (success)
            {
                var result = new ProtoCombatValues(army.aggression, army.combatOdds, army.armyID);
                result.ResponseType = DisplayMessages.Success;
                return result;
            }
            return new ProtoMessage(DisplayMessages.Error) { Message = "adjust standing orders" };
        }

        public static ProtoMessage ExamineArmiesInFief(string fiefID, Client client)
        {
            if (string.IsNullOrWhiteSpace(fiefID))
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
            }
            // get fief
            Fief fief = null;
            foreach (Army army1 in Globals_Game.armyMasterList.Values)
            {
                Console.WriteLine($"{army1.armyID} is in : {army1.location}");
            }

            Globals_Game.fiefMasterList.TryGetValue(fiefID, out fief);
            if (fief == null)
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.ErrorGenericFiefUnidentified;
                return error;
            }
            // Check character is in fief, owns fief, or is admin
            if (!PermissionManager.isAuthorized(PermissionManager.canSeeFiefOrAdmin, client.myPlayerCharacter, fief))
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                return error;
            }
            // get list of armies
            var armies = new List<ProtoArmyOverview>();
            foreach (var armyID in fief.armies)
            {
                Army army = null;
                Globals_Game.armyMasterList.TryGetValue(armyID, out army);
                if (army != null)
                {
                    armies.Add(new ProtoArmyOverview(army));
                }
            }
            // Return array of overviews
            var armyList = new ProtoGenericArray<ProtoArmyOverview>(armies.ToArray());
            armyList.ResponseType = DisplayMessages.Success;
            return armyList;
        }

        public static ProtoMessage GetProvince(string pID)
        {
            if (Globals_Game.provinceMasterList.ContainsKey(pID))
            {
                ProtoMessage province = new ProtoProvince(
                    Globals_Game.provinceMasterList[pID]
                    );
                Console.WriteLine($"province {pID}");
                province.ResponseType = DisplayMessages.Success;
                return province;
            }
            else
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.Error;
                return error;
            }
        }

        public static ProtoMessage Attack(string attackerID, string defenderID, Client client)
        {
            DisplayMessages attackerErrorMessage, defenderErrorMessage;
            // Get attacker and defender
            var armyAttacker = Utility_Methods.GetArmy(attackerID, out attackerErrorMessage);
            var armyDefender = Utility_Methods.GetArmy(defenderID, out defenderErrorMessage);
            if (armyAttacker == null)
            {
                return new ProtoMessage(attackerErrorMessage);
            }
            if (armyDefender == null)
            {
                return new ProtoMessage(defenderErrorMessage);
            }
            if (!PermissionManager.isAuthorized(PermissionManager.ownsArmyOrAdmin, client.myPlayerCharacter, armyAttacker))
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                return error;
            }
            // In the event an army has no troops, return error, log event and clean up
            if (armyAttacker.troops == null || armyAttacker.CalcArmySize() == 0 || armyDefender.troops == null || armyDefender.CalcArmySize() == 0)
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.Error;
                Globals_Server.logError("Found an army with no troops- Performing clean up");
                if (armyAttacker.troops == null || armyAttacker.CalcArmySize() == 0)
                {
                    armyAttacker.DisbandArmy();
                }
                else
                {
                    armyDefender.DisbandArmy();
                }
                return error;
            }
            ProtoMessage attackResult = null;
            if (armyAttacker.ChecksBeforeAttack(armyDefender, out attackResult))
            {
                // GiveBattle returns necessary messages
                ProtoBattle battleResults = null;
                try
                {
                    var isVictorious = Battle.GiveBattle(armyAttacker, armyDefender, out battleResults);
                    battleResults.ResponseType = DisplayMessages.BattleResults;
                    return battleResults;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    return null;
                }
            }
            return attackResult;
        }

        public static ProtoMessage ViewJournalEntries(string scope, Client client)
        {
            // Get list of journal entries for scope
            var entries = client.myPastEvents.getJournalEntrySet(scope, Globals_Game.clock.currentYear, Globals_Game.clock.currentSeason);
            var entryList = new ProtoJournalEntry[entries.Count];
            var i = 0;
            foreach (var entry in entries)
            {
                var newEntry = new ProtoJournalEntry(entry.Value);
                entryList[i] = newEntry;
                i++;
            }
            var result = new ProtoGenericArray<ProtoJournalEntry>(entryList);
            result.ResponseType = DisplayMessages.JournalEntries;
            return result;
        }

        public static ProtoMessage ViewJournalEntry(uint journalID, Client client)
        {
            JournalEntry jEntry = null;
            client.myPastEvents.entries.TryGetValue(journalID, out jEntry);
            if (jEntry == null)
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.ErrorGenericMessageInvalid;
                return error;
            }
            var reply = new ProtoJournalEntry(jEntry);
            reply.ResponseType = DisplayMessages.Success;
            return reply;
        }

        public static ProtoMessage SpyArmy(string armyID, string charID, Client client)
        {
            DisplayMessages armyErr, charErr;
            var army = Utility_Methods.GetArmy(armyID, out armyErr);
            var spy = Utility_Methods.GetCharacter(charID, out charErr);

            Globals_Game.armyMasterList.TryGetValue(armyID, out army);
            // Ensure character and army are valid
            if (spy == null)
            {
                return new ProtoMessage(charErr);
            }
            if (army == null)
            {
                return new ProtoMessage(armyErr);
            }
            // Ensure spy is pc's character
            if (
                !PermissionManager.isAuthorized(PermissionManager.ownsCharNotCapturedOrAdmin, client.myPlayerCharacter,
                    spy))
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                return error;
            }
            // Check can actually spy on this character
            ProtoMessage checkMsg = null;
            if (!spy.SpyCheck(army, out checkMsg))
            {
                return checkMsg;
            }
            ProtoMessage result = null;
            spy.SpyOn(army, out result);
            return result;
        }

        /// <summary>
        /// Process a client's request to spy on a character
        /// </summary>
        /// <param name="charID">The spy's character ID</param>
        /// <param name="targetID">The target's character ID</param>
        /// <param name="client">Client who sent the request</param>
        /// <returns>Result of attempting to spy</returns>
        public static ProtoMessage SpyCharacter(string charID, string targetID, Client client)
        {
            DisplayMessages charErr, targetErr;
            var target = Utility_Methods.GetCharacter(targetID, out targetErr);
            var spy = Utility_Methods.GetCharacter(charID, out charErr);
            // Ensure character and army are valid
            if (spy == null)
            {
                return new ProtoMessage(charErr);
            }
            if (target == null)
            {
                return new ProtoMessage(targetErr);
            }
            // Ensure spy is pc's character
            if (
                !PermissionManager.isAuthorized(PermissionManager.ownsCharNotCapturedOrAdmin, client.myPlayerCharacter,
                    spy))
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericUnauthorised);
            }
            // Check can actually spy on this character
            ProtoMessage checkMsg = null;
            if (!spy.SpyCheck(target, out checkMsg))
            {
                return checkMsg;
            }
            ProtoMessage result = null;
            spy.SpyOn(target, out result);
            return result;
        }

        /// <summary>
        /// Process a client's request to spy on a fief
        /// </summary>
        /// <param name="charID">Character ID of spy</param>
        /// <param name="fiefID">Fief ID of fief to spy on</param>
        /// <param name="client">Client who sent the request</param>
        /// <returns>Result of attempt to spy</returns>
        public static ProtoMessage SpyFief(string charID, string fiefID, Client client)
        {
            DisplayMessages fiefErrMsg, charErrMsg;
            var fief = Utility_Methods.GetFief(fiefID, out fiefErrMsg);
            var spy = Utility_Methods.GetCharacter(charID, out charErrMsg);
            // Ensure character and army are valid
            if (spy == null)
                return new ProtoMessage(charErrMsg);
            if (fief == null)
                return new ProtoMessage(fiefErrMsg);
            // Ensure spy is pc's character
            if (
                !PermissionManager.isAuthorized(PermissionManager.ownsCharNotCapturedOrAdmin, client.myPlayerCharacter,
                    spy))
                return new ProtoMessage(DisplayMessages.ErrorGenericUnauthorised);
            // Check can actually spy on this character
            ProtoMessage checkMsg = null;
            if (!spy.SpyCheck(fief, out checkMsg))
            {
                return checkMsg;
            }
            ProtoMessage result = null;
            spy.SpyOn(fief, out result);
            return result;
        }


        /// <summary>
        ///     Kidnap a target character. Client must own kidnapper, and both targetID and kidnapperID must be valid Character IDs
        /// </summary>
        /// <param name="targetID">Character ID of target Character</param>
        /// <param name="kidnapperID">Character ID of kidnapper</param>
        /// <param name="client">Client who requested kidnapping (must own kidnapper)</param>
        /// <returns>Result of attempted kidnapping, or an error message</returns>
        public static ProtoMessage Kidnap(string targetID, string kidnapperID, Client client)
        {
            DisplayMessages targetErrMsg;
            DisplayMessages kidnapperErrMsg;
            var target = Utility_Methods.GetCharacter(targetID, out targetErrMsg);
            var kidnapper = Utility_Methods.GetCharacter(kidnapperID, out kidnapperErrMsg);
            if (kidnapper == null)
            {
                return new ProtoMessage(kidnapperErrMsg);
            }
            if (target == null)
            {
                return new ProtoMessage(targetErrMsg);
            }
            if (
                !PermissionManager.isAuthorized(PermissionManager.ownsCharNotCapturedOrAdmin, client.myPlayerCharacter,
                    kidnapper))
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericUnauthorised);
            }
            ProtoMessage result;
            kidnapper.Kidnap(target, out result);
            return result;
        }

        /// <summary>
        ///     Fief all captives in a location. Location can be "all" for all held captives across all Fiefs, or a Fief ID for
        ///     captives in that fief
        /// </summary>
        /// <param name="captiveLocation">Fief ID or "all"</param>
        /// <param name="client">Client who requested to view captives</param>
        /// <returns>ProtoGenericArray of ProtoCharacterOverview containing details of all captives, or an error message</returns>
        public static ProtoMessage ViewCaptives(string captiveLocation, Client client)
        {
            if (string.IsNullOrWhiteSpace(captiveLocation))
            {
                return new ProtoMessage(DisplayMessages.ErrorGenericMessageInvalid);
            }
            List<Character> captiveList = null;
            if (captiveLocation.Equals("all"))
            {
                captiveList = client.myPlayerCharacter.myCaptives;
            }
            else
            {
                // If not all captives, check for fief captives
                DisplayMessages errorMsg;
                var fief = Utility_Methods.GetFief(captiveLocation, out errorMsg);
                if (fief != null)
                {
                    // Ensure has permission
                    if (
                        !PermissionManager.isAuthorized(PermissionManager.ownsFiefOrAdmin, client.myPlayerCharacter,
                            fief))
                    {
                        var error = new ProtoMessage();
                        error.ResponseType = DisplayMessages.ErrorGenericUnauthorised;
                        return error;
                    }
                    captiveList = fief.gaol;
                }
                else
                {
                    // error
                    var error = new ProtoMessage();
                    error.ResponseType = errorMsg;
                    return error;
                }
            }
            if (captiveList != null && captiveList.Count > 0)
            {
                var captives = new ProtoGenericArray<ProtoCharacterOverview>();
                captives.fields = new ProtoCharacterOverview[captiveList.Count];
                var i = 0;
                foreach (var captive in captiveList)
                {
                    var captiveDetails = new ProtoCharacterOverview(captive);
                    captiveDetails.showLocation(captive);
                    captives.fields[i] = captiveDetails;
                    i++;
                }
                captives.ResponseType = DisplayMessages.Success;
                return captives;
            }
            var NoCaptives = new ProtoMessage();
            NoCaptives.ResponseType = DisplayMessages.FiefNoCaptives;
            return NoCaptives;
        }

        public static ProtoMessage ViewCaptive(string charID, Client client)
        {
            DisplayMessages charErr;
            var captive = Utility_Methods.GetCharacter(charID, out charErr);
            if (captive == null)
            {
                return new ProtoMessage(charErr);
            }
            if (!client.myPlayerCharacter.myCaptives.Contains(captive))
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.NotCaptive;
                return error;
            }
            var captiveDetails = new ProtoCharacter(captive);
            captiveDetails.ResponseType = DisplayMessages.Success;
            return captiveDetails;
        }

        public static ProtoMessage RansomCaptive(string charID, Client client)
        {
            DisplayMessages charErr;
            var captive = Utility_Methods.GetCharacter(charID, out charErr);
            if (captive == null)
            {
                return new ProtoMessage(charErr);
            }
            if (!client.myPlayerCharacter.myCaptives.Contains(captive))
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.NotCaptive;
                return error;
            }
            if (!string.IsNullOrWhiteSpace(captive.ransomDemand))
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.RansomAlready;
                return error;
            }
            client.myPlayerCharacter.RansomCaptive(captive);
            var success = new ProtoMessage();
            success.ResponseType = DisplayMessages.Success;
            success.Message = captive.CalculateRansom().ToString();
            return success;
        }

        public static ProtoMessage ReleaseCaptive(string charID, Client client)
        {
            DisplayMessages charErr;
            Character captive = null;
            captive = Utility_Methods.GetCharacter(charID, out charErr);
            if (captive == null)
            {
                return new ProtoMessage(charErr);
            }
            if (!client.myPlayerCharacter.myCaptives.Contains(captive))
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.NotCaptive;
                return error;
            }
            client.myPlayerCharacter.ReleaseCaptive(captive);
            var success = new ProtoMessage();
            success.ResponseType = DisplayMessages.Success;
            return success;
        }

        /// <summary>
        ///     Process a client request to kill a captive
        /// </summary>
        /// <param name="charID">ID of character to execute</param>
        /// <param name="client">client who sent request</param>
        /// <returns>Result of this request</returns>
        public static ProtoMessage ExecuteCaptive(string charID, Client client)
        {
            DisplayMessages charErr;
            var captive = Utility_Methods.GetCharacter(charID, out charErr);
            if (captive == null)
            {
                return new ProtoMessage(charErr);
            }
            if (!client.myPlayerCharacter.myCaptives.Contains(captive))
            {
                return new ProtoMessage(DisplayMessages.NotCaptive);
            }
            client.myPlayerCharacter.ExecuteCaptive(captive);
            return new ProtoMessage(DisplayMessages.Success);
        }

        public static ProtoMessage RespondRansom(uint jEntryID, bool pay, Client client)
        {
            JournalEntry jEntry = null;
            Globals_Game.pastEvents.entries.TryGetValue(jEntryID, out jEntry);
            if (jEntry == null)
            {
                var error = new ProtoMessage();
                error.ResponseType = DisplayMessages.JournalEntryUnrecognised;
                return error;
            }
            ProtoMessage ransomResult = null;
            if (jEntry.RansomResponse(pay, out ransomResult))
            {
                ransomResult = new ProtoMessage();
                ransomResult.ResponseType = DisplayMessages.Success;
                return ransomResult;
            }
            return ransomResult;
        }

        public static ProtoMessage ViewWorldMap()
        {
            ProtoWorldMap response = new ProtoWorldMap(Globals_Game.gameMapLayout, Globals_Game.fiefMasterList);
            response.ResponseType = DisplayMessages.Success;
            return response;
        }

        private ProtoMessage GenericMessage(Actions action, DisplayMessages responseType, string message = "", string[]? fields = null )
        {
            if(fields == null)
                fields = Array.Empty<string>();
            return new ProtoMessage()
            {
                ActionType = action,
                Message = message,
                MessageFields = fields,
                ResponseType = responseType
            };
        }

        private byte[] SerializeMessage<T>(T message) where T : class
        {
            using var stream = new MemoryStream();
            Serializer.Serialize(stream, message);
            return stream.ToArray();
        }

        #region Testing
        private void PrintByteArray(byte[] bytes)
        {
            var sb = new StringBuilder("new byte[] { ");
            foreach (var b in bytes)
            {
                sb.Append(b + ", ");
            }
            sb.Append("}");
            Console.WriteLine(sb.ToString());
        }
        #endregion
    }
}
