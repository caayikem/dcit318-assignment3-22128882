using System;
using System.Collections.Generic;

namespace financeManagementSystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            FinanceApp financeApp = new FinanceApp();
            financeApp.Run();
        }
    }

    // record type for Transaction

    public record Transaction
    
       ( 
        int Id,  
         DateTime Date,
         decimal Amount, 
         string Category
        );

    // interface for payment processing

    public interface ITransactionProcessor
    {
        void Process(Transaction transaction);
    }

    // implementation for bank transfer, mobile money and crypto wallet

    public class BankTransferProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine
                ($"\n\nBank Transfer: \n-----------\n Amount: GHC {transaction.Amount:N2} \n Category: {transaction.Category} \n Date: {transaction.Date}");
        }
    }

    public class MobileMoneyProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine
                ($"\n\nMobile Money: \n------------\n Amount: GHC {transaction.Amount:N2} \n Category: {transaction.Category} \n Date: {transaction.Date}");
        }
    }

    public class CryptoWalletProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine
                ($"\n\nCrypto Wallet: \n-------------\n Amount: GHC {transaction.Amount:N2} \n Category: {transaction.Category} \n Date: {transaction.Date}");
        }
    }

    // general and sealed accounts

    public class Account
    {
        public string AccountNumber { get; set; }
        public decimal Balance { get; protected set; }
        public Account(string accountNumber, decimal initialbalance)
        {
          AccountNumber = accountNumber;
          Balance = initialbalance;
        }

        public virtual void ApplyTransaction(Transaction transaction)
        {
            Balance -= transaction.Amount;
        }

       
    }

    public sealed class SavingsAccount : Account
    {
        public SavingsAccount(string accountNumber, decimal initialbalance) : base(accountNumber, initialbalance)
        {

        }

        public override void ApplyTransaction(Transaction transaction)
        {
            if (transaction.Amount > Balance)
            {
                Console.WriteLine($"Insufficient funds. Could not complete transaction. Balance: GHC {Balance:N2}");
            }

            else
            {
                Balance -= transaction.Amount;
                Console.WriteLine($"Transaction completed successfully. Your current balance is now GHC {Balance:N2}");

            }

        }
    }

    // integrated and simlutaion

    public class FinanceApp
    {
        private List<Transaction> _transactions = new();

        public void Run()
        {
            SavingsAccount account1 = new SavingsAccount("800922120001", 1000.00m);
            Console.WriteLine($"Savings Account created successfully. \nAccount Number: {account1.AccountNumber} \nInitial Balance: GHC {account1.Balance:N2} \n Thank you for choosing our service.");

            // transaction records

            Transaction transact1 = new Transaction(1, DateTime.Now, 550.00m, "Groceries");
            Transaction transact2 = new Transaction(2, DateTime.Now, 350.00m, "Utilities");
            Transaction transact3 = new Transaction(3, DateTime.Now, 200.00m, "Entertainment");


            // bank transfer processor == btp, mobile money processor == mmp, crypto wallet processor == cwp
            
            BankTransferProcessor btp = new BankTransferProcessor();
            MobileMoneyProcessor mmp = new MobileMoneyProcessor();
            CryptoWalletProcessor cwp = new CryptoWalletProcessor();

            mmp.Process(transact1);
            account1.ApplyTransaction(transact1); 
            
            btp.Process(transact2);
            account1.ApplyTransaction(transact2);

            cwp.Process(transact3);
            account1.ApplyTransaction(transact3);



            _transactions.AddRange(new[] { transact1, transact2, transact3 });
        }
    }

}
