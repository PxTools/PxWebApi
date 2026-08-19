globalThis.PxWeb2Config = {
  language: {
    supportedLanguages: [{ shorthand: "en", languageName: "English" }],
    defaultLanguage: "en",
    fallbackLanguage: "en",
    showDefaultLanguageInPath: true,
    positionInPath: "after",
  },
  baseApplicationPath: "/",
  apiUrl: "//localhost:8081/api/v2",
  maxDataCells: 150000,
  useDynamicContentInTitle: false,
  showBreadCrumbOnStartPage: false,
  specialCharacters: [".", "..", ":", "-", "...", "*"],
  variableFilterExclusionList: {
    en: [
      "observations",
      "year",
      "quarter",
      "month",
      "every other year",
      "every fifth year",
    ],
  },
  homePage: {
    en: "", // Set to your English homepage URL
  },
};
