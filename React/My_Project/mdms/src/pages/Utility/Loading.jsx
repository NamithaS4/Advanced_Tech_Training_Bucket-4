import React from 'react';
import UtilityLayout from '../../components/layout/UtilityLayout';

export default function Loading() {
  return (
    <UtilityLayout title="Loading...">
      <div className="flex justify-center mt-4">
        <div className="w-8 h-8 border-4 border-blue-500 border-t-transparent rounded-full animate-spin"></div>
      </div>
    </UtilityLayout>
  );
}
