using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace FinanceTrackerWithInterfaces
{
    // Интерфейсы
    public interface ITransaction
    {
        string Category { get; }
        double Amount { get; }
    }

    public interface ITransactionCategory
    {
        void AddCategory(string category);
        List<string> GetCategories();
    }

    public interface INotification
    {
        void ShowNotification(string message);
    }

    public interface IFinanceTracker : ITransactionCategory, INotification
    {
        void SetIncome(double newIncome);
        void AddTransaction(string category, double amount);
        double CalculateTotal();
        List<ITransaction> GetTransactions();
        double GetIncome();
    }

    // Реализация финансового трекера
    public class FinanceTracker : IFinanceTracker
    {
        private double income = 0;
        private readonly List<ITransaction> transactions = new List<ITransaction>();
        private readonly List<string> categories = new List<string>();

        public void SetIncome(double newIncome)
        {
            income = newIncome;
        }

        public void AddTransaction(string category, double amount)
        {
            double totalSpent = CalculateTotal();
            if (totalSpent + amount > income)
            {
                ShowNotification("Сумма транзакции не может превышать доход");
                return;
            }
            transactions.Add(new Transaction(category, amount));
        }

        public double CalculateTotal()
        {
            return transactions.Sum(t => t.Amount);
        }

        public List<ITransaction> GetTransactions()
        {
            return transactions;
        }

        public double GetIncome()
        {
            return income;
        }

        public void AddCategory(string category)
        {
            if (!categories.Contains(category))
            {
                categories.Add(category);
            }
        }

        public List<string> GetCategories()
        {
            return categories;
        }

        public void ShowNotification(string message)
        {
            MessageBox.Show(message, "Уведомление", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private class Transaction : ITransaction
        {
            public string Category { get; }
            public double Amount { get; }

            public Transaction(string category, double amount)
            {
                Category = category;
                Amount = amount;
            }
        }
    }

    // Главное окно приложения
    public class MainForm : Form
    {
        private readonly IFinanceTracker financeTracker;
        private ComboBox categoryComboBox;
        private TextBox amountTextBox;
        private TextBox incomeTextBox;
        private Button addTransactionButton;
        private Button updateTotalButton;
        private ListBox transactionListBox;
        private Label totalLabel;
        private Button showSummaryButton;
        private Button showTableButton;
        private Form summaryForm;
        private Form tableForm;

        public MainForm(IFinanceTracker tracker)
        {
            financeTracker = tracker;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Финансовый трекер";
            Size = new System.Drawing.Size(420, 500);

            Label incomeLabel = new Label
            {
                Text = "Доход:",
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(80, 25)
            };
            Controls.Add(incomeLabel);

            incomeTextBox = new TextBox
            {
                Location = new System.Drawing.Point(100, 10),
                Size = new System.Drawing.Size(100, 25)
            };
            Controls.Add(incomeTextBox);

            Button setIncomeButton = new Button
            {
                Text = "Установить доход",
                Location = new System.Drawing.Point(210, 10),
                Size = new System.Drawing.Size(150, 30)
            };
            setIncomeButton.Click += SetIncomeButton_Click;
            Controls.Add(setIncomeButton);

            Label categoryLabel = new Label
            {
                Text = "Категория:",
                Location = new System.Drawing.Point(10, 50),
                Size = new System.Drawing.Size(80, 25)
            };
            Controls.Add(categoryLabel);

            categoryComboBox = new ComboBox
            {
                Location = new System.Drawing.Point(100, 50),
                Size = new System.Drawing.Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            categoryComboBox.Items.AddRange(new string[] { "Развлечения", "Еда", "Транспорт", "Одежда", "Жилье", "Здоровье", "Образование", "Прочее" });
            Controls.Add(categoryComboBox);

            Label amountLabel = new Label
            {
                Text = "Сумма:",
                Location = new System.Drawing.Point(10, 90),
                Size = new System.Drawing.Size(80, 25)
            };
            Controls.Add(amountLabel);

            amountTextBox = new TextBox
            {
                Location = new System.Drawing.Point(100, 90),
                Size = new System.Drawing.Size(100, 25)
            };
            Controls.Add(amountTextBox);

            addTransactionButton = new Button
            {
                Text = "Добавить транзакцию",
                Location = new System.Drawing.Point(210, 90),
                Size = new System.Drawing.Size(150, 30)
            };
            addTransactionButton.Click += AddTransactionButton_Click;
            Controls.Add(addTransactionButton);

            updateTotalButton = new Button
            {
                Text = "Обновить общую сумму",
                Location = new System.Drawing.Point(10, 130),
                Size = new System.Drawing.Size(200, 30)
            };
            updateTotalButton.Click += UpdateTotalButton_Click;
            Controls.Add(updateTotalButton);

            showSummaryButton = new Button
            {
                Text = "Показать сводку",
                Location = new System.Drawing.Point(220, 130),
                Size = new System.Drawing.Size(150, 30)
            };
            showSummaryButton.Click += ShowSummaryButton_Click;
            Controls.Add(showSummaryButton);

            showTableButton = new Button
            {
                Text = "Показать таблицу",
                Location = new System.Drawing.Point(10, 170),
                Size = new System.Drawing.Size(200, 30)
            };
            showTableButton.Click += ShowTableButton_Click;
            Controls.Add(showTableButton);

            transactionListBox = new ListBox
            {
                Location = new System.Drawing.Point(10, 210),
                Size = new System.Drawing.Size(360, 200)
            };
            Controls.Add(transactionListBox);

            totalLabel = new Label
            {
                Location = new System.Drawing.Point(10, 420),
                Size = new System.Drawing.Size(360, 30),
                Text = $"Общая сумма потраченных денег: {financeTracker.CalculateTotal()}",
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };
            Controls.Add(totalLabel);
        }

        private void SetIncomeButton_Click(object sender, EventArgs e)
        {
            if (double.TryParse(incomeTextBox.Text, out double income))
            {
                financeTracker.SetIncome(income);
                MessageBox.Show("Доход успешно установлен!", "Уведомление", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите корректную сумму дохода.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddTransactionButton_Click(object sender, EventArgs e)
        {
            if (categoryComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите категорию.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!double.TryParse(amountTextBox.Text, out double amount))
            {
                MessageBox.Show("Пожалуйста, введите корректную сумму транзакции.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string category = categoryComboBox.SelectedItem.ToString();
            financeTracker.AddTransaction(category, amount);

            UpdateTransactionsListBox();
            totalLabel.Text = $"Общая сумма потраченных денег: {financeTracker.CalculateTotal()}";
        }

        private void UpdateTotalButton_Click(object sender, EventArgs e)
        {
            totalLabel.Text = $"Общая сумма потраченных денег: {financeTracker.CalculateTotal()}";
        }

        private void ShowSummaryButton_Click(object sender, EventArgs e)
        {
            if (summaryForm == null)
            {
                summaryForm = new SummaryForm(financeTracker);
                summaryForm.FormClosed += (s, args) => summaryForm = null;
                summaryForm.Show();
            }
            else
            {
                summaryForm.BringToFront();
            }
        }

        private void ShowTableButton_Click(object sender, EventArgs e)
        {
            if (tableForm == null)
            {
                tableForm = new TableForm(financeTracker);
                tableForm.FormClosed += (s, args) => tableForm = null;
                tableForm.Show();
            }
            else
            {
                tableForm.BringToFront();
            }
        }

        private void UpdateTransactionsListBox()
        {
            transactionListBox.Items.Clear();
            foreach (var transaction in financeTracker.GetTransactions())
            {
                transactionListBox.Items.Add($"Категория: {transaction.Category}, Сумма: {transaction.Amount}");
            }
        }
    }

    // Окно сводки
    public class SummaryForm : Form
    {
        private readonly IFinanceTracker financeTracker;

        public SummaryForm(IFinanceTracker tracker)
        {
            financeTracker = tracker;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Сводка транзакций";
            Size = new System.Drawing.Size(400, 300);

            Label summaryLabel = new Label
            {
                Text = "Сводка:",
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(80, 25)
            };
            Controls.Add(summaryLabel);

            Label incomeLabel = new Label
            {
                Text = $"Общий доход: {financeTracker.GetIncome()}",
                Location = new System.Drawing.Point(10, 50),
                Size = new System.Drawing.Size(200, 25)
            };
            Controls.Add(incomeLabel);

            Label spentLabel = new Label
            {
                Text = $"Общие расходы: {financeTracker.CalculateTotal()}",
                Location = new System.Drawing.Point(10, 90),
                Size = new System.Drawing.Size(200, 25)
            };
            Controls.Add(spentLabel);
        }
    }

    // Окно таблицы транзакций
    public class TableForm : Form
    {
        private readonly IFinanceTracker financeTracker;

        public TableForm(IFinanceTracker tracker)
        {
            financeTracker = tracker;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Таблица транзакций";
            Size = new System.Drawing.Size(400, 300);

            ListBox transactionListBox = new ListBox
            {
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(360, 240)
            };
            Controls.Add(transactionListBox);

            foreach (var transaction in financeTracker.GetTransactions())
            {
                transactionListBox.Items.Add($"Категория: {transaction.Category}, Сумма: {transaction.Amount}");
            }
        }
    }

    // Точка входа
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(new FinanceTracker()));
        }
    }
}
