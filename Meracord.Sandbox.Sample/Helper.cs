using System;
using System.Linq;
using System.Collections.Generic;
using Meracord.API.Common.Enumeration;
using Meracord.API.Common.Transport;
using Meracord.Sandbox.Custom;
using Meracord.API;
using Meracord.API.Reporting;
using AccountStatus = Meracord.API.Reporting.AccountStatus;
using PaymentSummary = Meracord.API.Reporting.PaymentSummary;
using Transfer = Meracord.API.Transfer;

namespace Meracord.Sandbox
{
    internal static class Helper
    {
        /// <summary>
        /// In development mode SSL certificates are generaly not trusted.
        /// This call allows us to process with an untrusted SSL certificate on the web server.
        /// This should never be enabled in production code.
        /// </summary>
        public static void OverrideCertificateHandling() {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = (obj, certificate, chain, errors) => true;
        }

        /// <summary>
        /// Generate a random aplha numeric string
        /// </summary>
        public static string RandomAlphaNumericString(int length) {
            var value = Guid.NewGuid().ToString().Replace("-", "");
            return value.Substring(value.Length - length, length);
        }

        /// <summary>
        /// Generate a random numeric string
        /// </summary>
        public static string RandomNumericString(int length) {
            string value = Guid.NewGuid().ToString() + Guid.NewGuid().ToString();
            value = System.Text.RegularExpressions.Regex.Replace(value, @"[^\d]", "").Replace("-", "");
            return value.Substring(0, length);
        }

        public static void ShowResults(string title, DateTime nextDate) {
            ShowHeader(title);
            Console.WriteLine("NextValidDebitDate: {0}", nextDate.ToShortDateString());
        }

        public static void ShowResults(string title, string message) {
            ShowHeader(title);
            Console.WriteLine(message);
        }

        public static void ShowHeader(string title) {
            string displayHeader = "".PadRight(70, char.Parse("="));
            Console.WriteLine();
            Console.WriteLine(displayHeader);
            Console.WriteLine("== {0}", title);
            Console.WriteLine(displayHeader);
        }

        public static void ShowHeader(string title, int rows) {
            string displayHeader = "".PadRight(70, char.Parse("="));
            Console.WriteLine();
            Console.WriteLine(displayHeader);
            Console.WriteLine("== {0}  Rows: {1}", title, rows);
            Console.WriteLine(displayHeader);
        }

        public static void ShowResults(string title, PaymentCancelResult results) {
            ShowHeader(title);
            foreach (var result in results.Payments) {
                Console.WriteLine("Payment: {0} Ref#: {1} Status: {2} Desc: {3}", result.PaymentToken,
                    result.Payment.PaymentReferenceId, result.PaymentStatus.Id, result.PaymentStatus.Description);


            }
            Console.WriteLine();
        }

        public static void ShowResults(string title, PaymentScheduleResult result) {
            ShowHeader(title);
            Console.WriteLine("SessionId     {0}", result.SessionId);
            Console.WriteLine("SessionDate   {0}", result.SessionDate);
            Console.WriteLine("AccountNumber {0}", result.AccountNumber);
            Console.WriteLine("CustomerId    {0}", result.CustomerId);
            Console.WriteLine("IsSuccessful  {0}", result.Success);
            Console.WriteLine("Payment:      {0}{1} {2}", result.Payment.PaymentDate.ToString("yyyy-MM-dd"), result.Payment.PaymentAmount, result.PaymentToken);

            if (result.Exceptions != null) {
                Console.WriteLine();
                foreach (ExceptionMessage ex in result.Exceptions) {
                    Console.WriteLine("    Exception: [{0}] - {1}, {2}", ((int)ex.ExceptionType), ex.ExceptionType, ex.Message);
                }
            }

            Console.WriteLine();
        }

        public static void ShowResults(string title, DebitResult result) {
            ShowHeader(title);
            Console.WriteLine("SessionId     {0}", result.SessionId);
            Console.WriteLine("SessionDate   {0}", result.SessionDate);
            Console.WriteLine("AccountNumber {0}", result.AccountNumber);
            Console.WriteLine("CustomerId    {0}", result.CustomerId);
            Console.WriteLine("IsSuccessful  {0}", result.Success);
            foreach (var debit in result.Debits) {
                Console.WriteLine("Debit:        {0}{1} {2} {3}", debit.DebitId.ToString().PadRight(10), debit.DebitDate.ToString("yyyy-MM-dd"), debit.DebitAmount, debit.PaymentProfileToken);
            }

            if (result.Exceptions != null) {
                Console.WriteLine();
                foreach (ExceptionMessage ex in result.Exceptions) {
                    Console.WriteLine("    Exception: [{0}] - {1}, {2}", ((int)ex.ExceptionType), ex.ExceptionType, ex.Message);
                }
            }

            Console.WriteLine();
        }

        public static void ShowResults(string title, API.Account.AccountDetails result) {
            ShowHeader(title);
            Console.WriteLine("AccountNumber {0}", result.AccountNumber);
            Console.WriteLine("DateSetup     {0}", result.DateSetup);
        }

        public static void ShowResults(string title, AccountResult result) {
            ShowHeader(title);
            Console.WriteLine("SessionId     {0}", result.SessionId);
            Console.WriteLine("SessionDate   {0}", result.SessionDate);
            Console.WriteLine("AccountNumber {0}", result.AccountNumber);
            Console.WriteLine("CustomerId   {0}", result.CustomerId);
            Console.WriteLine("HoldType      {0}", result.HoldType);
            Console.WriteLine("IsSuccessful  {0}", result.Success);

            if (result.Exceptions != null)
                foreach (ExceptionMessage ex in result.Exceptions) {
                    Console.WriteLine("    Exception: [{0}] - {1}, {2}", ((int)ex.ExceptionType), ex.ExceptionType, ex.Message);
                }
            Console.WriteLine();
        }

        public static void ShowResults(string title, PayeeAccountResult result) {
            ShowHeader(title);
            Console.WriteLine("SessionId     {0}", result.SessionId);
            Console.WriteLine("SessionDate   {0}", result.SessionDate);
            Console.WriteLine("AccountNumber {0}", result.AccountNumber);
            Console.WriteLine("CustomerId   {0}", result.CustomerId);
            Console.WriteLine("HoldType      {0}", result.HoldType);
            Console.WriteLine("AccountStatus {0}", result.AccountStatus);
            Console.WriteLine("IsSuccessful  {0}", result.Success);
            Console.WriteLine();
            if (result.BusinessAddress != null) {
                Console.WriteLine("Business Address Success: {0}", result.BusinessAddress.Success);
                Console.WriteLine("    Standarized Address:");
                Console.WriteLine();
                Console.WriteLine("    {0}", result.BusinessAddress.Address.Street);
                Console.WriteLine("    {0}, {1} {2}", result.BusinessAddress.Address.City, result.BusinessAddress.Address.State, result.BusinessAddress.Address.PostalCode);
                Console.WriteLine();
                Console.WriteLine("    Standarization Response:");
                foreach (string msg in result.BusinessAddress.Response) {
                    Console.WriteLine("        Business Address Validation: {0}", msg);
                }
                Console.WriteLine();
            }

            if (result.MailingAddress != null) {
                Console.WriteLine("Mailing Address Success:  {0}", result.MailingAddress.Success);
                foreach (string msg in result.MailingAddress.Response) {
                    Console.WriteLine("    Mailing Address  Validation: {0}", msg);
                }
                Console.WriteLine();
            }

            if (result.Exceptions != null)
                foreach (ExceptionMessage ex in result.Exceptions) {
                    Console.WriteLine("    Exception: [{0}] - {1}, {2}", ((int)ex.ExceptionType), ex.ExceptionType, ex.Message);
                }
            Console.WriteLine();
        }


        public static void ShowResults(string title, DocumentResult result) {
            ShowHeader(title);
            Console.WriteLine("SessionId     {0}", result.SessionId);
            Console.WriteLine("SessionDate   {0}", result.SessionDate);
            Console.WriteLine("AccountNumber {0}", result.AccountNumber);
            Console.WriteLine("CustomerId   {0}", result.CustomerId);
            Console.WriteLine("IsSuccessful  {0}", result.Success);

            if (result.Exceptions != null)
                foreach (ExceptionMessage ex in result.Exceptions) {
                    Console.WriteLine("    Exception: [{0}] - {1}, {2}", ((int)ex.ExceptionType), ex.ExceptionType, ex.Message);
                }
            Console.WriteLine();
        }


        public static void ShowResults(string title, ControlGroup[] groups) {
            ShowHeader(title, groups.Count());
            foreach (ControlGroup group in groups) {
                Console.WriteLine("{0} {1}", group.GroupNumber, group.GroupDescription);
                Console.WriteLine();
                return; //Only show one row
            }
        }


        public static void ShowResults(string title, API.Reporting.AccountDetails[] accounts) {
            ShowHeader(title, accounts.Count());
            foreach (API.Reporting.AccountDetails account in accounts) {
                Console.WriteLine("{0} {1} {2}", account.AccountNumber, account.CustomerId.PadRight(12), account.DateSetup.ToShortDateString());
                Console.WriteLine();
                return; //Only show one row
            }
        }

        public static void DisplayException(string title, Exception ex) {
            ShowHeader(title);
            DisplayException(ex);
            Console.WriteLine();
        }

        public static void DisplayException(Exception ex) {
            Console.WriteLine();
            Console.WriteLine(ex.Message);
            if (ex.InnerException != null) {
                Console.WriteLine();
                Console.WriteLine(ex.InnerException.Message);
            }
        }

        public static void ShowResults(string title, AccountStatus[] accounts) {
            ShowHeader(title, accounts.Count());
            foreach (AccountStatus account in accounts) {
                Console.WriteLine("{0} {1} {2}", account.AccountNumber, account.CustomerId.PadRight(12), account.ChangeDate.ToShortDateString());
                Console.WriteLine();
                return; //Only show one row
            }
        }

        public static void ShowResults(string title, PaymentSummary[] payments) {
            ShowHeader(title, payments.Count());
            foreach (PaymentSummary payment in payments)
            {
                Console.WriteLine("{0} {1} {2}", payment.AccountNumber, payment.CustomerId.PadRight(12), payment.ChangeDate.ToShortDateString());
                Console.WriteLine();
                return; //Only show one row
            }
        }

        public static void ShowResults(string title, PaymentScheduleDetails result) {
            ShowHeader(title);
            if (result != null)
            {
                if (result.Payment != null)
                {
                    Console.WriteLine("{0} {1} {2}", result.Payment.AccountNumber, result.Payment.CustomerId.PadRight(12), result.Payment.DateModified.ToShortDateString());
                }
                else
                {
                    Console.WriteLine("{0} {1}", result.PaymentToken, result.PaymentStatus);
                }
            }
            Console.WriteLine();
        }

        public static void ShowResults(string title, MyAccount[] accounts) {
            ShowHeader(title, accounts.Count());
            foreach (MyAccount account in accounts) {
                Console.WriteLine("{0} {1} {2}", account.AccountNumber.PadRight(16),
                    account.ReserveBalance.ToString("#,##0.00").PadLeft(12),
                    account.ChangeDate.ToShortDateString());
                Console.WriteLine();
                return; //Only show one row
            }
        }

        public static void ShowResults(string title, TransactionHistory[] transactions) {
            var count = 0;
            ShowHeader(title, transactions.Count());
            foreach (var trans in transactions) {
                Console.WriteLine(
                    "{0} {1} {2} {3} {4}",
                    trans.TransactionId.ToString().PadRight(8),
                    trans.SourceAccountNumber.PadRight(15),
                    trans.TransactionType.PadRight(25),
                    trans.AllocationType.PadRight(5),
                    trans.Amount.ToString("C")
                );

                count += 1;
                if (count > 5) {
                    Console.WriteLine();
                    return;
                }
            }
            Console.WriteLine();
        }

        public static void ShowResults(string title, Transfers[] transactions) {
            var count = 0;
            ShowHeader(title, transactions.Count());
            foreach (var trans in transactions) {
                Console.WriteLine(
                    "{0} {1} {2} {3}",
                    trans.SourceAccountNumber.PadRight(16),
                    trans.DestinationAccountNumber.PadRight(16),
                    trans.TransactionTypeId,
                    trans.Amount.ToString("C")
                );

                count += 1;
                if (count > 5) {
                    Console.WriteLine();
                    return;
                }
            }
            Console.WriteLine();
        }

        public static void ShowResults(string title, ReportingSession session) {
            ShowHeader(title);
            Console.WriteLine(session.DataQuery);
            Console.WriteLine();
        }

        public static void ShowResults(string title, BankProfileReference[] profileReferences) {
            ShowHeader(title, profileReferences.Count());
            var i = 0;
            foreach (var profile in profileReferences) {
                i++; if (i > 4) continue;
                Console.WriteLine("{0} {1} {2} {3}", profile.PaymentProfileToken, profile.RoutingNumber, profile.AccountNumberMask, profile.AccountName);
            }
            Console.WriteLine();
        }

        public static void ShowResults(string title, BankProfileResult result) {
            ShowHeader(title);
            Console.WriteLine("SessionId     {0}", result.SessionId);
            Console.WriteLine("SessionDate   {0}", result.SessionDate);
            Console.WriteLine("AccountNumber {0}", result.AccountNumber);
            Console.WriteLine("CustomerId   {0}", result.CustomerId);
            Console.WriteLine("IsSuccessful  {0}", result.Success);
            Console.WriteLine("DocumentUid   {0}", result.DocumentUid);
            Console.WriteLine();

            if (result.Exceptions != null)
                foreach (ExceptionMessage ex in result.Exceptions) {
                    Console.WriteLine("    Exception: [{0}] - {1}, {2}", ((int)ex.ExceptionType), ex.ExceptionType, ex.Message);
                }
            Console.WriteLine();
        }

        public static void ShowResults(string title, TransferResult result) {
            ShowHeader(title);
            Console.WriteLine("SessionId     {0}", result.SessionId);
            Console.WriteLine("SessionDate   {0}", result.SessionDate);
            Console.WriteLine("AccountNumber {0}", result.AccountNumber);
            Console.WriteLine("CustomerId   {0}", result.CustomerId);
            Console.WriteLine("Source        {0}", result.SourceAccountNumber);
            Console.WriteLine("Destination   {0}", result.DestinationAccountNumber);
            Console.WriteLine("ExecutionDate {0}", result.ExecutionDate);
            Console.WriteLine("Amount        {0}", result.Amount.ToString("#,##0.00"));
            Console.WriteLine("IsSuccessful  {0}", result.Success);
            Console.WriteLine();

            if (result.Exceptions != null)
                foreach (ExceptionMessage ex in result.Exceptions) {
                    Console.WriteLine("    Exception: [{0}] - {1}, {2}", ((int)ex.ExceptionType), ex.ExceptionType, ex.Message);
                }
            Console.WriteLine();
        }

        public static void ShowResults(string title, Transfer.AdministrativeAccounts[] accounts) {
            var count = 0;
            ShowHeader(title, accounts.Count());
            foreach (Transfer.AdministrativeAccounts account in accounts) {
                Console.WriteLine("{0} {1}", account.AccountNumber.PadRight(16), account.AccountName);
                count += 1;
                if (count > 5) {
                    Console.WriteLine();
                    return;
                }
            }
            Console.WriteLine();
        }


        public static void ShowResults(string title, RefundResult result) {
            ShowHeader(title);
            Console.WriteLine("SessionId     {0}", result.SessionId);
            Console.WriteLine("SessionDate   {0}", result.SessionDate);
            Console.WriteLine("AccountNumber {0}", result.AccountNumber);
            Console.WriteLine("CustomerId   {0}", result.CustomerId);
            Console.WriteLine("Method        {0}", result.RefundMethod);
            Console.WriteLine("Amount        {0}", result.Amount.ToString("#,##0.00"));
            Console.WriteLine("IsSuccessful  {0}", result.Success);
            Console.WriteLine("ScheduleDate  {0}", result.ScheduledDate.HasValue ? result.ScheduledDate.Value.ToShortDateString() : "");

            if (result.NotificationEmails != null) {
                foreach (string email in result.NotificationEmails)
                    Console.WriteLine("Email         {0}", email);
            }

            foreach (var ex in result.Exceptions) {
                Console.WriteLine("Exception:    {0}", ex.Message);
            }
        }

        public static void ShowResults(string title, RefundDetail[] result, string groupNumber, string customerId) {
            ShowHeader(title);

            Console.WriteLine("GroupNumber     {0}", groupNumber);
            Console.WriteLine("CustomerID        {0}", customerId);
            Console.WriteLine("Refunds Found   {0}\n\n", result.Length);

            foreach (RefundDetail detail in result) {
                Console.WriteLine("Amount          {0}", detail.Amount.ToString("#,##0.00"));
                Console.WriteLine("Date            {0}", detail.RefundDate.ToShortDateString());
                Console.WriteLine("Method          {0}", ((SettlementDeliveryMethod)detail.DeliveryMethod).ToString());
                Console.WriteLine("Status          {0}", ((AgreementStatus)detail.Status).ToString());
                Console.WriteLine("----------------------------------------------------------------");
            }
        }
    }
}