using ProtoMessageClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JominiAI
{
    /// <summary>
	/// This agent takes random actions. It is usefull to rapidly test Player class functions to check everything is working.
	/// </summary>
    public class RandomAgent : Agent
    {
        public RandomAgent(string username, string password) : base(username, password)
        {
            
        }

        /// <summary>
		///     Randomly pick an action in the list of possible actions as well as random parameters.
		/// </summary>
        /// <returns>The best action found</returns>
        /// <returns>gameStateScore: score of the best action found</returns>
        public override MainActions findNextAction(GameState currentGS, out string[] arguments)
        {
            RedProtoPlayerCharacter currentPC = currentGS.myPC;
            RedProtoFief currentFief = currentGS.getCurrentFief();
            currentGS.tryGetMainArmy(out RedProtoArmy mainArmy);
            RedProtoFief homeFief = currentGS.getHomeFief();

            Random rnd = new Random(DateTime.Now.Millisecond);
            List<MainActions> possibleActions = getPossibleActions(currentGS);
            while(true)
                if (possibleActions.Count > 0)
                {
                    MainActions randAction = possibleActions[rnd.Next(possibleActions.Count)];
                    switch (randAction)
                    {
                        case MainActions.NPCmoveDirection:
                            arguments = new string[] { dayCostToTravelToAdjacentFiefs.Keys.ToArray()[rnd.Next(dayCostToTravelToAdjacentFiefs.Count)].ToString() };
                            return MainActions.NPCmoveDirection;
                        case MainActions.ArmyRecruit:
                            int nbTroopsToRecruit = rnd.Next(Math.Min(currentFief.militia, homeFief.treasury/2000));
                            string randArmyID = currentGS.myArmies[rnd.Next(currentGS.myArmies.Count())].armyID;
                            arguments = new string[] { randArmyID, nbTroopsToRecruit.ToString() };
                            return MainActions.ArmyRecruit;
                        case MainActions.ArmyMaintain:
                            List<RedProtoArmy> myArmiesCopy = new List<RedProtoArmy>(currentGS.myArmies);
                            RedProtoArmy armyToMaintain = null;
                            while (true)
                            {
                                if (myArmiesCopy.Count == 0)
                                    throw new Exception("Couldn't find an army to maintain, it shoudln't happen");
                                armyToMaintain = myArmiesCopy[rnd.Next(myArmiesCopy.Count)];
                                if (!armyToMaintain.isMaintained && armyToMaintain.maintCost <= homeFief.treasury)
                                    break;
                                else
                                    myArmiesCopy.Remove(armyToMaintain);
                            }
                            arguments = new string[] { armyToMaintain.armyID };
                            return MainActions.ArmyMaintain;
                        case MainActions.ArmyDropOff:
                            RedProtoArmy randArmy = null;
                            List<RedProtoArmy> copyMyArmies = new List<RedProtoArmy>(currentGS.myArmies);
                            while (true)
                            {
                                if (copyMyArmies.Count() == 0)
                                    throw new Exception("Couldn't find a suitable army to drop off troops from, it shouldn't happen");
                                randArmy = copyMyArmies[rnd.Next(copyMyArmies.Count())];
                                if (!string.IsNullOrWhiteSpace(randArmy.leaderName))
                                    break;
                                else
                                    copyMyArmies.Remove(randArmy);
                            }
                            string[] troopsToDrop = new string[7];
                            int index = 0;
                            foreach (uint nbTroop in randArmy.troops)
                            {
                                troopsToDrop[index] = rnd.Next((int)nbTroop + 1).ToString();
                                index++;
                            }
                            String leftForCharID;
                            if (currentGS.allyPCids.Count > 0 && rnd.Next(100) > 50)
                            {
                                leftForCharID = currentGS.allyPCids.ToArray()[rnd.Next(currentGS.allyPCids.Count)];
                            }
                            else
                                leftForCharID = currentPC.charID;
                            arguments = (new string[] { randArmy.armyID }).Concat(troopsToDrop).Concat(new string[] {leftForCharID }).ToArray();
                            return MainActions.ArmyDropOff;
                        case MainActions.ArmyPickUp:
                            /*List<ProtoDetachment> possibleDetachmentsToPick = new List<ProtoDetachment>();
                            foreach (ProtoDetachment protoDetachment in currentGameState.detachmentInCurrentFiefList)
                                if (protoDetachment.leftFor.Equals(currentPC.charID))
                                    possibleDetachmentsToPick.Add(protoDetachment);
                            ProtoDetachment detachmentToPick = possibleDetachmentsToPick[rnd.Next(possibleDetachmentsToPick.Count)];
                            String[] strArray = new String[1];
                            strArray[0] = detachmentToPick.id;
                            responseType = PickUpTroops(currentArmy.armyID, strArray).ResponseType;
                            if (responseType == DisplayMessages.Success)
                                Console.WriteLine("Picked up troops left by " + detachmentToPick.leftBy + " in army " + currentArmy.armyID + ", nb troops: " + detachmentToPick.troops[0] + ", " + detachmentToPick.troops[1] + ", " + detachmentToPick.troops[2] + ", " + detachmentToPick.troops[3] + ", " + detachmentToPick.troops[4] + ", " + detachmentToPick.troops[5] + ", " + detachmentToPick.troops[6]);
                            else
                                throw new Exception("Failed to PickUpTroops, ResponseType= " + responseType);*/
                            break;
                        // !!! TMP: TO UNCOMMENT !!!!
                        case MainActions.ArmyDisband:
                        RedProtoArmy armyToDisband = currentGS.myArmies[rnd.Next(currentGS.myArmies.Count)];
                        arguments = new string[] { armyToDisband.armyID };
                        return MainActions.ArmyDisband;
                        case MainActions.ArmyPillageCurrentFief:
                            RedProtoArmy randArmyBBis = null;
                            foreach(RedProtoArmy myArmy in currentGS.myArmies)
                                if(currentGS.tryGetArmyFiefID(myArmy.armyID, out string armyFiefID))
                                    if(armyFiefID.Equals(currentFief.fiefID))
                                    {
                                        randArmyBBis = myArmy;
                                        break;
                                    }
                            if (randArmyBBis == null)
                                throw new Exception("Couldn't find a suitable army to pillage the current fief, it shouldn't happen");
                            arguments = new string[] { randArmyBBis.armyID };
                            return MainActions.ArmyPillageCurrentFief;
                        case MainActions.ArmyAdjustCombatValues:
                            String randArmyIDbis = currentGS.myArmies[rnd.Next(currentGS.myArmies.Count)].armyID;
                            byte randAggroLevel = Convert.ToByte(rnd.Next(2 + 1));
                            byte randOddsValue = Convert.ToByte(rnd.Next(9 + 1));
                            arguments = new string[] { randArmyIDbis, randAggroLevel.ToString(), randOddsValue.ToString() };
                            return MainActions.ArmyAdjustCombatValues;
                        case MainActions.ArmyAttack:
                            RedProtoArmy target = null;
                            List<string> armiesInFiefIdList = currentFief.armyIDs.ToList();
                            while (true)
                            {
                                if (armiesInFiefIdList.Count <= 0)
                                    throw new Exception("There's no suitable army to attack in current fief");
                                string randArmyIDbbis = armiesInFiefIdList[rnd.Next(armiesInFiefIdList.Count)];
                                if(currentGS.tryGetArmy(randArmyIDbbis, out RedProtoArmy randArmyBis))
                                    if (!randArmyBis.ownerName.Equals(currentPC.firstName + " " + currentPC.familyName))
                                    {
                                        target = randArmyBis;
                                        break;
                                    }
                                armiesInFiefIdList.Remove(randArmyIDbbis);
                            }
                            arguments = new string[] { mainArmy.armyID, target.armyID };
                            return MainActions.ArmyAttack;
                        case MainActions.SiegeBesiegeCurrentFief:
                            RedProtoArmy armyToSiege = null;
                            foreach (RedProtoArmy myArmy in currentGS.myArmies)
                                if (currentGS.tryGetArmyFiefID(myArmy.armyID, out string armyFiefID))
                                    if (armyFiefID.Equals(currentFief.fiefID))
                                    {
                                        armyToSiege = myArmy;
                                        break;
                                    }
                            if (armyToSiege == null)
                                throw new Exception("Couldn't find a suitable army to besiege the current fief, it shouldn't happen");
                            arguments = new string[] { armyToSiege.armyID };
                            return MainActions.SiegeBesiegeCurrentFief;
                        case MainActions.SiegeStorm:
                            String siegeIDToStorm = currentGS.mySieges[rnd.Next(currentGS.mySieges.Count)].siegeID;
                            arguments = new string[] { siegeIDToStorm };
                            return MainActions.SiegeStorm;
                        case MainActions.SiegeReduction:
                            String siegeIDToReduce = currentGS.mySieges[rnd.Next(currentGS.mySieges.Count)].siegeID;
                            arguments = new string[] { siegeIDToReduce };
                            return MainActions.SiegeReduction;
                        case MainActions.SiegeNegotiation:
                            String siegeIDToNegociate = currentGS.mySieges[rnd.Next(currentGS.mySieges.Count)].siegeID;
                            arguments = new string[] { siegeIDToNegociate };
                            return MainActions.SiegeNegotiation;
                        case MainActions.SiegeEnd:
                            String siegeIDToEnd = currentGS.mySieges[rnd.Next(currentGS.mySieges.Count)].siegeID;
                            arguments = new string[] { siegeIDToEnd };
                            return MainActions.SiegeEnd;
                        case MainActions.FiefAutoAdjustExpenditure:
                            String randFiefID = currentGS.myFiefs[rnd.Next(currentGS.myFiefs.Count)].fiefID;
                            arguments = new string[] { randFiefID };
                            return MainActions.FiefAutoAdjustExpenditure;
                        case MainActions.FiefTransferFundsToPlayer:
                            String randPlayerID = currentGS.ExistingPlayers[rnd.Next(currentGS.ExistingPlayers.Count)].pcID;
                            int randAmount = rnd.Next(homeFief.treasury + 1);
                            arguments = new string[] { randPlayerID, randAmount.ToString() };
                            return MainActions.FiefTransferFundsToPlayer;
                        case MainActions.FiefTransferFunds:
                            bool searchingFief = true;
                            RedProtoFief fiefFrom = null;
                            RedProtoFief fiefTo;
                            List<RedProtoFief> knownFiefs = new List<RedProtoFief>(currentGS.notMyFiefs.Concat(currentGS.myFiefs));
                            while (true)
                            {
                                if (knownFiefs.Count() == 0)
                                    throw new Exception("Coudln't find a suitable fief to transfer funds to, it shouldn't happen");
                                fiefTo = knownFiefs[rnd.Next(knownFiefs.Count())];
                                if (!fiefTo.fiefID.Equals(homeFief.fiefID))
                                    break;
                                knownFiefs.Remove(fiefTo);
                            }

                            List<RedProtoFief> myFiefListCopy = new List<RedProtoFief>(currentGS.myFiefs);
                            while (searchingFief)
                            {
                                if (myFiefListCopy.Count == 0)
                                    throw new Exception("The player doesn't own a fief suitable to do a transfer");
                                RedProtoFief randFief = myFiefListCopy[rnd.Next(myFiefListCopy.Count)];
                                if (randFief.treasury > 0)
                                {
                                    fiefFrom = randFief;
                                    searchingFief = false;
                                }
                                else
                                    myFiefListCopy.Remove(randFief);
                            }
                            int randomAmount = rnd.Next(fiefFrom.treasury + 1);
                            arguments = new string[] { fiefFrom.fiefID, fiefTo.fiefID, randomAmount.ToString() };
                            return MainActions.FiefTransferFunds;
                        case MainActions.FiefGrantFiefTitle:
                            bool fiefToGrantFound = false;
                            ProtoFief fiefToGrant = null;
                            List<ProtoFief> myFiefListCopyBis = new List<ProtoFief>(currentGS.myFiefs.Count);
                            while (!fiefToGrantFound)
                            {
                                if (myFiefListCopyBis.Count == 0)
                                    throw new Exception("The character doesn't own a suitable fief to grant");
                                ProtoFief randFief = myFiefListCopyBis[rnd.Next(myFiefListCopyBis.Count)];
                                if (randFief.ancestralOwner == null)
                                {
                                    fiefToGrant = randFief;
                                    fiefToGrantFound = true;
                                }
                                else
                                    myFiefListCopyBis.Remove(randFief);
                            }
                            String charIDToGrant = currentGS.notMyFiefs[rnd.Next(currentGS.notMyFiefs.Count)].ownerID;
                            arguments = new string[] { fiefToGrant.fiefID, charIDToGrant };
                            return MainActions.FiefGrantFiefTitle;
                        case MainActions.NPCfire:
                            RedProtoNPC NPCtoFire = currentGS.myEmployeeNPCs[rnd.Next(currentGS.myEmployeeNPCs.Count)];
                            arguments = new string[] { NPCtoFire.charID };
                            return MainActions.NPCfire;
                        case MainActions.NPCexitKeep:
                            arguments = new string[] { currentPC.charID };
                            return MainActions.NPCexitKeep;
                        case MainActions.NPCtryEnterKeep:
                            arguments = new string[] { currentPC.charID };
                            return MainActions.NPCtryEnterKeep;
                        case MainActions.NPCcamp:
                            byte nbDaysToCamp = (byte)rnd.Next((int)Math.Floor(currentPC.days) + 1);
                            arguments = new string[] { currentPC.charID, nbDaysToCamp.ToString() };
                            return MainActions.NPCcamp;
                        case MainActions.NPCremoveFromEntourage:
                            RedProtoNPC npcToRemove = null;
                            List<RedProtoNPC> myNpcListCopy = new List<RedProtoNPC>(currentGS.myEmployeeNPCs.Concat(currentGS.myFamilyNPCs));
                            while (true)
                            {
                                if (myNpcListCopy.Count <= 0)
                                    throw new Exception("Couldn't find a suitable NPC to be removed from the entourage");
                                npcToRemove = myNpcListCopy[rnd.Next(myNpcListCopy.Count)];
                                myNpcListCopy.Remove(npcToRemove);
                                if (npcToRemove.inEntourage)
                                    break;
                            }
                            arguments = new string[] { npcToRemove.charID };
                            return MainActions.NPCremoveFromEntourage;
                        case MainActions.NPCaddToEntourage:
                            RedProtoNPC charToAdd = null;
                            List<RedProtoNPC> myNpcListCopyBis = new List<RedProtoNPC>(currentGS.myEmployeeNPCs.Concat(currentGS.myFamilyNPCs));
                            while (true)
                            {
                                if (myNpcListCopyBis.Count <= 0)
                                    throw new Exception("Couldn't find a suitable NPC to be added to the entourage");
                                charToAdd = myNpcListCopyBis[rnd.Next(myNpcListCopyBis.Count)];
                                myNpcListCopyBis.Remove(charToAdd);
                                if (charToAdd.location.Equals(currentFief.fiefID) && !charToAdd.inEntourage)
                                    break;
                            }
                            arguments = new string[] { charToAdd.charID };
                            return MainActions.NPCaddToEntourage;
                        case MainActions.NPCunbarCharacters:
                            List<RedProtoFief> fiefWithBarredCharsList = new List<RedProtoFief>();
                            foreach (RedProtoFief myFief in currentGS.myFiefs)
                                if (myFief.barredCharactersId != null && myFief.barredCharactersId.Length > 0)
                                    fiefWithBarredCharsList.Add(myFief);
                            RedProtoFief fiefToUnbarFrom = fiefWithBarredCharsList[rnd.Next(fiefWithBarredCharsList.Count)];
                            List<string> barredCharIdListCopy = new List<string>(fiefToUnbarFrom.barredCharactersId);
                            String[] charIDToUnbarArray = new String[rnd.Next(barredCharIdListCopy.Count)];
                            for (int i = 0; i < charIDToUnbarArray.Length; i++)
                            {
                                string charToUnbarId = barredCharIdListCopy[rnd.Next(barredCharIdListCopy.Count)];
                                barredCharIdListCopy.Remove(charToUnbarId);
                                charIDToUnbarArray[i] = charToUnbarId;
                            }
                            arguments = new string[] { fiefToUnbarFrom.fiefID }.Concat(charIDToUnbarArray).ToArray();
                            return MainActions.NPCunbarCharacters;
                        case MainActions.NPCbarNationalities:
                            List<RedProtoFief> fiefThatCanBarNationalitiesList = new List<RedProtoFief>();
                            foreach (RedProtoFief myFief in currentGS.myFiefs)
                            {
                                bool hasBaredAllNationalities = true;
                                foreach (String nationality in nationalities)
                                    if (!myFief.barredNationalities.Contains(nationality))
                                        hasBaredAllNationalities = false;
                                if (!possibleActions.Contains(MainActions.NPCbarNationalities) && !hasBaredAllNationalities)
                                    fiefThatCanBarNationalitiesList.Add(myFief);
                            }
                            RedProtoFief fiefToBarFrom = fiefThatCanBarNationalitiesList[rnd.Next(fiefThatCanBarNationalitiesList.Count)];
                            List<String> possibleNationalitiesToBar = new List<String>();
                            foreach (String nationality in nationalities)
                                if (!fiefToBarFrom.barredNationalities.Contains(nationality))
                                    possibleNationalitiesToBar.Add(nationality);
                            String[] nationalityIDToBarArray = new String[rnd.Next(possibleNationalitiesToBar.Count)];
                            for (int i = 0; i < nationalityIDToBarArray.Length; i++)
                            {
                                String nationalityToUnbar = possibleNationalitiesToBar[rnd.Next(possibleNationalitiesToBar.Count)];
                                possibleNationalitiesToBar.Remove(nationalityToUnbar);
                                nationalityIDToBarArray[i] = nationalityToUnbar;
                            }
                            arguments = new string[] { fiefToBarFrom.fiefID }.Concat(nationalityIDToBarArray).ToArray();
                            return MainActions.NPCbarNationalities;
                        case MainActions.NPCunbarNationalities:
                            List<RedProtoFief> fiefThatCanUnbarNationalitiesList = new List<RedProtoFief>();
                            foreach (RedProtoFief myFief in currentGS.myFiefs)
                                if (myFief.barredNationalities != null && myFief.barredNationalities.Length > 0)
                                    fiefThatCanUnbarNationalitiesList.Add(myFief);
                            RedProtoFief fiefToUnbarFromBis = fiefThatCanUnbarNationalitiesList[rnd.Next(fiefThatCanUnbarNationalitiesList.Count)];
                            List<String> possibleNationalitiesToUnbar = new List<String>();
                            foreach (String nationality in nationalities)
                                if (fiefToUnbarFromBis.barredNationalities.Contains(nationality))
                                    possibleNationalitiesToUnbar.Add(nationality);
                            String[] nationalityIDToUnbarArray = new String[rnd.Next(possibleNationalitiesToUnbar.Count)];
                            for (int i = 0; i < nationalityIDToUnbarArray.Length; i++)
                            {
                                String nationalityToUnbar = possibleNationalitiesToUnbar[rnd.Next(possibleNationalitiesToUnbar.Count)];
                                possibleNationalitiesToUnbar.Remove(nationalityToUnbar);
                                nationalityIDToUnbarArray[i] = nationalityToUnbar;
                            }
                            arguments = new string[] { fiefToUnbarFromBis.fiefID }.Concat(nationalityIDToUnbarArray).ToArray();
                            return MainActions.NPCunbarNationalities;
                    }
                    possibleActions.Remove(randAction);
                }
                else
                {
                    arguments = null;
                    return MainActions.None;
                }
        }
    }
}