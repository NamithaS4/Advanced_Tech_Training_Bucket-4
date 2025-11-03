import React from 'react';

export default function UtilityLayout({ title, description, children }) {
  return (
    <div className="min-h-screen w-screen flex flex-col justify-center items-center bg-gray-100 dark:bg-gray-900 text-center p-6">
      <div className="bg-white dark:bg-gray-800 shadow-lg rounded-lg p-10 max-w-md w-full">
        <h1 className="text-2xl font-bold mb-4 text-gray-900 dark:text-white">
          {title}
        </h1>
        {description && (
          <p className="text-gray-600 dark:text-gray-300 mb-6">{description}</p>
        )}
        {children}
      </div>
    </div>
  );
}
