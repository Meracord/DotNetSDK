using System;
using Meracord.Sandbox.Factories;
using Meracord.Sandbox.Helpers;
using NoteWorld.DataServices;
using System.Linq;

namespace Meracord.Sandbox.Example
{
    /// <summary>
    /// Sample method calls using the API DataSession.Account object
    /// </summary>
    internal class TransferMethods
    {
        private static DataSession _session;

        /// <summary>
        /// Execute sample method calls
        /// </summary>
        public static void Perform(string clientId)
        {
            try
            {
                var groupNumber = Settings.GroupNumber;

                _session = SessionFactory.Create();

                var consumerAccountNumber = FindAccount(groupNumber, clientId);

                var sourceAccountNumber = GetAdministrativeAccount();

                // Identifying accounts for tranfers is different than most API calls.
                // The source or destination account may be of a type that cannot be identified by a ClientID.
                // So we use the Meracord account number for transfers.
                CreateTransfer(sourceAccountNumber, consumerAccountNumber);

            }
            catch (Exception ex)
            {
                Helper.DisplayException(ex);
            }
        }

        /// <summary>
        /// Execute Account.Find() method
        /// </summary>
        private static string FindAccount(string groupNumber, string clientId)
        {
            var result = _session.Account.Find(groupNumber, clientId);

            Helper.ShowResults("Account.Find()", result);

            if (result != null)
            {
                return result.AccountNumber;
            }

            return string.Empty;
        }

        /// <summary>
        /// Execute Transfer.Create() method
        /// </summary>
        private static string CreateTransfer(string sourceAccountNumber, string destinationAccountNumber)
        {
            var result = _session.Transfer.Create(sourceAccountNumber, destinationAccountNumber, 10.0, DateTime.Today);

            Helper.ShowResults("Transfer.Create()", result);

            if (result.Success)
            {
                return result.AccountNumber;
            }

            return string.Empty;
        }

        /// <summary>
        /// Execute Transfer.GetAdministrativeAccounts() method
        /// </summary>
        private static string GetAdministrativeAccount()
        {
            // Get a list of Administrative Accounts
            var result = _session.Transfer.GetAllAdministrativeAccounts();

            Helper.ShowResults("Transfer.GetAdministrativeAccounts()", result);

            var account = result.FirstOrDefault();

            if (account != null)
            {
                // Return the first Administrative AccountNumber
                return account.AccountNumber;
            }

            // None found, so return empty string
            return string.Empty;
        }


    }
}