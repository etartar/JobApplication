using JobApplicationLibrary.Models;
using NUnit.Framework;
using Moq;
using JobApplicationLibrary.Interfaces;

namespace JobApplicationLibrary.UnitTest
{
    public class ApplicationEvaluateUnitTest
    {
        [SetUp]
        public void Setup()
        {

        }

        /// UnitOfWork_Condition_ExpectedResult
        /// Test isimlendirme kuralý
        [Test]
        public void Application_WithUnderAge_TransferredToAutoRejected()
        {
            // Arrange
            ApplicationEvaluator evaluator = new ApplicationEvaluator(null);
            JobApplication form = new JobApplication
            {
                Applicant = new Applicant
                {
                    Age = 17
                }
            };

            // Action
            var result = evaluator.Evaluate(form);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(ApplicationResult.AutoRejected, result);
        }

        [Test]
        public void Application_WithNoTechStack_TransferredToAutoRejected()
        {
            // Arrange
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.DefaultValue = DefaultValue.Mock;
            mockValidator.Setup(s => s.IsValid(It.IsAny<string>())).Returns(true);
            mockValidator.Setup(s => s.CountryDataProvider.CountryData.Country).Returns("TURKEY");

            ApplicationEvaluator evaluator = new ApplicationEvaluator(mockValidator.Object);
            JobApplication form = new JobApplication
            {
                Applicant = new Applicant
                {
                    Age = 38
                },
                TechStackList = new() { "" }
            };

            // Action
            var result = evaluator.Evaluate(form);

            // Assert
            Assert.AreEqual(ApplicationResult.AutoRejected, result);
        }

        [Test]
        public void Application_WithTechStackOver75P_TransferredToAutoAccepted()
        {
            // Arrange
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.DefaultValue = DefaultValue.Mock;
            mockValidator.Setup(s => s.IsValid(It.IsAny<string>())).Returns(true);
            mockValidator.Setup(s => s.CountryDataProvider.CountryData.Country).Returns("TURKEY");

            ApplicationEvaluator evaluator = new ApplicationEvaluator(mockValidator.Object);
            JobApplication form = new JobApplication
            {
                Applicant = new Applicant
                {
                    Age = 38
                },
                TechStackList = new() { "C#", "RabbitMQ", "Microservice", "Visual Studio" },
                YearsOfExperience = 16
            };

            // Action
            var result = evaluator.Evaluate(form);

            // Assert
            Assert.AreEqual(ApplicationResult.AutoAccepted, result);
        }

        [Test]
        public void Application_WithInvalidIdentityNumber_TransferredToHR()
        {
            // Arrange
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.DefaultValue = DefaultValue.Mock;
            mockValidator.Setup(s => s.IsValid(It.IsAny<string>())).Returns(false);
            mockValidator.Setup(s => s.CountryDataProvider.CountryData.Country).Returns("TURKEY");

            ApplicationEvaluator evaluator = new ApplicationEvaluator(mockValidator.Object);
            JobApplication form = new JobApplication
            {
                Applicant = new Applicant
                {
                    Age = 38
                }
            };

            // Action
            var result = evaluator.Evaluate(form);

            // Assert
            Assert.AreEqual(ApplicationResult.TransferredToHR, result);
        }

        [Test]
        public void Application_WithOfficeLocation_TransferredToHR()
        {
            // Arrange
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("SPAIN");

            //var mockCountryData = new Mock<ICountryData>();
            //mockCountryData.Setup(i => i.Country).Returns("SPAIN");

            //var mockProvider = new Mock<ICountryDataProvider>();
            //mockProvider.Setup(i => i.CountryData).Returns(mockCountryData.Object);

            //mockValidator.Setup(i => i.CountryDataProvider).Returns(mockProvider.Object);

            ApplicationEvaluator evaluator = new ApplicationEvaluator(mockValidator.Object);
            JobApplication form = new JobApplication
            {
                Applicant = new Applicant
                {
                    Age = 38
                }
            };

            // Action
            var result = evaluator.Evaluate(form);

            // Assert
            Assert.AreEqual(ApplicationResult.TransferredToCTO, result);
        }
    }
}