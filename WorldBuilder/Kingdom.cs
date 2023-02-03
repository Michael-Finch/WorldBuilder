using System;

namespace WorldBuilder
{
    internal class Kingdom
    {
        public Kingdom()
        {
            Name = "The Kingdom";
            PhysicalArea = 150000;
            PopulationDensity = 80;
            Age = 500;
            PercentArable = 45;
        }

        public string Name { get; set; }
        public int PhysicalArea { get; set; }
        public int PopulationDensity { get; set; }
        public int Age { get; set; }
        public int PercentArable { get; set; }

        //Random number generator
        Random rnd = new Random();

        //Method to randomly determine kingdom's arable land and update population density
        public void setDensity(string densityString)
        {
            if(densityString.Equals("Desolate"))
            {
                PercentArable = rnd.Next(11, 21);
                PopulationDensity = 20;
            }
            else if(densityString.Equals("Low"))
            {
                PercentArable = rnd.Next(21, 31);
                PopulationDensity = 40;
            }
            else if (densityString.Equals("Settled"))
            {
                PercentArable = rnd.Next(31, 44);
                PopulationDensity = 60;
            }
            else if (densityString.Equals("Average"))
            {
                PercentArable = rnd.Next(44, 55);
                PopulationDensity = 80;
            }
            else if (densityString.Equals("High"))
            {
                PercentArable = rnd.Next(55, 66);
                PopulationDensity = 100;
            }
            else if (densityString.Equals("Maximum"))
            {
                PercentArable = rnd.Next(66, 76);
                PopulationDensity = 120;
            }
        }

        //Method to do calculations for kingdom's physical area and return a string
        public string calculatePhysicalArea()
        {
            double arableLand = (double)PhysicalArea * PercentArable / 100;
            double wilderness = PhysicalArea - arableLand;

            string physicalAreaString = Name + " covers an area of " + PhysicalArea.ToString() + " square miles. Of this, " +
                                        PercentArable.ToString() + "% (" + arableLand.ToString() + " square miles) is arable land, and " +
                                        (100 - PercentArable).ToString() + "% (" + wilderness.ToString() + " square miles) is wilderness.";

            return physicalAreaString;
        }

        //Method to do calculations for kingdom's total population and return string
        public string calculateTotalPopulation()
        {
            int totalPopulation = PhysicalArea * PopulationDensity;
            string totalPopulationString = Name + " has a total population of " + totalPopulation.ToString() + " people.";

            return totalPopulationString;
        }

        //Method to do calculations for the kingdom's settlements and display information
        public string calculateSettlements()
        {
            int totalPopulation = PhysicalArea * PopulationDensity;
            int remainingPopulation = totalPopulation;
            double populationRoot = Math.Sqrt(totalPopulation);
            double modifier = (rnd.Next(1, 5) + rnd.Next(1, 5) + 10); //Between 12-20, average of 15

            //The population of the largest settlement = the square root of the kingdom's total population multiplied by a random modifier
            int largestPopulation = (int)(populationRoot * modifier);
            remainingPopulation -= largestPopulation;

            //The second largest settlement will be 20-80% the size of the largest
            modifier = (rnd.Next(1, 5) + rnd.Next(1, 5)) * 0.1; //Between 0.2-0.8, average of 0.5
            int secondLargestPopulation = (int)(largestPopulation * modifier);
            remainingPopulation -= secondLargestPopulation;

            string settlementsString = "The largest settlement has a population of " + largestPopulation + " people. " +
                                       "The second largest has a population of " + secondLargestPopulation + " people. " +
                                       "The remaining " + remainingPopulation + " people live in numerous small towns, villages, " +
                                       "isolated dwellings, etc.";

            return settlementsString;
        }

        //Method to do calculations for the kingdom's castles and display information
        public string calculateCastles()
        {
            int totalPopulation = PhysicalArea * PopulationDensity;

            //Calculate the number of ruined castles
            int ruinedCastlesTotal = (int)(Math.Sqrt(Age) * (totalPopulation / 5000000f));
            int ruinedCastlesCivilized = (int)(0.75 * ruinedCastlesTotal);
            int ruinedCastlesWilderness = ruinedCastlesTotal - ruinedCastlesCivilized;

            //Calculate the number of active castles
            int activeCastlesTotal = totalPopulation / 50000;
            int activeCastlesCivilized = (int)(0.75 * activeCastlesTotal);
            int activeCastlesWilderness = activeCastlesTotal - activeCastlesCivilized;

            string castlesString = Name + " has " + activeCastlesTotal + " active castles and " + ruinedCastlesTotal + " ruined castles. Of these, " +
                                   activeCastlesCivilized + " active castles and " + ruinedCastlesCivilized + " ruined castles are in civilized lands, and " +
                                   activeCastlesWilderness + " active castles and " + ruinedCastlesWilderness + " ruined castles are in the wilderness, along borders, etc.";

            return castlesString;
        }
    }
}