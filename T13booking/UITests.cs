using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace T13booking
{
    class UITests
    {
        // catches stale element for some reason even with implicit wait, for now using Thread.sleep
        private IWebDriver _driver;
        public IWebDriver driver
        {
            get { Thread.Sleep(1000); return _driver; }
            set { _driver = value; }
        }

        [SetUp]
        public void SetUp()
        {
            driver = new ChromeDriver(@"C:\Users\work\source\repos\task13\task13\T13booking");
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            driver.Navigate().GoToUrl("https://flights.booking.com/");
        }

        [Test]
        public void Test1()
        {

            // -Select english language in the language dropdown.
            driver.FindElement(By.XPath("//nav//button")).Click();
            driver.FindElement(By.XPath("//span[text()='English (US)']/..")).Click();

            // - Search for flights from Kyiv Borispil(KBP) to Copenhagen(CPH) for 1 adult person for the nearest date(one way)
            driver.FindElement(By.XPath("//div[text()='One-way']/../..")).Click(); // click one way
            driver.FindElement(By.XPath("//div[@data-testid='searchbox_origin']")).Click();
            driver.FindElement(By.XPath("//div[@data-testid='autocomplete_result']")).Click();
            driver.FindElement(By.XPath("//input[@data-testid='searchbox_origin_input']")).SendKeys("KBP");
            driver.FindElement(By.XPath("//div//span[text()='KBP']")).Click();
            //driver.FindElement(By.XPath("//div[@aria-label='Where to?']")).Click(); // this focuses automatically upon origin chosen
            driver.FindElement(By.XPath("//input[@data-testid='searchbox_destination_input']")).SendKeys("CPH");
            driver.FindElement(By.XPath("//div//span[text()='CPH']")).Click();
            //driver.FindElement(By.XPath("//div[@data-testid='searchbox_calendar']")).Click(); // click When? -- this focuses automatically upon destination chosen
            //driver.FindElement(By.XPath("//td[contains(@class, 'today')]")).Click(); // clicks on today's date, maybe not ideal solution
            driver.FindElement(By.XPath("//div[contains(@class, 'Calendar')][2]//span[text()='2']/..")).Click(); // let's click in near future for now
            driver.FindElement(By.XPath("//button[@data-testid='searchbox_submit']")).Click(); // click search

            /*- Ensure that correct flight is displayed in search results
            - Verify
                    'Results' box is displayed and shows text "<somenumber> results" in the filter in left sidebar
                    'Flight times'box is displayed and has texts "Departs from Міжнародний аеропорт «Бориспіль»" and "Arrives to Copenhagen Airport" in the filter in left sidebar
                    3 tabs in the results list are displayed: 'best', 'cheapest', 'fastest'
            */
            // search might not return results if today is almost over :)
            string actual = driver.FindElement(By.XPath("//div[contains(text(),'results')]")).Text;
            Assert.True(actual.Contains("results"));
            Assert.True(driver.PageSource.Contains("Departs from Boryspil International Airport"));
            Assert.True(driver.PageSource.Contains("Arrives to Copenhagen Airport"));
            string bestText = driver.FindElement(By.XPath("//div[contains(text(),'Best')]")).Text;
            Assert.True(bestText.Contains("Best"));
            string cheapestText = driver.FindElement(By.XPath("//div[contains(text(),'Cheapest')]")).Text;
            Assert.True(cheapestText.Contains("Cheapest"));
            string fastestText = driver.FindElement(By.XPath("//div[contains(text(),'Fastest')]")).Text;
            Assert.True(fastestText.Contains("Fastest"));

            //- Click to 'Fastest' tab in the results list
            driver.FindElement(By.XPath("//div[contains(text(),'Fastest')]/..")).Click();

            //- Find the link to the last page in the resuls list, and click on it 
            driver.FindElement(By.XPath("//div[@class='css-177s61e'][last()]")).Click(); // hopefully css selector is stable

            //- Find the last record, take the time of the fly in this record and verify that this time is equal to the "Max journey time" value in the box "Journey time" in the filter in left sidebar
            
            string timeFlyCard = driver.FindElement(By.XPath("//div[contains(@id, 'flightcard')][last()]/descendant::div[contains(@class, 'Text-module')][5]")).Text;   //div[contains(@id, 'flightcard')][last()]
            string maxTime = driver.FindElement(By.XPath("//div[text()='Max journey time']/following-sibling::div")).Text;
            StringAssert.AreEqualIgnoringCase(timeFlyCard, maxTime);

            //- Select one of the proposed offers in the results list
            driver.FindElement(By.XPath("//div[contains(@id, 'flightcard')][last()]//button")).Click();

            //- Click 'Select' in the popup
            driver.FindElement(By.XPath("//span[text()='Select']/..")).Click();

            //- Get the active value in the journey bar in the top of the page.If it's equal to 'Ticket type' then click 'Next' button on the loaded page if it's equal to 'Who's flying ? '
            //then go to next step of the testcase, otherwise throw an error.  ---- probably outdated

            driver.FindElement(By.XPath("//span[text()='Next']/..")).Click();  // click Next - takes to Who's flying

            //- Verify that step 'Who's flying ? ' is active in the journey bar in the top of the page.
            IWebElement flyingEl = driver.FindElement(By.XPath("//ol/li[3]"));
            string flyingClassText = flyingEl.GetAttribute("class");
            StringAssert.Contains(flyingClassText, "active");  // the active step contains word "active" in its class value

            //- In the opened form in the step 'Who's flying ? ' fill all required fields and click 'Next' button


            /*- Verify that step 'Customize your flight' is active in the journey bar in the top of the page.
            - Check if there's exists button 'Add' in 'Something else? ' section. If it exists, click it, select '1 bag' option and click to 'Done'.
            - Click 'Next' button
            - Verify that step 'Check and pay' is active in the journey bar in the top of the page.
            - Verify that 'Contact details' contains correct email and phone and 'Traveler details' contains correct first name, last name, gender, birthday and passport data. 
            */
            Thread.Sleep(10000);

        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }

    }
}
