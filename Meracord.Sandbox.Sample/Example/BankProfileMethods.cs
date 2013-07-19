using System;
using Meracord.Sandbox.Factories;
using Meracord.Sandbox.Helpers;
using Meracord.API.Common.Enumeration;
using Meracord.API.Common.Factories;
using Meracord.API.Common.Transport;

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
        public static BankProfile Perform(string customerId)
        {
            try
            {
                var documentPath = Settings.DocumentPath;
                var groupNumber = Settings.GroupNumber;
                var bankProfile = GetBankProfile(Helper.RandomNumericString(17));
                var document = DocumentFactory.Create(documentPath, DocumentType.AchAuthorization);

                var session = SessionFactory.Create();

                // Call BankProfile.Create()
                Helper.ShowResults("BankProfile.Create()", session.BankProfile.Create(groupNumber, customerId, bankProfile, document));

                // Call BankProfile.Exists()
                Helper.ShowResults("BankProfile.Exists()", session.BankProfile.Exists(groupNumber, customerId, bankProfile));

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
        private static BankProfile GetBankProfile(string accountNumber)
        {
            return BankProfile.Create(
                (int) BankAccountType.Checking, 
                "BankProfile Test",
                Helpers.BankRoutingNumber.BankOfAmerica, 
                accountNumber,
                false
                );
        }

    }
}