using System;
using Meracord.API.Common.Transport;
using Meracord.Sandbox.Example;

namespace Meracord.Sandbox
{
    internal class Program
    {
        private static string CustomerId { get; set; }
        private static BankProfile BankProfile { get; set; }

        static void Main(string[] args)
        {
            // Save a reference to the account just created
            CustomerId = AccountMethods.Perform();

            // Save a reference to the BankProfile just created
            BankProfile = BankProfileMethods.Perform(CustomerId);

            DebitMethods.Perform(CustomerId, BankProfile);
            TransferMethods.Perform(CustomerId);
            PayeeMethods.Perform();
            ReportingService.Perform(CustomerId);
            Console.WriteLine("");
            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}