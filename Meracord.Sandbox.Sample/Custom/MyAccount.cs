using System;
using NoteWorld.DataServices.Common.Enumeration;

namespace Meracord.Sandbox.Custom
{
    /// <summary>
    /// Example of custom class that can be instantiated using Link-to-Entities and Meracord API
    /// </summary>
    internal class MyAccount
    {
        public MyAccount(string accountNumber, double reserveBalance, DateTime changeDate, int holdType, int accountStatus)
        {
            AccountNumber = accountNumber;
            ReserveBalance = reserveBalance;
            ChangeDate = changeDate;
            HoldType = (HoldType)holdType;
            AccountStatus = (AccountStatus)accountStatus;
        }

        public string AccountNumber { get; private set; }
        public double ReserveBalance { get; private set; }
        public DateTime ChangeDate { get; private set; }
        public HoldType HoldType { get; private set; }
        public AccountStatus AccountStatus { get; private set; }
    }
}
