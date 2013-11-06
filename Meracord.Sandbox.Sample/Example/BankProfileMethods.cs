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
        public static Guid Perform(string accountNumber)
        {
            try
            {
                var groupNumber = Settings.GroupNumber;
                var bankProfile = GetBankProfile(Helper.RandomNumericString(17));

                var session = SessionFactory.Create();

                //Call BankProfile.Create()
                var bankProfileResult = session.BankProfile.Create(accountNumber, bankProfile);

                Helper.ShowResults("BankProfile.Create()",
                    bankProfileResult
                    );

                Helper.ShowResults("BankProfile.Find()",
                    session.BankProfile.Find(accountNumber)
                    );

                // Return the PaymentProfileToken generated in BankProfile.Create()
                return bankProfileResult.PaymentProfileToken;
            }
            catch (Exception ex)
            {
                Helper.DisplayException(ex);
            }

            return Guid.Empty;
        }

        /// <summary>
        /// Helper method to generate BankProfile object
        /// </summary>
        public static BankProfile GetBankProfile(string accountNumber)
        {
            var documentPath = Settings.DocumentPath;
            var document = DocumentFactory.Create(documentPath, DocumentType.AchAuthorization);

            return BankProfile.Create(
                (int) BankAccountType.Checking, 
                "BankProfile Test",
                Helpers.BankRoutingNumber.BankOfAmerica, 
                accountNumber,
                false,
                document
                );
        }

    }
}