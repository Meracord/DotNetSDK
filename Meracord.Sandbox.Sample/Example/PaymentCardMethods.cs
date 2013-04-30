using System;
using Meracord.Sandbox.Factories;
using Meracord.Sandbox.Helpers;
using NoteWorld.DataServices;
using NoteWorld.DataServices.Common.Factories;
using Transport = NoteWorld.DataServices.Common.Transport;
using NoteWorld.DataServices.Common.Enumeration;
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
        public static void Perform(string clientId)
        {
            try
            {
                var groupNumber = Settings.GroupNumber;

                var session = SessionFactory.Create();

                // Build a PaymentCard object
                var paymentCard = GetPaymentCard();

                // Build a CardPaymentSchedule object
                var schedule = GetCardPaymentSchedule(groupNumber, clientId, paymentCard);

                // Call API method to create card payment schedule
                CreateSchedule(session, schedule);

                // Test Schedule.Validate()
                PaymentCardValidate(schedule);

                // Call API method to discover payment card profiles
                // The card returned will be the one created in CreateSchedule call above.
                // Note that activating a card profile is an asynchronise process dependant on processes outside of Meracord,
                // so we may have to wait a little bit before it is activated.
                var cardProfile = FindForAccount(session, groupNumber, clientId);

                // Call API method to create card payment schedule uaing an existing card profile
                CreateScheduleWithToken(session, groupNumber, clientId, cardProfile);

            }
            catch (Exception ex)
            {
                Helper.DisplayException(ex);
            }
        }

        private static void CreateSchedule(DataSession session, Transport.CardPaymentSchedule schedule)
        {
            // Test CardPaymentSchedule to see if it is valid
            schedule.Validate();

            // Call PaymentCard.CreateSchedule()
            Helper.ShowResults("PaymentCard.CreateSchedule()", session.PaymentCard.CreateSchedule(schedule));
        }

        private static void CreateScheduleWithToken(DataSession session, string groupNumber, string clientId, Transport.PaymentCardProfileReference profile)
        {
            var paymentCardToken = profile.PaymentCardToken;
            var schedule = GetCardPaymentScheduleWithExistingProfile(groupNumber, clientId, paymentCardToken);

            // Test CardPaymentSchedule to see if it is valid
            schedule.Validate();

            // Call PaymentCard.CreateSchedule()
            Helper.ShowResults("PaymentCard.CreateScheduleWithToken()", session.PaymentCard.CreateScheduleWithToken(schedule));
        }

        private static Transport.PaymentCardProfileReference FindForAccount(DataSession session, string groupNumber, string clientId)
        {
            var tries = 4;
            while (tries > 0)
            {
                var paymentCardProfiles = session.PaymentCard.FindForAccount(groupNumber, clientId);
                var activeProfile = paymentCardProfiles.ToList().FirstOrDefault(x => x.IsActive);

                if (activeProfile != null)
                {
                    Helper.ShowResults("PaymentCard.FindForAccount()", paymentCardProfiles);
                    return activeProfile;
                }

                Console.WriteLine("PaymentCard profile not active.  Retry in 2 seconds...");
                System.Threading.Thread.Sleep(2000); // wait 2 seconds
                tries--;
            }

            throw new ApplicationException("Timeout reached waiting for PaymentCard profile to become active. It is possible the profile will never be activated do to processing constraints.");
        }

        private static void PaymentCardValidate(Transport.CardPaymentSchedule schedule)
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
        private static Transport.Document GetDocument()
        {
            var documentPath = Settings.DocumentPath;
            return DocumentFactory.Create(documentPath, DocumentType.PaymentCardAuthorization);
        }

        /// <summary>
        /// Helper method to create a PaymentCard object
        /// </summary>
        private static Transport.PaymentCard GetPaymentCard()
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
        private static Transport.CardPaymentSchedule GetCardPaymentSchedule(string groupNumber, string clientId, Transport.PaymentCard paymentCard)
        {
            var paymentFrequency = PaymentFrequency.Monthly;
            var paymentCount = 2;
            var firstPaymentDate = DateTime.Today;
            var paymentAmount = 525.50;
            var agreement = GetDocument();

            var allocations = PaymentCardFactory.NewAllocationList();

            allocations.Add(
                DebitFactory.CreateAllocation(
                    Transport.AssignmentCode.AccountReserves, paymentAmount
                    )
                );

            var schedule = PaymentCardFactory.CreateSchedule(
                groupNumber, clientId, paymentCard,
                firstPaymentDate, paymentCount, paymentFrequency,
                paymentAmount, agreement, allocations
                );

            return schedule;
        }

        /// <summary>
        /// Helper method to create a CardPaymentScheduleWithExistingProfile object
        /// </summary>
        private static Transport.CardPaymentScheduleWithToken GetCardPaymentScheduleWithExistingProfile(string groupNumber, string clientId, Guid paymentCardToken)
        {
            const PaymentFrequency paymentFrequency = PaymentFrequency.Monthly;
            const int paymentCount = 2;
            var firstPaymentDate = DateTime.Today;
            const double paymentAmount = 525.50;

            var allocations = PaymentCardFactory.NewAllocationList();

            allocations.Add(
                DebitFactory.CreateAllocation(
                    Transport.AssignmentCode.AccountReserves, paymentAmount
                    )
                );

            var schedule = PaymentCardFactory.CreateSchedule(
                groupNumber, clientId, paymentCardToken,
                firstPaymentDate, paymentCount, paymentFrequency,
                paymentAmount, allocations
                );

            return schedule;
        }

    }
}