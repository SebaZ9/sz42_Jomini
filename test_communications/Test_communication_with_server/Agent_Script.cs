using ProtoMessageClient;
using System;

namespace JominiAI
{
    /// <summary>
	/// This agent's purpose is to create scripts to rapidly test things in the game.
    /// The functions begining by 'tomato' are supposed to be performed with the 'test' account and the other ones are supposed to be performed with the 'helen' account
	/// </summary>
    public class ScriptAgent : Agent 
    {
        public ScriptAgent(string username, string password) : base(username, password)
        {

        }

        /// <summary>
        ///     
        /// </summary>
        public void errorSeasonUpdate()
        {

            Console.WriteLine("SeasonUpdate().ResponseType= " + SeasonUpdate().ResponseType);

            //GameState gameState = getGSandSumarize();
        }

        /// <summary></summary>
        public void Test_0()
        {
            Console.WriteLine("Test 0");

            ProtoFief protoFief;
            ProtoMessage protoMessage;

            protoFief = (ProtoFief)((ProtoFief)viewCurrentFief());
            //Console.WriteLine("ProtoFiefDetails.ToString()= " + ProtoFiefDetails.ToString());
            //Console.WriteLine("ProtoFiefDetails.Message= " + ProtoFiefDetails.Message);
            //Console.WriteLine("ProtoFiefDetails.MessageFields= " + ProtoFiefDetails.MessageFields);
            Console.WriteLine("fief_ID= " + protoFief.fiefID);
            Console.WriteLine("fief_population= " + protoFief.population);
            Console.WriteLine("fief_ancestral_owner= " + protoFief.ancestralOwner.charName);

            protoMessage = TryForChild();

            protoMessage = RecruitTroops(((ProtoPlayerCharacter)viewCurrentPC()).armyID, 15);

            protoMessage = DisbandArmy("Army_1");

            protoFief = (ProtoFief)Move(MoveDirections.E);
            protoFief = (ProtoFief)((ProtoFief)viewCurrentFief());
            Console.WriteLine("\nfief_ID= " + protoFief.fiefID);

            protoFief = (ProtoFief)Move(MoveDirections.NW);
            protoFief = (ProtoFief)((ProtoFief)viewCurrentFief());
            Console.WriteLine("\nfief_ID= " + protoFief.fiefID);

            protoMessage = SpyFief("ESW05", currentPCid);

            protoMessage = PillageFief(((ProtoPlayerCharacter)viewCurrentPC()).armyID);

            protoMessage = BesiegeCurrentFief(((ProtoPlayerCharacter)viewCurrentPC()).armyID);

            protoMessage = EndSiege("Siege_1");

            protoFief = (ProtoFief)Move(MoveDirections.NE);
            Console.WriteLine("\nfief_ID= " + protoFief.fiefID);

            protoMessage = Kidnap("Char_196", currentPCid);
        }

        /// <summary></summary>
        public void Test_1()
        {
            Console.WriteLine(" Test 1");

            Console.WriteLine("ArmyStatus().fields[0].armySize= " + ((ProtoGenericArray<ProtoArmyOverview>)ListArmies()).fields[0].armySize);
            Console.WriteLine("ArmyStatus().fields[1].armySize= " + ((ProtoGenericArray<ProtoArmyOverview>)ListArmies()).fields[1].armySize);
            Console.WriteLine("c.purse= " + ((ProtoPlayerCharacter)viewCurrentPC()).purse);

            Console.WriteLine("HireTroops(1000)" + RecruitTroops(((ProtoPlayerCharacter)viewCurrentPC()).armyID, 1000).ResponseType);

            Console.WriteLine("ArmyStatus().fields[0].armySize= " + ((ProtoGenericArray<ProtoArmyOverview>)ListArmies()).fields[0].armySize);
            Console.WriteLine("ArmyStatus().fields[1].armySize= " + ((ProtoGenericArray<ProtoArmyOverview>)ListArmies()).fields[1].armySize);
            Console.WriteLine("c.purse= " + ((ProtoPlayerCharacter)viewCurrentPC()).purse);
        }

        /// <summary></summary>
        public void Test_2()
        {
            Console.WriteLine("Test 2");

            GameState gameState = ObtainCurrentGameState();
            RedProtoPlayerCharacter currentCharacter = gameState.myPC;


            RedProtoFief currentFief = gameState.getCurrentFief();


            String armyID = ((ProtoGenericArray<ProtoArmyOverview>)ListArmies()).fields[1].armyID;
            uint[] trps = ((ProtoArmy)ViewArmy(armyID)).troops;
            foreach (uint trp in trps)
                Console.WriteLine("trp: " + trp);

            foreach (string currentFiefArmyId in currentFief.armyIDs)
                Console.WriteLine("protoArmyOverview.armyID: " + currentFiefArmyId);

            uint[] intArray = new uint[7];
            intArray[0] = 1;
            intArray[1] = 0;
            intArray[2] = 0;
            intArray[3] = 0;
            intArray[4] = 0;
            intArray[5] = 0;
            intArray[6] = 1;
            Console.WriteLine("DropOffTroops(intArray).ResponseType= " + DropOffTroops(((ProtoPlayerCharacter)viewCurrentPC()).armyID, intArray, currentCharacter.charID).ResponseType);

            trps = ((ProtoArmy)ViewArmy("Army_4")).troops;
            foreach (uint trp in trps)
                Console.WriteLine("trp: " + trp);
            foreach (string armyInCurrentFiefId in currentFief.armyIDs)
                Console.WriteLine("protoArmyOverview.armyID: " + armyInCurrentFiefId);
            foreach (ProtoDetachment protoDetachment in ((ProtoGenericArray<ProtoDetachment>)ListDetachments("Army_4")).fields)
                foreach (uint trp in protoDetachment.troops)
                    Console.WriteLine("trp in detachment " + protoDetachment.id + ": " + trp);

            trps = ((ProtoArmy)ViewArmy("Army_4")).troops;
            foreach (uint trp in trps)
                Console.WriteLine("trp: " + trp);

            String[] strArray = new String[1];
            strArray[0] = ((ProtoGenericArray<ProtoDetachment>)ListDetachments("Army_4")).fields[0].id;
            Console.WriteLine("PickUpTroops.ResponseType= " + PickUpTroops("Army_4", strArray).ResponseType);

            trps = ((ProtoArmy)ViewArmy("Army_4")).troops;
            foreach (uint trp in trps)
                Console.WriteLine("trp: " + trp);

            foreach (ProtoArmyOverview armyInFief in ((ProtoFief)viewCurrentFief()).armies)
                Console.WriteLine("armyInFief.armyID: " + armyInFief.armyID);
        }

        /// <summary></summary>
        public void Test_3()
        {
            Console.WriteLine("Test 3");

            Console.WriteLine("Move(MoveDirections.E).ResponseType= " + Move(MoveDirections.E).ResponseType);

            Console.WriteLine("DisbandArmy.ResponseType= " + DisbandArmy("Army_1").ResponseType);
        }

        /// <summary></summary>
        public void Test_4()
        {
            Console.WriteLine("\nTest 4");

            //Console.WriteLine("Move(MoveDirections.E).ResponseType= " + Move(MoveDirections.E).ResponseType);

            Console.WriteLine("ViewArmy(Army_1).location= " + ((ProtoArmy)ViewArmy("Army_4")).location);

            foreach (ProtoCharacterOverview protoCharacterOverview in ((ProtoFief)viewCurrentFief()).charactersInFief)
                Console.WriteLine("FiefDetails().charactersInFief.charID: " + protoCharacterOverview.charID);

            Console.WriteLine("currentGameState.myProfile.location= " + ((ProtoFief)viewCurrentFief()).fiefID);
            foreach (ProtoArmyOverview protoArmyOverview in ((ProtoFief)viewCurrentFief()).armies)
                Console.WriteLine("armies in current fief: " + protoArmyOverview.armyID);

            foreach (ProtoArmyOverview protoArmyOverview in ((ProtoGenericArray<ProtoArmyOverview>)ListArmies()).fields)
                Console.WriteLine("protoArmyOverview.armyID: " + protoArmyOverview.armyID + " and leaderID= " + protoArmyOverview.leaderID);

            Console.WriteLine("AppointLeader.ResponseType= " + AppointLeader("Army_4", "Char_390").ResponseType);

            foreach (ProtoArmyOverview protoArmyOverview in ((ProtoGenericArray<ProtoArmyOverview>)ListArmies()).fields)
                Console.WriteLine("protoArmyOverview.armyID: " + protoArmyOverview.armyID + " and leaderID= " + protoArmyOverview.leaderID);

            Console.WriteLine("AppointLeader.ResponseType= " + AppointLeader("Army_4", "Char_5580").ResponseType); // Doesn't work

            foreach (ProtoArmyOverview protoArmyOverview in ((ProtoGenericArray<ProtoArmyOverview>)ListArmies()).fields)
                Console.WriteLine("protoArmyOverview.armyID: " + protoArmyOverview.armyID + " and leaderID= " + protoArmyOverview.leaderID);
        }

        /// <summary></summary>
        public void Test_5()
        {
            Console.WriteLine("\nTest 5");

            Console.WriteLine("FiefDetails().siege= " + ((ProtoFief)viewCurrentFief()).siege);

            Console.WriteLine("currentGameState.myProfile.location= " + ((ProtoFief)viewCurrentFief()).fiefID);
            Console.WriteLine("Move(MoveDirections.E).ResponseType= " + Move(MoveDirections.E).ResponseType);
            Console.WriteLine("currentGameState.myProfile.location= " + ((ProtoFief)viewCurrentFief()).fiefID);
            Console.WriteLine("Move(MoveDirections.E).ResponseType= " + Move(MoveDirections.E).ResponseType);
            Console.WriteLine("currentGameState.myProfile.location= " + ((ProtoFief)viewCurrentFief()).fiefID);
            Console.WriteLine("Move(MoveDirections.E).ResponseType= " + Move(MoveDirections.E).ResponseType);
            Console.WriteLine("currentGameState.myProfile.location= " + ((ProtoFief)viewCurrentFief()).fiefID);

            Console.WriteLine("PillageFief().ResponseType: " + PillageFief(((ProtoPlayerCharacter)viewCurrentPC()).armyID).ResponseType);
            Console.WriteLine("PillageFief().ResponseType: " + PillageFief(((ProtoPlayerCharacter)viewCurrentPC()).armyID).ResponseType);
        }

        /// <summary>
        ///     Disband all armies and tries to recruit from fief => Makes the server crash
        /// </summary>
        public void Test_6()
        {
            Console.WriteLine("\nTest 6");

            Console.WriteLine("Profile().days= " + ((ProtoPlayerCharacter)viewCurrentPC()).days);

            Console.WriteLine("DisbandArmy(Army_1).ResponseType: " + DisbandArmy("Army_1").ResponseType);
            Console.WriteLine("Profile().days= " + ((ProtoPlayerCharacter)viewCurrentPC()).days);

            Console.WriteLine("DisbandArmy(Army_4).ResponseType: " + DisbandArmy("Army_4").ResponseType);
            Console.WriteLine("Profile().days= " + ((ProtoPlayerCharacter)viewCurrentPC()).days);
            Console.WriteLine("HireTroops(15).ResponseType: " + RecruitTroops(((ProtoPlayerCharacter)viewCurrentPC()).armyID, 15).ResponseType); // Makes server crash
        }

        /// <summary></summary>
        public void Test_7()
        {
            Console.WriteLine("\nTest 7");
            GameState currentGameState = ObtainCurrentGameState();

            Console.WriteLine("currentGameState.myProfile.location= " + ((ProtoFief)viewCurrentFief()).fiefID);
            Console.WriteLine("Move(MoveDirections.E).ResponseType= " + Move(MoveDirections.E).ResponseType);
            Console.WriteLine("currentGameState.myProfile.location= " + ((ProtoFief)viewCurrentFief()).fiefID);
            Console.WriteLine("Move(MoveDirections.E).ResponseType= " + Move(MoveDirections.E).ResponseType);
            Console.WriteLine("currentGameState.myProfile.location= " + ((ProtoFief)viewCurrentFief()).fiefID);
            Console.WriteLine("Move(MoveDirections.E).ResponseType= " + Move(MoveDirections.E).ResponseType);
            Console.WriteLine("currentGameState.myProfile.location= " + ((ProtoFief)viewCurrentFief()).fiefID);

            Console.WriteLine("SiegeCurrentFief().ResponseType: " + BesiegeCurrentFief(((ProtoPlayerCharacter)viewCurrentPC()).armyID).ResponseType);

            //Console.WriteLine("SiegeRoundStorm(((ProtoGenericArray<ProtoSiegeOverview>)SiegeList()).fields[0].siegeID).ResponseType: " + SiegeRoundStorm(((ProtoGenericArray<ProtoSiegeOverview>)SiegeList()).fields[0].siegeID).ResponseType);

            //Console.WriteLine("SiegeRoundReduction(((ProtoGenericArray<ProtoSiegeOverview>)SiegeList()).fields[0].siegeID).ResponseType: " + SiegeRoundReduction(((ProtoGenericArray<ProtoSiegeOverview>)SiegeList()).fields[0].siegeID).ResponseType);

            //Console.WriteLine("SiegeRoundNegotiate(((ProtoGenericArray<ProtoSiegeOverview>)SiegeList()).fields[0].siegeID).ResponseType: " + SiegeRoundNegotiate(((ProtoGenericArray<ProtoSiegeOverview>)SiegeList()).fields[0].siegeID).ResponseType);

            //Console.WriteLine("EndSiege(((ProtoGenericArray<ProtoSiegeOverview>)SiegeList()).fields[0].siegeID).ResponseType: " + EndSiege(((ProtoGenericArray<ProtoSiegeOverview>)SiegeList()).fields[0].siegeID).ResponseType);

            GameState gameState = getGSandSumarize();
        }

        /// <summary></summary>
        public void Test_8()
        {
            Console.WriteLine("\nTest 8");

            Console.WriteLine("FiefDetails().treasury= " + ((ProtoFief)viewCurrentFief()).treasury);

            Console.WriteLine("ViewArmy(Army_4).maintCost= " + ((ProtoArmy)ViewArmy("Army_4")).maintCost);

            Console.WriteLine("MaintainArmy(Army_4).ResponseType= " + MaintainArmy("Army_4").ResponseType);  // Not enough funds

            Console.WriteLine("FiefDetails().treasury= " + ((ProtoFief)viewCurrentFief()).treasury);
        }

        /// <summary></summary>
        public void Test_9()
        {
            Console.WriteLine("\nTest 9");

            Console.WriteLine("FiefDetails().fiefID= " + ((ProtoFief)viewCurrentFief()).fiefID);

            Console.WriteLine("FiefDetails().treasury= " + ((ProtoFief)viewCurrentFief()).treasury);

            Console.WriteLine("TransferFunds(EPM02, EPM02, 100).ResponseType= " + TransferFunds("EPM02", "EPM02", 100).ResponseType); // Transfer to same fief

            Console.WriteLine("FiefDetails().treasury= " + ((ProtoFief)viewCurrentFief()).treasury);

            Console.WriteLine("TransferFunds(null, EPM03, 1000).ResponseType= " + TransferFunds(null, "EPM03", 1000).ResponseType);

            Console.WriteLine("FiefDetails().treasury= " + ((ProtoFief)viewCurrentFief()).treasury);

            Console.WriteLine("TransferFunds(null, ELA03, 1000).ResponseType= " + TransferFunds(null, "ELA03", 1000).ResponseType);

            Console.WriteLine("FiefDetails().treasury= " + ((ProtoFief)viewCurrentFief()).treasury);

            Console.WriteLine("TransferFunds(ELA03, null, 1000).ResponseType= " + TransferFunds("ELA03", null, 1000).ResponseType); // From a fief not owned by the player, and it works!!!

            Console.WriteLine("FiefDetails().treasury= " + ((ProtoFief)viewCurrentFief()).treasury);
        }

        /// <summary></summary>
        public void Test_10()
        {
            Console.WriteLine("\nTest 10");

            Console.WriteLine("GrantFiefTitle(EPM01, Char_102).ResponseType= " + GrantFiefTitle("EPM01", "Char_102").ResponseType);

            Console.WriteLine("GrantFiefTitle(EPM02, Char_102).ResponseType= " + GrantFiefTitle("EPM02", "Char_102").ResponseType);

            Console.WriteLine("GrantFiefTitle(EPM03, Char_102).ResponseType= " + GrantFiefTitle("EPM03", "Char_102").ResponseType);

            Console.WriteLine("GrantFiefTitle(EGL05, Char_102).ResponseType= " + GrantFiefTitle("EGL05", "Char_102").ResponseType);

            Console.WriteLine("FiefDetails().ancestralOwner= " + ((ProtoFief)viewCurrentFief()).ancestralOwner.charID);
        }

        /// <summary></summary>
        public void Test_11()
        {
            Console.WriteLine("\nTest 11");

            //printMyNPCs();

            Console.WriteLine("fireNPC(currentPCid).ResponseType= " + fireNPC(currentPCid).ResponseType);
            Console.WriteLine("fireNPC(currentPCid).ResponseType= " + fireNPC("Char_356").ResponseType);
        }

        /// <summary></summary>
        public void Test_13()
        {
            Console.WriteLine("\nTest 13");

            DisbandArmy("Army_4");

            while (1 != 0)
            {
                Console.WriteLine("\nMove(MoveDirections.E).ResponseType= " + Move(MoveDirections.E).ResponseType);
                Console.WriteLine("currentGameState.myProfile.location= " + ((ProtoFief)viewCurrentFief()).fiefID);

                var charsInFief = ((ProtoFief)viewCurrentFief()).charactersInFief;
                if (charsInFief != null)
                    foreach (var charInFief in charsInFief)
                        Console.WriteLine("charInFief.charID: " + charInFief.charID);

                Console.WriteLine("EnterExitKeep(currentPCid).Message= " + EnterExitKeep(currentPCid).Message);

                charsInFief = ((ProtoFief)viewCurrentFief()).charactersInFief;
                if (charsInFief != null)
                    foreach (var charInFief in charsInFief)
                        Console.WriteLine("charInFief.charID: " + charInFief.charID);

                Console.WriteLine("EnterExitKeep(currentPCid).Message= " + EnterExitKeep(currentPCid).Message);

                WaitForKey();
            }
        }

        /// <summary></summary>
        public void Test_14()
        {
            Console.WriteLine("\nTest 14");

            Console.WriteLine("Camp(currentPCid, 10).ResponseType= " + Camp(currentPCid, 200).ResponseType);
        }

        /// <summary></summary>
        public void Test_15()
        {
            Console.WriteLine("\nTest 15");

            var addRemoveEntourage = AddRemoveEntourage("Char_356");
            if (addRemoveEntourage != null)
            {
                Console.WriteLine("AddRemoveEntourage(Char_356).ResponseType= " + addRemoveEntourage.ResponseType);
                PrintMyNPCs();
            }
            addRemoveEntourage = AddRemoveEntourage("Char_390");
            if (addRemoveEntourage != null)
            {
                Console.WriteLine("AddRemoveEntourage(Char_390).ResponseType= " + addRemoveEntourage.ResponseType);
                PrintMyNPCs();
            }
            addRemoveEntourage = AddRemoveEntourage("Char_356");
            if (addRemoveEntourage != null)
            {
                Console.WriteLine("AddRemoveEntourage(Char_390).ResponseType= " + addRemoveEntourage.ResponseType);
                PrintMyNPCs();
            }
        }

        /// <summary></summary>
        public void Test_16()
        {
            Console.WriteLine("\nTest 16");

            var barredChars = ((ProtoFief)viewCurrentFief()).barredCharacters;
            if (barredChars != null)
                foreach (var barredChar in barredChars)
                    Console.WriteLine("barredChar: " + barredChar.charID);

            Console.WriteLine("FiefDetails().fiefID, new string[] { Char_356, Char_695 }).ResponseType= " + BarCharacters(((ProtoFief)viewCurrentFief()).fiefID, new string[] { "Char_356", "Char_695" }).ResponseType);
            
            barredChars = ((ProtoFief)viewCurrentFief()).barredCharacters;
            if (barredChars != null)
                foreach (var barredChar in barredChars)
                    Console.WriteLine("barredChar: " + barredChar.charID);

            Console.WriteLine("FiefDetails().fiefID, new string[] { Char_356, Char_695 }).ResponseType= " + UnbarCharacters(((ProtoFief)viewCurrentFief()).fiefID, new string[] { "Char_356", "Char_695" }).ResponseType);

            barredChars = ((ProtoFief)viewCurrentFief()).barredCharacters;
            if (barredChars != null)
                foreach (var barredChar in barredChars)
                    Console.WriteLine("barredChar: " + barredChar.charID);
        }

        /// <summary>
        ///     If char is without wife, he can propose to Char_333
        /// </summary>
        public void Test_17()
        {
            Console.WriteLine("\nTest 17");

            Console.WriteLine("Marry(Char_333).ResponseType= " + Marry("Char_158", "Char_333").ResponseType);

            ProtoGenericArray<ProtoJournalEntry> unreadJournalEntries = ViewJournalEntries("unread");
            Console.WriteLine("unreadJournalEntries.ResponseType= " + unreadJournalEntries.ResponseType);
            foreach (ProtoJournalEntry unreadJournalEntry in unreadJournalEntries.fields)
            {
                Console.Write("\nunreadJournalEntry.type: " + unreadJournalEntry.type + " and unreadJournalEntry.personae IDs: ");
                foreach (ProtoCharacterOverview personae in unreadJournalEntry.personae)
                    Console.Write(" " + personae.charID);
            }
        }

        /// <summary>
        ///     To check I can maintain my army on a fief I don't own and that it is always the treasury of my home fief that will pay for it
        /// </summary>
        public void Test_18()
        {
            Console.WriteLine("\nTest 18");


            GameState gameState = ObtainCurrentGameState();
            RedProtoPlayerCharacter currentCharacter = gameState.myPC;
            RedProtoFief homefief = gameState.getHomeFief();
            RedProtoFief currentFief = gameState.getCurrentFief();


            MoveToESR03();

            Console.WriteLine("c.purse= " + ((ProtoPlayerCharacter)viewCurrentPC()).purse);
            Console.WriteLine("gameState.homeFief.treasury= " + homefief.treasury);
            Console.WriteLine("gameState.currentFief.treasury= " + currentFief.treasury);

            Console.WriteLine("ViewArmy(Army_4).maintCost= " + ((ProtoArmy)ViewArmy("Army_4")).maintCost);
            Console.WriteLine("MaintainArmy(Army_4).ResponseType= " + MaintainArmy("Army_4").ResponseType);

            gameState = ObtainCurrentGameState();
            Console.WriteLine("c.purse= " + ((ProtoPlayerCharacter)viewCurrentPC()).purse);
            Console.WriteLine("gameState.homeFief.treasury= " + homefief.treasury);
            Console.WriteLine("gameState.currentFief.treasury= " + currentFief.treasury);
        }

        /// <summary>
        /// MoveToFief and check day cost
        /// </summary>
        public void Test_19()
        {
            Console.WriteLine("\nTest 19");

            Console.WriteLine("MoveToFief(ESR03).ResponseType= " + MoveToFief("ESR03").ResponseType);

            Console.WriteLine("currentGameState.myProfile.location= " + ((ProtoFief)viewCurrentFief()).fiefID);

            Console.WriteLine("Profile().days= " + ((ProtoPlayerCharacter)viewCurrentPC()).days);
        }

        /// <summary>
        /// Deep copy functions
        /// </summary>
        public void Test_20()
        {
            Console.WriteLine("\nTest 20");

            ProtoArmyOverview protoArmyOverview  = new ProtoArmyOverview();
            /*protoArmyOverview.leaderID = "leaderID";
            protoArmyOverview.leaderName = "leaderName";*/
            DeepCopier.DeepCopyProtoArmyOverview(protoArmyOverview);

            ProtoFief protoFief = new ProtoFief();
            /*protoFief.barredNationalities = new string[] { "barredNationalities" };*/
            DeepCopier.DeepCopyProtoFief(protoFief);

            GameState gameState = ObtainCurrentGameState();
            gameState.deepCopy();

        }

        /// <summary></summary>
        public void Test_21()
        {
            Console.WriteLine("\nTest 21");

            MoveToFief("ESR03");
            Console.WriteLine("currentGameState.myProfile.location= " + ((ProtoFief)viewCurrentFief()).fiefID);

            ViewCharsInFief();

            Console.WriteLine("SpyCharacter(Char_281, currentPCid).ResponseType= " + SpyCharacter("Char_281", currentPCid).ResponseType);
            Console.WriteLine("SpyCharacter(Char_40, currentPCid).ResponseType= " + SpyCharacter("Char_40", currentPCid).ResponseType);
        }

        /// <summary>
        /// Spy fief + Spy character + Spy Army
        /// </summary>
        public void Test_22()
        {
            Console.WriteLine("\nTest 22");

            MoveToFief("ESR03");
            Console.WriteLine("currentGameState.myProfile.location= " + ((ProtoFief)viewCurrentFief()).fiefID);

            ProtoFief fiefDetails = ((ProtoFief)viewCurrentFief());

            ProtoFief spiedFief = (ProtoFief)SpyFief("ESR03", currentPCid);
            Console.WriteLine("spiedFief.ResponseType= " + spiedFief.ResponseType);

            ProtoCharacter spiedChar = (ProtoCharacter)SpyCharacter("Char_281", currentPCid);
            Console.WriteLine("spiedChar.ResponseType= " + spiedChar.ResponseType);

            /*ProtoArmy spiedArmy = (ProtoArmy)SpyArmy("", currentPCid);
            Console.WriteLine("spiedArmy.ResponseType= " + spiedArmy.ResponseType);*/

            ViewCharsInFief();

            /*Console.WriteLine("DisbandArmy(Army_4).ResponseType= " + DisbandArmy("Army_4").ResponseType);

            Console.WriteLine("EnterExitKeep(currentPCid).ResponseType= " + EnterExitKeep(currentPCid).ResponseType);

            viewCharsInFief();*/
        }

        /// <summary></summary>
        public void Test_23()
        {
            Console.WriteLine("\nTest 23");

            //MoveToFief("ESR03");

            MoveToFief("EPM02");

            Console.WriteLine("FiefDetails().owner= " + ((ProtoFief)viewCurrentFief()).owner);

            Console.WriteLine("FiefDetails().ownerID= " + ((ProtoFief)viewCurrentFief()).ownerID);

            Console.WriteLine("Profile().playerID= " + ((ProtoPlayerCharacter)viewCurrentPC()).playerID);
        }

        /// <summary></summary>
        public void Test_24()
        {
            Console.WriteLine("\nTest 24");

            //Console.WriteLine("ViewCharacter(Profile().spouse).location= " + ViewCharacter(Profile().spouse).location);

        }

        /// <summary>
        /// Hire and Fire NPC
        /// </summary>
        public void Test_25()
        {
            Console.WriteLine("\nTest 25");

            MoveToFief("ESR03");

            ViewCharsInFief();

            Console.WriteLine("hireNPC(Char_3856, 10000).ResponseType" + hireNPC("Char_3856", 10000).ResponseType);
            Console.WriteLine("Tools.GetLeadershipValue(new RedProtoCharacter(ViewCharacter('Char_3856')))= " + Tools.GetLeadershipValue(new RedProtoCharacter((ProtoCharacter)ViewCharacter("Char_3856"))));

            PrintMyNPCs();

            Console.WriteLine("fireNPC(Char_3856).ResponseType" + fireNPC("Char_3856").ResponseType);

            PrintMyNPCs();
        }

        /// <summary>
        ///     getTravelDayCost()
        /// </summary>
        public void Test_26()
        {
            Console.WriteLine("\nTest 26");

            Console.WriteLine("getTravelDayCost('ESR03')= " + getTravelDayCost("ESR03"));

        }

        /// <summary>
        ///     getTravelDayCost()
        /// </summary>
        public void Test_27()
        {
            Console.WriteLine("\nTest 27");

            string[] directions = GetAvailableTravelDirections().MessageFields;
            if(directions != null)
                foreach (string direction in directions)
                    Console.WriteLine("direction: " + direction);
        }

        /// <summary></summary>
        public void Test_28()
        {
            Console.WriteLine("\nTest 28");

            MoveToESR03();
            Console.WriteLine("PillageFief(((ProtoPlayerCharacter)viewCurrentPC()).armyID).ResponseType= " + PillageFief(((ProtoPlayerCharacter)viewCurrentPC()).armyID).ResponseType);

            ProtoJournalEntry[] UnreadJournalEntriesArray = ViewJournalEntries("unread").fields;
            ProtoJournalEntry tmp;
            if (UnreadJournalEntriesArray != null)
                foreach (ProtoJournalEntry protoJournalEntry in UnreadJournalEntriesArray)
                    tmp = protoJournalEntry;
        }

        /// <summary>
        ///     Recruit troops and then move to fief 'EPM03' (owner='helen') and besiege it
        /// </summary>
        public void Complex_Eval_Army_0_test()
        {
            GameState gameState = ObtainCurrentGameState();

            if (!ExecuteAction(MainActions.ArmyRecruit, new string[] { gameState.myPC.armyID, gameState.getCurrentFief().militia.ToString() }, gameState, out gameState))
                throw new Exception("Failed to execute the Action");
            if (!ExecuteAction(MainActions.NPCmoveToFief, new string[] { "EPM03" }, gameState, out gameState))
                throw new Exception("Failed to execute the Action");
            if (!ExecuteAction(MainActions.SiegeBesiegeCurrentFief, new string[] { gameState.myPC.armyID }, gameState, out gameState))
                throw new Exception("Failed to execute the Action");
        }

        /// <summary>
        ///     besiege one of Char_158's fief (EPM01)
        /// </summary>
        public void Simple_Eval_Fief_move_1_test()
        {
            GameState gameState = ObtainCurrentGameState();

            if (!ExecuteAction(MainActions.SiegeBesiegeCurrentFief, new string[] { gameState.myPC.armyID }, gameState, out gameState))
                throw new Exception("Failed to execute the Action");
        }

        /// <summary>
        ///     pillage one of Char_158's fief (EPM02)
        /// </summary>
        public void Simple_Eval_Army_BarCharacter_0_test()
        {
            GameState gameState = ObtainCurrentGameState();
            if (!ExecuteAction(MainActions.NPCmoveToFief, new string[] { "EPM02" }, gameState, out gameState))
                throw new Exception("Failed to execute the Action");
            if (!ExecuteAction(MainActions.ArmyPillageCurrentFief, new string[] { ((ProtoPlayerCharacter)viewCurrentPC()).armyID }, gameState, out gameState))
                throw new Exception("Failed to execute the Action");
        }

        /// <summary></summary>
        public void Simple_Eval_Army_Attack_0_test()
        {
            Simple_Eval_Army_BarCharacter_0_test();
        }

        /// <summary></summary>
        public void Simple_Eval_Army_Attack_1_test()
        {
            Simple_Eval_Army_BarCharacter_0_test();
        }

        /// <summary></summary>
        public void Simple_Eval_Army_pillage_0_test()
        {
            Simple_Eval_Army_BarCharacter_0_test();
        }

        /// <summary></summary>
        public void Simple_Eval_Army_besiege_0_test()
        {
            Simple_Eval_Army_BarCharacter_0_test();
        }

        /// <summary>Go to fief 'ENU02' owned by 'test' and besiege it</summary>
        public void Simple_Eval_Army_stormSiege_0_helen()
        {
            GameState gameState = ObtainCurrentGameState();

            if (!ExecuteAction(MainActions.SiegeBesiegeCurrentFief, new string[] { gameState.myArmies[0].armyID }, gameState, out gameState))
                throw new Exception("Failed to execute the Action");
        }

        /// <summary></summary>
        public void Simple_Eval_Army_negociateSiege_0_helen()
        {
            Simple_Eval_Army_stormSiege_0_helen();
        }

        /// <summary></summary>
        public void Simple_Eval_Army_endSiege_0_helen()
        {
            Simple_Eval_Army_stormSiege_0_helen();
        }

        /// <summary>
        ///     bar his spouse from homefief
        /// </summary>
        public void Simple_Eval_Fief_unbar_0_helen()
        {
            GameState gameState = ObtainCurrentGameState();

            if (!ExecuteAction(MainActions.NPCbarCharacters, new string[] { "EPM02", gameState.myPC.spouse }, gameState, out gameState))
                throw new Exception("Failed to execute the Action");
        }

        /// <summary>
        ///     exit the keep
        /// </summary>
        public void Simple_Eval_Fief_enterKeep_0_helen()
        {
            GameState gameState = ObtainCurrentGameState();

            if (!ExecuteAction(MainActions.NPCexitKeep, new string[] { gameState.myPC.charID }, gameState, out gameState))
                throw new Exception("Failed to execute the Action");
        }

        /// <summary>
        ///      Move to fief 'ESW07' and hire the NPC '2183' and appoint him baillif of fief 'EPM02'
        /// </summary>
        public void Simple_Eval_Fief_addEntourage_0_helen()
        {
            GameState gameState = ObtainCurrentGameState();

            if (!ExecuteAction(MainActions.NPCmoveToFief, new string[] { "ESW07" }, gameState, out gameState))
                throw new Exception("Failed to execute the Action");
            if (!ExecuteAction(MainActions.NPChire, new string[] { "Char_2183", 50000.ToString() }, gameState, out gameState))
                throw new Exception("Failed to execute the Action");
            if (!ExecuteAction(MainActions.NPCappointBaillif, new string[] { "EPM02", "Char_2183" }, gameState, out gameState))
                throw new Exception("Failed to execute the Action");
        }

        /// <summary>
        ///     ""
        ///     Add to entourage the NPC 'Char_2183'
        ///     Move to fief 'EPM02'
        /// </summary>
        public void Simple_Eval_Fief_removeEntourage_0_helen()
        {
            Simple_Eval_Fief_addEntourage_0_helen();

            GameState gameState = ObtainCurrentGameState();

            if (!ExecuteAction(MainActions.NPCaddToEntourage, new string[] { "Char_2183" }, gameState, out gameState))
                throw new Exception("Failed to execute the Action");
            if (!ExecuteAction(MainActions.NPCmoveToFief, new string[] { "EPM02" }, gameState, out gameState))
                throw new Exception("Failed to execute the Action");
        }

        /// <summary>
        ///      Move to fief 'ESW07' and hire the NPC '2183'
        /// </summary>
        public void Simple_Eval_Fief_appointBaillif_0_helen()
        {
            GameState gameState = ObtainCurrentGameState();

            if (!ExecuteAction(MainActions.NPCmoveToFief, new string[] { "ESW07" }, gameState, out gameState))
                throw new Exception("Failed to execute the Action");
            if (!ExecuteAction(MainActions.NPChire, new string[] { "Char_2183", 50000.ToString() }, gameState, out gameState))
                throw new Exception("Failed to execute the Action");
        }

        /// <summary>
        ///      Appoints NPCs 'Char_4000', 'Char_4016' and 'Char_4023' as baillifs
        /// </summary>
        public void Simple_Eval_Fief_removeBaillif_0_helen()
        {
            GameState gameState = ObtainCurrentGameState();

            if (!ExecuteAction(MainActions.NPCappointBaillif, new string[] { "EPM02" , "Char_4000" }, gameState, out gameState))
                throw new Exception("Failed to execute the Action");
            if (!ExecuteAction(MainActions.NPCappointBaillif, new string[] { "EPM03", "Char_4016" }, gameState, out gameState))
                throw new Exception("Failed to execute the Action");
            if (!ExecuteAction(MainActions.NPCappointBaillif, new string[] { "EGL05", "Char_4023" }, gameState, out gameState))
                throw new Exception("Failed to execute the Action");
        }

        /// <summary>
        ///     If I have a daughter ready for marriage, send a proposal to "Char_158"
        /// </summary>
        public void Complex_Eval_Family_acceptRejectProposal_0_test()
        {
            GameState gameState = ObtainCurrentGameState();

            foreach (RedProtoNPC myFamilyNPC in gameState.myFamilyNPCs)
                if (!myFamilyNPC.isMale && string.IsNullOrWhiteSpace(myFamilyNPC.spouse) && string.IsNullOrWhiteSpace(myFamilyNPC.fiancee) && string.IsNullOrWhiteSpace(myFamilyNPC.captor)
                            && gameState.currentYear - myFamilyNPC.birthYear >= 14)
                {
                    if (!ExecuteAction(MainActions.NPCmarryChild, new string[] { myFamilyNPC.charID, "Char_158" }, gameState, out gameState))
                        throw new Exception("Failed to execute the Action");
                    return;
                }
            throw new Exception("Coulnd't find a suitable daugter to marry");
        }

        /// <summary></summary>
        public GameState getGSandSumarize()
        {
            GameState gameState = ObtainCurrentGameState();
            RedProtoPlayerCharacter currentPC = gameState.myPC;
            RedProtoFief currenFief = gameState.getCurrentFief();
            RedProtoFief homeFief = gameState.getHomeFief();

            Console.WriteLine();
            Console.WriteLine("currentPCid= " + currentPCid);
            Console.WriteLine("currentPC.days= " + currentPC.days);
            Console.WriteLine("currenFief.fiefID= " + currenFief.fiefID);
            Console.WriteLine("currenFief.ownerID= " + currenFief.ownerID);
            Console.WriteLine("homeFief.treasury= " + homeFief.treasury);
            Console.WriteLine("Total Nb troops= " + calculMyTotalNbTroops(gameState));
            uint nbTroops = gameState.tryGetMainArmy(out RedProtoArmy mainArmy) ? Tools.CalculSumUintArray(mainArmy.troops) : 0;
            Console.WriteLine("Nb troops in mainArmy= " + nbTroops);
            Console.WriteLine("calculMyTotalNbTroops(gameState)= " + calculMyTotalNbTroops(gameState));
            Console.WriteLine("gameState.myArmies.Count= " + gameState.myArmies.Count);
            Console.WriteLine("");
            return gameState;
        }

        /// <summary></summary>
        private void ViewCharsInFief()
        {
            var charsInFief = ((ProtoFief)viewCurrentFief()).charactersInFief;
            if (charsInFief != null)
                foreach (var charInFief in charsInFief)
                    Console.WriteLine("charInFief.charID: " + charInFief.charID + " with role= " + charInFief.role + " and owner= " + charInFief.owner);
        }

        /// <summary>Propose to all the characters in the game until one meets the right criterias</summary>
        public void Test_Marry_All()
        {
            int index = 0;
            while(true)
            {
                index++;
                DisplayMessages respType =  Marry("Char_158", "Char_" + index).ResponseType;
                Console.WriteLine("Marry(Char_" + index + ").ResponseType: " + respType);
                if(respType != DisplayMessages.ErrorGenericCharacterUnidentified && respType != DisplayMessages.CharacterProposalMan 
                    && respType != DisplayMessages.CharacterProposalMarried && respType != DisplayMessages.CharacterProposalUnderage)
                    WaitForKey();
            }
        }

        /// <summary>
        ///     There is normally some PCs (Char_281) in this fief (but can't find them) => I modified the CSV so the Char_281 is outside of the keep
        /// </summary>
        public void MoveToESR03()
        {
            for(int i=0; i<9; i++)
            {
                Console.WriteLine("\nMove(MoveDirections.E).ResponseType= " + Move(MoveDirections.E).ResponseType);
                Console.WriteLine("currentGameState.myProfile.location= " + ((ProtoFief)viewCurrentFief()).fiefID);
            }
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine("\nMove(MoveDirections.E).ResponseType= " + Move(MoveDirections.SE).ResponseType);
                Console.WriteLine("currentGameState.myProfile.location= " + ((ProtoFief)viewCurrentFief()).fiefID);
            }

            Console.WriteLine("Profile().days= " + ((ProtoPlayerCharacter)viewCurrentPC()).days);
        }

        /// <summary>There is an NPC outside of keep (Char_413) in this fief</summary>
        private void MoveToELN01()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("\nMove(MoveDirections.E).ResponseType= " + Move(MoveDirections.E).ResponseType);
                Console.WriteLine("currentGameState.myProfile.location= " + ((ProtoFief)viewCurrentFief()).fiefID);
            }
                Console.WriteLine("\nMove(MoveDirections.E).ResponseType= " + Move(MoveDirections.SE).ResponseType);
                Console.WriteLine("currentGameState.myProfile.location= " + ((ProtoFief)viewCurrentFief()).fiefID);
        }

        /// <summary></summary>
        public void PrintMyNPCs()
        {
            ProtoCharacterOverview[] npcArray = ((ProtoGenericArray<ProtoCharacterOverview>)GetNPCList("Entourage")).fields;
            if (npcArray != null)
                foreach (ProtoCharacterOverview npcInFief in npcArray)
                    Console.WriteLine("Entourage => charID: " + npcInFief.charID);
            npcArray = ((ProtoGenericArray<ProtoCharacterOverview>)GetNPCList("Family")).fields;
            if (npcArray != null)
                foreach (ProtoCharacterOverview npcInFief in npcArray)
                    Console.WriteLine("Family => charID: " + npcInFief.charID);
            npcArray = ((ProtoGenericArray<ProtoCharacterOverview>)GetNPCList("Employ")).fields;
            if (npcArray != null)
                foreach (ProtoCharacterOverview npcInFief in npcArray)
                    Console.WriteLine("Employ => charID: " + npcInFief.charID);
        }

        /// <summary>To create a break</summary>
        private void WaitForKey()
        {
            Console.WriteLine("\nPress a key to continue");
            Console.ReadKey();
            Console.WriteLine("");
        }

        public override MainActions findNextAction(GameState gameState, out string[] arguments)
        {
            throw new NotImplementedException();
        }
    }
}