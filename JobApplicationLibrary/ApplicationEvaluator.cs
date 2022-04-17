using JobApplicationLibrary.Interfaces;
using JobApplicationLibrary.Models;
using JobApplicationLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JobApplicationLibrary
{
    public class ApplicationEvaluator
    {
        private const int minAge = 18;
        private const int autoAcceptedYearOfExperience = 15;
        private List<string> techStackList = new() { "C#", "RabbitMQ", "Microservice", "Visual Studio" };
        private readonly IIdentityValidator _validator;

        public ApplicationEvaluator(IIdentityValidator validator)
        {
            _validator = validator;
        }

        public ApplicationResult Evaluate(JobApplication form)
        {
            if (form.Applicant.Age < minAge)
                return ApplicationResult.AutoRejected;

            if (_validator.CountryDataProvider.CountryData.Country != "TURKEY")
                return ApplicationResult.TransferredToCTO;

            var validIdentity = _validator.IsValid(form.Applicant.IdentityNumber);

            if (!validIdentity)
                return ApplicationResult.TransferredToHR;

            var sr = GetTechStackSimilarityRate(form.TechStackList);

            if (sr < 25)
                return ApplicationResult.AutoRejected;

            if (sr > 75 && form.YearsOfExperience > autoAcceptedYearOfExperience)
                return ApplicationResult.AutoAccepted;

            return ApplicationResult.AutoAccepted;
        }

        private int GetTechStackSimilarityRate(List<string> techStacks)
        {
            var matchCount = techStacks
                                .Where(x => techStackList.Contains(x, StringComparer.OrdinalIgnoreCase))
                                .Count();

            return (int)((double)matchCount / techStackList.Count() * 100);
        }
    }

    public enum ApplicationResult
    {
        AutoRejected,
        TransferredToHR,
        TransferredToLead,
        TransferredToCTO,
        AutoAccepted
    }
}
