using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine.InputSystem;


public interface ILocalization
{
    static string BaseLanguage { get; } = "ru";  // Базовый язык
    IReadOnlyReactiveProperty<string> Language { get; } // Текущий выбранный язык (en, ru)
    IReadOnlyReactiveCollection<string> LanguageList { get; }
    void SetLanguage(string lang); // Смена языка
    List<string> UpdateLanguageList();
    ITextLocalization Text { get; } // сервис локализации для текста (для текстур и аудио другие сервисы)
}


public interface ITextLocalization : IDisposable
{
    ILocalizationBinding Bind(string key); // Получить байндинг, который внутри себя знает, как достать текст (key = "ru/ui/main_menu.title")
    IReadOnlyDictionary<string, LocalizationModule> Modules { get; } // < путь, модуль >
    string Resolve(string key);
    void Release(string key);
}


public interface ILocalizationBinding : IDisposable
{
    string Text { get; } // Text это текст для "ui.main_menu.title"
}

