using System;
using Meracord.Sandbox.Factories;
using Meracord.Sandbox.Helpers;
using Meracord.API;
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
        public static void Perform(string accountNumber)
        {
            try
            {
                _session = SessionFactory.Create();

                var sourceAccountNumber = GetAdministrativeAccount();

                CreateTransfer(sourceAccountNumber, accountNumber);

            }
            catch (Exception ex)
            {
                Helper.DisplayException(ex);
            }
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