﻿using System;
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
        public static void Perform(string customerId, BankProfile bankProfile)
        {
            try
            {
                var groupNumber = Settings.GroupNumber;

                _session = SessionFactory.Create();

                FindNextValidDebitDate();

                CreateNewDebit(groupNumber, customerId, bankProfile);

                CreateDebitWithDefaultBankProfile(groupNumber, customerId);

                FindByStatus(groupNumber, customerId);

                CancelAllDebit(groupNumber, customerId);

            }
            catch (Exception ex)
            {
                Helper.DisplayException(ex);
            }
        }

        /// <summary>
        /// Execute Debit.Create() method with Null PaymentProfile
        /// </summary>
        /// <remarks>
        ///   <para>
        ///   This call will only succeed if a bank profile has already been created, and only one profile exists in the Meracord system.
        ///   </para>
        ///   <para>
        ///   The PaymentProfile should only be omitted if your company's business model does not allow you to maintain banking information for your client, and your client creates their own bank profile.
        ///   </para>
        /// </remarks>
        private static void CreateDebitWithDefaultBankProfile(string groupNumber, string customerId)
        {
            // Build debit transport object to send to web service
            var debit = GetTransportDebit(groupNumber, customerId, null);
            // Remove PaymentProfile used by other test method. 
            debit.PaymentProfile = null;

            // Call Debit WebService Create Method
            var debitResult = _session.Debit.Create(debit);
            Helper.ShowResults("Debit.Create() 'No bank profile specified'", debitResult);
        }


        /// <summary>
        /// Execute Debit.Create() method
        /// </summary>
        private static void CreateNewDebit(string groupNumber, string customerId, BankProfile bankProfile)
        {
            // Build debit transport object to send to web service
            var debit = GetTransportDebit(groupNumber, customerId, bankProfile);

            // Call Debit WebService Create Method
            var debitResult = _session.Debit.Create(debit);
            Helper.ShowResults("Debit.Create()", debitResult);
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
        /// Helper method to generate Debit object
        /// </summary>
        private static Debit GetTransportDebit(string groupNumber, string customerId, BankProfile bankProfile)
        {
            // Service Provider's Debit Identifier (Debit PrimaryKey)
            string debitReferenceId = "353476"; 

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

    }
}