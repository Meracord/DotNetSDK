using System;
using Meracord.API;
using Meracord.Sandbox.Factories;
using Meracord.Sandbox.Helpers;
using Meracord.API.Common.Factories;
using Meracord.API.Common.Transport;

namespace Meracord.Sandbox.Example
{
    /// <summary>
    /// Sample method calls using the API DataSession.Account object
    /// </summary>
    internal class AccountMethods
    {
        private static DataSession _session;

        /// <summary>
        /// Execute sample method calls
        /// </summary>
        public static string Perform()
        {
            try
            {
                var groupNumber = Settings.GroupNumber;
                var newCustomerId = Helper.RandomAlphaNumericString(15);

                _session = SessionFactory.Create();


                var transportAccount = GetTransportAccount(groupNumber, newCustomerId);

                var newAccountNumber = AccountCreate(transportAccount);

                AccountPlaceHold(newAccountNumber);

                AccountReleaseHold(newAccountNumber);

                AccountEdit(transportAccount, newAccountNumber);

                AccountEditError(transportAccount);

                AccountFindByCustomerId(groupNumber, newCustomerId);

                return newAccountNumber;
            }
            catch (Exception ex)
            {
                Helper.DisplayException(ex);
            }

            return null;
        }

        /// <summary>
        /// Execute Account.PlaceHold() method
        /// </summary>
        private static void AccountPlaceHold(string accountNumber)
        {
            Helper.ShowResults("Account.PlaceHold()",
                _session.Account.PlaceHold(accountNumber)
                );
        }

        /// <summary>
        /// Execute Account.ReleaseHold() method
        /// </summary>
        private static void AccountReleaseHold(string accountNumber)
        {
            Helper.ShowResults("Account.ReleaseHold()",
                               _session.Account.ReleaseHold(accountNumber)
                );
        }

        /// <summary>
        /// Execute Account.Create() method
        /// </summary>
        private static string AccountCreate(Account transportAccount)
        {
            var response = _session.Account.Create(transportAccount);

            Helper.ShowResults("Account.Create()", response);

            return response.AccountNumber;
        }

        /// <summary>
        /// Execute Account.Edit() method
        /// </summary>
        private static void AccountEdit(Account transportAccount, string accountNumber)
        {
            var accountEdit = new AccountEdit
            {
                AccountNumber = accountNumber,
                Business = transportAccount.Business,
                Individual = transportAccount.Individual
            };

            //Modify the First name
            transportAccount.Individual.NameFirst = transportAccount.Individual.NameFirst + ".";

            Helper.ShowResults("Account.Edit()",
                _session.Account.Edit(accountEdit)
                );
        }

        /// <summary>
        /// Execute Account.Edit() method
        /// </summary>
        private static void AccountEditError(Account transportAccount)
        {
            var accountEdit = new AccountEdit
            {
                AccountNumber = "xxxxxxxxxxxx",
                Business = transportAccount.Business,
                Individual = transportAccount.Individual
            };


            Helper.ShowResults("Account.Edit() - cause exception",
                _session.Account.Edit(accountEdit)
                );
        }

        /// <summary>
        /// Execute Account.FindByCustomerId() method
        /// </summary>
        private static void AccountFindByCustomerId(string groupNumber, string newCustomerId)
        {
            Helper.ShowResults("Account.FindByCustomerId()",
                _session.Account.FindByCustomerId(groupNumber, newCustomerId)
                );
        }

        /// <summary>
        /// Create an Account object
        /// </summary>
        public static Account GetTransportAccount(string groupNumber, string newCustomerId)
        {
            return AccountFactory.Create(
                groupNumber, newCustomerId, null, "999-99-1234", "Calvin", "Consumer", null, 
                "1001 Pacific Ave, Ste 300", "Tacoma", "WA", "98092", 
                "calvinconsumer@Meracord.com", "999-999-9999"
                );
        }

        /// <summary>
        /// Create an Account object
        /// </summary>
        public static Account GetTransportBusinessAccount(string groupNumber, string newCustomerId)
        {
            return AccountFactory.CreateBusiness(
                groupNumber, newCustomerId, "99-9999999", "My Business Name",
                "1001 Pacific Ave, Ste 300", "Tacoma", "WA", "98092",
                "accountservices@mybusiness.com", "999-999-9999"
                );
        }

    }
}