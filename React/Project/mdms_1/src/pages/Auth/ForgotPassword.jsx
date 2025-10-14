import { Link } from "react-router-dom";
import AuthHeader from "../../components/layout/Sidebar/AuthHeader";
import { useTranslation } from "react-i18next";

export default function ForgotPassword() {
  const { t } = useTranslation();

  return (
    <div className="w-screen min-h-screen flex flex-col">
      <AuthHeader />
      <div className="flex flex-1 justify-center items-center bg-gray-100 dark:bg-gray-900">
        <div className="w-full max-w-sm text-center">
          <h2 className="text-lg mb-4 text-gray-900 dark:text-white">
            Forgot password
          </h2>
          <input
            type="email"
            placeholder={t("email")}
            className="w-full mb-4 px-3 py-2 border border-gray-300 bg-gray-300 rounded-2xl text-black dark:bg-gray-700 dark:text-white"
          />
          <Link to="/" className="text-blue-600 dark:text-blue-400 flex mb-4">
            {t("Login")}
          </Link>
          <button className="border border-black text-black py-1 px-16 rounded-full bg-transparent dark:text-white dark:border-white">
            {t("Send Reset Link")}
          </button>
        </div>
      </div>
    </div>
  );
}
