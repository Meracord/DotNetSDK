﻿using System;
using Meracord.Sandbox.Factories;
using Meracord.Sandbox.Helpers;
using Meracord.API;
using Meracord.API.Common.Enumeration;
using Meracord.API.Common.Factories;
using Meracord.API.Common.Transport;
using System.Linq;

namespace Meracord.Sandbox.Example
{
    /// <summary>
    /// Sample method calls using the API DataSession.Account object
    /// </summary>
    internal class PayeeMethods
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
                var referenceId = Helper.RandomAlphaNumericString(15);

                _session = SessionFactory.Create();

                var transportPayee = GetTransportPayee(groupNumber, referenceId);

                var result = _session.Payee.Create(transportPayee);
                Helper.ShowResults("Payee.Create()", result);

                var documentPath = Settings.DocumentPath;
                var document = DocumentFactory.Create(documentPath, DocumentType.PayeeAccountAuthorizationAgreement);
                Helper.ShowResults("Payee.CreateDocument()", CreateDocument(_session, groupNumber, referenceId, document));

                var transportPayeeContact = GetTransportPayeeContact(groupNumber, referenceId, transportPayee);
                result = _session.Payee.EditContact(transportPayeeContact);
                Helper.ShowResults("Payee.EditContact()", result);
            }
            catch (Exception ex)
            {
                Helper.DisplayException(ex);
            }

            return null;
        }

        private static DocumentResult CreateDocument(DataSession session, string groupNumber, string referenceId, Document document)
        {
            Console.WriteLine("Going to sleep for 5 seconds...");
            Console.WriteLine("The Payee.Create method performs an asynchronous operation with an ancillary system,\r\nso give the process a moment to complete before attempting to\r\nassociate an PayeeAccountAuthorizationAgreement");
            Console.WriteLine();
            System.Threading.Thread.Sleep(5000); // wait 5 seconds

            var tries = 4;
            while (tries > 0)
            {
                var response = session.Payee.AddDocument(groupNumber, referenceId, document);
                if (response.Success)
                {
                    return response;
                }
                var exception = response.Exceptions.FirstOrDefault();

                if (exception.ExceptionType != DataServiceExceptionType.AccountCustomerIdNotFound)
                {
                    throw new ApplicationException(exception.Message);
                }

                Console.WriteLine("Associated Payee Account has not been created yet.  Retry in 10 seconds...");
                System.Threading.Thread.Sleep(10000); // wait 10 seconds
                tries--;
            }

            throw new ApplicationException("Timeout reached waiting for Payee Account to be created.");
        }


        public static PayeeAccount GetTransportPayee(string groupNumber, string referenceId)
        {
            var payeeAccount = PayeeAccountFactory.Create(groupNumber, referenceId, "999-99-1234", "Big Business Company", "1001 Pacific Ave, Ste 300", "Tacoma", "WA", "98092", "help@mybusiness.com", "253-355-2323");
            payeeAccount.DisbursementMethod = (int) DisbursementMethod.Ach;
            payeeAccount.BankAccount = new BankProfile
            {
                AccountName = "My Business Name",
                AccountNumber = "9999999999",
                RoutingNumber = BankRoutingNumber.BankOfAmerica,
                AccountType = (int) BankAccountType.Checking
            };
            return payeeAccount;
        }

        public static PayeeContact GetTransportPayeeContact(string groupNumber, string referenceId, PayeeAccount payeeAccount)
        {
            var contact = new PayeeContact
            {
                GroupNumber = groupNumber,
                CustomerId = referenceId,
                Business = payeeAccount.Business,
                MailingAddress = payeeAccount.MailingAddress,
            };

            contact.Business.Name = "My Company";

            return contact;
        }

    }
}