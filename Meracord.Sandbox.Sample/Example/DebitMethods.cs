using System;
using System.Linq;
using Meracord.Sandbox.Factories;
using Meracord.Sandbox.Helpers;
using Meracord.API;
using Meracord.API.Common.Enumeration;
using Meracord.API.Common.Factories;
using Meracord.API.Common.Transport;

namespace Meracord.Sandbox.Example
{
    /// <summary>
    /// Sample method calls using the API DataSession.Debit object
    /// </summary>
    internal class DebitMethods
    {
        private static DataSession _session;

        /// <summary>
        /// Execute sample method calls
        /// </summary>
        public static void Perform(string customerId, Guid paymentProfileToken)
        {
            try
            {
                var groupNumber = Settings.GroupNumber;
                var bankProfile = BankProfileMethods.GetBankProfile(Helper.RandomNumericString(17));

                _session = SessionFactory.Create();

                FindNextValidDebitDate();

                CreateDebitWithToken(groupNumber, customerId, paymentProfileToken);

                CreateDebitWithNewBankProfile(groupNumber, customerId, bankProfile);

                FindByStatus(groupNumber, customerId);

                CancelAllDebit(groupNumber, customerId);

            }
            catch (Exception ex)
            {
                Helper.DisplayException(ex);
            }
        }


        /// <summary>
        /// Execute Debit.Create() method
        /// </summary>
        private static Guid CreateDebitWithNewBankProfile(string groupNumber, string customerId, BankProfile bankProfile)
        {
            // Build debit transport object to send to web service
            var debit = GetTransportDebitWithBankProfile(groupNumber, customerId, bankProfile);

            // Call Debit WebService Create Method
            var debitResult = _session.Debit.Create(debit);
            Helper.ShowResults("Debit.Create()", debitResult);

            return debitResult.Debits.First().PaymentProfileToken;
        }

        /// <summary>
        /// Execute Debit.Create() method
        /// </summary>
        private static void CreateDebitWithToken(string groupNumber, string customerId, Guid token)
        {
            // Build debit transport object to send to web service
            var debit = GetTransportDebitWithToken(groupNumber, customerId, token);

            // Call Debit WebService Create Method
            var debitResult = _session.Debit.Create(debit);
            Helper.ShowResults("Debit.Create() using existing profile", debitResult);
        }


        /// <summary>
        /// Execute Debit.FindNextValidDebitDate() method
        /// </summary>
        private static void FindNextValidDebitDate()
        {
            // Call Debit WebService FindNextValidDebitDate Method
            var nextValidDate = _session.Debit.FindNextValidDebitDate();
            Helper.ShowResults("Debit.FindNextValidDebitDate()", nextValidDate);
        }

        /// <summary>
        /// Execute Debit.CancelAll() method
        /// </summary>
        private static void CancelAllDebit(string groupNumber, string customerId)
        {
            // Call Debit WebService CancelAll Method
            var debitResult = _session.Debit.CancelAll(groupNumber, customerId);
            Helper.ShowResults("Debit.CancelAll()", debitResult);
        }

        /// <summary>
        /// Execute Debit.FindByStatus() method
        /// </summary>
        private static void FindByStatus(string groupNumber, string customerId)
        {
            // Call Debit WebService CancelAll Method
            var debitResult = _session.Debit.FindByStatus(groupNumber, customerId, (int)DebitState.Pending);
            Helper.ShowResults("Debit.FindByStatus()", debitResult);
        }

        /// <summary>
        /// Helper method to generate Debit object using BankProfile
        /// </summary>
        private static Debit GetTransportDebitWithBankProfile(string groupNumber, string customerId, BankProfile bankProfile)
        {
            // Service Provider's Debit Identifier (Debit PrimaryKey)
            string debitReferenceId = Guid.NewGuid().ToString(); 

            // The date to pull funds from consumers account
            DateTime debitDate = DateTime.Today.AddDays(5);

            // The total amount to pull funds from consumers account
            double debitAmount = 750.00;

            // Create empty collection of Allocations
            var allocList = DebitFactory.NewAllocationList();

            // Create an Allocation for MaintenanceFees, and add to Allocation collection
            allocList.Add(
                DebitFactory.CreateAllocation(AssignmentCode.MaintenanceFee1, 235.00)
                );

            // Create an Allocation for Consumer's Reserve Account, and add to Allocation collection
            allocList.Add(
                DebitFactory.CreateAllocation(AssignmentCode.AccountReserves, 500.00)
                );

            // Create an Allocation for Meracord Fees, and add to Allocation collection
            allocList.Add(
                DebitFactory.CreateAllocation(AssignmentCode.NoteWorldFee, 15.00)
                );

            // Call DebitFactory Create Method, and return Transport.Debit Instance
            return DebitFactory.Create(groupNumber, customerId, debitReferenceId, debitDate, bankProfile, debitAmount, allocList);
            
        }

        /// <summary>
        /// Helper method to generate Debit object using PaymentProfileToken
        /// </summary>
        private static Debit GetTransportDebitWithToken(string groupNumber, string customerId, Guid token)
        {
            // Service Provider's Debit Identifier (Debit PrimaryKey)
            string debitReferenceId = Guid.NewGuid().ToString();

            // The date to pull funds from consumers account
            DateTime debitDate = DateTime.Today.AddDays(5);

            // The total amount to pull funds from consumers account
            double debitAmount = 750.00;

            // Create empty collection of Allocations
            var allocList = DebitFactory.NewAllocationList();

            // Create an Allocation for MaintenanceFees, and add to Allocation collection
            allocList.Add(
                DebitFactory.CreateAllocation(AssignmentCode.MaintenanceFee1, 235.00)
                );

            // Create an Allocation for Consumer's Reserve Account, and add to Allocation collection
            allocList.Add(
                DebitFactory.CreateAllocation(AssignmentCode.AccountReserves, 500.00)
                );

            // Create an Allocation for Meracord Fees, and add to Allocation collection
            allocList.Add(
                DebitFactory.CreateAllocation(AssignmentCode.NoteWorldFee, 15.00)
                );

            // Call DebitFactory Create Method, and return Transport.Debit Instance
            return DebitFactory.Create(groupNumber, customerId, debitReferenceId, debitDate, token, debitAmount, allocList);

        }

    }
}