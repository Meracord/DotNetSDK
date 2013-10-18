using System;
using Meracord.Sandbox.Factories;
using Meracord.Sandbox.Helpers;
using Meracord.API;
using Meracord.API.Common.Factories;
using Meracord.API.Common.Transport;

namespace Meracord.Sandbox.Example
{
    /// <summary>
    /// Sample method calls using the API DataSession.Payment object
    /// </summary>
    internal class PaymentMethods
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

                var paymentToken = SchedulePaymentWithToken(groupNumber, customerId, paymentProfileToken);

                SchedulePaymentWithNewBankProfile(groupNumber, customerId, bankProfile);

                FindByToken(groupNumber, customerId, paymentToken);

                CancelAllPayment(groupNumber, customerId);

            }
            catch (Exception ex)
            {
                Helper.DisplayException(ex);
            }
        }


        /// <summary>
        /// Execute Payment.Schedule() method
        /// </summary>
        private static void SchedulePaymentWithNewBankProfile(string groupNumber, string customerId, BankProfile bankProfile)
        {
            // Build payment transport object to send to web service
            var payment = GetSchedulePaymentWithNewBankProfile(groupNumber, customerId, bankProfile);

            // Call Payment WebService Schedule Method
            var paymentResult = _session.Payment.Schedule(payment);
            Helper.ShowResults("Payment.Schedule()", paymentResult);
        }

        /// <summary>
        /// Execute Payment.Schedule() method
        /// </summary>
        private static Guid SchedulePaymentWithToken(string groupNumber, string customerId, Guid token)
        {
            // Build payment transport object to send to web service
            var payment = GetSchedulePaymentWithToken(groupNumber, customerId, token);

            // Call Payment WebService Schedule Method
            var paymentResult = _session.Payment.Schedule(payment);
            Helper.ShowResults("Payment.Schedule() using existing profile", paymentResult);
            return paymentResult.PaymentToken;
        }

        /// <summary>
        /// Execute Payment.CancelAll() method
        /// </summary>
        private static void CancelAllPayment(string groupNumber, string customerId)
        {
            // Call Payment WebService CancelAll Method
            var paymentResult = _session.Payment.CancelAll(groupNumber, customerId);
            Helper.ShowResults("Payment.CancelAll()", paymentResult);
        }

        /// <summary>
        /// Execute Payment.FindByTokens() method
        /// </summary>
        private static void FindByToken(string groupNumber, string customerId, Guid token)
        {
            // Call Payment WebService CancelAll Method
            var paymentResult = _session.Payment.FindByToken(groupNumber, customerId, token);
            Helper.ShowResults("Payment.FindByToken()", paymentResult);
        }

        /// <summary>
        /// Helper method to generate PaymentInstruction object using BankProfile
        /// </summary>
        private static PaymentInstruction GetSchedulePaymentWithNewBankProfile(string groupNumber, string customerId, BankProfile bankProfile)
        {
            // Service Provider's Payment Identifier (Payment PrimaryKey)
            string paymentReferenceId = Guid.NewGuid().ToString(); 

            // The date to pull funds from consumers account
            DateTime paymentDate = DateTime.Today.AddDays(5);

            // The total amount to pull funds from consumers account
            double paymentAmount = 750.00;

            // Create empty collection of Allocations
            var allocList = PaymentFactory.NewAllocationList();

            // Create an Allocation for MaintenanceFees, and add to Allocation collection
            allocList.Add(
                PaymentFactory.CreateAllocation(AssignmentCode.MaintenanceFee1, 235.00)
                );

            // Create an Allocation for Consumer's Reserve Account, and add to Allocation collection
            allocList.Add(
                PaymentFactory.CreateAllocation(AssignmentCode.AccountReserves, 500.00)
                );

            // Create an Allocation for Meracord Fees, and add to Allocation collection
            allocList.Add(
                PaymentFactory.CreateAllocation(AssignmentCode.NoteWorldFee, 15.00)
                );

            // Call PaymentFactory Create Method, and return Transport.PaymentInstruction Instance
            return PaymentFactory.Create(groupNumber, customerId, paymentReferenceId, paymentDate, bankProfile, paymentAmount, allocList);
            
        }

        /// <summary>
        /// Helper method to generate PaymentInstruction object using PaymentProfileToken
        /// </summary>
        private static PaymentInstruction GetSchedulePaymentWithToken(string groupNumber, string customerId, Guid token)
        {
            // Service Provider's Payment Identifier (Payment PrimaryKey)
            string paymentReferenceId = Guid.NewGuid().ToString();

            // The date to pull funds from consumers account
            DateTime paymentDate = DateTime.Today.AddDays(5);

            // The total amount to pull funds from consumers account
            double paymentAmount = 750.00;

            // Create empty collection of Allocations
            var allocList = PaymentFactory.NewAllocationList();

            // Create an Allocation for MaintenanceFees, and add to Allocation collection
            allocList.Add(
                PaymentFactory.CreateAllocation(AssignmentCode.MaintenanceFee1, 235.00)
                );

            // Create an Allocation for Consumer's Reserve Account, and add to Allocation collection
            allocList.Add(
                PaymentFactory.CreateAllocation(AssignmentCode.AccountReserves, 500.00)
                );

            // Create an Allocation for Meracord Fees, and add to Allocation collection
            allocList.Add(
                PaymentFactory.CreateAllocation(AssignmentCode.NoteWorldFee, 15.00)
                );

            // Call PaymentFactory Create Method, and return Transport.PaymentInstruction Instance
            return PaymentFactory.Create(groupNumber, customerId, paymentReferenceId, paymentDate, token, paymentAmount, allocList);
        }

    }
}