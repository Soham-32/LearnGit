using System.Collections.Generic;
using System.Linq;
using System;
using System.Globalization;

namespace AtCommon.ObjectFactories
{
    public static class TranslationsFactory
    {

        public static string GetExpectedTeamMemberEmailBody(string translatedSurveyEmailBody, string language)
        {
            string expectedEmailBody;
            List<string> expectedEmailBodyList;
            switch (language)
            {
                case "Japanese":
                    expectedEmailBodyList = translatedSurveyEmailBody.Split('。').ToList();
                    expectedEmailBodyList[3] = DateAndTimeStatement(language);
                    expectedEmailBody = string.Join("。", expectedEmailBodyList);
                    break;
                case "Chinese":
                    expectedEmailBodyList = translatedSurveyEmailBody.Split('。').ToList();
                    expectedEmailBodyList[4] = DateAndTimeStatement(language);
                    expectedEmailBody = string.Join("。", expectedEmailBodyList);
                    break;
                case "Korean":
                    expectedEmailBodyList = translatedSurveyEmailBody.Split('.').ToList();
                    expectedEmailBodyList[5] = DateAndTimeStatement(language);
                    expectedEmailBody = string.Join(".", expectedEmailBodyList);
                    break;


                case "Spanish":
                case "French":
                case "German":
                case "Hungarian":
                case "Portuguese":
                case "Turkish":
                case "English":
                    expectedEmailBodyList = translatedSurveyEmailBody.Split('.').ToList();
                    expectedEmailBodyList[4] = DateAndTimeStatement(language);
                    expectedEmailBody = string.Join(".", expectedEmailBodyList);
                    break;
                default:
                    expectedEmailBody = null;
                    break;
            }
            return expectedEmailBody;
        }

        public static string GetExpectedStakeholderEmailBody(string translatedSurveyEmailBody, string language, string teamName, string teamMemberName)
        {
            string expectedEmailBody;
            List<string> expectedEmailBodyList;
            switch (language)
            {
                case "English":
                    expectedEmailBodyList = translatedSurveyEmailBody.Split('.').ToList();
                    expectedEmailBodyList[4] = $"{teamName} Members:{teamMemberName}" + DateAndTimeStatement(language);
                    expectedEmailBody = string.Join(".", expectedEmailBodyList);
                    break;

                case "Japanese":
                    expectedEmailBodyList = translatedSurveyEmailBody.Split('。').ToList();
                    expectedEmailBodyList[3] = $"{teamName} メンバー:{teamMemberName}" + DateAndTimeStatement(language);
                    expectedEmailBody = string.Join("。", expectedEmailBodyList);
                    break;

                case "Chinese":
                    expectedEmailBodyList = translatedSurveyEmailBody.Split('。').ToList();
                    expectedEmailBodyList[4] = $"{teamName}会员:{teamMemberName}" + DateAndTimeStatement(language);
                    expectedEmailBody = string.Join("。", expectedEmailBodyList);    
                    break;

                case "Portuguese":
                    expectedEmailBodyList = translatedSurveyEmailBody.Split('.').ToList();
                    expectedEmailBodyList[4] = $"{teamName}Membros:{teamMemberName}" + DateAndTimeStatement(language);
                    expectedEmailBody = string.Join(".", expectedEmailBodyList);
                    break;

                case "Turkish":
                    expectedEmailBodyList = translatedSurveyEmailBody.Split('.').ToList();
                    expectedEmailBodyList[4] = $"{teamName}Üyeler:{teamMemberName}" + DateAndTimeStatement(language);
                    expectedEmailBody = string.Join(".", expectedEmailBodyList);
                    break;


                case "Korean":
                    expectedEmailBodyList = translatedSurveyEmailBody.Split('.').ToList();
                    var custom = expectedEmailBodyList[4];
                    expectedEmailBodyList.Remove(custom);
                    expectedEmailBodyList[4] = $"{teamName}회원:{teamMemberName}{custom}."+ DateAndTimeStatement(language);
                    expectedEmailBody = string.Join(".", expectedEmailBodyList);
                    break;

                case "Spanish":
                    expectedEmailBodyList = translatedSurveyEmailBody.Split('.').ToList();

                    expectedEmailBodyList[4] = $"😊{teamName} Miembros:{teamMemberName}" + DateAndTimeStatement(language).Replace("😊", "");
                    expectedEmailBody = string.Join(".", expectedEmailBodyList);
                    break;

                case "French":
                    expectedEmailBodyList = translatedSurveyEmailBody.Split('.').ToList();
                    expectedEmailBodyList[4] = $"{teamName} Membres:{teamMemberName}" + DateAndTimeStatement(language);
                    expectedEmailBody = string.Join(".", expectedEmailBodyList);
                    break;

                case "German":
                    expectedEmailBodyList = translatedSurveyEmailBody.Split('.').ToList();
                    expectedEmailBodyList[4] = $"{teamName} Mitglieder:{teamMemberName}" + DateAndTimeStatement(language);
                    expectedEmailBody = string.Join(".", expectedEmailBodyList);
                    break;

                case "Hungarian":
                    expectedEmailBodyList = translatedSurveyEmailBody.Split('.').ToList();
                    expectedEmailBodyList[4] = $"{teamName} Tagok:{teamMemberName}" + DateAndTimeStatement(language);
                    expectedEmailBody = String.Join(".", expectedEmailBodyList);
                    break;

                default:
                    expectedEmailBody = null;
                    break;
            }
            return expectedEmailBody;
        }

        public static string DateAndTimeStatement(string language)
        {
            var dateAndTime = DateTime.UtcNow.AddDays(7).Subtract(TimeSpan.FromMinutes(2));
            var month = dateAndTime.Month;
            var date = dateAndTime.Day.ToString("d");
            var year = dateAndTime.Year;
            var time = dateAndTime.ToString("h:mm");
            var meridian = dateAndTime.ToString("tt");
            var day = dateAndTime.DayOfWeek.ToString();
            var hour = dateAndTime.Hour;
            switch (language)
            {
                case "English":
                    return $"Click here to take the assessmentYou have until {day}, {dateAndTime:MMMM} {date}, {year} {time} {meridian} UTC to complete the assessment";

                case "Japanese":
                    var japaneseCulture = CultureInfo.GetCultureInfo("ja-JP");
                    var japaneseDateTimeFormat = japaneseCulture.DateTimeFormat;
                    var monthInJapanese = japaneseDateTimeFormat.MonthNames.FirstOrDefault(x => x.Contains(month.ToString()));
                    return $"ここをクリックして評価を受ける{year} 年{monthInJapanese} {date}日 {time} UTCまでに評価を完了してください";

                case "Chinese":
                    return $"点击此处进行评估您必须在{year}年{month}月{date}日 {time} UTC之前完成评估";

                case "Korean":
                    var koreanCulture = CultureInfo.GetCultureInfo("ko-KR");
                    var koreanDateTimeFormat = koreanCulture.DateTimeFormat;
                    var dayInKorean= koreanDateTimeFormat.GetDayName((DayOfWeek)Enum.Parse(typeof(DayOfWeek), day));
                    string partOfDay;

                    if (hour >= 0 && hour < 6)
                    {
                        partOfDay = "새벽";
                    }
                    else if (hour >= 6 && hour < 12)
                    {
                        partOfDay = "오전";
                    }
                    else if (hour >= 12 && hour < 18)
                    {
                        partOfDay = "오후";
                    }
                    else
                    {
                        partOfDay = "저녁";
                    }
                    return $"평가를완료하려면{year}년 {month}월 {date}일 {dayInKorean} {partOfDay} {time} UTC까지남았습니다";

                case "Portuguese":
                    var portugueseCulture = CultureInfo.GetCultureInfo("pt-BR");
                    var portugueseDateTimeFormat = portugueseCulture.DateTimeFormat;
                    var monthInPortuguese = portugueseDateTimeFormat.MonthNames[month - 1];
                    var dayInPortuguese = portugueseDateTimeFormat.GetDayName((DayOfWeek)Enum.Parse(typeof(DayOfWeek), day));
                    return $"CliqueaquiparafazeraavaliaçãoVocêtematé{dayInPortuguese},{date} de {monthInPortuguese} de {year} {time}UTCparaconcluiraavaliação";

                case "Turkish":
                    var turkishCulture = CultureInfo.GetCultureInfo("tr-TR");
                    var turkishDateTimeFormat = turkishCulture.DateTimeFormat;
                    var monthInTurkish = turkishDateTimeFormat.MonthNames[month - 1];
                    var dayInTurkish = turkishDateTimeFormat.GetDayName((DayOfWeek)Enum.Parse(typeof(DayOfWeek), day));
                    return $"DeğerlendirmeyekatılmakiçinburayıtıklayınDeğerlendirmeyitamamlamakiçin{date} {monthInTurkish} {year} {dayInTurkish} {time} UTCtarihinekadarsürenizvar";

                case "Spanish":
                    var spanishCulture = CultureInfo.GetCultureInfo("es-ES");
                    var spanishDateTimeFormat = spanishCulture.DateTimeFormat;
                    var monthInSpanish = spanishDateTimeFormat.MonthNames[month - 1];
                    var dayInSpanish = spanishDateTimeFormat.GetDayName((DayOfWeek)Enum.Parse(typeof(DayOfWeek), day));
                    return $"😊Haga clic aquí para realizar la evaluaciónTienes hasta el {dayInSpanish}, {date} de {monthInSpanish} de {year} {time}  UTC para completar la evaluación";

                case "French":
                    var frenchCulture = CultureInfo.GetCultureInfo("fr-FR");
                    var frenchDateTimeFormat = frenchCulture.DateTimeFormat;
                    var monthInFrench = frenchDateTimeFormat.MonthNames[month - 1];
                    var dayInFrench = frenchDateTimeFormat.GetDayName((DayOfWeek)Enum.Parse(typeof(DayOfWeek), day));
                    return $"Cliquez ici pour effectuer l´évaluationVous avez jusqu´au {dayInFrench} {date} {monthInFrench} {year} {time} UTC pour compléter l´évaluation";

                case "German":
                    var germanCulture = CultureInfo.GetCultureInfo("de-DE");
                    var germanDateTimeFormat = germanCulture.DateTimeFormat;
                    var monthInGerman = germanDateTimeFormat.MonthNames[month - 1];
                    var dayInGerman = germanDateTimeFormat.GetDayName((DayOfWeek)Enum.Parse(typeof(DayOfWeek), day));
                    return $"Klicken Sie hier, um die Bewertung durchzuführenSie haben bis zum {dayInGerman}, {date}. {monthInGerman} {year} {time}  UTC Zeit, um die Bewertung abzuschließen";

                case "Hungarian":
                    var hungarianCulture = CultureInfo.GetCultureInfo("hu-HU");
                    var hungarianDateTimeFormat = hungarianCulture.DateTimeFormat;
                    var monthInHungarian = hungarianDateTimeFormat.MonthNames[month - 1];
                    var dayInHungarian = hungarianDateTimeFormat.GetDayName((DayOfWeek)Enum.Parse(typeof(DayOfWeek), day));
                    return $"Kattintson ide az értékelés elvégzéséhezAz értékelés véglegesítésére {year}. {monthInHungarian} {date}.,  {dayInHungarian} {time} UTC-ig van lehetősége";

                default:
                    return "Invalid Language";
            }
        }

    }
}