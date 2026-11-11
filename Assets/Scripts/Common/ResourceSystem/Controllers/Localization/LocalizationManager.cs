using System.Collections.Generic;
using System.IO;
using System.Linq;
using UniRx;
using Unity.VisualScripting;


namespace ResourceSystem
{
    public interface ILocalization : ITextLocalizationService
    {
        static string BaseLanguage { get; } = "ru";  // ������� ����
        IReadOnlyReactiveProperty<string> Language { get; } // ������� ��������� ���� (en, ru)
        IReadOnlyReactiveCollection<string> LanguageList { get; }
        void SetLanguage(string lang); // ����� �����
        List<string> UpdateLanguageList();

    }

    public interface ITextLocalizationService
    {
        // FileLocalizationManager ������ ����������� ��� ������ (��� ������� � ����� ������ �������)
        IResourceController<Dictionary<string, string>> Manager { get; }

        IResourceBinding<string> Bind(string resourceKey, string dictKey);
    }


    public class LocalizationManager : ILocalization
    {
        private readonly ReactiveProperty<string> _language;
        public IReadOnlyReactiveProperty<string> Language => _language;
        private readonly ReactiveCollection<string> _languageList = new();
        public IReadOnlyReactiveCollection<string> LanguageList => _languageList;

        public LocalizationManager()
        {
            _language = new ReactiveProperty<string>(ILocalization.BaseLanguage);
            UpdateLanguageList();
        }

        // ----------- ITextLocalizationService -----------
        public IResourceController<Dictionary<string, string>> Manager { get; private set; } = new FileLocalizationManager();
        public IResourceBinding<string> Bind(string resourceKey, string dictKey)
        {
            var resource = Manager.Bind(resourceKey);
            return new TextLocalizationBinding(resource, dictKey);
        }

        // ----------- ILocalization -----------
        public void SetLanguage(string lang)
        {
            if (_language.Value != lang)
            {
                _language.Value = lang;
            }
        }

        /// <summary>
        /// ��������� ����� � DataPathManager.Localization
        /// </summary>
        public List<string> UpdateLanguageList()
        {
            var path = DataPathManager.Localization;

            if (!Directory.Exists(path))
            {
                _languageList.AddRange(new List<string> { ILocalization.BaseLanguage });
                return _languageList.ToList();
            }

            // ���� ��� �������� (en, ru, de...)
            _languageList.AddRange(Directory.GetDirectories(path)
                .Select(d => new DirectoryInfo(d).Name) // ����� ������ ��� �����
                .ToList()
            );

            return _languageList.ToList();
        }
    }
}