using System;
using System.Collections.Generic;
using System.Linq;

namespace WorldBuilder
{
    internal class Settlement
    {
        public Settlement()
        {
            Name = "The Settlement";
            Population = 10000;
            Type = "City";
        }

        public string Name { get; set; }
        public int Population { get; set; }
        public string Type { get; set; }

        //Random number generator
        Random rnd = new Random();

        //Dictionary pairing the names and Support-Values (SVs) for trades in the settlement
        //An SV is the number of citizens in a settlement required for there to be 1 of that trade
        //For example, for every 800 people in a settlement, there will be 1 baker in that settlement
        Dictionary<string, int> SVDictionary = new Dictionary<string, int>()
        {
            {"Bakers", 800},            {"Barbers", 350},           {"Bathers", 1900},          {"Beer-sellers", 1400}, {"Blacksmiths", 1500},      {"Bleachers", 2100},
            {"Bookbinders", 3000},      {"Booksellers", 6300},      {"Buckle Makers", 1400},    {"Butchers", 1200},     {"Carpenters", 550},        {"Chandlers", 700},
            {"Chicken Butchers", 1000}, {"Coopers", 700},           {"Copyists", 2000},         {"Cutlers", 2300},      {"Doctors", 1700},          {"Fishmongers", 1200},
            {"Furriers", 250},          {"Glovemakers", 2400},      {"Harness-makers", 2000},   {"Hatmakers", 950},     {"Hay Merchants", 2300},    {"Illuminators", 3900},
            {"Inns", 2000},             {"Jewelers", 400},          {"Locksmiths", 1900},       {"Magic Shops", 2800},  {"Maidservants", 250},      {"Masons", 500},
            {"Mercers", 700},           {"Old Clothes", 400},       {"Painters", 1500},         {"Pastrycooks", 500},   {"Plasterers", 1400},       {"Pursemakers", 1100},
            {"Roofers", 1800},          {"Ropemakers", 1900},       {"Rugmakers", 2000},        {"Saddlers", 1000},     {"Scabbardmakers", 850},    {"Sculptors", 2000},
            {"Shoemakers", 150},        {"Spice Merchants", 1400},  {"Tailors", 250},           {"Tanners", 2000},      {"Taverns", 400},           {"Watercarriers", 850},
            {"Weavers", 600},           {"Wine-sellers", 900},      {"Woodcarvers", 2400},      {"Woodsellers", 2400}
        };

        //Method to do calculations for settlement's size and return a string
        public string calculateSettlementSize()
        {
            //Determine what type of settlement this is
            //0-1000 people is a village
            if (Population <= 1000)
            {
                Type = "village";
            }
            //1001-8000 people is a town
            else if (Population <= 8000)
            {
                Type = "town";
            }
            //8001+ people is a city
            else
            {
                Type = "city";
            }

            //To randomize settlement density, roll 7d4 and drop the highest 2, and multiply the result by 15
            //Average is about 225 people/hectare
            //Divide this number by 2.5 to get people/acre
            List<int> settlementDensityDice = new List<int>();
            settlementDensityDice.Add(rnd.Next(1, 5));
            settlementDensityDice.Add(rnd.Next(1, 5));
            settlementDensityDice.Add(rnd.Next(1, 5));
            settlementDensityDice.Add(rnd.Next(1, 5));
            settlementDensityDice.Add(rnd.Next(1, 5));
            settlementDensityDice.Add(rnd.Next(1, 5));
            settlementDensityDice.Add(rnd.Next(1, 5));
            settlementDensityDice.Remove(settlementDensityDice.Max());
            settlementDensityDice.Remove(settlementDensityDice.Max());
            int settlementDensity = (int)(settlementDensityDice.Sum() * 15 / 2.5);

            int acres = Population / settlementDensity;

            string settlementSizeString = "The " + Type + " of " + Name + " covers approximately " + acres + " acres " +
                                          "with a population of " + Population + " people.";

            return settlementSizeString;
        }

        //Method to do calculations for settlement's trades and return a string
        public string calculateSettlementTrades()
        {
            string tradesString = "";
            int tradesCount = 0;
            foreach (KeyValuePair<string, int> i in SVDictionary)
            {
                tradesString += string.Format("{0} - {1} ", i.Key, Population / i.Value);
                if (tradesCount != 0 && tradesCount % 6 == 0)
                {
                    tradesString += "\n";
                }
            }

            return tradesString;
        }

        //Method to do calculations for settlement's misc. info and return a string
        public string calculateSettlementMisc()
        {
            int nobleHouseholds = Population / 200;
            int guards = Population / 150;
            int lawyers = Population / 650;
            int clergy = Population / 40;
            int priests = clergy / 25;
            int placesOfWorship = Population / 400;

            string miscString = "There are " + nobleHouseholds + " noble houses in " + Name + ", and the " + Type + " is guarded" +
                                " by " + guards + " guardsmen. " + Name + " has " + lawyers + " advocates to assist its citizens in legal matters." +
                                " For spiritual matters, the " + Type + " has " + clergy + " clergymen and " + priests + " priests serving " +
                                placesOfWorship + " temples and other places of worship.";

            return miscString;
        }
    }
}
