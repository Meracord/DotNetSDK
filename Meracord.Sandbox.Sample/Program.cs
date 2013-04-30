using System;
using Meracord.Sandbox.Example;
using NoteWorld.DataServices.Common.Transport;

namespace Meracord.Sandbox
{
    internal class Program
    {
        private static string ClientId { get; set; }
        private static BankProfile BankProfile { get; set; }

        static void Main(string[] args)
        {
            // Save a reference to the account just created
            ClientId = AccountMethods.Perform();
            // Save a reference to the BankProfile just created
            BankProfile = BankProfileMethods.Perform(ClientId);

            DebitMethods.Perform(ClientId, BankProfile);
            PaymentCardMethods.Perform(ClientId);
            SettlementMethods.Perform(ClientId);
            TransferMethods.Perform(ClientId);
            CreditorMethods.Perform();
            ReportingService.Perform(ClientId);
            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}