using System;
using System.Collections.Generic;
using System.Linq;

namespace JominiAI
{
    /// <summary>
	/// This AI agent is based on rules.
    /// It works with ideal ratios the agent try to reach and with limits.
    /// To keep it lisible I try to keep one level of priority for each action.
	/// </summary>
    public class RuleBasedAgent : Agent
    {
        bool serverSideFixed = true;

        private double troopValue = 500;

        private double targetRatioArmyTreasury; // (nbTroops*troopValue)/treasury
        private double minRatioArmyTreasury; // (nbTroops*troopValue)/treasury
        private double maxRatioArmyTreasury; // (nbTroops*troopValue)/treasury
        private double minRatioMyTroopsEnemyTroopsToAttackInAllyFief; // nbTroopsInMyArmy/nbTroopsInEnnemyArmy
        private double minRatioMyTroopsEnemyTroopsToAttackInNotAllyFief; // nbTroopsInMyArmy/nbTroopsInEnnemyArmy
        private double maxRatioNPCsalariesTreasuryToHireNPC; // bid/treasury
        private double minRatioNPCsalariesTreasuryToFireNPC; // bid/treasury
        private double minBesiegeVictoryOddsToStart;
        private double maxBesiegeVictoryOddsToEnd;
        private double minBesiegeVictoryOddsToStorm;
        private double minBesiegeEnnemyVictoryOddsToComeDefend;
        private double targetMargin;
        private int nbNPCmoveInArowToSearchNewEnemy;
        private int nbPotentialBridesToFindBeforeMarryingPC;
        private int nbPotentialBridesToFindBeforeMarryingMySon;

        public RuleBasedAgent(string username, string password) : base(username, password)
        {
            targetRatioArmyTreasury = 0.5;
             minRatioArmyTreasury = targetRatioArmyTreasury / 2;
             maxRatioArmyTreasury = targetRatioArmyTreasury * 2;
             minRatioMyTroopsEnemyTroopsToAttackInAllyFief = 0.75;
             minRatioMyTroopsEnemyTroopsToAttackInNotAllyFief = 1.25;
             maxRatioNPCsalariesTreasuryToHireNPC = 0.20;
             minRatioNPCsalariesTreasuryToFireNPC = 0.4;
             minBesiegeVictoryOddsToStart = 1.25;
             maxBesiegeVictoryOddsToEnd = 0.75;
             minBesiegeVictoryOddsToStorm = 1.5;
             minBesiegeEnnemyVictoryOddsToComeDefend = 0.5;
             targetMargin = 1.2;
             nbNPCmoveInArowToSearchNewEnemy = 5;
             nbPotentialBridesToFindBeforeMarryingPC = 5;
             nbPotentialBridesToFindBeforeMarryingMySon = nbPotentialBridesToFindBeforeMarryingPC * 2;
    }

        public override MainActions findNextAction(GameState currentGS, out string[] arguments)
        {
            RedProtoPlayerCharacter currentPC = currentGS.myPC;
            RedProtoFief currentFief = currentGS.getCurrentFief();
            RedProtoFief homeFief = currentGS.getHomeFief();
            string mainArmyFiefID = null;
            if(currentGS.tryGetMainArmy(out RedProtoArmy mainArmy))
                if (!string.IsNullOrWhiteSpace(mainArmy.armyID))
                    currentGS.tryGetArmyFiefID(mainArmy.armyID, out mainArmyFiefID);

            List<MainActions> possibleActions = getPossibleActions(currentGS);
            Random rnd = new Random(DateTime.Now.Millisecond);

            int targetNbEmployees = currentGS.myFiefs.Count + 1; // 1 baillif for each fief + 1 to kidnap
            if(mainArmy != null)
                if (!string.IsNullOrWhiteSpace(mainArmy.leaderName))
                    if (!mainArmy.leaderName.Equals(currentPC.firstName + " " + currentPC.familyName))
                        targetNbEmployees++;

            uint myTotalTroops = 0;
            foreach (RedProtoArmy myArmy in currentGS.myArmies)
                myTotalTroops += Tools.CalculSumUintArray(myArmy.troops);
            double ratioArmyTreasury = (troopValue * myTotalTroops) / homeFief.treasury;

            bool canRecruitTroops = ratioArmyTreasury * targetMargin < targetRatioArmyTreasury;
            bool notEnoughTroops = ratioArmyTreasury < minRatioArmyTreasury;
            bool tooManyTroops = ratioArmyTreasury > maxRatioArmyTreasury;
            bool notEnoughEmployees = currentGS.myEmployeeNPCs.Count < targetNbEmployees;
            bool tooManyEmployees = currentGS.myEmployeeNPCs.Count > targetNbEmployees;
            bool canMakeNewEnemy = nbConsecutiveNPCmoveDirections >= nbNPCmoveInArowToSearchNewEnemy || currentGS.enemyPCids.Count() == 0;


            if (possibleActions.Contains(MainActions.FiefTransferFunds))
            {
                foreach (RedProtoFief myFief in currentGS.myFiefs)
                    if (!myFief.fiefID.Equals(homeFief.fiefID) && myFief.treasury > 0)
                    {
                        arguments = new string[] { myFief.fiefID, homeFief.fiefID, myFief.treasury.ToString() };
                        return MainActions.FiefTransferFunds; // MainActions.FiefTransferFunds
                    }
            }
            if (possibleActions.Contains(MainActions.KidnapRespondRansom))
            {
                arguments = new string[] { currentGS.ransomJentryID.ToString(), true.ToString() };
                return MainActions.KidnapRespondRansom; // MainActions.KidnapRespondRansom
            }
            if (possibleActions.Contains(MainActions.KidnapReleaseCaptive) && charToReleaseIDs.Count > 0)
            {
                arguments = new string[] { charToReleaseIDs.First() };
                return MainActions.KidnapReleaseCaptive; // MainActions.KidnapReleaseCaptive
            }
            if (possibleActions.Contains(MainActions.NPCbarCharacters) && charToBarIDs.Count > 0)
            {
                string charToBarID = charToBarIDs.First();
                CanBarCharFromOneOfMyFiefs(currentGS, charToBarID, out RedProtoFief fief);
                arguments = new string[] { fief.fiefID, charToBarID };
                return MainActions.NPCbarCharacters; // MainActions.NPCbarCharacters
            }
            if (possibleActions.Contains(MainActions.NPCunbarCharacters))
            { 
                if (charToUnbarIDs.Count > 0)
                {
                    string charToUnbarID = charToUnbarIDs.First();
                    foreach (RedProtoFief myFief in currentGS.myFiefs)
                    {
                        if(myFief.barredCharactersId != null)
                            if (myFief.barredCharactersId.Contains(charToUnbarID))
                            {
                                arguments = new string[] { myFief.fiefID, charToUnbarID };
                                return MainActions.NPCunbarCharacters; // MainActions.NPCunbarCharacters
                            }
                    }
                    throw new Exception("Can't find a fief from which to unbar the character");
                }
            }
            if (possibleActions.Contains(MainActions.NPCtryEnterKeep))
            {
                arguments = new string[] { currentPC.charID };
                return MainActions.NPCtryEnterKeep; // MainActions.NPCtryEnterKeep
            }
            if (possibleActions.Contains(MainActions.NPCaddToEntourage))
            {
                foreach (RedProtoNPC myFamilyNPC in currentGS.myFamilyNPCs)
                    if (!myFamilyNPC.inEntourage && myFamilyNPC.location.Equals(currentFief.fiefID) && !currentFief.ownerID.Equals(currentPCid))
                    {
                        arguments = new string[] { myFamilyNPC.charID };
                        return MainActions.NPCaddToEntourage; // MainActions.NPCaddToEntourage
                    }
                foreach (RedProtoNPC myEmployeeNPC in currentGS.myEmployeeNPCs)
                    if (! myEmployeeNPC.inEntourage && myEmployeeNPC.location.Equals(currentFief.fiefID))
                    {
                        if (!string.IsNullOrWhiteSpace(currentFief.bailiffID))
                        {
                            if (!currentFief.bailiffID.Equals(myEmployeeNPC.charID))
                            {
                                arguments = new string[] { myEmployeeNPC.charID };
                                return MainActions.NPCaddToEntourage; // MainActions.NPCaddToEntourage
                            }
                        }  
                        else
                        {
                            arguments = new string[] { myEmployeeNPC.charID };
                            return MainActions.NPCaddToEntourage; // MainActions.NPCaddToEntourage
                        }
                    } 
            }
            if (possibleActions.Contains(MainActions.NPCremoveFromEntourage))
            {
                foreach(RedProtoNPC myFamilyNPC in currentGS.myFamilyNPCs)
                    if (myFamilyNPC.inEntourage && currentFief.ownerID.Equals(currentPCid) && currentPC.inKeep)
                    {
                        arguments = new string[] { myFamilyNPC.charID };
                        return MainActions.NPCremoveFromEntourage; // MainActions.NPCremoveFromEntourage
                    }
                foreach (RedProtoNPC myEmployeeNPC in currentGS.myEmployeeNPCs)
                    if (myEmployeeNPC.inEntourage && !string.IsNullOrWhiteSpace(currentFief.bailiffID))
                        if(currentFief.bailiffID.Equals(myEmployeeNPC.charID))
                        {
                            arguments = new string[] { myEmployeeNPC.charID };
                            return MainActions.NPCremoveFromEntourage; // MainActions.NPCremoveFromEntourage
                        }
            }
            if (possibleActions.Contains(MainActions.NPCappointHeir))
            {
                string moreSuitableHeirId = null;
                double moreSuitableHeirLeadership = double.NegativeInfinity;
                foreach (RedProtoNPC myNPC in currentGS.myFamilyNPCs)
                    if (!String.IsNullOrWhiteSpace(myNPC.familyID) && myNPC.isMale && !myNPC.isHeir)
                        if (myNPC.familyID.Equals(currentPC.familyID))
                        {
                            double myNPCleadership = Tools.GetLeadershipValue(myNPC);
                            if (myNPCleadership > moreSuitableHeirLeadership)
                            {
                                moreSuitableHeirId = myNPC.charID;
                                moreSuitableHeirLeadership = myNPCleadership;
                            }
                        }
                bool appointNewHeir = false;
                if (String.IsNullOrWhiteSpace(currentPC.myHeirId))
                    appointNewHeir = true;
                else
                {
                    if (!currentGS.tryGetCharacter(currentPC.myHeirId, out RedProtoCharacter heir))
                        throw new Exception("Couldn't find the character '" + currentPC.myHeirId + "', it shouldn't happen");
                    if (!(heir is RedProtoNPC))
                        throw new Exception("The heir should be an NPC");
                    if (moreSuitableHeirLeadership > Tools.GetLeadershipValue(heir))
                        appointNewHeir = true;
                }
                if (appointNewHeir)
                {
                    arguments = new string[] { moreSuitableHeirId };
                    return MainActions.NPCappointHeir; // MainActions.NPCappointHeir
                }
            }
            if (possibleActions.Contains(MainActions.NPCtryForChild) & string.IsNullOrWhiteSpace(currentPC.myHeirId))
            {
                arguments = new string[] { };
                return MainActions.NPCtryForChild; // MainActions.NPCtryForChild
            }
            if (possibleActions.Contains(MainActions.NPCmarryPC))
            {
                int nbKnownPotentialBrides = 0;
                RedProtoNPC bestPotentialBride = null;
                double bestPotentialBrideScore = double.NegativeInfinity;
                foreach (RedProtoNPC notMyNPC in currentGS.notMyNPCs)
                    if (!notMyNPC.isMale && string.IsNullOrWhiteSpace(notMyNPC.spouse) && string.IsNullOrWhiteSpace(notMyNPC.fiancee) && string.IsNullOrWhiteSpace(notMyNPC.captor) && !notMyNPC.familyID.Equals(currentPC.familyID)
                        && currentGS.currentYear - notMyNPC.birthYear >= 14 && !currentGS.enemyPCids.Contains(notMyNPC.father)) // Check age (only if we have an estimation of the current year))
                    {
                        nbKnownPotentialBrides++;
                        if (estimateBrideScore(currentGS, notMyNPC) > bestPotentialBrideScore)
                        {
                            bestPotentialBride = notMyNPC;
                            bestPotentialBrideScore = estimateBrideScore(currentGS, notMyNPC);
                        }
                    }
                if (nbKnownPotentialBrides >= nbPotentialBridesToFindBeforeMarryingPC || (string.IsNullOrWhiteSpace(currentPC.myHeirId) && nbKnownPotentialBrides > 0))
                {
                    arguments = new string[] { bestPotentialBride.charID };
                    return MainActions.NPCmarryPC; // MainActions.NPCmarry
                }  
            }
            if (possibleActions.Contains(MainActions.NPCacceptRejectProposal))
            {
                if (!string.IsNullOrWhiteSpace(currentPC.spouse) && currentGS.tryGetNotMyNPC(currentGS.receivedProposalFromThisChar.charID, out RedProtoNPC npcWhoProposed))
                {
                    int nbKnownPotentialBrides = 0;
                    RedProtoNPC bestPotentialBride;
                    double bestPotentialBrideScore = double.NegativeInfinity;
                    foreach (RedProtoNPC notMyNPC in currentGS.notMyNPCs)
                        if (!notMyNPC.isMale && notMyNPC.spouse == null && notMyNPC.fiancee == null && notMyNPC.captor == null && !notMyNPC.familyID.Equals(currentPC.familyID)
                            && currentGS.currentYear - notMyNPC.birthYear >= 14 && !currentGS.enemyPCids.Contains(notMyNPC.father)) // Check age (only if I have an estimation of the current year))
                        {
                            nbKnownPotentialBrides++;
                            if (estimateBrideScore(currentGS, notMyNPC) > bestPotentialBrideScore)
                            {
                                bestPotentialBride = notMyNPC;
                                bestPotentialBrideScore = estimateBrideScore(currentGS, notMyNPC);
                            }
                        }
                    if (nbKnownPotentialBrides >= nbPotentialBridesToFindBeforeMarryingMySon || string.IsNullOrWhiteSpace(currentPC.myHeirId))
                    {

                        if (estimateBrideScore(currentGS, npcWhoProposed) > bestPotentialBrideScore * 0.75) // Accept if it's not too far from the best available choice, as if I propose I'm not sure if it will be acccepted.
                            arguments = new string[] { "true" };
                        else
                            arguments = new string[] { "false" };
                        return MainActions.NPCacceptRejectProposal; // MainActions.NPCacceptRejectProposal
                    }
                }
                else
                {
                    arguments = new string[] { "false" }; // Don't have enough details about this npc to compare her with the other potential brides
                    return MainActions.NPCacceptRejectProposal; // MainActions.NPCacceptRejectProposal
                }
            }
            if (possibleActions.Contains(MainActions.NPCmarryChild))
            {
                RedProtoNPC bestSonToMarry = null;
                double bestSonToMarryScore = double.NegativeInfinity;
                foreach (RedProtoNPC myFamilyNPC in currentGS.myFamilyNPCs)
                    if (myFamilyNPC.isMale && myFamilyNPC.spouse == null && myFamilyNPC.fiancee == null && myFamilyNPC.captor == null && currentGS.currentYear - myFamilyNPC.birthYear >= 14) // Check age (only if we have an estimation of the current year))
                        if(estimateBrideScore(currentGS, myFamilyNPC) > bestSonToMarryScore)
                        {
                            bestSonToMarry = myFamilyNPC;
                            bestSonToMarryScore = estimateBrideScore(currentGS, myFamilyNPC);
                        }

                int nbKnownPotentialBrides = 0;
                RedProtoNPC bestPotentialBride = null;
                double bestPotentialBrideScore = double.NegativeInfinity;
                foreach (RedProtoNPC notMyNPC in currentGS.notMyNPCs)
                    if (!notMyNPC.isMale && notMyNPC.spouse == null && notMyNPC.fiancee == null && notMyNPC.captor == null && !notMyNPC.familyID.Equals(currentPC.familyID)
                        && currentGS.currentYear - notMyNPC.birthYear >= 14 && !currentGS.enemyPCids.Contains(notMyNPC.father)) // Check age (only if I have an estimation of the current year))
                    {
                        nbKnownPotentialBrides++;
                        if (estimateBrideScore(currentGS, notMyNPC) > bestPotentialBrideScore)
                        {
                            bestPotentialBride = notMyNPC;
                            bestPotentialBrideScore = estimateBrideScore(currentGS, notMyNPC);
                        }
                    }
                if (nbKnownPotentialBrides >= nbPotentialBridesToFindBeforeMarryingMySon)
                {
                    arguments = new string[] { bestSonToMarry.charID, bestPotentialBride.charID }; // Shouldn't be null
                    return MainActions.NPCmarryPC; // MainActions.NPCmarry
                }
            }
            if (possibleActions.Contains(MainActions.SiegeNegotiation) && possibleActions.Contains(MainActions.SiegeStorm) && possibleActions.Contains(MainActions.SiegeEnd))
            {
                arguments = new string[] { currentFief.siege };

                if (!currentGS.tryGetMySiege(currentFief.siege, out RedProtoSiegeDisplay siegeInCurrentFief))
                    throw new Exception("Couldn't find the siege '" + currentFief.siege + "', it shouldn't happen");
                if (!currentGS.tryGetArmy(siegeInCurrentFief.besiegerArmyID, out RedProtoArmy besiegerArmy))
                    throw new Exception("Couldn't find the army '" + siegeInCurrentFief.besiegerArmyID + "', it shouldn't happen");
                if (!currentGS.tryGetNotMyGarrison(siegeInCurrentFief.defenderGarrisonID, out RedProtoArmy defenderGarrison))
                    throw new Exception("Couldn't find the garrison '" + siegeInCurrentFief.defenderGarrisonID + "', it shouldn't happen");
                double victoryOdds;
                if(currentGS.tryGetArmy(siegeInCurrentFief.defenderAdditional, out RedProtoArmy defenderAdditional))
                    victoryOdds = estimateBesiegerVictoryOdds(besiegerArmy.troops, siegeInCurrentFief.keepLevel, defenderGarrison.troops, defenderAdditional.troops);
                else
                    victoryOdds = estimateBesiegerVictoryOdds(besiegerArmy.troops, siegeInCurrentFief.keepLevel, defenderGarrison.troops);
                if (victoryOdds < maxBesiegeVictoryOddsToEnd)
                    return MainActions.SiegeEnd;
                else
                {
                    if (victoryOdds > minBesiegeVictoryOddsToStorm)
                        return MainActions.SiegeStorm;
                    else
                        return MainActions.SiegeNegotiation;
                }
            }
            if (possibleActions.Contains(MainActions.KidnapRansomCaptive))
            {
                foreach (string myCaptiveID in currentGS.myCaptiveIDs)
                    if (!myRansomedCaptiveIDs.Contains(myCaptiveID)) // Check is not ransomed yet
                    {
                        arguments = new string[] { myCaptiveID };
                        return MainActions.KidnapRansomCaptive; // MainActions.KidnapRansomCaptive
                    }
            }
            if (possibleActions.Contains(MainActions.ArmyAttack) && serverSideFixed) // !!! FIRST NEED TO MODIFY SERVER SO WE CAN KNOW THE OWNER OF AN ARMY (the charID, not the name) !!!
            {
                if(!string.IsNullOrWhiteSpace(currentPC.myHeirId) && currentGS.allyPCids.Concat(new string[] { currentPCid }).Contains(currentFief.ownerID))
                {
                    string armyToAttackId = null;
                    foreach (string armyInFiefId in currentFief.armyIDs)
                    {
                        if (!currentGS.tryGetArmy(armyInFiefId, out RedProtoArmy armyInFief))
                            throw new Exception("Couldn't find the army '" + armyInFiefId + "', it shouldn't happen");
                        if (currentGS.enemyPCids.Contains(armyInFief.ownerName) && Tools.CalculSumUintArray(mainArmy.troops) / Tools.CalculSumUintArray(armyInFief.troops) > minRatioMyTroopsEnemyTroopsToAttackInAllyFief)
                        {
                            armyToAttackId = armyInFiefId;
                            break;
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(armyToAttackId))
                    {
                        arguments = new string[] { mainArmy.armyID, armyToAttackId };
                        return MainActions.ArmyAttack; // MainActions.ArmyAttack
                    }
                }
            }
            if (possibleActions.Contains(MainActions.ArmyAppointLeader))
            {
                if (mainArmy != null)
                {
                    RedProtoCharacter bestCharToLeadMainArmy = findBestCharToLeadMainArmy(currentGS);
                    bool isAlreadyLeader = false;
                    foreach(RedProtoArmy myArmy in currentGS.myArmies)
                        if(!string.IsNullOrWhiteSpace(myArmy.leaderName))
                            if(myArmy.leaderName.Equals(bestCharToLeadMainArmy.firstName + " " + bestCharToLeadMainArmy.familyName))
                            {
                                isAlreadyLeader = true;
                                break;
                            }
                    if (!isAlreadyLeader && bestCharToLeadMainArmy.location.Equals(currentFief.fiefID) && currentFief.armyIDs.Contains(mainArmy.armyID))
                    {
                        arguments = new string[] { mainArmy.armyID, bestCharToLeadMainArmy.charID };
                        return MainActions.ArmyAppointLeader; // MainActions.ArmyAppointLeader
                    }
                }
            }
            if (possibleActions.Contains(MainActions.NPCremoveBaillif))
            {
                double bestAvailableEmployeeLeadership = double.NegativeInfinity;
                double worstBaillifLeadership = double.PositiveInfinity;
                string worstBaillifFiefID = null;
                foreach (RedProtoNPC myEmployeeNPC in currentGS.myEmployeeNPCs)
                {
                    bool isBaillif = false;
                    foreach (RedProtoFief myFief in currentGS.myFiefs)
                        if (!string.IsNullOrWhiteSpace(myFief.bailiffID))
                            if (myFief.bailiffID.Equals(myEmployeeNPC.charID))
                            {
                                if(mainArmy != null)
                                    if(mainArmy.leaderName != null)
                                        if(myEmployeeNPC.charID.Equals(mainArmy.leaderName)) // The leader of the current army is considered as more important than the baillifs
                                        {
                                            arguments = new string[] { myFief.fiefID };
                                            return MainActions.NPCremoveBaillif; // MainActions.NPCremoveBaillif
                                        }
                                double baillifLeadership = Tools.GetLeadershipValue(myEmployeeNPC);
                                if (baillifLeadership < worstBaillifLeadership)
                                {
                                    worstBaillifFiefID = myFief.fiefID;
                                    worstBaillifLeadership = baillifLeadership;
                                }
                                isBaillif = true;
                                break;
                            }
                    if (!isBaillif)
                    {
                        double myEmployeeNPCleadership = Tools.GetLeadershipValue(myEmployeeNPC);
                        if (myEmployeeNPCleadership > bestAvailableEmployeeLeadership)
                            bestAvailableEmployeeLeadership = myEmployeeNPCleadership;
                    }
                }
                bool allFiefsHaveBaillif = true;
                foreach (RedProtoFief myFief in currentGS.myFiefs)
                    if (string.IsNullOrWhiteSpace(myFief.bailiffID))
                    {
                        allFiefsHaveBaillif = false;
                        break;
                    }
                if (allFiefsHaveBaillif && bestAvailableEmployeeLeadership > worstBaillifLeadership)
                {
                    arguments = new string[] { worstBaillifFiefID };
                    return MainActions.NPCremoveBaillif; // MainActions.NPCremoveBaillif
                }

            }
            if (possibleActions.Contains(MainActions.NPCappointBaillif))
            {
                foreach (RedProtoFief myFief in currentGS.myFiefs)
                    if (string.IsNullOrWhiteSpace(myFief.bailiffID))
                    {
                        RedProtoNPC bestNPCtoAppointBail = null;
                        double bestNPCtoAppointBailLeadership = double.NegativeInfinity;
                        foreach (RedProtoNPC myEmployeeNPC in currentGS.myEmployeeNPCs)
                        {
                            if (mainArmy != null)
                                if (mainArmy.leaderName != null)
                                    if (mainArmy.leaderName.Equals(myEmployeeNPC.firstName + " " + myEmployeeNPC.familyName)) // The leader of the current army is considered as more important than the baillifs
                                        continue;
                            double myEmployeeNPCleadership = Tools.GetLeadershipValue(myEmployeeNPC);
                            if (myEmployeeNPC.isMale && currentGS.currentYear - myEmployeeNPC.birthYear >= 14 
                                && myEmployeeNPCleadership > bestNPCtoAppointBailLeadership)
                            {
                                bool isAlreadyBaillif = false;
                                foreach (RedProtoFief myFiefBis in currentGS.myFiefs)
                                    if(!string.IsNullOrWhiteSpace(myFiefBis.bailiffID))
                                        if (myFiefBis.bailiffID.Equals(myEmployeeNPC.charID))
                                        {
                                            isAlreadyBaillif = true;
                                            break;
                                        }
                                if (!isAlreadyBaillif)
                                {
                                    bestNPCtoAppointBail = myEmployeeNPC;
                                    bestNPCtoAppointBailLeadership = myEmployeeNPCleadership;
                                }
                            }
                        }
                        if (bestNPCtoAppointBail != null)
                        {
                            arguments = new string[] { myFief.fiefID, bestNPCtoAppointBail.charID };
                            return MainActions.NPCappointBaillif; // MainActions.NPCappointBaillif
                        }
                    }
            }
            if (possibleActions.Contains(MainActions.NPCfire))
            {
                uint totalSalaries = 0;
                foreach (RedProtoNPC myNPC in currentGS.myEmployeeNPCs)
                    totalSalaries += myNPC.salary;
                if (tooManyEmployees || totalSalaries / homeFief.treasury > minRatioNPCsalariesTreasuryToFireNPC)
                {
                    RedProtoNPC myWorstNPC = null;
                    double myWorstNPCleadership = double.PositiveInfinity;
                    foreach (RedProtoNPC myNPC in currentGS.myEmployeeNPCs)
                    {
                        double myNPCleadership = Tools.GetLeadershipValue(myNPC);
                        if (myNPCleadership < myWorstNPCleadership)
                        {
                            myWorstNPC = myNPC;
                            myWorstNPCleadership = myNPCleadership;
                        }
                    }
                    if (string.IsNullOrWhiteSpace(myWorstNPC.armyID)) // If isn't leading an army
                    {
                        arguments = new string[] { myWorstNPC.charID };
                        return MainActions.NPCfire; // MainActions.NPCfire
                    }
                }
            }
            if (possibleActions.Contains(MainActions.NPChire))
            {
                RedProtoNPC bestNPCtoHire = null;
                double bestNPCtoHireLeadership = double.NegativeInfinity;
                foreach (RedProtoNPC notMyNPC in currentGS.notMyNPCs)
                {
                    if (currentFief.characterInFiefIDs.Contains(notMyNPC.charID))
                        if (currentGS.spiedCharacters.TryGetValue(notMyNPC.charID, out RedProtoCharacter spiedCharInFief))
                        {
                            RedProtoNPC spiedNPCinFief = (RedProtoNPC)spiedCharInFief;
                            if (spiedNPCinFief.isMale && currentGS.currentYear - spiedNPCinFief.birthYear >= 14 && spiedNPCinFief.employerId == null && string.IsNullOrWhiteSpace(spiedNPCinFief.familyID))
                            {
                                double charInFiefLeadership = Tools.GetLeadershipValue(spiedNPCinFief);
                                if (charInFiefLeadership > bestNPCtoHireLeadership)
                                {
                                    bestNPCtoHire = (RedProtoNPC)spiedNPCinFief;
                                    bestNPCtoHireLeadership = charInFiefLeadership;
                                }
                            }
                        }
                }
                if (!double.IsNegativeInfinity(bestNPCtoHireLeadership))
                {
                    uint totalSalaries = 0;
                    RedProtoNPC myWorstNPC = null;
                    double myWorstNPCleadership = double.PositiveInfinity;
                    foreach (RedProtoNPC myNPC in currentGS.myEmployeeNPCs)
                    {
                        double myNPCleadership = Tools.GetLeadershipValue(myNPC);
                        if (myNPCleadership < myWorstNPCleadership)
                        {
                            myWorstNPC = myNPC;
                            myWorstNPCleadership = myNPCleadership;
                        }
                        totalSalaries += myNPC.salary;
                    }
                    bool hireNewNPC = false;
                    if (notEnoughEmployees)
                        hireNewNPC = true;
                    else
                    {
                        if (bestNPCtoHireLeadership > myWorstNPCleadership)
                            hireNewNPC = true;
                    }
                    if (hireNewNPC)
                    {
                        double maxBid = Tools.calculDifferentialToDividendToReachTarget(totalSalaries, homeFief.treasury, maxRatioNPCsalariesTreasuryToHireNPC);
                        uint bid = 0;
                        if (!string.IsNullOrWhiteSpace(bestNPCtoHire.charID))
                            if (bestNPCtoHire.charID.Equals(lastOfferToNPCid))
                                bid = (uint)Math.Ceiling(lastOfferToNPCamount * 1.2);
                        if (bid == 0)
                        {
                            if (bestNPCtoHire.lastOfferAmount > 0)
                                bid = (uint)Math.Ceiling(bestNPCtoHire.lastOfferAmount * 1.2);
                            else
                                bid = (uint)(estimateNPCacceptableSalary(bestNPCtoHire) * 0.5);
                        }
                        if (bid < maxBid)
                        {
                            arguments = new string[] { bestNPCtoHire.charID, bid.ToString() };
                            return MainActions.NPChire; // MainActions.NPChire
                        }
                    }
                }
            }
            if (possibleActions.Contains(MainActions.FiefAutoAdjustExpenditure))
            {
                foreach(RedProtoFief myFief in currentGS.myFiefs)
                    if(!expenditureAdjustedThisSeasonFiefIDs.Contains(myFief.fiefID) && myFief.treasury < 0)
                    {
                        arguments = new string[] { myFief.fiefID };
                        return MainActions.FiefAutoAdjustExpenditure; // MainActions.FiefAutoAdjustExpenditure
                    }
            }
            if (possibleActions.Contains(MainActions.ArmyDropOff) && possibleActions.Contains(MainActions.ArmyDisband))
            {
                if (tooManyTroops)
                    foreach (RedProtoArmy myArmy in currentGS.myArmies)
                        if(! myArmy.isMaintained)
                        {
                            if (!string.IsNullOrWhiteSpace(myArmy.leaderName))
                            {
                                uint idealNbToDropOff = (uint)Math.Floor(-1 * Tools.calculDifferentialToDividendToReachTarget(calculMyTotalNbTroops(currentGS) * troopValue, homeFief.treasury, targetRatioArmyTreasury) / troopValue);
                                uint nbTroopsInArmy = Tools.CalculSumUintArray(myArmy.troops);
                                if (idealNbToDropOff < nbTroopsInArmy)
                                {
                                    string[] troopsToDropOff = new string[7];
                                    uint currentNbToDropOff = idealNbToDropOff;
                                    for(int i=0; i < troopsToDropOff.Length; i++)
                                    {
                                        uint localNbToDrop = Math.Min(myArmy.troops[i], currentNbToDropOff);
                                        troopsToDropOff[i] = currentNbToDropOff.ToString();
                                        currentNbToDropOff -= localNbToDrop;
                                    }
                                    string allyToGiveTroopsID;
                                    if (currentGS.allyPCids.Count() > 0)
                                        allyToGiveTroopsID = currentGS.allyPCids.ToArray()[rnd.Next(currentGS.allyPCids.Count())];
                                    else
                                        allyToGiveTroopsID = currentPCid;
                                    arguments = (new string[] { myArmy.armyID }).Concat(troopsToDropOff.Cast<string>()).Concat(new string[] { allyToGiveTroopsID }).ToArray();
                                    return MainActions.ArmyDropOff; // MainActions.ArmyDropOff
                                }
                            }
                            arguments = new string[] { myArmy.armyID };
                            return MainActions.ArmyDisband; // MainActions.ArmyDisband 1/2
                        }
            }
            if (possibleActions.Contains(MainActions.ArmyMaintain))
            {
                foreach (RedProtoArmy myArmy in currentGS.myArmies)
                    if (!myArmy.isMaintained && homeFief.treasury >= myArmy.maintCost)
                    {
                        arguments = new string[] { myArmy.armyID };
                        return MainActions.ArmyMaintain; // MainActions.ArmyMaintain
                    }
            }
            else
            {
                if (possibleActions.Contains(MainActions.ArmyDisband))
                    if(!mainArmy.isMaintained)
                    {
                        arguments = new string[] { mainArmy.armyID };
                        return MainActions.ArmyDisband; // MainActions.ArmyDisband 2/2
                    }
            }
            if (possibleActions.Contains(MainActions.ArmyRecruit))
            {
                if (canRecruitTroops)
                {
                    bool isAncestralOwner = false;
                    if (currentFief.ancestralOwnerId != null)
                        if (currentFief.ancestralOwnerId.Equals(currentPC.charID))
                            isAncestralOwner = true;
                    if (notEnoughTroops || isAncestralOwner)
                    {
                        int idealNbToRecruitIfAncestralOwner = (int)Math.Ceiling(Tools.calculDifferentialToDividendToReachTarget(calculMyTotalNbTroops(currentGS) * troopValue, homeFief.treasury, targetRatioArmyTreasury) / troopValue);
                        int idealNbToRecruit = isAncestralOwner ? idealNbToRecruitIfAncestralOwner : (int)(idealNbToRecruitIfAncestralOwner / 4);
                        int nbToRecruit = (int)Math.Floor(Math.Min(currentFief.militia, Math.Min(homeFief.treasury / troopValue, idealNbToRecruit)));
                        arguments = new string[] { mainArmy.armyID, nbToRecruit.ToString() };
                        return MainActions.ArmyRecruit; // MainActions.ArmyHire
                    }
                }
            }
            if (possibleActions.Contains(MainActions.NPCmoveToFief))
            {
                if (string.IsNullOrWhiteSpace(currentPC.myHeirId))
                {
                    if (!string.IsNullOrWhiteSpace(currentPC.spouse))
                    {
                        if (!currentGS.tryGetCharacter(currentPC.spouse, out RedProtoCharacter spouse))
                            throw new Exception("Couldn't find the character '" + currentPC.spouse + "', it shouldn't happen");
                        if (currentGS.tryGetFief(spouse.location, out RedProtoFief spouseLocation))
                            if (!spouse.isPregnant && string.IsNullOrWhiteSpace(spouseLocation.siege) && spouse.days > 1)
                            {
                                arguments = new string[] { spouseLocation.fiefID };
                                return MainActions.NPCmoveToFief; // MainActions.NPCmoveToFief 1/4
                            }
                    }
                }
                if (!string.IsNullOrWhiteSpace(mainArmyFiefID))
                    if (!mainArmyFiefID.Equals(currentPC.location))
                    {
                        arguments = new string[] { mainArmy.location };
                        return MainActions.NPCmoveToFief; // MainActions.NPCmoveToFief 2/4
                    }
                    else
                    {
                        string bestDestinationID = null;
                        double bestTravelScore = 0;
                        foreach (RedProtoSiegeDisplay siegeOnMyFief in currentGS.siegeOnMyFiefs)
                        {
                            if (!siegeOnMyFief.besiegedFiefID.Equals(currentGS.myPC.location))
                            {
                                if (!dayCostToTravelToKnownFiefs.TryGetValue(siegeOnMyFief.besiegedFiefID, out double travelDayCost))
                                    throw new Exception("Couldn't find the cost to travel to the fief' " + siegeOnMyFief.besiegedFiefID + "', it shouldn't happen");
                                if (travelDayCost <= currentPC.days)
                                {
                                    uint[] TroopsBesieger;
                                    if (currentGS.tryGetArmy(siegeOnMyFief.besiegerArmyID, out RedProtoArmy besiegerArmy))
                                        TroopsBesieger = besiegerArmy.troops;
                                    else
                                        TroopsBesieger = new uint[7] { uint.MaxValue, 0, 0, 0, 0, 0, 0 };

                                    double interestMultiplicator;
                                    if (serverSideFixed) // !!! Needs some fixing on the server side to work !!!
                                    {
                                        interestMultiplicator = 0;
                                        if (estimateBesiegerVictoryOdds(TroopsBesieger, siegeOnMyFief.keepLevel, new uint[7] { 0, 0, 0, 0, 0, 0, 0 }) > minBesiegeEnnemyVictoryOddsToComeDefend) // TO DO: add the garrison to get more precided odds
                                        {
                                            double ratioMyTroopsEnemyTroops = Tools.CalculSumUintArray(mainArmy.troops) / Tools.CalculSumUintArray(TroopsBesieger);
                                            if (ratioMyTroopsEnemyTroops > minRatioMyTroopsEnemyTroopsToAttackInAllyFief)
                                                interestMultiplicator = 0.1 * siegeOnMyFief.keepLevel * Math.Min(ratioMyTroopsEnemyTroops, 2);
                                        }
                                    }
                                    else
                                    {
                                        interestMultiplicator = 1;
                                    }

                                    
                                    double travelScore = interestMultiplicator * 100 / travelDayCost;
                                    if (travelScore > bestTravelScore)
                                    {
                                        bestDestinationID = siegeOnMyFief.besiegedFiefID;
                                        bestTravelScore = travelScore;
                                    }
                                }
                            }
                        }
                        if (bestTravelScore > 5)
                        {
                            arguments = new string[] { bestDestinationID };
                            return MainActions.NPCmoveToFief; // MainActions.NPCmoveToFief 3/4
                        }
                    }
            }
            if (possibleActions.Contains(MainActions.ArmyPillageCurrentFief) && !currentGS.allyPCids.Contains(currentFief.ownerID))
            {
                if (!string.IsNullOrWhiteSpace(mainArmyFiefID) && canMakeNewEnemy || currentGS.enemyPCids.Contains(currentFief.ownerID))
                    if (mainArmyFiefID.Equals(currentPC.location))
                        if (string.IsNullOrWhiteSpace(currentFief.bailiffID)) 
                        {
                            arguments = new string[] { mainArmy.armyID };
                            return MainActions.ArmyPillageCurrentFief; // MainActions.ArmyPillageFief
                        }
            }
            if (possibleActions.Contains(MainActions.SpyFief) && serverSideFixed)
            {
                if ((canMakeNewEnemy || currentGS.enemyPCids.Contains(currentFief.ownerID)) && !string.IsNullOrWhiteSpace(currentPC.myHeirId))
                {
                    arguments = new string[] { currentFief.fiefID, currentPC.charID };
                    return MainActions.SpyFief; // MainActions.SpyFief
                }
            }
            if (possibleActions.Contains(MainActions.SiegeBesiegeCurrentFief))
                if(!string.IsNullOrWhiteSpace(currentPC.myHeirId) && !currentGS.allyPCids.Contains(currentFief.ownerID) && fiefSpiedThisSeasonIDs.Contains(currentFief.fiefID))
                    if(!string.IsNullOrWhiteSpace(mainArmyFiefID) && canMakeNewEnemy || currentGS.enemyPCids.Contains(currentFief.ownerID))
                        if (mainArmyFiefID.Equals(currentPC.location))
                        {
                            if (! currentGS.spiedFiefs.TryGetValue(currentFief.fiefID, out RedProtoFief spiedFief))
                                throw new Exception("Couldn't find the spied fief '" + currentFief.fiefID + "', it shouldn't happen");
                            uint[] defenderGarrisonTroops = new uint[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
                            uint[] defenderAdditionalTroops = new uint[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
                            foreach (RedProtoArmy notMyGarrison in currentGS.notMyGarrisons)
                                if (notMyGarrison.location.Equals(currentPC.location))
                                {
                                    defenderGarrisonTroops = notMyGarrison.troops;
                                    break;
                                }
                            foreach (RedProtoArmy notMyArmy in currentGS.notMyArmies)
                                if (notMyArmy.location.Equals(currentPC.location) && notMyArmy.ownerName.Equals(currentFief.ownerID))
                                {
                                    defenderAdditionalTroops = notMyArmy.troops;
                                    break;
                                }
                            if (estimateBesiegerVictoryOdds(mainArmy.troops, spiedFief.keepLevel, defenderGarrisonTroops, defenderAdditionalTroops) > minBesiegeVictoryOddsToStart)
                            {
                                arguments = new string[] { mainArmy.armyID };
                                return MainActions.SiegeBesiegeCurrentFief;
                            }
                        }
            if (possibleActions.Contains(MainActions.SpyCharacter))
            {
                if(!string.IsNullOrWhiteSpace(currentPC.myHeirId) && canMakeNewEnemy)
                    foreach (RedProtoPlayerCharacter notMyPC in currentGS.notMyPCs)
                        if (currentFief.characterInFiefIDs.Contains(notMyPC.charID) && !charSpiedThisSeasonIDs.Contains(notMyPC.charID))
                        {
                            arguments = new string[] { notMyPC.charID, currentPCid };
                            return MainActions.SpyCharacter; // MainActions.SpyCharacter
                        }
                if(notEnoughEmployees || (string.IsNullOrWhiteSpace(currentPC.myHeirId) && string.IsNullOrWhiteSpace(currentPC.spouse)))
                    foreach (RedProtoNPC notMyNPC in currentGS.notMyNPCs)
                        if (currentFief.characterInFiefIDs.Contains(notMyNPC.charID) && !charSpiedThisSeasonIDs.Contains(notMyNPC.charID))
                        {
                            arguments = new string[] { notMyNPC.charID, currentPCid };
                            return MainActions.SpyCharacter; // MainActions.SpyCharacter
                        }
            }
            if (possibleActions.Contains(MainActions.KidnapCharacter) & serverSideFixed)
            {
                foreach (string characterInFiefID in currentFief.characterInFiefIDs)
                {
                    if (currentGS.enemyPCids.Contains(characterInFiefID))
                    {
                        currentGS.tryGetNotMyPC(characterInFiefID, out RedProtoPlayerCharacter notMyPC); // No check as it shoudln't be null
                        if (string.IsNullOrWhiteSpace(notMyPC.captor))
                            foreach (RedProtoNPC myEmployeeNPC in currentGS.myEmployeeNPCs) // Check if I have a suitable unnocupied NPC to kidnap him
                                if (myEmployeeNPC.location.Equals(currentFief.fiefID))
                                {
                                    bool isNPCimportant = false;
                                    foreach (RedProtoArmy myArmy in currentGS.myArmies)
                                        if (!string.IsNullOrWhiteSpace(myArmy.leaderName))
                                            if (myArmy.leaderName.Equals(myEmployeeNPC.firstName + " " + myEmployeeNPC.familyName))
                                            {
                                                isNPCimportant = true;
                                                break;
                                            }
                                    if (!isNPCimportant)
                                        foreach (RedProtoFief myFief in currentGS.myFiefs)
                                            if(!string.IsNullOrWhiteSpace(myFief.bailiffID))
                                                if (myFief.bailiffID.Equals(myEmployeeNPC.charID))
                                                {
                                                    isNPCimportant = true;
                                                    break;
                                                }
                                    if (!isNPCimportant)
                                    {
                                        arguments = new string[] { notMyPC.charID, myEmployeeNPC.charID };
                                        return MainActions.KidnapCharacter; // MainActions.KidnapCharacter
                                    }
                                }
                    }
                }
            }
            if (possibleActions.Contains(MainActions.SpyArmy))
            {
                if (!string.IsNullOrWhiteSpace(currentPC.myHeirId))
                {
                    bool foundEnemyArmyToSpy = false;
                    string armyToSpyId = null;
                    foreach (string armyInFiefID in currentFief.armyIDs)
                    {
                        if (!armyInFiefID.Contains("Garrison"))
                        {
                            bool isMine = false;
                            foreach (RedProtoArmy myArmy in currentGS.myArmies)
                                if (myArmy.armyID.Equals(armyInFiefID))
                                {
                                    isMine = true;
                                    break;
                                }
                            if (!isMine && !armySpiedThisSeasonIDs.Contains(armyInFiefID))
                            {
                                if (armyToSpyId == null)
                                    armyToSpyId = armyInFiefID;
                                if (currentGS.tryGetArmy(armyInFiefID, out RedProtoArmy armyInFief))
                                    if (currentGS.enemyPCids.Contains(armyInFief.ownerName))
                                    {
                                        armyToSpyId = armyInFiefID;
                                        foundEnemyArmyToSpy = true;
                                        break;
                                    }
                            }
                        }
                    }
                    if (foundEnemyArmyToSpy)
                    {
                        arguments = new string[] { armyToSpyId, currentPC.charID };
                        return MainActions.SpyArmy; // MainActions.SpyArmy
                    }
                }
            }
            if (possibleActions.Contains(MainActions.ArmyAttack))
            {
                if(!string.IsNullOrWhiteSpace(currentPC.myHeirId))
                {
                    if (!string.IsNullOrWhiteSpace(mainArmyFiefID) && (canMakeNewEnemy || currentGS.enemyPCids.Contains(currentFief.ownerID)))
                        if (mainArmyFiefID.Equals(currentPC.location))
                        {
                            string armyToAttackId = null;
                            foreach (string armyInFiefId in currentFief.armyIDs)
                            {
                                if (!currentGS.tryGetArmy(armyInFiefId, out RedProtoArmy armyInFief))
                                    throw new Exception("Couldn't find the army '" + armyInFiefId + "', it shouldn't happen");
                                if (currentGS.enemyPCids.Contains(armyInFief.ownerName) && Tools.CalculSumUintArray(mainArmy.troops) / Tools.CalculSumUintArray(armyInFief.troops) > minRatioMyTroopsEnemyTroopsToAttackInNotAllyFief)
                                {
                                    armyToAttackId = armyInFiefId;
                                    break;
                                }
                            }
                            if (armyToAttackId != null)
                            {
                                arguments = new string[] { mainArmy.armyID, armyToAttackId };
                                return MainActions.ArmyAttack; // MainActions.ArmyAttack
                            }
                        }
                }
            }
            if (possibleActions.Contains(MainActions.NPCmoveToFief))
            {
                RedProtoFief bestDestination = null;
                double bestTravelScore = 0;
                foreach (RedProtoFief knownFief in currentGS.myFiefs.Concat(currentGS.notMyFiefs))
                {
                    if (!knownFief.fiefID.Equals(currentGS.myPC.location))
                    {
                        if (! dayCostToTravelToKnownFiefs.TryGetValue(knownFief.fiefID, out double travelDayCost))
                            throw new Exception("Couldn't find the cost to travel to the fief' " + knownFief.fiefID + "', it shouldn't happen");
                        if(travelDayCost <= currentPC.days)
                        {
                            double interestMultiplicator = 0;
                            if (currentGS.allyPCids.Contains(knownFief.ownerID))
                                interestMultiplicator = 0;
                            if (currentGS.enemyPCids.Contains(knownFief.ownerID) && !fiefVisitedThisSeasonIDs.Contains(knownFief.fiefID))
                                interestMultiplicator = 0.5;
                            if (knownFief.ownerID.Equals(currentPC.charID) && canRecruitTroops && !knownFief.hasRecruited)
                            {
                                double idealNbToRecruit = Tools.calculDifferentialToDividendToReachTarget(calculMyTotalNbTroops(currentGS) * troopValue, homeFief.treasury, targetRatioArmyTreasury) / troopValue;
                                interestMultiplicator = 0.75;
                                if (knownFief.ancestralOwnerId != null)
                                    if (knownFief.ancestralOwnerId.Equals(currentPC.charID))
                                        interestMultiplicator = 1;
                                if (notEnoughTroops)  // In this case this is urgent to go recruit some troops
                                    interestMultiplicator = 10;
                                interestMultiplicator *= Math.Min(1, knownFief.militia / idealNbToRecruit); // To privilege fiefs in which there's enough troops to reach the ideal ratio
                            }
                            foreach (RedProtoNPC myEmployeeNPC in currentGS.myEmployeeNPCs)
                            {
                                if (myEmployeeNPC.location.Equals(knownFief.fiefID))
                                {
                                    if (!string.IsNullOrWhiteSpace(knownFief.bailiffID))
                                    {
                                        if (!knownFief.bailiffID.Equals(myEmployeeNPC.charID))
                                        {
                                            interestMultiplicator = 100; // An employee is not being used
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        interestMultiplicator = 100; // An employee is not being used
                                        break;
                                    }
                                }     
                                if(myEmployeeNPC.inEntourage && !string.IsNullOrWhiteSpace(knownFief.bailiffID))
                                    if(knownFief.bailiffID.Equals(myEmployeeNPC.charID))
                                    {
                                        interestMultiplicator = 100; // Need to bring the baillif to his fief
                                        break;
                                    }
                            }
                            foreach (RedProtoNPC myFamilyNPC in currentGS.myFamilyNPCs)
                                if (myFamilyNPC.location.Equals(knownFief.fiefID) && !knownFief.ownerID.Equals(currentPCid))
                                {
                                    interestMultiplicator = 1000; // A family member isn't in one of my fiefs
                                    break;
                                }

                            double travelScore = interestMultiplicator * 100 / travelDayCost;
                            if (travelScore > bestTravelScore)
                            {
                                bestDestination = knownFief;
                                bestTravelScore = travelScore;
                            }
                        }
                    }
                }
                if (bestTravelScore > 5)
                {
                    arguments = new string[] { bestDestination.fiefID };
                    return MainActions.NPCmoveToFief; // MainActions.NPCmoveToFief 4/4
                }
            }
            if (possibleActions.Contains(MainActions.NPCmoveDirection))
            {
                List<MoveDirections> moveProbabilities = new List<MoveDirections>();
                foreach(MoveDirections costToTravelToAdjacentFiefsKey in dayCostToTravelToAdjacentFiefs.Keys)
                {
                    if (!dayCostToTravelToAdjacentFiefs.TryGetValue(costToTravelToAdjacentFiefsKey, out double costToTravelToAdjacentFiefsValue))
                        throw new Exception("Couldn't find the key '" + costToTravelToAdjacentFiefsKey.ToString() + "', it shouldn't happen");
                    if (costToTravelToAdjacentFiefsValue <= currentPC.days)
                    {
                        int proba = 16 - (int)costToTravelToAdjacentFiefsValue; // To more often pick the fastest path, without completely discarding the other options (except 91 days one, as it takes a whole season to travel)
                        if (nbConsecutiveNPCmoveDirections >= 2)
                        {
                            if (!oppositeDirections.TryGetValue(lastNPCmoveDirection, out MoveDirections previousDirectionOpposite))
                                throw new Exception("Couldn't find the opposite direction of '" + previousDirectionOpposite + "', it shouldn't happen");
                            if (previousDirectionOpposite == costToTravelToAdjacentFiefsKey)
                                proba = 1; // We diminish the chances of going backward
                        }
                        for (int i = 0; i < proba; i++)
                            moveProbabilities.Add(costToTravelToAdjacentFiefsKey);
                    }
                        
                }
                arguments = new string[] { moveProbabilities[rnd.Next(moveProbabilities.Count)].ToString() };
                return MainActions.NPCmoveDirection; // MainActions.NPCmove
            }

            arguments = new string[0];
            return MainActions.None;
        }

        /// <summary>
		///     Estimates the odds to win for the besieger
		/// </summary>
		/// <param name="besiegerTroops"></param>
        /// <param name="keepLevel"></param>
        /// <param name="defenderGarrisonTroops"></param>
        /// <param name="defenderAdditionalTroops">If the defender has an army present in the fief that has the right aggro level</param>
		/// <returns>The odds to win for the besieger</returns>
        private double estimateBesiegerVictoryOdds(uint[] besiegerTroops, double keepLevel, uint[] defenderGarrisonTroops, uint[] defenderAdditionalTroops = null)
        {
            uint nbTroopsDefenderAdditional = 0;
            if (defenderAdditionalTroops != null)
                nbTroopsDefenderAdditional = Tools.CalculSumUintArray(defenderAdditionalTroops);
            double besiegerVictoryOdds = Tools.CalculSumUintArray(besiegerTroops) / (keepLevel * 1000 + Tools.CalculSumUintArray(defenderGarrisonTroops) + nbTroopsDefenderAdditional);
            return besiegerVictoryOdds;
        }

        /// <summary>
		///     Find the character with the most combat skill
		/// </summary>
		/// <param name="gameState"></param>
		/// <returns>The character with the most combat skill</returns>
        private RedProtoCharacter findBestCharToLeadMainArmy(GameState gameState)
        {
            RedProtoPlayerCharacter currentPC = gameState.myPC;
            RedProtoCharacter bestCharToLeadCurrentArmy = null;
            double bestCharToLeadCurrentArmyCombatSkill = double.NegativeInfinity;
            foreach (RedProtoNPC myEmployee in gameState.myEmployeeNPCs)
                if (myEmployee.combat > bestCharToLeadCurrentArmyCombatSkill)
                {
                    bestCharToLeadCurrentArmy = myEmployee;
                    bestCharToLeadCurrentArmyCombatSkill = myEmployee.combat;
                }
            if (bestCharToLeadCurrentArmyCombatSkill < currentPC.combat)
                bestCharToLeadCurrentArmy = currentPC;
            return bestCharToLeadCurrentArmy;
        }

        /// <summary>
		///     Estimates the bride score of a female NPC
		/// </summary>
		/// <param name="gameState"></param>
        /// <param name="potentialBride">NPC from which to estimate a bride score</param>
		/// <returns>Bride score</returns>
        private double estimateBrideScore(GameState gameState, RedProtoNPC potentialBride)
        {
            return (potentialBride.health * 10 + 100 - gameState.currentYear - potentialBride.birthYear) / 2; // Score /100 except if char age is > 100
        }
    }
}