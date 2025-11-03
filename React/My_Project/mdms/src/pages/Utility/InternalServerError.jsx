import React from 'react';
import UtilityLayout from '../../components/layout/UtilityLayout';

export default function InternalServerError() {
  return (
    <UtilityLayout
      title="500 INTERNAL SERVER ERROR"
      description="Something went wrong on our end. Please try again later."
    >
      <a href="/" className="text-blue-500 hover:underline">
        Reload Page
      </a>
    </UtilityLayout>
  );
}
