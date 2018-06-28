//  <copyright file="MultiTenantLocalizationDictionaryProvider.cs" project="CodeZero.Identity.Common" solution="CodeZero">
//      Copyright (c) 2018 CodeZero Framework.  All rights reserved.
//  </copyright>
//  <author>Nasr Aldin M.</author>
//  <email>nasr2ldin@gmail.com</email>
//  <website>https://nasraldin.com/codezero</website>
//  <github>https://nasraldin.github.io/CodeZero</github>
//  <date>01/01/2018 01:00 AM</date>
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CodeZero.Collections.Extensions;
using CodeZero.Dependency;
using CodeZero.Localization.Dictionaries;

namespace CodeZero.Localization
{
    /// <summary>
    /// Extends <see cref="ILocalizationDictionaryProvider"/> to add tenant and database based localization.
    /// </summary>
    public class MultiTenantLocalizationDictionaryProvider : ILocalizationDictionaryProvider
    {
        public ILocalizationDictionary DefaultDictionary
        {
            get { return GetDefaultDictionary(); }
        }

        public IDictionary<string, ILocalizationDictionary> Dictionaries
        {
            get { return GetDictionaries(); }
        }

        private readonly ConcurrentDictionary<string, ILocalizationDictionary> _dictionaries;

        private string _sourceName;

        private readonly ILocalizationDictionaryProvider _internalProvider;

        private readonly IIocManager _iocManager;
        private ILanguageManager _languageManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiTenantLocalizationDictionaryProvider"/> class.
        /// </summary>
        public MultiTenantLocalizationDictionaryProvider(ILocalizationDictionaryProvider internalProvider, IIocManager iocManager)
        {
            _internalProvider = internalProvider;
            _iocManager = iocManager;
            _dictionaries = new ConcurrentDictionary<string, ILocalizationDictionary>();
        }

        public void Initialize(string sourceName)
        {
            _sourceName = sourceName;
            _languageManager = _iocManager.Resolve<ILanguageManager>();
            _internalProvider.Initialize(_sourceName);
        }

        protected virtual IDictionary<string, ILocalizationDictionary> GetDictionaries()
        {
            var languages = _languageManager.GetLanguages();

            foreach (var language in languages)
            {
                _dictionaries.GetOrAdd(language.Name, s => CreateLocalizationDictionary(language));
            }

            return _dictionaries;
        }

        protected virtual ILocalizationDictionary GetDefaultDictionary()
        {
            var languages = _languageManager.GetLanguages();
            if (!languages.Any())
            {
                throw new CodeZeroException("No language defined!");
            }

            var defaultLanguage = languages.FirstOrDefault(l => l.IsDefault);
            if (defaultLanguage == null)
            {
                throw new CodeZeroException("Default language is not defined!");
            }

            return _dictionaries.GetOrAdd(defaultLanguage.Name, s => CreateLocalizationDictionary(defaultLanguage));
        }

        protected virtual IMultiTenantLocalizationDictionary CreateLocalizationDictionary(LanguageInfo language)
        {
            var internalDictionary =
                _internalProvider.Dictionaries.GetOrDefault(language.Name) ??
                new EmptyDictionary(CultureInfo.GetCultureInfo(language.Name));

            var dictionary =  _iocManager.Resolve<IMultiTenantLocalizationDictionary>(new
            {
                sourceName = _sourceName,
                internalDictionary = internalDictionary
            });

            return dictionary;
        }

        public virtual void Extend(ILocalizationDictionary dictionary)
        {
            //Add
            ILocalizationDictionary existingDictionary;
            if (!_internalProvider.Dictionaries.TryGetValue(dictionary.CultureInfo.Name, out existingDictionary))
            {
                _internalProvider.Dictionaries[dictionary.CultureInfo.Name] = dictionary;
                return;
            }

            //Override
            var localizedStrings = dictionary.GetAllStrings();
            foreach (var localizedString in localizedStrings)
            {
                existingDictionary[localizedString.Name] = localizedString.Value;
            }
        }
    }
}