import React from 'react';
import UtilityLayout from '../../components/layout/UtilityLayout';

export default function AccessDenied() {
  return (
    <UtilityLayout
      title="Access Denied"
      description="You don’t have permission to view this page."
    >
      <a href="/login" className="text-blue-500 hover:underline">
        Go to Login
      </a>
    </UtilityLayout>
  );
}
