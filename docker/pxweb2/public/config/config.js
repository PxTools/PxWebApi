window.PxWeb2Config = {
  language: {
    supportedLanguages: [
      { shorthand: 'en', languageName: 'English' },
    ],
    defaultLanguage: 'en',
    fallbackLanguage: 'en',
    showDefaultLanguageInPath: true,
  },
  baseApplicationPath: '/',
  apiUrl: '//localhost:8081/api/v2',
  maxDataCells: 150000,
  specialCharacters: ['.', '..', ':', '-', '...', '*'],
  variableFilterExclusionList: {
    en: [
      'observations',
      'year',
      'quarter',
      'month',
      'every other year',
      'every fifth year',
    ]
  },
};