import { Link } from 'react-router-dom';
import Header from '../../components/layout/Header/Header';
import { useTranslation } from 'react-i18next';

export default function Login() {
  const { t } = useTranslation();

  return (
    <div className="w-screen min-h-screen flex flex-col">
      <Header />
      <div className="flex flex-1 justify-center items-center bg-gray-100 dark:bg-gray-900">
        <div className="w-full max-w-sm text-center">
          <h2 className="text-xl mb-4 text-black dark:text-white">
            Login Form
          </h2>
          <input
            type="email"
            placeholder={t('email')}
            className="w-full mb-4 px-3 py-2 border border-gray-300 bg-gray-300 rounded-2xl text-black dark:bg-gray-700 dark:text-white"
          />
          <input
            type="password"
            placeholder={t('password')}
            className="w-full mb-4 px-3 py-2 border border-gray-300 bg-gray-300 rounded-2xl text-black dark:bg-gray-700 dark:text-white"
          />
          <div className="flex justify-between items-center text-sm mb-4">
            <label className="inline-flex items-center">
              <input type="checkbox" className="mr-2 align-middle w-3 h-3" />
              <span className="text-black dark:text-white">
                {t('Remember Me')}
              </span>
            </label>
            <Link
              to="/forgot-password"
              className="font-bold text-blue-700 dark:text-blue-400"
            >
              {t('Forgot Password')}
            </Link>
          </div>

          <button className="border border-black px-24 text-black py-1 rounded-full bg-transparent dark:text-white dark:border-white">
            {t('Login')}
          </button>
        </div>
      </div>
    </div>
  );
}
