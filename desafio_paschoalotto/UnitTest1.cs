using System;
using System.Data;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

[TestFixture]
public class TypingTest
{
    private IWebDriver driver;

    [SetUp]
    public void Setup()
    {
        driver = new ChromeDriver();
    }

    [TearDown]
    public void TearDown()
    {
        driver.Quit();
    }

    [Test]
    public void TypingTestScenario()
    {
        driver.Navigate().GoToUrl("https://10fastfingers.com/typing-test/portuguese");

        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));

        IWebElement inputField = driver.FindElement(By.Id("inputfield"));

        // Aguarde até que o campo de entrada esteja visível
        wait.Until(driver => inputField.Displayed);

        // Inicia o timer pressionando a barra de espaço pela primeira vez
        inputField.SendKeys(" ");
        wait.Until(driver => driver.FindElement(By.Id("timer")).Text != "1:00");

        IWebElement wordsContainer = driver.FindElement(By.Id("row1"));
        var words = wordsContainer.FindElements(By.TagName("span"));

        foreach (var word in words)
        {
            foreach (char letter in word.Text)
            {
                inputField.SendKeys(letter.ToString());
                System.Threading.Thread.Sleep(100);
            }
            inputField.SendKeys(" "); // Adiciona espaço após cada palavra
            System.Threading.Thread.Sleep(100);
        }

        // Aguarde até que a tabela de resultados seja exibida
        wait.Until(driver => driver.FindElement(By.Id("wpm")).Displayed);

        // Captura as informações da tabela de resultados
        IWebElement resultTable = driver.FindElement(By.TagName("tbody"));

        // Captura o valor do WPM dentro da tag <strong>
        string wpm = resultTable.FindElement(By.Id("wpm")).FindElement(By.TagName("strong")).Text;

        // Captura o valor de Keystrokes_Bom da classe "correct"
        string keystrokesCorrect = resultTable.FindElement(By.Id("keystrokes")).FindElement(By.ClassName("correct")).Text;

        // Captura o valor de Keystrokes_Ruim da classe "wrong"
        string keystrokesWrong = resultTable.FindElement(By.Id("keystrokes")).FindElement(By.ClassName("wrong")).Text;

        // Captura o valor de Keystrokes_Total do <small>
        string keystrokesTotal = resultTable.FindElement(By.Id("keystrokes")).Text;
        keystrokesTotal = keystrokesTotal.Substring(keystrokesTotal.LastIndexOf(')') + 1).Trim();

        // Captura as demais informações
        string accuracy = resultTable.FindElement(By.Id("accuracy")).Text;
        string correctWords = resultTable.FindElement(By.Id("correct")).Text;
        string wrongWords = resultTable.FindElement(By.Id("wrong")).Text;

        // Extrai apenas os números de Accuracy, Correct words e Wrong words
        string accuracyValue = GetNumericValue(accuracy);
        string correctWordsValue = GetNumericValue(correctWords);
        string wrongWordsValue = GetNumericValue(wrongWords);

        // Imprime as informações
        Console.WriteLine("Words per Minute: " + wpm);
        Console.WriteLine("Keystrokes_Bom: " + keystrokesCorrect);
        Console.WriteLine("Keystrokes_Ruim: " + keystrokesWrong);
        Console.WriteLine("Keystrokes_Total: " + keystrokesTotal);
        Console.WriteLine("Accuracy: " + accuracyValue + "%");
        Console.WriteLine("Correct words: " + correctWordsValue);
        Console.WriteLine("Wrong words: " + wrongWordsValue);

        // Inicializa a conexão com o banco de dados
        string connectionString = "server=localhost;database=desafio_paschoalotto;uid=root;";
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            // Crie uma consulta SQL para inserir os dados
            string insertQuery = "INSERT INTO resultado (wpm, keystrokesCorrect, keystrokesWrong, keystrokesTotal, accuracy, correctWords, wrongWords) VALUES (@wpm, @keystrokesCorrect, @keystrokesWrong, @keystrokesTotal, @accuracy, @correctWords, @wrongWords)";

            // Use um comando SQL parametrizado para evitar injeção de SQL
            using (MySqlCommand cmd = new MySqlCommand(insertQuery, connection))
            {
                cmd.Parameters.AddWithValue("@wpm", wpm);
                cmd.Parameters.AddWithValue("@keystrokesCorrect", keystrokesCorrect);
                cmd.Parameters.AddWithValue("@keystrokesWrong", keystrokesWrong);
                cmd.Parameters.AddWithValue("@keystrokesTotal", keystrokesTotal);
                cmd.Parameters.AddWithValue("@accuracy", accuracyValue);
                cmd.Parameters.AddWithValue("@correctWords", correctWordsValue);
                cmd.Parameters.AddWithValue("@wrongWords", wrongWordsValue);

                // Execute a consulta
                cmd.ExecuteNonQuery();
            }
        }
    }

    private string GetNumericValue(string input)
    {
        // Extrai apenas os números de uma string
        return new string(input.Where(char.IsDigit).ToArray());
    }
}
