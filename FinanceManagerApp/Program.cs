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
            Size = new System.Drawing.Size(400, 400);

            // Создание и настройка элементов управления

            // ComboBox для выбора категории транзакции
            categoryComboBox = new ComboBox
            {
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(200, 25),
                DropDownStyle = ComboBoxStyle.DropDownList // чтобы пользователь мог выбирать только из списка
            };
            // Добавление категорий по умолчанию
            categoryComboBox.Items.AddRange(new string[] { "Развлечения", "Еда", "Транспорт", "Одежда", "Жилье", "Здоровье", "Образование", "Прочее" });
            Controls.Add(categoryComboBox);

            // TextBox для ввода суммы транзакции
            amountTextBox = new TextBox
            {
                Location = new System.Drawing.Point(10, 45),
                Size = new System.Drawing.Size(200, 25)
            };
            Controls.Add(amountTextBox);

            // TextBox для ввода заработка
            incomeTextBox = new TextBox
            {
                Location = new System.Drawing.Point(10, 80),
                Size = new System.Drawing.Size(200, 25),
                Text = "Введите ваш заработок"
            };
            incomeTextBox.Enter += IncomeTextBox_Enter; // Добавляем обработчик события входа в текстовое поле
            incomeTextBox.Leave += IncomeTextBox_Leave; // Добавляем обработчик события выхода из текстового поля
            Controls.Add(incomeTextBox);

            // Button для добавления транзакции
            addTransactionButton = new Button
            {
                Location = new System.Drawing.Point(10, 120),
                Size = new System.Drawing.Size(200, 25),
                Text = "Добавить транзакцию"
            };
            addTransactionButton.Click += AddTransactionButton_Click; // добавляем обработчик события нажатия кнопки
            Controls.Add(addTransactionButton);

            // ListBox для отображения списка транзакций
            transactionListBox = new ListBox
            {
                Location = new System.Drawing.Point(10, 160),
                Size = new System.Drawing.Size(350, 200)
            };
            Controls.Add(transactionListBox);

            // Label для отображения общей суммы потраченных денег
            totalLabel = new Label
            {
                Location = new System.Drawing.Point(10, 370),
                Size = new System.Drawing.Size(350, 20),
                Text = $"Общая сумма потраченных денег: {financeTracker.calculateTotal()}",
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };
            Controls.Add(totalLabel);
        }

        private void AddTransactionButton_Click(object sender, EventArgs e)
        {
            // Обработчик события нажатия кнопки "Добавить транзакцию"

            // Получаем выбранную категорию и введенную сумму
            string category = categoryComboBox.SelectedItem.ToString();
            double amount = 0;
            if (!double.TryParse(amountTextBox.Text, out amount))
            {
                MessageBox.Show("Пожалуйста, введите корректную сумму.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Получаем введенный заработок
            double income = 0;
            if (!double.TryParse(incomeTextBox.Text, out income))
            {
                MessageBox.Show("Пожалуйста, введите корректный заработок.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            financeTracker.setIncome(income); // Установить введенный заработок

            // Добавляем транзакцию
            financeTracker.addTransaction(category, amount);

            // Обновляем список транзакций и общую сумму потраченных денег
            UpdateTransactionsListBox();
            totalLabel.Text = $"Общая сумма потраченных денег: {financeTracker.calculateTotal()}";
        }

        private void UpdateTransactionsListBox()
        {
            // Очищаем список транзакций и добавляем новые элементы
            transactionListBox.Items.Clear();
            foreach (var transaction in financeTracker.getTransactions())
            {
                transactionListBox.Items.Add($"Категория: {transaction.Category}, Сумма: {transaction.Amount}");
            }
        }

        private void IncomeTextBox_Enter(object sender, EventArgs e)
        {
            // Обработчик события входа в текстовое поле "Заработок"

            if (incomeTextBox.Text == "Введите ваш заработок")
            {
                incomeTextBox.Text = "";
            }
        }

        private void IncomeTextBox_Leave(object sender, EventArgs e)
        {
            // Обработчик события выхода из текстового поля "Заработок"

            if (incomeTextBox.Text == "")
            {
                incomeTextBox.Text = "Введите ваш заработок";
            }
        }
    }
}
        
    



