import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { ReactComponent as DarkModeIcon } from '../../../assets/icons/darkmode.svg';
import {ReactComponent as lightModeIcon} from '../../../assets/icons/lightmode.svg';


export default function AuthHeader() {
  const { t, i18n } = useTranslation();
  const [darkMode, setDarkMode] = useState(false);

  const toggleTheme = () => {
    setDarkMode((prev) => !prev);
    if (document.documentElement.classList.contains('dark')) {
      document.documentElement.classList.remove('dark');
    } else {
      document.documentElement.classList.add('dark');
    }
  };

  const changeLanguage = (e) => {
    i18n.changeLanguage(e.target.value);
  };

  return (
    <header className="w-full flex justify-between items-center px-6 py-4 bg-gray-300 text-black">
      <h1 className="text-xl font-bold">MDMS</h1>

      <div className="flex items-center gap-4">
      <button
        onClick={toggleTheme}
        aria-label="Toggle Theme"
        className={`relative w-12 h-8 flex items-center border-4 border-gray-500 rounded-full p-1 cursor-pointer transition-colors duration-300
          ${darkMode ? 'bg-gray-700 justify-end' : 'bg-gray-300 justify-start'}`}
      >
        <div
          className={`absolute w-5 h-5 rounded-full shadow-md transition-transform duration-300 transform
            ${darkMode ? 'translate-x-0 bg-white' : 'translate-x-0 bg-white-400'}`}
        >
          {darkMode ? (
            <img src={darkModeIcon} alt="Dark Mode Icon" className="w-5 h-5" />
          ) : (
            <img src={lightModeIcon} alt="Light Mode Icon" className="w-5 h-5" />
          )}
        </div>
      </button>

        <select
  onChange={changeLanguage}
  defaultValue={i18n.language}
  className="bg-transparent text-black px-2 py-1 rounded focus:outline-none"
>
  <option value="en">en</option>
  <option value="fr">fr</option>
  <option value="hi">hi</option>
</select>
      </div>
    </header>
  );
}