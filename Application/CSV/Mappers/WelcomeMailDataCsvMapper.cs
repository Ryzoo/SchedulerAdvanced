﻿using Application.CSV.Models;
using CsvHelper.Configuration;

namespace Application.CSV.Mappers
{
    public sealed class WelcomeMailDataCsvMapper : ClassMap<WelcomeMailDataCsvModel>
    {
        public WelcomeMailDataCsvMapper()
        {
            Map(m => m.UserName)
                .Name(WelcomeMailDataCsvHeaders.UserName);
            Map(m => m.UserEmail)
                .Name(WelcomeMailDataCsvHeaders.UserEmail);
        }
    }
}