using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace PasswordGame.Models
{
    public class GameViewModel
    {
        public bool PasswordIsComplete { get; set; }
        public int LastRule { get; set; } = 1;
        public string HtmlResult { get; set; }

        public List<Rule> Rules => new List<Rule>()
        {
            new Rule("Ваш пароль должен содержать более 5 символов", CheckLengthBy5),
            new Rule("Ваш пароль должен содержать цифру", CheckByDigit),
            new Rule("Ваш пароль должен содержать заглавную букву", CheckByUppercaseLetter),
            new Rule("Ваш пароль должен содержать специальный символ", CheckSpecialSymbol),
            new Rule("Все цифры пароля в сумме должно давать 25", CheckDigitsSumm25),
            new Rule("Ваш пароль должен содержать месяц на англиском языке", CheckMonth),
            new Rule("Ваш пароль должен содержать текущий час в Токио", CheckHourInTokyo),

        };

        private bool CheckHourInTokyo(string password)
        {
            var responce = CheckHourInTokyoAsync(password);
            responce.Wait();
            return responce.Result;

            async Task<bool> CheckHourInTokyoAsync(string password)
            {
                TimeApiResponse apiResponse = new TimeApiResponse();
                using (HttpClient httpClient = new HttpClient())
                {
                    string urlAPI = "https://timeapi.io/api/Time/current/zone?timeZone=Asia/Tokyo";

                    var response = await httpClient.GetAsync(urlAPI);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseString = await response.Content.ReadAsStringAsync();

                        apiResponse = JsonConvert.DeserializeObject<TimeApiResponse>(responseString);
                    }
                }

                return password.Contains(apiResponse.hour.ToString());
            }
        }


        private bool CheckMonth(string password)
        {
            return Regex.IsMatch(password, @"(January|February|March|April|May|June|July|August|September|October|November|December)", RegexOptions.IgnoreCase);
        }

        public bool CheckLengthBy5(string password)
        {
            return password.Length >= 5;
        }

        public bool CheckByDigit(string password)
        {
            return password.Any(char.IsDigit);
        }

        public bool CheckByUppercaseLetter(string password)
        {
            return Regex.IsMatch(password, @"[A-Z]");
        }

        public bool CheckSpecialSymbol(string password)
        {
            return Regex.IsMatch(password, @"[~!@#$%^&*()+=_\|/-]");
        }

        public bool CheckDigitsSumm25(string password)
        {
            var digits = password.Where(char.IsDigit);
            int sum = digits.Sum(c => Int32.Parse(c.ToString()));
            return sum == 25;
        }
    }

    public class Rule
    {
        public delegate bool RuleCheck(string password);

        public string Text { get; set; }
        public RuleCheck Action { get; set; }

        public Rule(string text, RuleCheck action)
        {
            Text = text;
            Action = action;
        }
    }

    public class TimeApiResponse
    {
        public int hour { get; set; }
    }
}
