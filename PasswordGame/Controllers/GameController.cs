using Microsoft.AspNetCore.Mvc;
using PasswordGame.Models;

namespace PasswordGame.Controllers
{
    public class GameController : Controller
    {
        private GameViewModel _model = new GameViewModel();

        public IActionResult Index()
        {
            return View(_model);
        }

        public IActionResult CheckPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return View("Index");
            }

            List<string> results = new List<string>();
            string backgroundColor = "white";
            _model.HtmlResult = string.Empty;

            for (int i = 0; i < _model.LastRule; i++)
            {
                if (i >= _model.Rules.Count)
                {
                    _model.PasswordIsComplete = true;

                    results.Clear();

                    results.Add(@$"
					<div style='height:25px;'>
					</div>
					<div class='card align-self-center mx-auto'
						 style='width:fit-content; height: auto; padding:10px; max-width:500px;background-color:white'>
					<a><strong>Ваш пароль создан как: {password}</strong> </a>
					</div>");
                    break;
                }
                if (_model.Rules[i].Action(password))
                {
                    _model.LastRule++;
                    backgroundColor = "green";
                }
                else
                {
                    backgroundColor = "red";
                }

                results.Add(@$"
					<div style='height:25px;'>
					</div>
					<div class='card align-self-center mx-auto'
						 style='width:fit-content; height: auto; padding:10px; max-width:500px;background-color:{backgroundColor}'>
					<a><strong>Правило {i + 1}:</strong> {_model.Rules[i].Text}</a>
					</div>");
            }

            results.Reverse();

            _model.HtmlResult = string.Join("", results);

            return View("Index", _model);
        }
    }
}
