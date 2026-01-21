using System.Collections.Generic;
using System.IO;
using System.Linq;
using UniRx;
using Unity.VisualScripting;


namespace ResourceSystem
{
    public interface ILocalization
    {
        static string BaseLanguage { get; } = "ru";  // Базовый язык
        IReadOnlyReactiveProperty<string> Language { get; } // Текущий выбранный язык (en, ru)
        IReadOnlyReactiveCollection<string> LanguageList { get; }
        void SetLanguage(string lang); // Смена языка
        List<string> UpdateLanguageList();
        TextLocalizationService Text { get; } // сервис локализации для текста (для текстур и аудио другие сервисы)
    }


    public class LocalizationManager : ILocalization
    {
        private readonly ReactiveProperty<string> _language;
        public IReadOnlyReactiveProperty<string> Language => _language;
        private readonly ReactiveCollection<string> _languageList = new();
        public IReadOnlyReactiveCollection<string> LanguageList => _languageList;
        public TextLocalizationService Text { get; }

        public LocalizationManager()
        {
            _language = new ReactiveProperty<string>(ILocalization.BaseLanguage);
            Text = new TextLocalizationService();
            UpdateLanguageList();
        }

        public void SetLanguage(string lang)
        {
            if (_language.Value != lang)
            {
                _language.Value = lang;
            }
        }

        /// <summary>
        /// Сканирует папки в DataPathManager.Localization
        /// </summary>
        public List<string> UpdateLanguageList()
        {
            var path = DataPathManager.Localization;

            if (!Directory.Exists(path))
            {
                _languageList.AddRange(new List<string> { ILocalization.BaseLanguage });
                return _languageList.ToList();
            }

            // Ищем все подпапки (en, ru, de...)
            _languageList.AddRange(Directory.GetDirectories(path)
                .Select(d => new DirectoryInfo(d).Name) // Берем только имя папки
                .ToList()
            );

            return _languageList.ToList();
        }
    }
}