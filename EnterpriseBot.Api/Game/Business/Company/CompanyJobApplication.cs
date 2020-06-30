﻿using EnterpriseBot.Api.Game.Essences;
using EnterpriseBot.Api.Models.ModelCreationParams.Business;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings.LocalizationSettings;
using EnterpriseBot.Api.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static EnterpriseBot.Api.Utils.UserInputUtils;
using static EnterpriseBot.Api.Utils.Constants;

namespace EnterpriseBot.Api.Game.Business.Company
{
    public class CompanyJobApplication
    {
        protected CompanyJobApplication() { }

        #region model
        public long Id { get; protected set; }

        public virtual CompanyJob Job { get; protected set; }
        public virtual Player Applicant { get; protected set; }

        public string Resume { get; protected set; }
        #endregion

        #region actions
        public static GameResult<CompanyJobApplication> Create(CompanyJobApplicationCreationParams pars,
            UserInputRequirements inputRequirements)
        {
            if(!CheckResume(pars.Resume))
            {
                var req = inputRequirements;

                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = string.Format(req.Resume.English,
                                                   ResumeMaxLength),
                    RussianMessage = string.Format(req.Resume.Russian,
                                                   ResumeMaxLength)
                };
            }

            if(pars.Job.IsOccupied)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Cannot apply for a job, as the job is already taken",
                    RussianMessage = "Нельзя подать заявку на работу, так как работа уже занята"
                };
            }

            return new CompanyJobApplication
            {
                Job = pars.Job,
                Applicant = pars.Applicant,
                Resume = pars.Resume
            };
        }
        #endregion
    }
}
