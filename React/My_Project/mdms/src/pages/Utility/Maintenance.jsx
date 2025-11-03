import React from 'react';
import UtilityLayout from '../../components/layout/UtilityLayout';

export default function Maintenance() {
  return (
    <UtilityLayout
      title="Maintenance Page"
      description="We’re currently performing maintenance. Please check back soon."
    >
      <p className="text-gray-500 text-sm">Estimated downtime: 30 minutes</p>
    </UtilityLayout>
  );
}
