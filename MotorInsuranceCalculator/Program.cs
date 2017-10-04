using System;
using System.Collections.Generic;
using System.Linq;

namespace MotorInsuranceCalculation
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime tempDob, tempStartDate, tempDateOfClaim;
            DateTime startDate = DateTime.Now;
            DateTime dateOfBirth = DateTime.Now;
            DateTime dateOfClaim = DateTime.Now;
            string name = "";
            string occupation = "";
            string claimResponse = "";
            string driverResponse = "";
            string tempName, tempOccupation, tempClaimResponse, tempDriverResponse, output;
            int numberOfClaims = 0;
            int numberOfDrivers = 0;
            Boolean additionalClaim = false;
            Boolean finalDriver = false;
            Boolean startDateValid = false;
            Boolean nameValid, occupationValid, dobValid, claimResponseValid, driverResponseValid, dateOfClaimValid;
            List<Driver> drivers = new List<Driver>();

            Console.WriteLine("Motor Calculation Insurance Program");
            Console.WriteLine("Please enter the proposed start date of the policy in the format dd/mm/yyyy:");
            //validate the start date of the claim
            while (!startDateValid)
            {
                if (DateTime.TryParse(Console.ReadLine(), out tempStartDate))
                {
                    startDate = tempStartDate;
                    startDateValid = true;
                }
                else
                {
                    Console.WriteLine("Please enter a valid start date in the format dd/mm/yyyy:");
                }
            }
            while (finalDriver == false && (numberOfDrivers < 5))
            {
                nameValid = false;
                occupationValid = false;
                dobValid = false;
                claimResponseValid = false;
                driverResponseValid = false;
                Console.WriteLine("Please enter the driver's name:");
                while (!nameValid)
                {
                    tempName = Console.ReadLine();
                    if (tempName.Length >= 1)
                    {
                        name = tempName;
                        nameValid = true;
                    }
                    else
                    {
                        Console.WriteLine("Please enter the driver's name:");
                    }
                }
                Console.WriteLine("Please enter the driver's occupation:");
                while (!occupationValid)
                {
                    tempOccupation = Console.ReadLine();
                    if (tempOccupation.Length >= 1)
                    {
                        occupation = tempOccupation;
                        occupationValid = true;
                    }
                    else
                    {
                        Console.WriteLine("Please enter the driver's occupation:");
                    }
                }
                Console.WriteLine("Please enter the driver's date of birth in the format dd/mm/yyyy:");
                //validate driver's date of birth.
                while (!dobValid)
                {
                    if (DateTime.TryParse(Console.ReadLine(), out tempDob))
                    {
                        dateOfBirth = tempDob;
                        dobValid = true;
                    }
                    else
                    {
                        Console.WriteLine("Please enter a valid date of birth in the format dd/mm/yyyy:");
                    }
                }
                Driver aDriver = new Driver(name, occupation, dateOfBirth);
                Console.WriteLine("Are there any previous claims associated with this driver? Y/N:");
                while (!claimResponseValid)
                {
                    tempClaimResponse = Console.ReadLine();
                    if (tempClaimResponse == "Y" || tempClaimResponse == "N")
                    {
                        claimResponse = tempClaimResponse;
                        claimResponseValid = true;
                    }
                    else
                    {
                        Console.WriteLine("Are there any previous claims associated with this driver? Y/N:");
                    }
                }
                if (claimResponse == "Y")
                {
                    additionalClaim = true;
                }
                while (additionalClaim && (numberOfClaims < 5))
                {
                    dateOfClaimValid = false;
                    Console.WriteLine("Please enter the date of the claim in the format dd/mm/yyyy:");
                    while (!dateOfClaimValid)
                    {
                        if (DateTime.TryParse(Console.ReadLine(), out tempDateOfClaim))
                        {
                            dateOfClaim = tempDateOfClaim;
                            dateOfClaimValid = true;
                        }
                        else
                        {
                            Console.WriteLine("Please enter a valid claim date in the format dd/mm/yyyy:");
                        }
                    }
                    aDriver.addClaim(dateOfClaim);
                    numberOfClaims++;
                    if (numberOfClaims < 5)
                    {
                        Console.WriteLine("Are there any other claims associated with this driver? Y/N");
                        claimResponseValid = false;
                        while (!claimResponseValid)
                        {
                            tempClaimResponse = Console.ReadLine();
                            if (tempClaimResponse == "Y" || tempClaimResponse == "N")
                            {
                                claimResponse = tempClaimResponse;
                                claimResponseValid = true;
                            }
                            else
                            {
                                Console.WriteLine("Are there any previous claims associated with this driver? Y/N:");
                            }
                        }
                        if (claimResponse == "Y")
                        {
                            additionalClaim = true;
                        }
                        else
                        {
                            additionalClaim = false;
                        }
                    }
                }
                drivers.Add(aDriver);
                numberOfDrivers++;
                if (numberOfDrivers < 5)
                {
                    Console.WriteLine("Would you like to include another driver? Y/N:");
                    while (!driverResponseValid)
                    {
                        tempDriverResponse = Console.ReadLine();
                        if (tempDriverResponse == "Y" || tempDriverResponse == "N")
                        {
                            driverResponse = tempDriverResponse;
                            driverResponseValid = true;
                        }
                        else
                        {
                            Console.WriteLine("Would you like to include another driver? Y/N:");
                        }
                    }
                    if (driverResponse == "Y")
                    {
                        finalDriver = false;
                    }
                    else
                    {
                        finalDriver = true;
                    }
                }

            }


            output = calculatePremium(startDate, drivers);
            Console.WriteLine(output);

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        static String calculatePremium(DateTime theStartDate, List<Driver> theDrivers)
        {
            String response, claimsResponse;
            Boolean startDateOk, youngestAgeOk, oldestAgeOk;
            Driver youngestDriver, oldestDriver;
            List<Driver> orderedDrivers;
            double premium = 500.00;
            String premiumString;

            //check that the start date of the claim is not earlier than today's date.
            startDateOk = checkStartDate(theStartDate);
            if (!startDateOk)
            {
                response = "Start Date of Policy";
                return response;
            }

            //amend the premium based on the drivers' occupations.
            premium = occupationAmend(theDrivers, premium);

            //order the drivers by ascending order of age
            orderedDrivers = theDrivers.OrderByDescending(driver => driver.getDateOfBirth()).ToList();
            youngestDriver = orderedDrivers[0];
            oldestDriver = orderedDrivers[orderedDrivers.Count - 1];

            //check that the youngest driver is at least 21 at the claim start date.
            youngestAgeOk = youngestOver21(youngestDriver, theStartDate);
            if (!youngestAgeOk)
            {
                response = "Age of Youngest Driver: " + youngestDriver.getName();
                return response;
            }

            //check that the oldest driver is not over 75 at the claim start date.
            oldestAgeOk = oldestUnder76(oldestDriver, theStartDate);
            if (!oldestAgeOk)
            {
                response = "Age of Oldest Driver: " + oldestDriver.getName();
                return response;
            }

            //amend the premium based on the drivers' ages.
            premium = ageAmend(youngestDriver, premium, theStartDate);

            //check if there are too many claims
            claimsResponse = checkClaims(theDrivers);
            if (claimsResponse.Length > 0)
            {
                response = claimsResponse;
                return response;
            }

            //amend the premium based on the existing claims.
            premium = claimsAmend(theDrivers, theStartDate, premium);

            //format the premium
            premium = Math.Truncate(premium * 100) / 100;
            premiumString = string.Format("{0:N2}", premium);

            response = "Premium is £" + premiumString;
            return response;
        }

        static Boolean checkStartDate(DateTime theStartDate)
        {
            DateTime amendedDate;
            Boolean dateOk = true;

            //add a day to the start date to ensure that a start date of today is not flagged.
            amendedDate = theStartDate.AddDays(1);
            if (amendedDate < DateTime.Now)
            {
                dateOk = false;
            }
            return dateOk;
        }

        static Double occupationAmend(List<Driver> drivers, Double thePremium)
        {
            Boolean chauffeurFound = false;
            Boolean accountantFound = false;
            Double amendedPremium = thePremium;

            foreach (Driver aDriver in drivers)
            {
                String driverOccupation = aDriver.getOccupation();
                if (driverOccupation == "Chauffeur")
                {
                    chauffeurFound = true;
                }
                if (driverOccupation == "Accountant")
                {
                    accountantFound = true;
                }
            }
            if (chauffeurFound)
            {
                amendedPremium += (amendedPremium / 100) * 10;
            }
            if (accountantFound)
            {
                amendedPremium -= (amendedPremium / 10);
            }
            return amendedPremium;
        }

        static Boolean youngestOver21(Driver youngestDriver, DateTime theStartDate)
        {
            DateTime youngestDateOfBirth;
            TimeSpan youngestTimeSpan;
            Double ageAtClaimDate;
            Boolean ageOk = true;

            youngestDateOfBirth = youngestDriver.getDateOfBirth();
            youngestTimeSpan = theStartDate - youngestDateOfBirth;
            ageAtClaimDate = (Convert.ToDouble(youngestTimeSpan.Days) / 365.25);
            if (ageAtClaimDate < 21) //if the youngest driver will be under 21 at the claim start date
            {
                ageOk = false;
            }
            return ageOk;
        }

        static Boolean oldestUnder76(Driver oldestDriver, DateTime theStartDate)
        {
            DateTime oldestDateOfBirth;
            TimeSpan oldestTimeSpan;
            Double ageAtClaimDate;
            Boolean ageOk = true;

            oldestDateOfBirth = oldestDriver.getDateOfBirth();
            oldestTimeSpan = theStartDate - oldestDateOfBirth;
            ageAtClaimDate = (Convert.ToDouble(oldestTimeSpan.Days) / 365.25);
            if (ageAtClaimDate >= 76) //if the oldest driver will be 76 or older at the claim start date
            {
                ageOk = false;
            }
            return ageOk;
        }

        static Double ageAmend(Driver youngestDriver, Double thePremium, DateTime theStartDate)
        {
            Double amendedPremium = thePremium;
            DateTime youngestDateOfBirth;
            TimeSpan youngestTimeSpan;
            Double ageAtClaimDate;

            youngestDateOfBirth = youngestDriver.getDateOfBirth();
            youngestTimeSpan = theStartDate - youngestDateOfBirth;
            ageAtClaimDate = (Convert.ToDouble(youngestTimeSpan.Days) / 365.25);
            if (ageAtClaimDate >= 21 && ageAtClaimDate <= 25)
            {
                amendedPremium += (thePremium / 100) * 20;
            }
            else if (ageAtClaimDate >= 26 && ageAtClaimDate <= 75)
            {
                amendedPremium -= (thePremium / 10);
            }
            return amendedPremium;
        }

        static String checkClaims(List<Driver> drivers)
        {
            String response = "";
            String driverName = "";
            List<DateTime> driverClaims;
            int totalClaims = 0;
            Boolean driverClaimsOk = true;
            Boolean totalClaimsOk = true;

            foreach (Driver driver in drivers)
            {
                driverClaims = driver.getClaims();
                totalClaims += driverClaims.Count;
                if (driverClaims.Count > 2)
                {
                    driverClaimsOk = false;
                    driverName = driver.getName();
                }
                if (totalClaims > 3)
                {
                    totalClaimsOk = false;
                }
            }
            if (!driverClaimsOk)
            {
                response = "Driver has more than 2 claims: " + driverName;
            }
            else if (!totalClaimsOk)
            {
                response = "Policy has more than 3 claims";
            }
            return response;
        }

        static Double claimsAmend(List<Driver> drivers, DateTime theStartDate, Double aPremium)
        {
            Double amendedPremium = aPremium;
            List<DateTime> claims;
            TimeSpan claimTimeSpan;

            foreach (Driver driver in drivers)
            {
                claims = driver.getClaims();
                foreach (DateTime claimDate in claims)
                {
                    claimTimeSpan = theStartDate - claimDate;
                    if (claimTimeSpan.Days < 365)
                    {
                        amendedPremium += (amendedPremium / 100) * 20;
                    }
                    else if (claimTimeSpan.Days > 365 && claimTimeSpan.Days < 1825)
                    {
                        amendedPremium += (amendedPremium / 100) * 10;
                    }
                }
            }
            return amendedPremium;
        }
    }

    public class Driver
    {
        private String name;
        private String occupation { get; set; }
        private DateTime dateOfBirth { get; set; }
        private List<DateTime> claims = new List<DateTime>();

        public Driver(String aName, String anOccupation, DateTime dob)
        {
            name = aName;
            occupation = anOccupation;
            dateOfBirth = dob;
        }

        public String getName()
        {
            return name;
        }

        public String getOccupation()
        {
            return occupation;
        }

        public DateTime getDateOfBirth()
        {
            return dateOfBirth;
        }

        public List<DateTime> getClaims()
        {
            return claims;
        }

        public void addClaim(DateTime doc)
        {
            claims.Add(doc);
        }
    }
}