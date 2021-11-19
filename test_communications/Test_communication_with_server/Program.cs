using ProtoMessageClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace JominiAI
{
    /// <summary>
    ///     These are the available agent types
    /// </summary>
    public enum AgentTypes
    {
        RuleBased, Minimax, Random, Script
    }

    /// <summary>
    ///     Main class to run the agents
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            //AutoEvaluation(1);


            /*TextTestClient client = new TextTestClient();
            client.LogInAndConnect("helen", "potato");

            while(!client.IsConnectedAndLoggedIn()) {
                Thread.Sleep(0);
            }
            if(client.IsConnectedAndLoggedIn()) {
                Console.WriteLine("Logged in");

                ProtoMessage request = new ProtoMessage {
                    ActionType = Actions.ViewJournalEntries,
                    Message = "all"
                };
                client.net.Send(request);
                ProtoMessage journal = GetActionReply(Actions.ViewJournalEntries, client);
                Console.WriteLine($"Journal Length: {((ProtoGenericArray<ProtoJournalEntry>)journal).fields.Length}");

                client.LogOut();
            }*/


            //RunAgent("helen", "potato", AgentTypes.RuleBased, false);
            //RunAgent("helen", "potato", AgentTypes.Script);

            //Evaluation.Army_1();
            Console.WriteLine("\nMain() finished");
        }

        public static ProtoMessage GetActionReply(Actions action, TextTestClient client) {
            bool receivedActionReply = false;
            bool receivedUpdateReply = false;
            ProtoMessage reply;
            ProtoMessage actionReply = null;

            if (action == Actions.LogIn) {
                receivedUpdateReply = true;
            }
            int i = 0;
            do {
                Console.WriteLine("Check for message : " + i++);
                reply = client.CheckForProtobufMessage();
                if (reply == null) { // wait time expired.
                    client.LogOut();
                    return null;
                }

                if (reply.ActionType == action) {
                    Console.WriteLine("Correct action found: " + reply.ActionType.ToString() + " /w " + reply.ResponseType.ToString());
                    actionReply = reply;
                    receivedActionReply = true;
                    receivedUpdateReply = true;
                } else if (reply.ActionType == Actions.Update) {
                    Console.WriteLine("Update action found: " + reply.ActionType.ToString() + " /w " + reply.ResponseType.ToString());
                    if (reply.ResponseType == DisplayMessages.ErrorGenericMessageInvalid) {
                        actionReply = reply; // When the server found something wrong with the action.
                        receivedActionReply = true;
                    }
                    else if (reply.ResponseType == DisplayMessages.Success) {
                        ProtoClient protoClient = (ProtoClient)reply;
                        receivedUpdateReply = true;
                    }
                } else {
                    Console.WriteLine("Mismatching action found: " + reply.ActionType.ToString() + " /w " + reply.ResponseType.ToString());
                }

            } while (!receivedActionReply || !receivedUpdateReply);

            Console.WriteLine("Finished reply");
            return actionReply;
        }

        /// <summary>
        ///     
        /// </summary>
        public static void AutoEvaluation(int nbExecutions = 1)
        {
            //Program.RunAgent("helen", "potato", AgentTypes.RuleBased, true);

            Evaluation.executedEvaluationIDs = new List<string>();
            Evaluation.executedEvaluationResults = new List<bool>();
            Evaluation.executionTimes = new List<double>();
            Evaluation.waitingForServerAllTimes = new List<List<double>>();

            for (int i=0; i< nbExecutions; i++)
            {
                Console.WriteLine("\n\nStart eval: Army Maintanence.\n");
                Evaluation.Simple_Eval_Army_Maintain();
                Console.WriteLine("\n\nEnd eval: Army Maintanence.\n");
                break;

                Console.WriteLine("\n\n----- Start eval: Army Bar Character. ------\n");
                Evaluation.Simple_Eval_Army_BarCharacter();

                //Evaluation.Simple_Eval_Army_Attack_0(); // False => !!! FIRST NEED TO MODIFY SERVER SO WE CAN KNOW THE OWNER OF AN ARMY (the charID, not the name) !!!
                //Evaluation.Simple_Eval_Army_Attack_1(); // False => !!! FIRST NEED TO MODIFY SERVER SO WE CAN KNOW THE OWNER OF AN ARMY (the charID, not the name) !!!
                Console.WriteLine("\n\nStart eval: Army Pillage.\n");
                Evaluation.Simple_Eval_Army_pillage();

                Console.WriteLine("\n\nStart eval: Army Drop Off Disband.\n");
                Evaluation.Simple_Eval_Army_dropOff_disband();

                Console.WriteLine("\n\nStart eval: Army Drop Off.\n");
                Evaluation.Simple_Eval_Army_dropOff();

                Console.WriteLine("\n\nStart eval: Army Appoint Leader 0.\n");
                Evaluation.Simple_Eval_Army_appointLeader_0();

                Console.WriteLine("\n\nStart eval: Army Appoint Leader 1.\n");
                Evaluation.Simple_Eval_Army_appointLeader_1();

                //Evaluation.Simple_Eval_Army_besiege_0(); // False => !!! FIRST NEED TO FIX THE HEREDITARY PROBLEM SO I CAN USE THE SPYFIEF FUNCTIONALITY !!!
                Console.WriteLine("\n\nStart eval: Army Storm Siege.\n");
                Evaluation.Simple_Eval_Army_stormSiege();

                Console.WriteLine("\n\nStart eval: Army Negociate Siege.\n");
                Evaluation.Simple_Eval_Army_negociateSiege();

                Console.WriteLine("\n\nStart eval: Army End Siege.\n");
                Evaluation.Simple_Eval_Army_endSiege();

                Console.WriteLine("\n\nStart eval: Army Recruit.\n");
                Evaluation.Simple_Eval_Army_recruit();

                Console.WriteLine("\n\nStart eval: Family Marry.\n");
                Evaluation.Simple_Eval_Family_Marry();

                Console.WriteLine("\n\nStart eval: Family Try For Child.\n");
                Evaluation.Simple_Eval_Family_tryForChild();

                Console.WriteLine("\n\nStart eval: Family Appoint Heir.\n");
                Evaluation.Simple_Eval_Family_AppointHeir();

                Console.WriteLine("\n\nStart eval: Fief Fire.\n");
                Evaluation.Simple_Eval_Fief_fire();

                Console.WriteLine("\n\nStart eval: Fief Hire.\n");
                Evaluation.Simple_Eval_Fief_hire();

                Console.WriteLine("\n\nStart eval: Fief Move 0.\n");
                Evaluation.Simple_Eval_Fief_move_0();

                Console.WriteLine("\n\nStart eval: Fief Move 1.\n");
                Evaluation.Simple_Eval_Fief_move_1();

                Console.WriteLine("\n\nStart eval: Fief Move 2.\n");
                Evaluation.Simple_Eval_Fief_move_2();

                Console.WriteLine("\n\nStart eval: Fief Move 3.\n");
                Evaluation.Simple_Eval_Fief_move_3();

                Console.WriteLine("\n\nStart eval: Fief Unbar.\n");
                Evaluation.Simple_Eval_Fief_unbar();

                Console.WriteLine("\n\nStart eval: Fief Enter Keep.\n");
                Evaluation.Simple_Eval_Fief_enterKeep();

                Console.WriteLine("\n\nStart eval: Fief Add Entourage.\n");
                Evaluation.Simple_Eval_Fief_addEntourage();

                Console.WriteLine("\n\nStart eval: Fief Remove Entourage.\n");
                Evaluation.Simple_Eval_Fief_removeEntourage();

                Console.WriteLine("\n\nStart eval: Fief Appoint Baillif.\n");
                Evaluation.Simple_Eval_Fief_appointBaillif();

                Console.WriteLine("\n\nStart eval: Fief Remove Baillif.\n");
                Evaluation.Simple_Eval_Fief_removeBaillif();

                Console.WriteLine("\n\nStart eval: Fief Auto Adjust Expenditure.\n");
                Evaluation.Simple_Eval_Fief_autoAdjustExpenditure();

                Console.WriteLine("\n\nStart eval: Fief Transfer Funds.\n");
                Evaluation.Simple_Eval_Fief_transferFunds();

                Console.WriteLine("\n\nStart eval: Fief Spy Character.\n");
                Evaluation.Simple_Eval_Fief_spyCharacter();
            }

            int numberOfScenarios = Evaluation.executedEvaluationIDs.Count / nbExecutions;
            for (int s = 0; s < numberOfScenarios; s++)
            {
                int numberOfSuccesses = 0;
                for (int i = s; i < Evaluation.executedEvaluationIDs.Count; i += numberOfScenarios)
                {
                    if (Evaluation.executedEvaluationResults[i])
                        numberOfSuccesses++;
                }
                Console.WriteLine(numberOfSuccesses +  " / " + nbExecutions + " successes => " + Evaluation.executedEvaluationIDs[s]);
            }
            Console.WriteLine("");

            List<double> nbRequestsByCycle = new List<double>();
            List<double> timesWaitingForServerByCycle = new List<double>();
            foreach (List<double> waitingForServerTimes in Evaluation.waitingForServerAllTimes)
            {
                nbRequestsByCycle.Add(waitingForServerTimes.Count);
                timesWaitingForServerByCycle.Add(waitingForServerTimes.Sum());
            }
            Console.WriteLine("Number of single action executions= " + Evaluation.executionTimes.Count);
            Console.WriteLine("Average number of resquests sent for a cycle= " + nbRequestsByCycle.Average());
            Console.WriteLine("Minimum number of resquests sent for a cycle= " + nbRequestsByCycle.Min());
            Console.WriteLine("Maximum number of resquests sent for a cycle= " + nbRequestsByCycle.Max());
            Console.WriteLine("Average time waiting for server for a cycle= " + timesWaitingForServerByCycle.Average());
            Console.WriteLine("Minimum time waiting for server for a cycle= " + timesWaitingForServerByCycle.Min());
            Console.WriteLine("Maximum time waiting for server for a cycle= " + timesWaitingForServerByCycle.Max());
            Console.WriteLine("Average time of execution for a cycle= " + Evaluation.executionTimes.Average());
            Console.WriteLine("Maximum time of execution for a cycle= " + Evaluation.executionTimes.Max());
            Console.WriteLine("");
            
        }

        /// <summary>
        ///     Main function to run the agents
        /// </summary>
        /// <param name="username">account's username to log to</param>
        /// <param name="password">account's password to log to</param>
        /// <param name="agentType">Type of the agent to run</param>
        /// <param name="withBreaks">Indicates if there are prompts after each taken actions</param>
        /// <param name="maxEstimatedNbDays">Indicates how many days the agent will play in case the day counter of the server has been disabled and this is just an estimation, 
        /// though if he reaches a GameState where there's no possible action to execute, he will stop (e.g. End of the season or no heir and main character dying)</param>
        public static void RunAgent(string username, string password, AgentTypes agentType, bool withBreaks = true, int maxEstimatedNbDays = 0)
        {
            Agent agent;
            switch (agentType)
            {
                case AgentTypes.RuleBased:
                    agent = new RuleBasedAgent(username, password);
                    break;
                case AgentTypes.Minimax:
                    agent = new MinimaxAgent(username, password, 1);
                    break;
                case AgentTypes.Random:
                    agent = new RandomAgent(username, password);
                    break;
                case AgentTypes.Script:
                    agent = new ScriptAgent(username, password);
                    break;
                default:
                    throw new Exception("Case '" + agentType.ToString() + "' is missing");
            }
            if (agentType == AgentTypes.Script)
                (agent as ScriptAgent).Test_28(); // Script to run
            else
            {
                GameState finalGS = agent.play(withBreaks, out GameState startingGS, maxEstimatedNbDays);
                Console.WriteLine("The starting GameState score is: " + Tools.calculateGameStateScore(startingGS));
                Console.WriteLine("The final GameState score is: " + Tools.calculateGameStateScore(finalGS));
            }
        }
    }
}