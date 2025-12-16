window.PxWeb2Config = {
  language: {
    supportedLanguages: [{ shorthand: 'en', languageName: 'English' }],
    defaultLanguage: 'en',
    fallbackLanguage: 'en',
    showDefaultLanguageInPath: true,
  },
  baseApplicationPath: '/',
  apiUrl: '//localhost:8081/api/v2',
  maxDataCells: 150000,
  showBreadCrumbOnStartPage: false,
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
  homePage: {
    no: '', // Set to your Norwegian homepage URL
    sv: '', // Set to your Swedish homepage URL
    en: '', // Set to your English homepage URL
  },
};
