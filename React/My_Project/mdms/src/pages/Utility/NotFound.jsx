import React from 'react';
import UtilityLayout from '../../components/layout/UtilityLayout';

export default function NotFound() {
  return (
    <UtilityLayout
      title="404 NOT FOUND"
      description="The page you are looking for doesn’t exist."
    >
      <a href="/" className="text-blue-500 hover:underline">
        Go Back Home
      </a>
    </UtilityLayout>
  );
}
