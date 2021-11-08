using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace JominiAI
{
    /// <summary>
    ///     Contains the evaluation scenarios
    ///     If no GameObject file is specified, then the csv used is 'gameObjects_modified_Default.csv'
    /// </summary>
    static class Evaluation
    {
        public static List<string> executedEvaluationIDs = new List<string>();
        public static List<bool> executedEvaluationResults = new List<bool>();
        public static List<double> executionTimes = new List<double>();
        public static List<List<double>> waitingForServerAllTimes = new List<List<double>>();

        public static string username = "helen";
        public static string password = "potato";

        /// <summary>
        ///     Scenario: 'helen' account has only one little army that isn't maintained
        ///     
        ///     Expected result: 'helen' should maintain the army
        /// </summary>
        public static void Simple_Eval_Army_Maintain()
        {
            new ScriptAgent(username , password).LoadScenario("Simple_Eval_Army_Maintain_0");

            runSimpleEvaluation("Simple_Eval_Army_Maintain_0");
        }

        /// <summary>
        ///     Scenario: 'test' account pillage one of the 'helen' account fief
        ///     
        ///     Expected result: The PC pillaging the fief would become an enemy and would be barred from all his fiefs
        /// </summary>
        public static void Simple_Eval_Army_BarCharacter()
        {
            new ScriptAgent(username , password).LoadScenario("Default");

            new ScriptAgent(username, password).Simple_Eval_Army_BarCharacter_0_test();
            runSimpleEvaluation("Simple_Eval_Army_BarCharacter_0");
        }

        /// <summary>
        ///     Scenario: 'test' account becomes an enemy of 'helen' and has an army well smaller than 'helen' in the same fief as 'helen' PC (in one of 'helen' fief)
        ///     
        ///     Modification on server side:
        ///     - 'test' PC has only one army of size 1
        /// 
        ///     Expected result: 'helen' => The PC pillaging the fief should become an enemy and his army should be attacked
        /// </summary>
        public static void Simple_Eval_Army_Attack_0()
        {
            new ScriptAgent(username , password).LoadScenario("Simple_Eval_Army_Attack_0");

            new ScriptAgent(username , password).Simple_Eval_Army_Attack_0_test();
            runSimpleEvaluation("Simple_Eval_Army_Attack_0");
        }

        /// <summary>
        ///     Scenario: 'test' account becomes an enemy of 'helen' and has an army well bigger than 'helen' on the same fief as 'helen' PC
        ///     
        ///     Modification on server side: 
        ///     -'Char_158' begins with one army of size 99999 that is maintained
        /// 
        ///     Expected result: 'helen' => The PC pillaging the fief would become an enemy and his army wouldn't be attacked
        /// </summary>
        public static void Simple_Eval_Army_Attack_1()
        {
            new ScriptAgent(username , password).LoadScenario("Simple_Eval_Army_Attack_1");

            new ScriptAgent(username , password).Simple_Eval_Army_Attack_1_test();
            runSimpleEvaluation("Simple_Eval_Army_Attack_1");
        }

        /// <summary>
        ///     Scenario: 'helen' PC starts in one of 'test' fief and there's no baillif
        ///     
        ///     Modification on server side:
        ///     -'helen' PC is in fief 'ENU02'
        /// 
        ///     Expected result: 'helen' should pillage the fief as he has 0 enemies
        /// </summary>
        public static void Simple_Eval_Army_pillage()
        {
            new ScriptAgent(username , password).LoadScenario("Simple_Eval_Army_pillage_0");

            new ScriptAgent(username , password).Simple_Eval_Army_pillage_0_test();
            runSimpleEvaluation("Simple_Eval_Army_pillage_0");
        }

        /// <summary>
        ///     Scenario: 'helen' account have one (and only one) very big army that is not maintained but it doesn't have much treasury
        ///     
        ///     Modification on server side: 
        ///     -'Char_158' begins with only one army of size 99999 that is maintained
        ///     
        ///     Expected result: The rulebased agent shouldn't drop-off troops or disband the army as the army is already maintained and will cost nothing more for the duration of this season
        /// </summary>
        public static void Simple_Eval_Army_dropOff_disband()
        {
            new ScriptAgent(username , password).LoadScenario("Simple_Eval_Army_dropOff_disband_0");

            runSimpleEvaluation("Simple_Eval_Army_dropOff_disband_0");
        }

        /// <summary>
        ///     Scenario: 'helen' account have one (and only one) very big army that is maintained but it doesn't have much treasury
        ///     
        ///     Modification on server side: 'Char_158' begins with one army of size 29999 that isn't maintained
        ///     
        ///     Expected result: The rulebased agent should drop-off some troops
        /// </summary>
        public static void Simple_Eval_Army_dropOff()
        {
            new ScriptAgent(username , password).LoadScenario("Simple_Eval_Army_dropOff_0");

            runSimpleEvaluation("Simple_Eval_Army_dropOff_0");
        }

        /// <summary>
        ///     Scenario: Will have a 'mighty NPC' with more combat skills than the main character, and they are on the same fief
        ///     
        ///     Modification on server side:
        ///     - 'helen' owns the NPC "Char_425"
        ///     
        ///     Expected result: The rule-based agent should appoint the 'mighty NPC' as leader of the main army => Then check if there's some anomalies related to the new army leadership
        /// </summary>
        public static void Simple_Eval_Army_appointLeader_0()
        {
            new ScriptAgent(username , password).LoadScenario("Simple_Eval_Army_appointLeader_0");

            runSimpleEvaluation("Simple_Eval_Army_appointLeader_0");
        }

        /// <summary>
        ///     Scenario: 'Char_158' isn't leading an army and his 2 armies (of different sizes) are in the current fief
        ///     
        ///     Modification on server side: 'helen' => 'Char_158' isn't leading an army and and army of sizes 10 and 15 are present in the current fief
        ///     
        ///     Expected result: 'Char_158' should appoint himself leader of the biggest army
        /// </summary>
        public static void Simple_Eval_Army_appointLeader_1()
        {
            new ScriptAgent(username , password).LoadScenario("Simple_Eval_Army_appointLeader_1");

            runSimpleEvaluation("Simple_Eval_Army_appointLeader_1");
        }

        /// <summary>
        ///     // !!! FIRST NEED TO FIX THE HEREDITARY PROBLEM SO I CAN USE THE SPYFIEF FUNCTIONALITY !!!
        ///     Scenario: 'test' account becomes an enemy of 'helen' and 'helen' PC is in one of 'test' fief with a very big army
        ///     
        ///     Modification on server side:
        ///     -'helen' PC begins with one army of size 99999 that is maintained
        ///     -'helen' PC is in fief 'ENU02'
        ///     
        ///     Expected result: 'helen' should spy the fief and then besiege it
        /// </summary>
        public static void Simple_Eval_Army_besiege()
        {
            new ScriptAgent(username , password).LoadScenario("Simple_Eval_Army_besiege_0");

            new ScriptAgent(username , password).Simple_Eval_Army_besiege_0_test();
            runSimpleEvaluation("Simple_Eval_Army_besiege_0");
        }

        /// <summary>
        ///     Scenario: 'helen' PC is besieging one of 'test' fiefs with a very big army
        ///     
        ///     Modification on server side:
        ///     - 'helen' PC has only one army of size 99999 that is maintained
        ///     - 'helen' PC is in fief 'ENU02' (owner is 'test')
        ///     
        ///     Expected result: 'Char_158' should storm the siege
        /// </summary>
        public static void Simple_Eval_Army_stormSiege()
        {
            new ScriptAgent(username , password).LoadScenario("Simple_Eval_Army_stormSiege_0");

            new ScriptAgent(username , password).Simple_Eval_Army_stormSiege_0_helen();
            runSimpleEvaluation("Simple_Eval_Army_stormSiege_0");
        }

        /// <summary>
        ///     Scenario: 'helen' PC is besieging one of 'test' fiefs with an army of size keepLevel*1000
        ///     
        ///     Modification on server side:
        ///     -'helen' PC has only one army of size 4000 ('ENU02' has a keep lvl 4) that is maintained
        ///     - 'helen' PC is in fief 'ENU02' (owner is 'test')
        ///     
        ///     Expected result: 'Char_158' should negociate the siege
        /// </summary>
        public static void Simple_Eval_Army_negociateSiege()
        {
            new ScriptAgent(username , password).LoadScenario("Simple_Eval_Army_negociateSiege_0");

            new ScriptAgent(username , password).Simple_Eval_Army_negociateSiege_0_helen();
            runSimpleEvaluation("Simple_Eval_Army_negociateSiege_0");
        }

        /// <summary>
        ///     Scenario: 'helen' PC is besieging one of 'test' fiefs with a very small army
        ///     
        /// Modification on server side:
        ///     - 'helen' PC is in fief 'ENU02' (owner is 'test')
        /// 
        ///     Expected result: 'Char_158' should end the siege
        /// </summary>
        public static void Simple_Eval_Army_endSiege()
        {
            new ScriptAgent(username , password).LoadScenario("Simple_Eval_Army_endSiege_0");

            new ScriptAgent(username , password).Simple_Eval_Army_endSiege_0_helen();
            runSimpleEvaluation("Simple_Eval_Army_endSiege_0");
        }

        /// <summary>
        ///     Scenario: 'Char_158' has a good amount of treasury and a little army
        ///     
        ///     Modification on server side:
        ///     - Treasury of fief 'EPM02' (homefief) changed to 9999999
        ///     - Population of fief 'EPM01' changed to 9999999
        ///     
        ///     Expected result: 'Char_158' should recruit
        /// </summary>
        public static void Simple_Eval_Army_recruit()
        {
            new ScriptAgent(username , password).LoadScenario("Simple_Eval_Army_recruit_0");

            runSimpleEvaluation("Simple_Eval_Army_recruit_0");
        }

        /// <summary>
        ///     Scenario: 'helen' PC is without wife and without heir and is situated in a fief where this is a suitable NPC to marry
        ///     
        ///     Modification on server side:
        ///     - Char_158: No wife
        ///     - Char_158: No NPCs
        ///     - 'Char_1189' is in the same fief as 'helen' PC
        ///     
        ///     Expected result: 'helen' should try to marry the suitable NPC
        /// </summary>
        public static void Simple_Eval_Family_Marry()
        {
            new ScriptAgent(username , password).LoadScenario("Simple_Eval_Family_Marry_0");

            runSimpleEvaluation("Simple_Eval_Family_Marry_0");
        }

        /// <summary>
        ///     Scenario: 'helen' PC is on the same fief as his spouse and has no suitable heir
        ///     
        ///     Modification on server side:
        ///     - Char_158: No NPCs except his spouse
        ///     
        ///     Expected result: 'helen' should try to have a child
        /// </summary>
        public static void Simple_Eval_Family_tryForChild()
        {
            new ScriptAgent(username , password).LoadScenario("Simple_Eval_tryForChild_0");

            runSimpleEvaluation("Simple_Eval_tryForChild_0");
        }

        /// <summary>
        ///     Scenario: 'helen' as a suitable heir but no heir at the moment
        ///     
        ///     Expected result: 'helen' should appoint the NPC as heir
        /// </summary>
        public static void Simple_Eval_Family_AppointHeir()
        {
            new ScriptAgent(username , password).LoadScenario("Default");

            runSimpleEvaluation("Simple_Eval_Family_AppointHeir_0");
        }

        /// <summary>
        ///     Scenario: 'helen' account has a lot of employees
        ///     
        ///     Modification on server side:
        ///     - All NPCs from 'Char_4000' to 'Char_4044' now have as employer 'Char_158'
        ///     
        ///     Expected action: Fire an employee
        /// </summary>
        public static void Simple_Eval_Fief_fire()
        {
            new ScriptAgent(username , password).LoadScenario("Simple_Eval_Fief_fire_0");

            runSimpleEvaluation("Simple_Eval_Fief_fire_0");
        }

        /// <summary>
        ///     Scenario: 'helen' account has no NPCs and a good amount of treasury and there's available NPCs in the current fief
        ///     
        ///     Expected result: 'helen' should hire NPCs
        /// </summary>
        public static void Simple_Eval_Fief_hire()
        {
            new ScriptAgent(username , password).LoadScenario("Default");

            runSimpleEvaluation("Simple_Eval_Fief_hire_0");
        }

        /// <summary>
        ///     Scenario: 'helen' PC has no available heir and is on a different fief from his spouse
        ///     
        ///     Modification on server side:
        ///     - Char_158: No NPCs except his spouse
        ///     - 'helen' PC is in fief 'EPM01'
        ///     
        ///     Expected action: Should move to the fief where his spouse is
        /// </summary>
        public static void Simple_Eval_Fief_move_0()
        {
            new ScriptAgent(username , password).LoadScenario("Simple_Eval_Fief_move_0");

            runSimpleEvaluation("Simple_Eval_Fief_move_0");
        }

        /// <summary>
        ///     Scenario: 'test' account besieges one of the 'helen' account fief (not the fief where 'helen' PC is) with an army of big size (compared to the keep level)
        ///     and 'helen' has one army of same size
        ///     
        ///     Modification on server side: 
        ///     - 'helen' has one army of size 25000 and it is maintained
        ///     - 'test' has one army of size 25000 and it is maintained
        ///     - 'test' PC is in fief 'EPM03'
        ///     
        ///     Expected result: 'helen' PC should move to the besieged fief
        /// </summary>
        public static void Simple_Eval_Fief_move_1()
        {
            new ScriptAgent(username , password).LoadScenario("Simple_Eval_Fief_move_1");

            new ScriptAgent(username, password).Simple_Eval_Fief_move_1_test();
            runSimpleEvaluation("Simple_Eval_Fief_move_1");
        }

        /// <summary>
        ///     Scenario: 'Char_158' has a lot of treasury, a little army and he is situated at the fief 'ESW05' that is near 'EPM02', 
        ///     and a bit further from 'EPM03' which has a very big population. There's no NPCs in 'helen' PCs current fief.
        ///     
        ///     Modification on server side:
        ///     - 'helen' PC is in fief 'ESW05'
        ///     - All characters (except 'helen' PC) present in fief 'ESW05' are moved to the fief 'ESW04'
        ///     - Treasury of fief 'EPM02' (homefief) changed to 9999999
        ///     - Population of fief 'EPM03' changed to 9999999
        ///     
        ///     Expected result: 'Char_158' should want go to the fief with the biggest population to recruit
        /// </summary>
        public static void Simple_Eval_Fief_move_2()
        {
            new ScriptAgent(username , password).LoadScenario("Simple_Eval_Fief_move_2");

            runSimpleEvaluation("Simple_Eval_Fief_move_2");
        }

        /// <summary>
        ///     Scenario: 'helen' PC has a big enough army (no need to recruit) and there's no other character in the current fief
        ///     
        ///     Modification on server side: 
        ///     - 'helen' has only one army of size 300 that is maintained
        ///     - No character in current fief except 'helen' PC
        /// 
        ///     Expected result: 'helen' should move in a random direction as at the start he only knows the location of his fiefs and has no reason to go to one of his fiefs.
        /// </summary>
        public static void Simple_Eval_Fief_move_3()
        {
            new ScriptAgent(username , password).LoadScenario("Simple_Eval_Fief_move_3");

            runSimpleEvaluation("Simple_Eval_Fief_move_3");
        }

        /// <summary>
        ///     Scenario: 'helen' PC owns an NPC that is barred from one of his fiefs
        /// 
        ///     Expected result: 'helen' should unbar the NPC
        /// </summary>
        public static void Simple_Eval_Fief_unbar()
        {
            new ScriptAgent(username , password).LoadScenario("Default");

            new ScriptAgent(username , password).Simple_Eval_Fief_unbar_0_helen();
            runSimpleEvaluation("Simple_Eval_Fief_unbar_0");
        }

        /// <summary>
        ///     Scenario: 'helen' PC is outside of keep
        /// 
        ///     Expected result: 'helen' should enter the keep
        /// </summary>
        public static void Simple_Eval_Fief_enterKeep()
        {
            new ScriptAgent(username , password).LoadScenario("Default");

            new ScriptAgent(username , password).Simple_Eval_Fief_enterKeep_0_helen();
            runSimpleEvaluation("Simple_Eval_Fief_enterKeep_0");
        }

        /// <summary>
        ///     Scenario: 'helen' PC has an NPC ('Char_2183') in the current fief which is baillif of another of his fiefs
        /// 
        ///     Expected result: 'helen' PC should add the NPC to his entourage
        /// </summary>
        public static void Simple_Eval_Fief_addEntourage()
        {
            new ScriptAgent(username , password).LoadScenario("Default");

            new ScriptAgent(username , password).Simple_Eval_Fief_addEntourage_0_helen();
            runSimpleEvaluation("Simple_Eval_Fief_addEntourage_0");
        }

        /// <summary>
        ///     Scenario: 'helen' PC has an NPC ('Char_2183') in his entourage which is the baillif of the current fief
        /// 
        ///     Expected result: 'helen' PC should remove the NPC from his entourage
        /// </summary>
        public static void Simple_Eval_Fief_removeEntourage()
        {
            new ScriptAgent(username , password).LoadScenario("Default");

            new ScriptAgent(username , password).Simple_Eval_Fief_removeEntourage_0_helen();
            runSimpleEvaluation("Simple_Eval_Fief_removeEntourage_0");
        }

        /// <summary>
        ///     Scenario: 'helen' PC has an unused NPC in his entourage
        /// 
        ///     Expected result: 'helen' PC should appoint the NPC as baillif of one of his fiefs
        /// </summary>
        public static void Simple_Eval_Fief_appointBaillif()
        {
            new ScriptAgent(username , password).LoadScenario("Default");

            new ScriptAgent(username , password).Simple_Eval_Fief_appointBaillif_0_helen();
            runSimpleEvaluation("Simple_Eval_Fief_appointBaillif_0");
        }

        /// <summary>
        ///     Scenario: 'helen' PC has a baillif for each one of his fiefs and has an unused NPC that is better qualified to be baillif than one of the current ones
        /// 
        ///     Modification on server side:
        ///     - All NPCs from 'Char_4000' to 'Char_4044' now have as employer 'Char_158'
        /// 
        ///     Expected result: 'helen' PC should remove the bailiff of one of his fiefs
        /// </summary>
        public static void Simple_Eval_Fief_removeBaillif()
        {
            new ScriptAgent(username , password).LoadScenario("Simple_Eval_Fief_removeBaillif_0");

            new ScriptAgent(username , password).Simple_Eval_Fief_removeBaillif_0_helen();
            runSimpleEvaluation("Simple_Eval_Fief_removeBaillif_0");
        }

        /// <summary>
        ///     Scenario: 'helen' PCs home fief has a negative treasury
        /// 
        ///     Expected result: 'helen' PC should auto adjust the expenditures of his fief
        /// </summary>
        public static void Simple_Eval_Fief_autoAdjustExpenditure()
        {
            new ScriptAgent(username , password).LoadScenario("Simple_Eval_Fief_autoAdjustExpenditure_0");

            runSimpleEvaluation("Simple_Eval_Fief_autoAdjustExpenditure_0");
        }

        /// <summary>
        ///     Scenario: 'helen' PC has some treasury in a fief that isn't his homeFief
        /// 
        ///     Modification on server side:
        ///     - Treasury of fief 'EPM03' changed to 100
        /// 
        ///     Expected result: 'helen' PC should transfer the funds to his homefief
        /// </summary>
        public static void Simple_Eval_Fief_transferFunds()
        {
            new ScriptAgent(username , password).LoadScenario("Simple_Eval_Fief_transferFunds_0");

            runSimpleEvaluation("Simple_Eval_Fief_transferFunds_0");
        }

        /// <summary>
        ///     Scenario: 'helen' PC is on a fief with unknown NPCs and doesn't have enough NPCs
        /// 
        ///     Expected result: 'helen' PC should spy an NPC
        /// </summary>
        public static void Simple_Eval_Fief_spyCharacter()
        {
            new ScriptAgent(username , password).LoadScenario("Default");

            runSimpleEvaluation("Simple_Eval_Fief_spyCharacter_0");
        }





        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



        /// <summary>
        ///     Scenario: 'test' PC send a proposal to the daugter of 'helen' PC
        ///     
        ///     Modification on server side:
        ///     - 'Char_1189' is now the daugter of 'helen' PC
        ///     
        ///     Expected result: 'helen' should accept the proposal once it discoved enough potential brides
        /// </summary>
        public static void Complex_Eval_Family_acceptRejectProposal_0()
        {
            new ScriptAgent(username , password).LoadScenario("Complex_Eval_Family_acceptRejectProposal_0");

            new ScriptAgent(username, password).Complex_Eval_Family_acceptRejectProposal_0_test();
            //runEvaluation("Simple_Eval_Family_acceptRejectProposal_0");
            Program.RunAgent(username , password, AgentTypes.RuleBased, true);
        }


        /// <summary>
        ///     Scenario: 'helen' account without wife and with heir
        ///     
        ///     Modification on server side: 
        ///     - Char_158: No wife
        ///     
        ///     Expected result: Should propose or accept a proposal only once the required number of available brides is reached
        /// </summary>
        public static void Complex_Eval_Family_0()
        {
            new ScriptAgent(username , password).LoadScenario("Complex_Eval_Family_0");

            Program.RunAgent(username , password, AgentTypes.RuleBased);
        }

        /// <summary>
        ///     Scenario: 'helen' account without wife and without sons
        ///     
        ///     Modification on server side:
        ///     - Char_158: No wife
        ///     - Char_158: No NPCs
        ///     
        ///     Expected result: Should propose or accept a proposal as soon as possible
        /// </summary>
        public static void Complex_Eval_Family_1()
        {
            new ScriptAgent(username , password).LoadScenario("Complex_Eval_Family_1");

            Program.RunAgent(username , password, AgentTypes.RuleBased);
        }

        /// <summary>
        ///     Scenario: 'helen' account has no treasury
        ///     
        ///     Modification on server side:
        ///     - 'helen => 'Treasury of all of his fiefs changed to 0
        ///     
        ///     Expected result: The agent should lower the expenses and he shouldn't try to execute these actions: recruit troops, hire NPCs, transfer funds
        /// </summary>
        public static void Complex_Eval_Fief_1()
        {
            new ScriptAgent(username , password).LoadScenario("Complex_Eval_Fief_1");

            Program.RunAgent(username , password, AgentTypes.RuleBased);
        }


        ////////////////////////////// TESTS BELOW ARE THOSE THAT FIRST NEED THE SERVER SIDE TO BE FIXED //////////////////////////////////////////////////////


        /// <summary>
        ///     !!! PROBLEM ON SERVER SIDE: a player can't recruit if he has no army !!!
        ///     
        ///     Scenario: 'Char_158' has no armies 
        ///     
        ///     Modification on server side: 
        ///     - 'Char_158' has no armies
        ///     
        ///     Expected result: 'Char_158' should recruit troops
        /// </summary>
        public static void Eval_Army_6()
        {
            new ScriptAgent(username , password).LoadScenario("TO DO"); // TO DO

            Program.RunAgent(username , password, AgentTypes.RuleBased);
        }

        /// <summary>
        ///     !!! THE SERVER DOESN'T HANDLE CORRECTLY THE DEATH OF THE PC !!!
        /// 
        ///     Scenario: 'helen' account as an heir and the PlayerCharacter dies
        ///     
        ///     Expected result: Same as usually, but with the new heir being the PlayerCharacter.
        /// </summary>
        public static void Eval_Family_3()
        {
            new ScriptAgent(username , password).LoadScenario("Default");

            // => Manually appoint an heir and then try to kill the PC
            Program.RunAgent(username , password, AgentTypes.RuleBased);
        }

        /// <summary>
        ///     Run the evaluation on the rule-based agent with the 'helen' account and report the success status.
        /// </summary>
        /// <param name="evalID"></param>
        private static void runSimpleEvaluation(string evalID)
        {
            RuleBasedAgent ruleBasedHelen = new RuleBasedAgent(username , password);
            GameState gameState = new GameState();
            while (true)
            {
                ruleBasedHelen.waitingForServerTimes = new List<double>();
                Stopwatch timer = new Stopwatch();
                timer.Start();
                gameState = ruleBasedHelen.ObtainCurrentGSandUpdateAgentLists(gameState);
                gameState = ruleBasedHelen.playOneAction(gameState, out MainActions actionChosen, out string[] actionArguments, out bool executionSuccess, out bool endOfSeason);
                timer.Stop();
                executionTimes.Add(timer.Elapsed.TotalSeconds);
                waitingForServerAllTimes.Add(ruleBasedHelen.waitingForServerTimes);

                if (!executionSuccess)
                    throw new Exception("The evaluation encountered a problem");

                bool evalFinished = false;
                bool evalSuccess = false;
                if(endOfSeason){
                    evalFinished = true;
                    evalSuccess = false;
                }
                else
                    switch (evalID)
                    {
                        case "Simple_Eval_Army_Maintain_0":
                            if (actionChosen == MainActions.ArmyMaintain)
                            {
                                evalFinished = true;
                                evalSuccess = true;
                            }
                            if (actionChosen == MainActions.NPCmoveDirection || actionChosen == MainActions.NPCmoveToFief)
                            {
                                evalFinished = true;
                                evalSuccess = false;
                            }
                            break;
                        case "Simple_Eval_Army_BarCharacter_0":
                            if (actionChosen == MainActions.NPCbarCharacters)
                            {
                                evalFinished = true;
                                evalSuccess = true;
                            }
                            if (actionChosen == MainActions.NPCmoveDirection || actionChosen == MainActions.NPCmoveToFief)
                            {
                                evalFinished = true;
                                evalSuccess = false;
                            }
                            break;
                        case "Simple_Eval_Army_Attack_0":
                            if (actionChosen == MainActions.ArmyAttack)
                            {
                                evalFinished = true;
                                evalSuccess = true;
                            }
                            if (actionChosen == MainActions.NPCmoveDirection || actionChosen == MainActions.NPCmoveToFief)
                            {
                                evalFinished = true;
                                evalSuccess = false;
                            }
                            break;
                        case "Simple_Eval_Army_Attack_1":
                            if (actionChosen == MainActions.NPCmoveDirection || actionChosen == MainActions.NPCmoveToFief)
                            {
                                evalFinished = true;
                                evalSuccess = true;
                            }
                            if (actionChosen == MainActions.ArmyAttack)
                            {
                                evalFinished = true;
                                evalSuccess = false;
                            }
                            break;
                        case "Simple_Eval_Army_pillage_0":
                            if (actionChosen == MainActions.ArmyPillageCurrentFief)
                            {
                                evalFinished = true;
                                evalSuccess = true;
                            }
                            if (actionChosen == MainActions.NPCmoveDirection || actionChosen == MainActions.NPCmoveToFief)
                            {
                                evalFinished = true;
                                evalSuccess = false;
                            }
                            break;
                        case "Simple_Eval_Army_besiege_0":
                            if (actionChosen == MainActions.SiegeBesiegeCurrentFief)
                            {
                                evalFinished = true;
                                evalSuccess = true;
                            }
                            if (actionChosen == MainActions.NPCmoveDirection || actionChosen == MainActions.NPCmoveToFief)
                            {
                                evalFinished = true;
                                evalSuccess = false;
                            }
                            break;
                        case "Simple_Eval_Army_dropOff_disband_0":
                            if (actionChosen == MainActions.NPCmoveDirection || actionChosen == MainActions.NPCmoveToFief)
                            {
                                evalFinished = true;
                                evalSuccess = true;
                            }
                            if (actionChosen == MainActions.ArmyDisband || actionChosen == MainActions.ArmyDropOff)
                            {
                                evalFinished = true;
                                evalSuccess = false;
                            }
                            break;
                        case "Simple_Eval_Army_dropOff_0":
                            if (actionChosen == MainActions.ArmyDropOff)
                            {
                                evalFinished = true;
                                evalSuccess = true;
                            }
                            if (actionChosen == MainActions.NPCmoveDirection || actionChosen == MainActions.NPCmoveToFief)
                            {
                                evalFinished = true;
                                evalSuccess = false;
                            }
                            break;
                        case "Simple_Eval_Army_appointLeader_0":
                            if (actionChosen == MainActions.ArmyAppointLeader)
                            {
                                if (actionArguments[1].Equals("Char_425"))
                                {
                                    evalFinished = true;
                                    evalSuccess = true;
                                }
                                else
                                {
                                    evalFinished = true;
                                    evalSuccess = false;
                                }
                            }
                            if (actionChosen == MainActions.NPCmoveDirection || actionChosen == MainActions.NPCmoveToFief)
                            {
                                evalFinished = true;
                                evalSuccess = false;
                            }
                            break;
                        case "Simple_Eval_Army_appointLeader_1":
                            if (actionChosen == MainActions.ArmyAppointLeader)
                            {
                                if (actionArguments[0].Equals("Army_999"))
                                {
                                    evalFinished = true;
                                    evalSuccess = true;
                                }
                                else
                                {
                                    evalFinished = true;
                                    evalSuccess = false;
                                }
                            }
                            if (actionChosen == MainActions.NPCmoveDirection || actionChosen == MainActions.NPCmoveToFief)
                            {
                                evalFinished = true;
                                evalSuccess = false;
                            }
                            break;
                        case "Simple_Eval_Army_stormSiege_0":
                            if (actionChosen == MainActions.SiegeStorm)
                            {
                                evalFinished = true;
                                evalSuccess = true;
                            }
                            if (actionChosen == MainActions.NPCmoveDirection || actionChosen == MainActions.NPCmoveToFief || actionChosen == MainActions.SiegeReduction 
                                || actionChosen == MainActions.SiegeNegotiation || actionChosen == MainActions.SiegeEnd)
                            {
                                evalFinished = true;
                                evalSuccess = false;
                            }
                            break;
                        case "Simple_Eval_Army_negociateSiege_0":
                            if (actionChosen == MainActions.SiegeNegotiation)
                            {
                                evalFinished = true;
                                evalSuccess = true;
                            }
                            if (actionChosen == MainActions.NPCmoveDirection || actionChosen == MainActions.NPCmoveToFief || actionChosen == MainActions.SiegeReduction
                                || actionChosen == MainActions.SiegeStorm || actionChosen == MainActions.SiegeEnd)
                            {
                                evalFinished = true;
                                evalSuccess = false;
                            }
                            break;
                        case "Simple_Eval_Army_endSiege_0":
                            if (actionChosen == MainActions.SiegeEnd)
                            {
                                evalFinished = true;
                                evalSuccess = true;
                            }
                            if (actionChosen == MainActions.NPCmoveDirection || actionChosen == MainActions.NPCmoveToFief || actionChosen == MainActions.SiegeReduction
                                || actionChosen == MainActions.SiegeStorm || actionChosen == MainActions.SiegeNegotiation)
                            {
                                evalFinished = true;
                                evalSuccess = false;
                            }
                            break;
                        case "Simple_Eval_Army_recruit_0":
                            if (actionChosen == MainActions.ArmyRecruit)
                            {
                                evalFinished = true;
                                evalSuccess = true;
                            }
                            if (actionChosen == MainActions.NPCmoveDirection || actionChosen == MainActions.NPCmoveToFief)
                            {
                                evalFinished = true;
                                evalSuccess = false;
                            }
                            break;
                        case "Simple_Eval_Family_Marry_0":
                            if (actionChosen == MainActions.NPCmarryPC)
                            {
                                evalFinished = true;
                                evalSuccess = true;
                            }
                            if (actionChosen == MainActions.NPCmoveDirection || actionChosen == MainActions.NPCmoveToFief)
                            {
                                evalFinished = true;
                                evalSuccess = false;
                            }
                            break;
                        case "Simple_Eval_tryForChild_0":
                            if (actionChosen == MainActions.NPCtryForChild)
                            {
                                evalFinished = true;
                                evalSuccess = true;
                            }
                            if (actionChosen == MainActions.NPCmoveDirection || actionChosen == MainActions.NPCmoveToFief)
                            {
                                evalFinished = true;
                                evalSuccess = false;
                            }
                            break;
                        case "Simple_Eval_Family_AppointHeir_0":
                            if (actionChosen == MainActions.NPCappointHeir)
                            {
                                evalFinished = true;
                                evalSuccess = true;
                            }
                            if (actionChosen == MainActions.NPCmoveDirection || actionChosen == MainActions.NPCmoveToFief)
                            {
                                evalFinished = true;
                                evalSuccess = false;
                            }
                            break;
                        case "Simple_Eval_Fief_fire_0":
                            if (actionChosen == MainActions.NPCfire)
                            {
                                evalFinished = true;
                                evalSuccess = true;
                            }
                            if (actionChosen == MainActions.NPCmoveDirection || actionChosen == MainActions.NPCmoveToFief)
                            {
                                evalFinished = true;
                                evalSuccess = false;
                            }
                            break;
                        case "Simple_Eval_Fief_hire_0":
                            if (actionChosen == MainActions.NPChire)
                            {
                                evalFinished = true;
                                evalSuccess = true;
                            }
                            if (actionChosen == MainActions.NPCmoveDirection || actionChosen == MainActions.NPCmoveToFief)
                            {
                                evalFinished = true;
                                evalSuccess = false;
                            }
                            break;
                        case "Simple_Eval_Fief_move_0":
                            if (actionChosen == MainActions.NPCmoveDirection || actionChosen == MainActions.NPCmoveToFief)
                            {
                                if (actionArguments[0].Equals("EPM02"))
                                {
                                    evalFinished = true;
                                    evalSuccess = true;
                                }
                                else
                                {
                                    evalFinished = true;
                                    evalSuccess = false;
                                }
                            }
                            break;
                        case "Simple_Eval_Fief_move_1":
                            if (actionChosen == MainActions.NPCmoveDirection || actionChosen == MainActions.NPCmoveToFief)
                            {
                                if (actionArguments[0].Equals("EPM03"))
                                {
                                    evalFinished = true;
                                    evalSuccess = true;
                                }
                                else
                                {
                                    evalFinished = true;
                                    evalSuccess = false;
                                }
                            }
                            break;
                        case "Simple_Eval_Fief_move_2":
                            if (actionChosen == MainActions.NPCmoveDirection || actionChosen == MainActions.NPCmoveToFief)
                            {
                                if (actionArguments[0].Equals("EPM03"))
                                {
                                    evalFinished = true;
                                    evalSuccess = true;
                                }
                                else
                                {
                                    evalFinished = true;
                                    evalSuccess = false;
                                }
                            }
                            break;
                        case "Simple_Eval_Fief_move_3":
                            if (actionChosen == MainActions.NPCmoveDirection)
                            {
                                evalFinished = true;
                                evalSuccess = true;
                            }
                            if (actionChosen == MainActions.NPCmoveToFief)
                            {
                                evalFinished = true;
                                evalSuccess = false;
                            }
                            break;
                        case "Simple_Eval_Fief_unbar_0":
                            if (actionChosen == MainActions.NPCunbarCharacters)
                            {
                                evalFinished = true;
                                evalSuccess = true;
                            }
                            if (actionChosen == MainActions.NPCmoveDirection || actionChosen == MainActions.NPCmoveToFief)
                            {
                                evalFinished = true;
                                evalSuccess = false;
                            }
                            break;
                        case "Simple_Eval_Fief_enterKeep_0":
                            if (actionChosen == MainActions.NPCtryEnterKeep)
                            {
                                evalFinished = true;
                                evalSuccess = true;
                            }
                            if (actionChosen == MainActions.NPCmoveDirection || actionChosen == MainActions.NPCmoveToFief)
                            {
                                evalFinished = true;
                                evalSuccess = false;
                            }
                            break;
                        case "Simple_Eval_Fief_addEntourage_0":
                            if (actionChosen == MainActions.NPCaddToEntourage)
                            {
                                if (actionArguments[0].Equals("Char_2183"))
                                {
                                    evalFinished = true;
                                    evalSuccess = true;
                                }
                                else
                                {
                                    evalFinished = true;
                                    evalSuccess = false;
                                }
                            }
                            if (actionChosen == MainActions.NPCmoveDirection || actionChosen == MainActions.NPCmoveToFief)
                            {
                                evalFinished = true;
                                evalSuccess = false;
                            }
                            break;
                        case "Simple_Eval_Fief_removeEntourage_0":
                            if (actionChosen == MainActions.NPCremoveFromEntourage)
                            {
                                if (actionArguments[0].Equals("Char_2183"))
                                {
                                    evalFinished = true;
                                    evalSuccess = true;
                                }
                                else
                                {
                                    evalFinished = true;
                                    evalSuccess = false;
                                }
                            }
                            if (actionChosen == MainActions.NPCmoveDirection || actionChosen == MainActions.NPCmoveToFief)
                            {
                                evalFinished = true;
                                evalSuccess = false;
                            }
                            break;
                        case "Simple_Eval_Fief_appointBaillif_0":
                            if (actionChosen == MainActions.NPCappointBaillif)
                            {
                                evalFinished = true;
                                evalSuccess = true;
                            }
                            if (actionChosen == MainActions.NPCmoveDirection || actionChosen == MainActions.NPCmoveToFief)
                            {
                                evalFinished = true;
                                evalSuccess = false;
                            }
                            break;
                        case "Simple_Eval_Fief_removeBaillif_0":
                            if (actionChosen == MainActions.NPCremoveBaillif)
                            {
                                evalFinished = true;
                                evalSuccess = true;
                            }
                            if (actionChosen == MainActions.NPCmoveDirection || actionChosen == MainActions.NPCmoveToFief)
                            {
                                evalFinished = true;
                                evalSuccess = false;
                            }
                            break;
                        case "Simple_Eval_Fief_autoAdjustExpenditure_0":
                            if (actionChosen == MainActions.FiefAutoAdjustExpenditure)
                            {
                                evalFinished = true;
                                evalSuccess = true;
                            }
                            if (actionChosen == MainActions.NPCmoveDirection || actionChosen == MainActions.NPCmoveToFief)
                            {
                                evalFinished = true;
                                evalSuccess = false;
                            }
                            break;
                        case "Simple_Eval_Fief_transferFunds_0":
                            if (actionChosen == MainActions.FiefTransferFunds)
                            {
                                if (actionArguments[0].Equals("EPM03") && actionArguments[1].Equals("EPM02") && actionArguments[2].Equals("100"))
                                {
                                    evalFinished = true;
                                    evalSuccess = true;
                                }
                                else
                                {
                                    evalFinished = true;
                                    evalSuccess = false;
                                }
                            }
                            if (actionChosen == MainActions.NPCmoveDirection || actionChosen == MainActions.NPCmoveToFief)
                            {
                                evalFinished = true;
                                evalSuccess = false;
                            }
                            break;
                        case "Simple_Eval_Fief_spyCharacter_0":
                            if (actionChosen == MainActions.SpyCharacter)
                            {
                                evalFinished = true;
                                evalSuccess = true;
                            }
                            if (actionChosen == MainActions.NPCmoveDirection || actionChosen == MainActions.NPCmoveToFief)
                            {
                                evalFinished = true;
                                evalSuccess = false;
                            }
                            break;
                        default:
                            throw new Exception("evaluation ID '" + evalID + "' not recognized");
                    }

                if (evalFinished)
                {
                    executedEvaluationIDs.Add(evalID);
                    executedEvaluationResults.Add(evalSuccess);
                    Console.WriteLine("=========================================> Eval finished: " + evalID + " => " + evalSuccess);
                    break;
                }
            }
        }
    }
}