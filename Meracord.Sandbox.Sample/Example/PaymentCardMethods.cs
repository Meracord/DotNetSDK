using System;
using Meracord.API;
using Meracord.Sandbox.Factories;
using Meracord.Sandbox.Helpers;
using Meracord.API.Common.Enumeration;
using Meracord.API.Common.Factories;
using Meracord.API.Common.Transport;
using System.Linq;

namespace Meracord.Sandbox.Example
{
    /// <summary>
    /// Sample method calls using the API DataSession.BankProfile object
    /// </summary>
    internal class PaymentCardMethods
    {
        /// <summary>
        /// Execute sample method calls
        /// </summary>
        public static void Perform(string customerId)
        {
            try
            {
                var groupNumber = Settings.GroupNumber;

                var session = SessionFactory.Create();

                // Build a PaymentCard object
                var paymentCard = GetPaymentCard();

                // Build a CardPaymentSchedule object
                var schedule = GetCardPaymentSchedule(groupNumber, customerId, paymentCard);

                // Call API method to create card payment schedule
                CreateSchedule(session, schedule);

                // Test Schedule.Validate()
                PaymentCardValidate(schedule);

                // Call API method to discover payment card profiles
                // The card returned will be the one created in CreateSchedule call above.
                // Note that activating a card profile is an asynchronise process dependant on processes outside of Meracord,
                // so we may have to wait a little bit before it is activated.
                var cardProfile = FindForAccount(session, groupNumber, customerId);

                // Call API method to create card payment schedule uaing an existing card profile
                CreateScheduleWithToken(session, groupNumber, customerId, cardProfile);

            }
            catch (Exception ex)
            {
                Helper.DisplayException(ex);
            }
        }

        private static void CreateSchedule(DataSession session, CardPaymentSchedule schedule)
        {
            // Test CardPaymentSchedule to see if it is valid
            schedule.Validate();

            // Call PaymentCard.CreateSchedule()
            Helper.ShowResults("PaymentCard.CreateSchedule()", session.PaymentCard.CreateSchedule(schedule));
        }

        private static void CreateScheduleWithToken(DataSession session, string groupNumber, string customerId, PaymentCardProfileReference profile)
        {
            var paymentCardToken = profile.PaymentCardToken;
            var schedule = GetCardPaymentScheduleWithExistingProfile(groupNumber, customerId, paymentCardToken);

            // Test CardPaymentSchedule to see if it is valid
            schedule.Validate();

            // Call PaymentCard.CreateSchedule()
            Helper.ShowResults("PaymentCard.CreateScheduleWithToken()", session.PaymentCard.CreateScheduleWithToken(schedule));
        }

        private static PaymentCardProfileReference FindForAccount(DataSession session, string groupNumber, string customerId)
        {
            var tries = 4;
            while (tries > 0)
            {
                var paymentCardProfiles = session.PaymentCard.FindForAccount(groupNumber, customerId);
                var activeProfile = paymentCardProfiles.ToList().FirstOrDefault(x => x.IsActive);

                if (activeProfile != null)
                {
                    Helper.ShowResults("PaymentCard.FindForAccount()", paymentCardProfiles);
                    return activeProfile;
                }

                Console.WriteLine("PaymentCard profile not active.  Retry in 5 seconds...");
                System.Threading.Thread.Sleep(5000); // wait 5 seconds
                tries--;
            }

            throw new ApplicationException("Timeout reached waiting for PaymentCard profile to become active. It is possible the profile will never be activated do to processing constraints.");
        }

        private static void PaymentCardValidate(CardPaymentSchedule schedule)
        {
            try
            {
                // Make one of the properties on CardPaymentSchedule object invalid
                schedule.PaymentCard.CardNumber = "121212121212";
                schedule.Validate();
            }
            catch (Exception ex)
            {
                Helper.DisplayException("CardPaymentSchedule.Validate() - Fails", ex);
            }
        }




        /// <summary>
        /// Helper method to create a document object
        /// </summary>
        private static Document GetDocument()
        {
            var documentPath = Settings.DocumentPath;
            return DocumentFactory.Create(documentPath, DocumentType.PaymentCardAuthorization);
        }

        /// <summary>
        /// Helper method to create a PaymentCard object
        /// </summary>
        private static PaymentCard GetPaymentCard()
        {
            PaymentCardMethod cardMethod = PaymentCardMethod.CreditCard;
            PaymentCardType cardType = PaymentCardType.Visa;
            string cardNumber = PaymentCardFactory.TestCardNumber.Visa1;
            string cardCode = "103";
            int expirationYear = DateTime.Today.AddYears(2).Year;
            int expirationMonth = DateTime.Today.Month;
            string firstName = "John";
            string lastName = "Doe";
            string street = "1001 Any Street";
            string city = "Any Town";
            string state = "WA";
            string postalCode = "98402";
            string companyName = null;
            bool isBusinessCard = false;

            return PaymentCardFactory.CreateCard(
                cardMethod, cardType, cardNumber, cardCode, expirationYear, expirationMonth, 
                firstName, lastName, street, city, state, postalCode, companyName, isBusinessCard
                );
        }

        /// <summary>
        /// Helper method to create a CardPaymentSchedule object
        /// </summary>
        private static CardPaymentSchedule GetCardPaymentSchedule(string groupNumber, string customerId, PaymentCard paymentCard)
        {
            var paymentFrequency = PaymentFrequency.Monthly;
            var paymentCount = 2;
            var firstPaymentDate = DateTime.Today;
            var paymentAmount = 525.50;
            var agreement = GetDocument();

            var allocations = PaymentCardFactory.NewAllocationList();

            allocations.Add(
                DebitFactory.CreateAllocation(
                    AssignmentCode.AccountReserves, paymentAmount
                    )
                );

            var schedule = PaymentCardFactory.CreateSchedule(
                groupNumber, customerId, paymentCard,
                firstPaymentDate, paymentCount, paymentFrequency,
                paymentAmount, agreement, allocations
                );

            return schedule;
        }

        /// <summary>
        /// Helper method to create a CardPaymentScheduleWithExistingProfile object
        /// </summary>
        private static CardPaymentScheduleWithToken GetCardPaymentScheduleWithExistingProfile(string groupNumber, string customerId, Guid paymentCardToken)
        {
            const PaymentFrequency paymentFrequency = PaymentFrequency.Monthly;
            const int paymentCount = 2;
            var firstPaymentDate = DateTime.Today;
            const double paymentAmount = 525.50;

            var allocations = PaymentCardFactory.NewAllocationList();

            allocations.Add(
                DebitFactory.CreateAllocation(
                    AssignmentCode.AccountReserves, paymentAmount
                    )
                );

            var schedule = PaymentCardFactory.CreateSchedule(
                groupNumber, customerId, paymentCardToken,
                firstPaymentDate, paymentCount, paymentFrequency,
                paymentAmount, allocations
                );

            return schedule;
        }

    }
}