import i18n from "i18next";
import { reactI18nextModule } from "react-i18next";

import translationEN from './languages/locales/en/translation.json';
import translationVN from './languages/locales/vn/translation.json';

// the translations
const resources = {
  en: {
    translation: translationEN
  },
  vn: {
    translation: translationVN
  }
};

if (localStorage.getItem('lang') === null || localStorage.getItem('lang') === undefined || (localStorage.getItem('lang') !== 'en' && localStorage.getItem('lang') !== 'vn')) {
  localStorage.setItem('lang', 'en');
}

i18n
  .use(reactI18nextModule) // passes i18n down to react-i18next
  .init({
    resources,
    lng: localStorage.getItem('lang') || 'en',

    keySeparator: false, // we do not use keys in form messages.welcome

    interpolation: {
      escapeValue: false // react already safes from xss
    }
  });

export default i18n;