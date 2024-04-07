using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FinanceTrackerWithInterface
{
    public interface ITransaction // интерфейс для транзакции
    {
        string Category { get; } // свойство для категории
        double Amount { get; } // свойство для суммы
    }

    public interface ITransactionCategory // интерфейс для категорий транзакций
    {
        void addCategory(string category); // метод для добавления категории
        List<string> getCategories(); // метод для получения списка категорий
    }

    public interface INotification // интерфейс для уведомлений
    {
        void showNotification(string message); // метод для показа уведомления
    }

    public class FinanceTracker : ITransaction, ITransactionCategory, INotification
    {
        private double income = 0; // общий доход от транзакций
        private List<ITransaction> transactions = new List<ITransaction>(); // список транзакций
        private List<string> categories = new List<string>(); // список категорий

        // Реализация свойства Category из интерфейса ITransaction
        public string Category { get; private set; }

        // Реализация свойства Amount из интерфейса ITransaction
        public double Amount { get; private set; }

        public void setIncome(double newIncome)
        {
            income = newIncome; // установить новый доход
        }

        public void addTransaction(string category, double amount)
        {
            double totalSpent = calculateTotal();
            if (totalSpent + amount > income)
            {
                showNotification("Сумма транзакции не может превышать доход");
                return;
            }
            transactions.Add(new Transaction(category, amount));
        }

        public double calculateTotal()
        {
            double total = 0;
            foreach (var transaction in transactions)
            {
                total += transaction.Amount;
            }
            return total;
        }

        public List<ITransaction> getTransactions()
        {
            return transactions;
        }

        public double getIncome()
        {
            return income;
        }

        public void addCategory(string category)
        {
            if (category != "Общие" && category != "Одиночные")
            {
                categories.Add(category);
            }
        }

        public List<string> getCategories()
        {
            return categories;
        }

        public void showNotification(string message)
        {
            MessageBox.Show(message, "Уведомление", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private class Transaction : ITransaction
        {
            public string Category { get; } // реализация свойства Category
            public double Amount { get; } // реализация свойства Amount

            public Transaction(string category, double amount)
            {
                Category = category;
                Amount = amount;
            }
        }
    }
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
    public class MainForm : Form
    {
        private readonly FinanceTracker financeTracker = new FinanceTracker();
        private ComboBox categoryComboBox;
        private TextBox amountTextBox;
        private TextBox incomeTextBox;
        private Button addTransactionButton;
        private ListBox transactionListBox;
        private Label totalLabel;

        public MainForm()
        {
            InitializeComponent();
        }
        private void InitializeComponent()
        {
            Text = "Отслеживание финансов";
        }
    }

}

