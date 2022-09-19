import i18n from "i18next";
import { initReactI18next } from "react-i18next";

import translationEN from './languages/locales/en/translation.json';
import translationVN from './languages/locales/vn/translation.json';

// the translations
const resources = {
  en: {
    translation: translationEN
  },
  vi: {
    translation: translationVN
  }
};

if (localStorage.getItem('lang') === null || localStorage.getItem('lang') === undefined || (localStorage.getItem('lang') !== 'en' && localStorage.getItem('lang') !== 'vi')) {
  localStorage.setItem('lang', 'en');
}

i18n
  .use(initReactI18next) // passes i18n down to react-i18next
  .init({
    resources,
    lng: localStorage.getItem('lang') || 'en',

    interpolation: {
      escapeValue: false // react already safes from xss
    }
  });

export default i18n;