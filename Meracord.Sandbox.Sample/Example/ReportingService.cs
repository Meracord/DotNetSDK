﻿using System;
using System.Linq;
using Meracord.Sandbox.Helpers;
using Meracord.API;
using Meracord.API.Reporting;
using Meracord.Sandbox.Custom;

namespace Meracord.Sandbox.Example
{
    /// <summary>
    /// Sample method calls using the API ReportingSession.DataContext REST Service object
    /// </summary>
    internal class ReportingService
    {
        private static ReportingSession _session;

        /// <summary>
        /// Execute sample method calls
        /// </summary>
        public static void Perform(string accountNumber)
        {
            try
            {
                var groupNumber = Settings.GroupNumber;

                CreateReportingSession();

                Helper.ShowResults("ControlGroup Query",
                    FindControlGroup()
                    );

                Helper.ShowResults("AccountDetails Query",
                    FindAccountDetails(groupNumber)
                    );

                Helper.ShowResults("AccountStatus Query",
                    FindAccountStatus(accountNumber)
                    );

                Helper.ShowResults("PaymentSummary Query",
                    FindScheduledPayment(accountNumber)
                    );

                Helper.ShowResults("Transaction History Query By Account",
                    AccountTransactionHistory(accountNumber)
                    );

                Helper.ShowResults("Transfers Query By Account",
                    AccountTransfers(accountNumber)
                    );

                Helper.ShowResults("Custom Account Projection",
                    AccountProjection(groupNumber)
                    );

                Helper.ShowResults("Last DataQuery Submitted to Server",
                    _session
                    );
            }
            catch (Exception ex)
            {
                Helper.DisplayException(ex);
            }
        }

        /// <summary>
        /// Create a ReportingSession object
        /// </summary>
        /// <remarks>
        /// In this sample code the ReportingSession is instantiated and used for all calls.
        /// The life span of this object is limited to five miniutes on the application server.
        /// </remarks>
        private static void CreateReportingSession()
        {
            var baseServiceAddress = Settings.BaseServiceAddress;
            var userId = Settings.UserId;
            var password = Settings.Password;

            var config = new ReportingSessionConfiguration
            {
                UserID = userId,
                Password = password,
                BaseServiceAddress = baseServiceAddress,
                RequestTimeout = 30
            };

            _session = new ReportingSession(config);
        }

        /// <summary>
        /// Query the DataContext.ControlGroup for all ControlGroups
        /// </summary>
        /// <remarks>
        /// Will only return rows that the Authenticated User has permissions
        /// </remarks>
        private static ControlGroup[] FindControlGroup()
        {
            IQueryable<ControlGroup> qryGroup =
                from p in _session.DataContext.ControlGroup
                select p;

            return qryGroup.ToArray();
        }

        /// <summary>
        /// Query the DataContext.AccountDetails filtering by ControlGroup
        /// </summary>
        private static AccountDetails[] FindAccountDetails(string groupNumber)
        {
            IQueryable<AccountDetails> qryAccount =
                from p in _session.DataContext.AccountDetails
                where p.GroupNumber == groupNumber
                select p;

            return qryAccount.ToArray();
        }

        /// <summary>
        /// Query the DataContext.AccountStatus filtering by ControlGroup and customerId
        /// </summary>
        private static AccountStatus[] FindAccountStatus(string accountNumber)
        {
            IQueryable<AccountStatus> qryAccountStatus =
                from p in _session.DataContext.AccountStatus
                where p.AccountNumber == accountNumber
                select p;

            return qryAccountStatus.ToArray();
        }

        /// <summary>
        /// Query the DataContext.AccountStatus using a projection to return only needed columns
        /// </summary>
        private static MyAccount[] AccountProjection(string groupNumber)
        {
            IQueryable<MyAccount> qryAccountProjection =
                from p in _session.DataContext.AccountStatus
                where p.GroupNumber == groupNumber
                select new MyAccount(
                    p.AccountNumber, p.ReserveBalance, p.ChangeDate, p.HoldTypeId, p.AccountStatusId
                    );

            return qryAccountProjection.ToArray();
        }

        /// <summary>
        /// Query the DataContext.PaymentSummary filtering by ControlGroup and customerId
        /// </summary>
        private static PaymentSummary[] FindScheduledPayment(string accountNumber)
        {
            IQueryable<PaymentSummary> qryPaymentSummary =
                from p in _session.DataContext.PaymentSummary
                where p.AccountNumber == accountNumber
                select p;

            return qryPaymentSummary.ToArray();
        }

        /// <summary>
        /// Query the DataContext.TransactionHistory filtering by AccountNumber
        /// </summary>
        private static TransactionHistory[] AccountTransactionHistory(string accountNumber)
        {
            IQueryable<TransactionHistory> qryTransactionHistory =
                from p in _session.DataContext.TransactionHistory
                where p.SourceAccountNumber == accountNumber || p.DestinationAccountNumber == accountNumber
                orderby p.TransactionDate, p.TransactionId
                select p;

            return qryTransactionHistory.ToArray();
        }


        /// <summary>
        /// Query the DataContext.Transfers filtering by AccountNumber
        /// </summary>
        private static Transfers[] AccountTransfers(string accountNumber)
        {
            IQueryable<Transfers> qryTransfers =
                from p in _session.DataContext.Transfers
                where p.SourceAccountNumber == accountNumber || p.DestinationAccountNumber == accountNumber
                orderby p.ExecutionDate
                select p;

            return qryTransfers.ToArray();
        }
        
    }
}