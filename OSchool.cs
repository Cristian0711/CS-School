using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualBasic.CompilerServices;
using System.IO;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace ConsoleApp3
{
    public class OSchool
    {
        public struct orar
        {
            public int day;
            public List<string> hours;

            public void init()
            {
                hours = new List<string>();
            }
        }

        public struct ore
        {
            public int startMin, startHour, endMin, endHour;
            public void clear()
            {
                startMin = 0;
                startHour = 0;
                endHour = 0;
                endMin = 0;
            }
        }

        public struct Links
        {
            public string className, classLink;
            public static implicit operator Links(string value)
            {
                return new Links() { className = value };
            }
        }

        public string mail, passwd, nClass;

        public List<orar> oreOrar = new List<orar>();
        public List<ore> programOre = new List<ore>();
        public List<Links> links = new List<Links>();

        public bool Login(IWebDriver driver, string email, string password)
        {
            CUtils utils = new CUtils();

            if (!utils.driverExists(driver))
                return false;

            string chrome = "https://accounts.google.com/login";
            driver.Navigate().GoToUrl(chrome);

            int failCount = 0;
            if (!utils.driverExists(driver))
                return false;

            while (utils.driverExists(driver) && !utils.elementExists(driver, By.Id("identifierId")))
            {
                if (failCount >= 5)
                    return false;
                failCount++;
                System.Threading.Thread.Sleep(2000);
            }

            if (utils.driverExists(driver))
                driver.FindElement(By.Id("identifierId")).SendKeys(email);
            else
                return false;
            failCount = 0;


            while (utils.driverExists(driver) && !utils.elementExists(driver, By.Id("identifierNext")))
            {
                if (failCount >= 5)
                    return false;
                failCount++;
                System.Threading.Thread.Sleep(2000);
            }

            if (utils.driverExists(driver))
                driver.FindElement(By.Id("identifierNext")).Click();
            else
                return false;
            failCount = 0;

            while (utils.driverExists(driver) && !utils.elementExists(driver, By.Name("password")))
            {
                if (failCount >= 5)
                    return false;
                failCount++;
                System.Threading.Thread.Sleep(2000);
            }
            if (utils.driverExists(driver))
                driver.FindElement(By.Name("password")).SendKeys(password);
            else
                return false;
            failCount = 0;

            while (utils.driverExists(driver) && !utils.elementExists(driver, By.Id("passwordNext")))
            {
                if (failCount >= 5)
                    return false;
                failCount++;
                System.Threading.Thread.Sleep(2000);
            }
            if (utils.driverExists(driver))
                driver.FindElement(By.Id("passwordNext")).Click();
            else
                return false;

            failCount = 0;

            while (utils.driverExists(driver) && utils.elementExists(driver, By.Id("passwordNext")))
            {
                if (failCount >= 5)
                    return false;
                failCount++;
                System.Threading.Thread.Sleep(1000);
            }
            return true;
        }

        public bool GetSchedule(IWebDriver driver, ref List<orar> ore, ref List<ore> l_ore, string schoolClass)
        {
            CUtils utils = new CUtils();
            if (!utils.driverExists(driver))
                return false;

            driver.Navigate().GoToUrl("http://orar.ienachita.com/");
            new SelectElement(driver.FindElement(By.Name("forms"))).SelectByText(schoolClass);
            System.Threading.Thread.Sleep(500);

            ore m_ore = new ore();
            m_ore.clear();
            for (int i = 0; i <= 7; i++)
            {
                if (utils.driverExists(driver))
                {
                    By by = By.XPath("/html/body/table/tbody/tr/td[2]/table/tbody/tr[2]/td[" + i + "]/small");
                    if (utils.elementExists(driver, by))
                    {
                        string hoursText = driver.FindElement(by).Text;
                        m_ore.clear();

                        int j = 0;
                        foreach (char c in hoursText)
                        {
                            if (Char.IsDigit(c))
                            {
                                int cifra = int.Parse(c.ToString());
                                if (j == 0 || j == 1)
                                {
                                    m_ore.startHour = m_ore.startHour * 10 + cifra;
                                }
                                else if (j == 2 || j == 3)
                                {
                                    m_ore.startMin = m_ore.startMin * 10 + cifra;
                                }
                                else if (j == 4 || j == 5)
                                {
                                    m_ore.endHour = m_ore.endHour * 10 + cifra;
                                }
                                else if (j == 6 || j == 7)
                                {
                                    m_ore.endMin = m_ore.endMin * 10 + cifra;
                                }
                                j++;
                            }
                        }
                        l_ore.Add(m_ore);
                    }
                }
                else
                    return false;
            }

            //Get the hours
            for (int j = 3; j <= 7; j++)
            {
                orar gOrar = new orar();
                gOrar.init();
                for (int i = 2; i <= 7; i++)
                {
                    if (utils.driverExists(driver))
                    {
                        By by = By.XPath("//table/tbody/tr/td[2]/table/tbody/tr[" + j + "]/td[" + i + "]");
                        if (utils.elementExists(driver, by))
                        {
                            if (driver.FindElement(by).Text != null)
                            {
                                int hoursCount = Int32.Parse(driver.FindElement(by).Size.Width.ToString());
                                hoursCount /= 150;
                                string ora = driver.FindElement(by).Text;
                                for (int k = 0; k < hoursCount; k++)
                                {
                                    string name = "";
                                    foreach (char c in ora)
                                    {
                                        if (c != '\n')
                                            name += c;
                                        else
                                            break;
                                    }
                                    gOrar.hours.Add(name);
                                }
                            }
                        }
                    }
                    else
                        return false;
                }
                ore.Add(gOrar);
            }
            return true;
        }

        public bool JoinClass(IWebDriver driver, string link, int startTime, int endTime)
        {
            CUtils utils = new CUtils();

            driver.Navigate().GoToUrl(link);

            int failCount = 0;
            while (utils.driverExists(driver) && !utils.elementExists(driver, By.Id("yDmH0d")))
            {
                if (failCount >= 5)
                    return false;
                failCount++;
                System.Threading.Thread.Sleep(2000);
            }
            failCount = 0;

            while (utils.driverExists(driver) && utils.getBetween(driver.FindElement(By.Id("yDmH0d")).Text, "https://meet.google.com/lookup/", "\n") == "")
            {
                int currentTimeInMin = DateTime.Now.Hour * 60 + DateTime.Now.Minute;
                if (currentTimeInMin >= startTime && endTime > currentTimeInMin)
                {

                }
                else
                    return false;
                System.Threading.Thread.Sleep(500);
            }
            if (utils.driverExists(driver))
                driver.Navigate().GoToUrl("https://" + utils.getBetween(driver.FindElement(By.Id("yDmH0d")).Text, "https://", "\n"));
            else
                return false;

            string XPath = "//*[@id=\"yDmH0d\"]/c-wiz/div/div/div[5]/div[3]/div/div/div[2]/div/div/div[1]/div/div[4]/div[1]/div/div/div";
            while (utils.driverExists(driver) && !utils.elementExists(driver, By.XPath(XPath)))
            {
                if (failCount >= 5)
                    return false;
                failCount++;
                System.Threading.Thread.Sleep(2000);
            }
            if (utils.driverExists(driver))
                driver.FindElement(By.XPath(XPath)).Click();
            else
                return false;
            failCount = 0;

            XPath = "//*[@id=\"yDmH0d\"]/c-wiz/div/div/div[5]/div[3]/div/div/div[2]/div/div/div[2]/div/div[2]/div/div[1]/div[1]";

            while (utils.driverExists(driver) && !utils.elementExists(driver, By.XPath(XPath)))
            {
                if (failCount >= 5)
                    return false;
                failCount++;
                System.Threading.Thread.Sleep(2000);
            }
            System.Threading.Thread.Sleep(2000);
            if (utils.driverExists(driver))
                driver.FindElement(By.XPath(XPath)).Click();
            else
                return false;
            failCount = 0;

            return true;
        }

        public void loadLinks()
        {
            FileStream fs = File.Open("links.txt", FileMode.Open);
            if (fs != null)
            {
                var sr = new StreamReader(fs);

                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    Links link = new Links();
                    link.className = line;
                    link.classLink = sr.ReadLine();
                    links.Add(link);
                }
            }
        }

        public void loadAccount()
        {
            CUtils utils = new CUtils();
            if (!File.Exists("account.txt"))
            {
                Console.WriteLine("It seems that you don't have an account, enter your credentials!");
                Console.WriteLine("Email:");
                mail = Console.ReadLine();
                Console.WriteLine("Password:");
                passwd = Console.ReadLine();
                Console.WriteLine("Class (11 C):");
                nClass = Console.ReadLine();

                FileStream fs = File.Create("account.txt");
                var sr = new StreamWriter(fs);

                sr.WriteLine(mail);
                sr.WriteLine(utils.EncodePasswordToBase64(passwd));
                sr.WriteLine(nClass);

                sr.Close();

                Console.Clear();
                Console.WriteLine("Credentials have been saved succesfully!");
            }
            else
            {
                FileStream fs = File.Open("account.txt", FileMode.Open);
                var sr = new StreamReader(fs);

                mail = sr.ReadLine();
                passwd = utils.DecodeFrom64(sr.ReadLine());
                nClass = sr.ReadLine();

                sr.Close();

                Console.WriteLine("Credentials loaded!");
            }
        }
    }
}
