using NewsAlertDeletor.Enums;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAlertDeletor
{
    class Program
    {
        static void Main(string[] args)
        {
            var login = "";
            var password = "";

            using (IWebDriver driver = new ChromeDriver(@"..\..\..\.."))
            {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));

                //Login to Google Account and get redirected to Alerts page
                driver.Navigate().GoToUrl("https://accounts.google.com/ServiceLogin?hl=en&passive=true&continue=https://www.google.com/alerts&service=alerts");

                IWebElement identifier = driver.FindElement(By.Id("identifierId"));
                identifier.SendKeys(login);
                SafeClick(driver, ByType.Id, "identifierNext");

                IWebElement passwordInput = driver.FindElement(By.XPath("//*[@id='password']/div[1]/div/div[1]/input"));
                passwordInput.SendKeys(password);
                SafeClick(driver, ByType.Id, "passwordNext");

                //Find the alerts container
                IWebElement alerts = driver.FindElement(By.Id("manage-alerts-div"));
                int item = 1;
                try
                {
                    while (true)
                    {
                        SafeClick(driver, ByType.XPath, $"//*[@id='manage-alerts-div']/ul/li[{item}]/div[1]/div/div[2]/span[2]");
                        System.Threading.Thread.Sleep(500);
                        item++;
                    }
                }
                catch (NoSuchElementException)
                {
                    Console.WriteLine("No more alerts to delete!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("New exception thrown!");
                    Console.WriteLine(ex.Message);
                    //Keep the process alive when an error occurs
                    while (true) ;
                }
            }
            Environment.Exit(0);
        }

        private static void SafeClick(IWebDriver driver, ByType bt, string searchStr)
        {
            bool success = false;
            IWebElement elem;
            while (!success)
            {
                elem = FindByType(driver, bt, searchStr);
                try
                {
                    elem.Click();
                    success = true;
                }
                catch (StaleElementReferenceException)
                {
                    Console.WriteLine("Stale: " + searchStr);
                }
                catch (ElementClickInterceptedException)
                {
                    Console.WriteLine("Intercepted: " + searchStr);
                }
                catch (ElementNotInteractableException ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(searchStr);
                    Console.WriteLine();
                }
            }
        }

        private static IWebElement FindByType(IWebDriver driver, ByType bt, string searchStr)
        {
            switch (bt)
            {
                case ByType.Id:
                    return driver.FindElement(By.Id(searchStr));

                case ByType.XPath:
                    return driver.FindElement(By.XPath(searchStr));

                case ByType.ClassName:
                    return driver.FindElement(By.ClassName(searchStr));
            }
            return null;
        }
    }
}
