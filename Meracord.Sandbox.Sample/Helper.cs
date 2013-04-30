using System;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using Meracord.Sandbox.Custom;
using NoteWorld.DataServices;
using NoteWorld.DataServices.Common.Enumeration;
using NoteWorld.DataServices.Common.Transport;
using NoteWorld.DataServices.Debit;
using NoteWorld.DataServices.Reporting;
using AccountStatus = NoteWorld.DataServices.Reporting.AccountStatus;
using Transfer = NoteWorld.DataServices.Transfer;

namespace Meracord.Sandbox
{
    internal static class Helper
    {
        /// <summary>
        /// In development mode SSL certificates are generaly not trusted.
        /// This call allows us to process with an untrusted SSL certificate on the web server.
        /// This should never be enabled in production code.
        /// </summary>
        public static void OverrideCertificateHandling()
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = (obj, certificate, chain, errors) => true;
        }

        /// <summary>
        /// Generate a random aplha numeric string
        /// </summary>
        public static string RandomAlphaNumericString(int length)
        {
            var value = Guid.NewGuid().ToString().Replace("-", "");
            return value.Substring(value.Length - length, length);
        }

        /// <summary>
        /// Generate a random numeric string
        /// </summary>
        public static string RandomNumericString(int length)
        {
            string value = Guid.NewGuid().ToString() + Guid.NewGuid().ToString();
            value = System.Text.RegularExpressions.Regex.Replace(value, @"[^\d]", "").Replace("-", "");
            return value.Substring(0, length);
        }

        public static void ShowResults(string title, DateTime nextDate)
        {
            ShowHeader(title);
            Console.WriteLine("NextValidDebitDate: {0}", nextDate.ToShortDateString());
        }

        public static void ShowResults(string title, string message)
        {
            ShowHeader(title);
            Console.WriteLine(message);
        }

        public static void ShowHeader(string title)
        {
            string displayHeader = "".PadRight(70, char.Parse("="));
            Console.WriteLine();
            Console.WriteLine(displayHeader);
            Console.WriteLine("== {0}", title);
            Console.WriteLine(displayHeader);
        }

        public static void ShowHeader(string title, int rows)
        {
            string displayHeader = "".PadRight(70, char.Parse("="));
            Console.WriteLine();
            Console.WriteLine(displayHeader);
            Console.WriteLine("== {0}  Rows: {1}", title, rows);
            Console.WriteLine(displayHeader);
        }

        public static void ShowResults(string title, IList<DebitView> debits)
        {
            ShowHeader(title);
            foreach (var debit in debits)
            {
                var debitStatus = (DebitStatus)debit.DebitStatusId;
                Console.WriteLine("Debit: {0}{1}{2}{3}", debit.DebitID.ToString().PadRight(10), debit.DebitDate.ToString("yyyy-MM-dd").PadRight(12), debit.DebitAmount.ToString("#,###.00").PadLeft(12), debitStatus.ToString().PadLeft(10));
                Console.WriteLine("   Reserves:{0}{1}", "".PadRight(17), debit.ReserveAmount.ToString("#,###.00").PadLeft(12));
                foreach (var alloc in debit.Allocations)
                {
                    Console.WriteLine("   Allocation: {0}{1}{2}", alloc.Sequence.ToString().PadRight(4), alloc.Assignment.PadLeft(6), alloc.Amount.ToString("#,###.00").PadLeft(16));
                }
            }
            Console.WriteLine();
        }


        public static void ShowResults(string title, DebitResult result)
        {
            ShowHeader(title);
            Console.WriteLine("SessionId     {0}", result.SessionId);
            Console.WriteLine("SessionDate   {0}", result.SessionDate);
            Console.WriteLine("AccountNumber {0}", result.AccountNumber);
            Console.WriteLine("ClientId      {0}", result.ClientId);
            Console.WriteLine("IsSuccessful  {0}", result.Success);
            foreach (var debit in result.Debits)
            {
                Console.WriteLine("Debit:        {0}{1} {2}", debit.DebitId.ToString().PadRight(10), debit.DebitDate.ToString("yyyy-MM-dd"), debit.DebitAmount);
            }

            if (result.Exceptions != null)
            {
                Console.WriteLine();
                foreach (ExceptionMessage ex in result.Exceptions)
                {
                    Console.WriteLine("    Exception: [{0}] - {1}, {2}", ((int)ex.ExceptionType), ex.ExceptionType, ex.Message);
                }
            }

            Console.WriteLine();
        }

        public static void ShowResults(string title, SettlementResult result)
        {
            ShowHeader(title);
            Console.WriteLine("SessionId          {0}", result.SessionId);
            Console.WriteLine("SessionDate        {0}", result.SessionDate);
            Console.WriteLine("AccountNumber      {0}", result.AccountNumber);
            Console.WriteLine("ClientId           {0}", result.ClientId);
            Console.WriteLine("IsSuccessful       {0}", result.Success);
            Console.WriteLine("AgreementId        {0}", result.AgreementId);
            Console.WriteLine("AgreementStatus    {0}", result.AgreementStatusId);
            Console.WriteLine("ReserveAmount      {0}", result.ReserveAmount.ToString("$#,##0.00"));
            Console.WriteLine("AgreementAmount    {0}", result.AgreementAmount.ToString("$#,##0.00"));
            Console.WriteLine("IsApprovalRequired {0}", result.IsApprovalRequired);
            Console.WriteLine("IsTermAgreement    {0}", result.IsTermAgreement);
            Console.WriteLine("HasDocument        {0}", result.HasDocument);
            Console.WriteLine("ApprovExpDate      {0}", result.ApprovalExpirationDate);

            foreach (var s in result.Settlements)
            {
                Console.WriteLine("    SettlementId = {0}, {1}, {2}", s.SettlementId, s.Amount.ToString("$#,##0.00").PadLeft(11), s.SettlementStatusId);
            }

            if (result.Exceptions != null)
            {
                Console.WriteLine();
                foreach (ExceptionMessage ex in result.Exceptions)
                {
                    Console.WriteLine("    Exception: [{0}] - {1}, {2}", ((int)ex.ExceptionType), ex.ExceptionType, ex.Message);
                }
            }

            Console.WriteLine();
        }




        public static void ShowResults(string title, NoteWorld.DataServices.Account.AccountView result)
        {
            ShowHeader(title);
            Console.WriteLine("AccountNumber {0}", result.AccountNumber);
            Console.WriteLine("DateSetup     {0}", result.DateSetup);
        }


        public static void ShowResults(string title, IList<CreditorSummary> creditors)
        {
            var i = 0;
            ShowHeader(title, creditors.Count);
            foreach (var creditor in creditors)
            {
                Console.WriteLine("{0}{1}", creditor.Id.PadRight(11), creditor.CreditorName);
                i += 1;
                if (i > 5) break; //Only show 5 rows as sample
            }
            Console.WriteLine();
        }

        public static void ShowResults(string title, CreditorResult result)
        {
            ShowHeader(title);
            Console.WriteLine("SessionId     {0}", result.SessionId);
            Console.WriteLine("SessionDate   {0}", result.SessionDate);
            Console.WriteLine("CreditorId    {0}", result.Creditor != null ? result.Creditor.Id : "");
            Console.WriteLine("IsSuccessful  {0}", result.Success);

            if (result.Creditor != null)
            {
                var c = result.Creditor;
                Console.WriteLine();
                Console.WriteLine(" Name      {0}", c.CreditorName);
                Console.WriteLine(" Address   {0}", c.Address);
                Console.WriteLine("           {0}, {1} {2}", c.City, c.State, c.PostalCode);
                Console.WriteLine(" HasAch    {0}", c.HasAchAccount);

                if (c.DirectDepositProfile == null)
                {
                    Console.WriteLine();
                    Console.WriteLine(" DirectDepositProfileCount = 0");
                }
                else
                {
                    var profiles = c.DirectDepositProfile;
                    Console.WriteLine();
                    Console.WriteLine(" DirectDepositProfileCount = {0}", profiles.Count);
                    foreach (var p in profiles)
                    {
                        Console.WriteLine();
                        Console.WriteLine("   ProfileName   {0}", p.ProfileName);
                        Console.WriteLine("   CardType      {0}", p.CreditCardType);
                        Console.WriteLine("   RoutingNumber {0}", p.RoutingNumber);
                    }
                }
            }

            if (result.Exceptions != null)
                Console.WriteLine();
            foreach (ExceptionMessage ex in result.Exceptions)
            {
                Console.WriteLine("  Exception: [{0}] - {1}, {2}", ((int)ex.ExceptionType), ex.ExceptionType, ex.Message);
            }
            Console.WriteLine();
        }

        public static void ShowResults(string title, AccountResult result)
        {
            ShowHeader(title);
            Console.WriteLine("SessionId     {0}", result.SessionId);
            Console.WriteLine("SessionDate   {0}", result.SessionDate);
            Console.WriteLine("AccountNumber {0}", result.AccountNumber);
            Console.WriteLine("ClientId      {0}", result.ClientId);
            Console.WriteLine("HoldType      {0}", result.HoldType);
            Console.WriteLine("IsSuccessful  {0}", result.Success);

            if (result.Exceptions != null)
                foreach (ExceptionMessage ex in result.Exceptions)
                {
                    Console.WriteLine("    Exception: [{0}] - {1}, {2}", ((int)ex.ExceptionType), ex.ExceptionType, ex.Message);
                }
            Console.WriteLine();
        }

        public static void ShowResults(string title, DocumentResult result)
        {
            ShowHeader(title);
            Console.WriteLine("SessionId     {0}", result.SessionId);
            Console.WriteLine("SessionDate   {0}", result.SessionDate);
            Console.WriteLine("AccountNumber {0}", result.AccountNumber);
            Console.WriteLine("ClientId      {0}", result.ClientId);
            Console.WriteLine("IsSuccessful  {0}", result.Success);

            if (result.Exceptions != null)
                foreach (ExceptionMessage ex in result.Exceptions)
                {
                    Console.WriteLine("    Exception: [{0}] - {1}, {2}", ((int)ex.ExceptionType), ex.ExceptionType, ex.Message);
                }
            Console.WriteLine();
        }


        public static void ShowResults(string title, ControlGroup[] groups)
        {
            ShowHeader(title, groups.Count());
            foreach (ControlGroup group in groups)
            {
                Console.WriteLine("{0} {1}", group.GroupNumber, group.GroupDescription);
                Console.WriteLine();
                return; //Only show one row
            }
        }


        public static void ShowResults(string title, AccountDetails[] accounts)
        {
            ShowHeader(title, accounts.Count());
            foreach (AccountDetails account in accounts)
            {
                Console.WriteLine("{0} {1} {2}", account.AccountNumber, account.ClientID.PadRight(12), account.DateSetup.ToShortDateString());
                Console.WriteLine();
                return; //Only show one row
            }
        }

        public static void DisplayException(string title, Exception ex)
        {
            ShowHeader(title);
            DisplayException(ex);
            Console.WriteLine();
        }

        public static void DisplayException(Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine(ex.Message);
            if (ex.InnerException != null)
            {
                Console.WriteLine();
                Console.WriteLine(ex.InnerException.Message);
            }
        }

        public static void ShowResults(string title, AccountStatus[] accounts)
        {
            ShowHeader(title, accounts.Count());
            foreach (AccountStatus account in accounts)
            {
                Console.WriteLine("{0} {1} {2}", account.AccountNumber, account.ClientID.PadRight(12), account.ChangeDate.ToShortDateString());
                Console.WriteLine();
                return; //Only show one row
            }
        }

        public static void ShowResults(string title, DebitSummary[] debits)
        {
            ShowHeader(title, debits.Count());
            foreach (DebitSummary debit in debits)
            {
                Console.WriteLine("{0} {1} {2}", debit.AccountNumber, debit.ClientID.PadRight(12), debit.ChangeDate.ToShortDateString());
                Console.WriteLine();
                return; //Only show one row
            }
        }

        public static void ShowResults(string title, MyAccount[] accounts)
        {
            ShowHeader(title, accounts.Count());
            foreach (MyAccount account in accounts)
            {
                Console.WriteLine("{0} {1} {2}", account.AccountNumber.PadRight(16),
                    account.ReserveBalance.ToString("#,##0.00").PadLeft(12),
                    account.ChangeDate.ToShortDateString());
                Console.WriteLine();
                return; //Only show one row
            }
        }

        public static void ShowResults(string title, TransactionHistory[] transactions)
        {
            var count = 0;
            ShowHeader(title, transactions.Count());
            foreach (var trans in transactions)
            {
                Console.WriteLine(
                    "{0} {1} {2} {3} {4}",
                    trans.TransactionId.ToString().PadRight(8),
                    trans.SourceAccountNumber.PadRight(15),
                    trans.TransactionTypeDescription.PadRight(25),
                    trans.AllocationType.PadRight(5),
                    trans.Amount.ToString("C")
                );

                count += 1;
                if (count > 5)
                {
                    Console.WriteLine();
                    return;
                }
            }
            Console.WriteLine();
        }

        public static void ShowResults(string title, ReportingSession session)
        {
            ShowHeader(title);
            Console.WriteLine(session.DataQuery);
            Console.WriteLine();
        }

        public static void ShowResults(string title, BankProfileResult result)
        {
            ShowHeader(title);
            Console.WriteLine("SessionId     {0}", result.SessionId);
            Console.WriteLine("SessionDate   {0}", result.SessionDate);
            Console.WriteLine("AccountNumber {0}", result.AccountNumber);
            Console.WriteLine("ClientId      {0}", result.ClientId);
            Console.WriteLine("IsSuccessful  {0}", result.Success);
            Console.WriteLine("DocumentUid   {0}", result.DocumentUid);
            Console.WriteLine();

            if (result.Exceptions != null)
                foreach (ExceptionMessage ex in result.Exceptions)
                {
                    Console.WriteLine("    Exception: [{0}] - {1}, {2}", ((int)ex.ExceptionType), ex.ExceptionType, ex.Message);
                }
            Console.WriteLine();
        }

        public static void ShowResults(string title, DirectDepositProfileResult result)
        {
            ShowHeader(title);
            Console.WriteLine("SessionId     {0}", result.SessionId);
            Console.WriteLine("SessionDate   {0}", result.SessionDate);
            Console.WriteLine("AccountNumber {0}", result.AccountNumber);
            Console.WriteLine("ClientId      {0}", result.ClientId);
            Console.WriteLine("IsSuccessful  {0}", result.Success);
            Console.WriteLine("BankAccount   {0}", result.BankAccountNumber);
            Console.WriteLine("RoutingNumber {0}", result.RoutingNumber);
            Console.WriteLine();

            if (result.Exceptions != null)
                foreach (ExceptionMessage ex in result.Exceptions)
                {
                    Console.WriteLine("    Exception: [{0}] - {1}, {2}", ((int)ex.ExceptionType), ex.ExceptionType, ex.Message);
                }
            Console.WriteLine();
        }

        public static void ShowResults(string title, PaymentCardResult result)
        {
            ShowHeader(title);
            Console.WriteLine("SessionId     {0}", result.SessionId);
            Console.WriteLine("SessionDate   {0}", result.SessionDate);
            Console.WriteLine("AccountNumber {0}", result.AccountNumber);
            Console.WriteLine("ClientId      {0}", result.ClientId);
            Console.WriteLine("CardToken     {0}", result.PaymentCardToken);
            Console.WriteLine("DocumentUid   {0}", result.DocumentUid);
            Console.WriteLine("IsSuccessful  {0}", result.Success);
            foreach (var payment in result.Payments)
            {
                Console.WriteLine("    PaymentId = {0}{1} {2}",
                    payment.PaymentId.ToString(CultureInfo.InvariantCulture).PadRight(10),
                    payment.PaymentDate.ToString("yyyy-MM-dd"),
                    payment.PaymentAmount);
            }

            if (result.Exceptions != null)
            {
                Console.WriteLine();
                foreach (ExceptionMessage ex in result.Exceptions)
                {
                    Console.WriteLine("    Exception: [{0}] - {1}, {2}", ((int)ex.ExceptionType), ex.ExceptionType, ex.Message);
                }
            }

            Console.WriteLine();
        }

        public static void ShowResults(string title, TransferResult result)
        {
            ShowHeader(title);
            Console.WriteLine("SessionId     {0}", result.SessionId);
            Console.WriteLine("SessionDate   {0}", result.SessionDate);
            Console.WriteLine("AccountNumber {0}", result.AccountNumber);
            Console.WriteLine("ClientId      {0}", result.ClientId);
            Console.WriteLine("Source        {0}", result.SourceAccountNumber);
            Console.WriteLine("Destination   {0}", result.DestinationAccountNumber);
            Console.WriteLine("ExecutionDate {0}", result.ExecutionDate);
            Console.WriteLine("Amount        {0}", result.Amount.ToString("#,##0.00"));
            Console.WriteLine("IsSuccessful  {0}", result.Success);
            Console.WriteLine();

            if (result.Exceptions != null)
                foreach (ExceptionMessage ex in result.Exceptions)
                {
                    Console.WriteLine("    Exception: [{0}] - {1}, {2}", ((int)ex.ExceptionType), ex.ExceptionType, ex.Message);
                }
            Console.WriteLine();
        }

        public static void ShowResults(string title, Transfer.AdministrativeAccountView[] accounts)
        {
            var count = 0;
            ShowHeader(title, accounts.Count());
            foreach (Transfer.AdministrativeAccountView account in accounts)
            {
                Console.WriteLine("{0} {1}", account.AccountNumber.PadRight(16), account.AccountName);
                count += 1;
                if (count > 5)
                {
                    Console.WriteLine();
                    return;
                }
            }
            Console.WriteLine();
        }

        public static void ShowResults(string title, PaymentCardProfileReference[] profiles)
        {
            var count = 0;
            ShowHeader(title, profiles.Count());
            foreach (PaymentCardProfileReference profile in profiles)
            {
                Console.WriteLine("{0} {1} {2} {3}", ((PaymentCardType)profile.PaymentCardTypeId).ToString().PadRight(12), profile.IsActive.ToString().PadRight(5), profile.CardNumber, profile.PaymentCardToken);
                count += 1;
                if (count > 5)
                {
                    Console.WriteLine();
                    return;
                }
            }
            Console.WriteLine();
        }



        public static void ShowResults(string title, RefundResult result)
        {
            ShowHeader(title);
            Console.WriteLine("SessionId     {0}", result.SessionId);
            Console.WriteLine("SessionDate   {0}", result.SessionDate);
            Console.WriteLine("AccountNumber {0}", result.AccountNumber);
            Console.WriteLine("ClientId      {0}", result.ClientId);
            Console.WriteLine("Method        {0}", result.RefundMethod);
            Console.WriteLine("Amount        {0}", result.Amount.ToString("#,##0.00"));
            Console.WriteLine("IsSuccessful  {0}", result.Success);
            Console.WriteLine("ScheduleDate  {0}", result.ScheduledDate.HasValue ? result.ScheduledDate.Value.ToShortDateString() : "");

            if (result.NotificationEmails != null)
            {
                foreach (string email in result.NotificationEmails)
                    Console.WriteLine("Email         {0}", email);
            }

            foreach (var ex in result.Exceptions)
            {
                Console.WriteLine("Exception:    {0}", ex.Message);
            }
        }

        public static void ShowResults(string title, RefundDetail[] result, string groupNumber, string clientId)
        {
            ShowHeader(title);

            Console.WriteLine("GroupNumber     {0}", groupNumber);
            Console.WriteLine("ClientId        {0}", clientId);
            Console.WriteLine("Refunds Found   {0}\n\n", result.Length);

            foreach (RefundDetail detail in result)
            {
                Console.WriteLine("Amount          {0}", detail.Amount.ToString("#,##0.00"));
                Console.WriteLine("Date            {0}", detail.RefundDate.ToShortDateString());
                Console.WriteLine("Method          {0}", ((SettlementDeliveryMethod)detail.DeliveryMethod).ToString());
                Console.WriteLine("Status          {0}", ((AgreementStatus)detail.Status).ToString());
                Console.WriteLine("----------------------------------------------------------------");
            }
        }
    }
}