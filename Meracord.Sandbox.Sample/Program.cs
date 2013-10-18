using System;
using Meracord.Sandbox.Example;

namespace Meracord.Sandbox
{
    internal class Program
    {
        private static string CustomerId { get; set; }
        private static Guid PaymentProfileToken { get; set; }

        static void Main(string[] args)
        {
            // Save a reference to the account just created
            CustomerId = AccountMethods.Perform();

            // Save a reference to the BankProfile just created
            PaymentProfileToken = BankProfileMethods.Perform(CustomerId);

            PaymentMethods.Perform(CustomerId, PaymentProfileToken);
            TransferMethods.Perform(CustomerId);
            PayeeMethods.Perform();
            ReportingService.Perform(CustomerId);
            Console.WriteLine("");
            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}