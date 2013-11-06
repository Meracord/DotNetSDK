using System;
using Meracord.Sandbox.Example;

namespace Meracord.Sandbox
{
    internal class Program
    {
        private static string AccountNumber { get; set; }
        private static Guid PaymentProfileToken { get; set; }

        static void Main(string[] args)
        {
            // Save a reference to the account just created
            AccountNumber = AccountMethods.Perform();

            // Save a reference to the BankProfile just created
            PaymentProfileToken = BankProfileMethods.Perform(AccountNumber);

            PaymentMethods.Perform(AccountNumber, PaymentProfileToken);
            TransferMethods.Perform(AccountNumber);
            PayeeMethods.Perform();
            ReportingService.Perform(AccountNumber);
            Console.WriteLine("");
            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}