using System;
using System.Collections.Generic;
using Meracord.Sandbox.Factories;
using Meracord.Sandbox.Helpers;
using Meracord.API;
using Meracord.API.Common.Enumeration;
using Meracord.API.Common.Factories;
using Meracord.API.Common.Transport;

namespace Meracord.Sandbox.Example
{
    /// <summary>
    /// Sample method calls using the API DataSession.Account object
    /// </summary>
    internal class SettlementMethods
    {
        private static DataSession _session;

        /// <summary>
        /// Execute sample method calls
        /// </summary>
        public static void Perform(string customerId)
        {
            try
            {
                var documentPath = Settings.DocumentPath;

                _session = SessionFactory.Create();

                var creditorId = FindCreditor();

                var contract = GetSettlementContract(customerId, creditorId, documentPath);

                // Execute Settlement.Create() method
                Helper.ShowResults("Settlement.Create()", 
                    _session.Settlement.Create(contract)
                    );

                var agreement = FindActiveSettlementAgreement(customerId);
                
                if (agreement != null)
                {
                    // Execute Settlement.Cancel() method
                    Helper.ShowResults("Settlement.Cancel()",
                        _session.Settlement.Cancel(agreement.GroupNumber, agreement.CustomerId, agreement.AgreementId)
                        );
                }

            }
            catch (Exception ex)
            {
                Helper.DisplayException(ex);
            }
        }

        /// <summary>
        /// Execute Settlement.Find() method
        /// </summary>
        private static SettlementAgreementReference FindActiveSettlementAgreement(string customerId)
        {
            // Build parameter list
            var groupNumber = Settings.GroupNumber;
            const AgreementState agreementState = AgreementState.Active;

            // Find all active settlement agreements for this consumer
            var activeAgreements = _session.Settlement.Find(groupNumber, customerId, agreementState);

            // Verify we have an active agreement
            if (activeAgreements.Length == 0)
            {
                Helper.ShowResults("Settlement.Find()", string.Format("Unable to locate active settlement contract for Group:{0}, CustomerId:{1}", groupNumber, customerId));
                return null;
            }

            // Show current status of active settlement agreement
            Helper.ShowResults("Settlement.Find()", string.Format("AgreementId: {0}, {1}", activeAgreements[0].AgreementId, activeAgreements[0].AgreementStatusId));

            // Get the AgreementId of the first settlement agreement returned in list
            return activeAgreements[0];
        }


        /// <summary>
        /// Helper method to build SettlementAgreement object
        /// </summary>
        private static SettlementAgreement GetSettlementContract(string customerId, string creditorId, string documentPath)
        {
            var groupNumber = Settings.GroupNumber;
            const string clientName = "Billy Bob";
            const string clientForBenefitName = "Betty Bob";
            const string payToTheOrderOf = "Pay the money to Bob";
            const SettlementApprovalRule approvalRule = null;
            const int deliveryMethod = (int)SettlementDeliveryMethod.NextDay;
            const double settlementFeeAmount = 50.00;
            // Agency's tracking number
            const string agencyTrackingNumber = "54636666";
            // Primary creditor identifier, most common would be a CreditCardNumber 
            const string creditorTrackingNumber1 = Helpers.CreditCardNumber.MasterCard2;
            // Secondary creditor identifier, could be something like Settlement Agreement Number
            const string creditorTrackingNumber2 = "34514-431344-41222";
            const string creditorAttentionName = "Settlement Dept";
            const string comment = "some comment goes here";

            var disbursements = GetDisbursements();

            var document = DocumentFactory.Create(documentPath, DocumentType.SettlementAgreement);

            return SettlementFactory.Create(
                groupNumber, customerId, clientName, clientForBenefitName, payToTheOrderOf, 
                approvalRule, deliveryMethod, agencyTrackingNumber,
                creditorId, creditorTrackingNumber1, creditorTrackingNumber2,
                creditorAttentionName, comment, settlementFeeAmount, disbursements, document
                );
        }

        /// <summary>
        /// Helper method to build Disbursement List
        /// </summary>
        private static List<SettlementDisbursement> GetDisbursements()
        {
            const int deliveryMethod = (int)SettlementDeliveryMethod.NextDay;

            var disbs = SettlementFactory.NewDisbursementList();
            for (var i = 0; i < 5; i++)
            {
                var settlementDate = DateTime.Today.AddDays(5).AddMonths(i);
                disbs.Add(SettlementFactory.CreateDisbursement(
                              settlementDate, deliveryMethod, 3562.34)
                    );
            }

            return disbs;
        }

        /// <summary>
        /// Execute Settlement.FindById() method
        /// </summary>
        private static string FindCreditor()
        {
            const string searchValue = "Test";

            // Creditor searchs are performed with a 'Contains clause', so this criteria will retrieve multiple creditors
            var response = _session.Creditor.FindById(searchValue);

            foreach (var creditor in response)
            {
                return creditor.Id; //Return the first creditor found
            }
            throw new ApplicationException("No creditors found with current criteria");
        }

    }
}