using System;
using Meracord.Sandbox.Factories;
using Meracord.Sandbox.Helpers;
using NoteWorld.DataServices.Common.Factories;
using Transport = NoteWorld.DataServices.Common.Transport;
using NoteWorld.DataServices.Common.Enumeration;

namespace Meracord.Sandbox.Example
{
    /// <summary>
    /// Sample method calls using the API DataSession.BankProfile object
    /// </summary>
    internal class BankProfileMethods
    {
        /// <summary>
        /// Execute sample method calls
        /// </summary>
        public static Transport.BankProfile Perform(string clientId)
        {
            try
            {
                var documentPath = Settings.DocumentPath;
                var groupNumber = Settings.GroupNumber;
                var bankProfile = GetBankProfile(Helper.RandomNumericString(17));
                var document = DocumentFactory.Create(documentPath, DocumentType.AchAuthorization);

                var session = SessionFactory.Create();

                // Call BankProfile.Create()
                Helper.ShowResults("BankProfile.Create()", session.BankProfile.Create(groupNumber, clientId, bankProfile, document));

                // Call BankProfile.Exists()
                Helper.ShowResults("BankProfile.Exists()", session.BankProfile.Exists(groupNumber, clientId, bankProfile));

                return bankProfile;
            }
            catch (Exception ex)
            {
                Helper.DisplayException(ex);
            }

            return null;
        }

        /// <summary>
        /// Helper method to generate BankProfile object
        /// </summary>
        private static Transport.BankProfile GetBankProfile(string accountNumber)
        {
            return Transport.BankProfile.Create(
                (int) BankAccountType.Checking, 
                "BankProfile Test",
                Helpers.BankRoutingNumber.BankOfAmerica, 
                accountNumber
                );
        }

    }
}