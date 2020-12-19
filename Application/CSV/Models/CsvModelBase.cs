﻿using System;
using Core.DomainModels;
using Core.Enums;
using Newtonsoft.Json;

namespace Application.CSV.Models
{
    public abstract class CsvModelBase<T>
    {
        public static Func<T, ScheduledMailModel> ToDomainModel =>
            mail => new ScheduledMailModel()
            {
                Params = JsonConvert.SerializeObject(mail),
                EmailType = EmailType.WelcomeMail,
            };
    }
}