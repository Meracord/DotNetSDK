using System;
using Meracord.Sandbox.Factories;
using Meracord.Sandbox.Helpers;
using NoteWorld.DataServices;
using NoteWorld.DataServices.Common.Factories;
using NoteWorld.DataServices.Common.Transport;

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
                var newClientId = Helper.RandomAlphaNumericString(15);

                _session = SessionFactory.Create();

                var transportAccount = GetTransportAccount(groupNumber, newClientId);

                AccountCreate(transportAccount);

                AccountPlaceHold(transportAccount);

                AccountReleaseHold(transportAccount);

                AccountEdit(transportAccount);

                AccountEditError(transportAccount);

                AccountRead(groupNumber, newClientId);

                return newClientId;
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
        private static void AccountPlaceHold(Account transportAccount)
        {
            Helper.ShowResults("Account.PlaceHold()",
                _session.Account.PlaceHold(transportAccount.GroupNumber, transportAccount.ClientId)
                );
        }

        /// <summary>
        /// Execute Account.ReleaseHold() method
        /// </summary>
        private static void AccountReleaseHold(Account transportAccount)
        {
            Helper.ShowResults("Account.ReleaseHold()",
                               _session.Account.ReleaseHold(transportAccount.GroupNumber, transportAccount.ClientId)
                );
        }

        /// <summary>
        /// Execute Account.Create() method
        /// </summary>
        private static void AccountCreate(Account transportAccount)
        {
            Helper.ShowResults("Account.Create()",
                _session.Account.Create(transportAccount)
                );
        }

        /// <summary>
        /// Execute Account.Edit() method
        /// </summary>
        private static void AccountEdit(Account transportAccount)
        {
            //Modify the First name
            transportAccount.NameFirst = transportAccount.NameFirst + ".";

            Helper.ShowResults("Account.Edit()",
                _session.Account.Edit(transportAccount)
                );
        }

        /// <summary>
        /// Execute Account.Edit() method
        /// </summary>
        private static void AccountEditError(Account transportAccount)
        {
            // Modify the ClientId, make it Invalid
            transportAccount.ClientId = transportAccount.ClientId.Substring(3);

            Helper.ShowResults("Account.Edit() - cause exception",
                _session.Account.Edit(transportAccount)
                );
        }

        /// <summary>
        /// Execute Account.Find() method
        /// </summary>
        private static void AccountRead(string groupNumber, string newClientId)
        {
            Helper.ShowResults("Account.Find()",
                _session.Account.Find(groupNumber, newClientId)
                );
        }

        /// <summary>
        /// Create an Account object
        /// </summary>
        public static Account GetTransportAccount(string groupNumber, string newClientId)
        {
            return AccountFactory.Create(
                groupNumber, newClientId, 0, 3, "999-99-1234", "Calvin", "Consumer", null, 
                "1001 Pacific Ave, Ste 300", "Tacoma", "WA", "98092", 
                "calvinconsumer@Meracord.com", "999-999-9999"
                );
        }

    }
}