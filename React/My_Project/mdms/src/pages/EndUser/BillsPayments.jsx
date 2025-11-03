import React, { useState } from 'react';
import Header from '../../components/layout/Header/Header';
import EndUserSidebar from '../../components/layout/Sidebar/EndUserSidebar';
import { endUserDataService } from '../../services/endUserDataService';
import { Link } from 'react-router-dom';

export default function BillsPayments() {
  const data = endUserDataService.getUserData();
  const bills = endUserDataService.getBills();
  const billdetail = endUserDataService.getUserData().payments;

  const [currentPage, setCurrentPage] = useState(1);
  const billsPerPage = 4;

  const totalPages = Math.ceil(bills.length / billsPerPage);
  const startIndex = (currentPage - 1) * billsPerPage;
  const displayedBills = bills.slice(startIndex, startIndex + billsPerPage);

  const findFullBill = (bill) => {
    const match = billdetail.find((b) => {
      const billMonth = bill.month?.toLowerCase();
      const billYear = bill.year;
      const billDate = new Date(b.date || `${billMonth} 1, ${billYear}`);

      const matchMonth = new Date(b.date || b.dateCreated || b.date)
        .toLocaleString('en-US', {
          month: 'short',
        })
        .toLowerCase();

      return (
        (b.receiptId && b.receiptId === bill.receiptId) ||
        (new Date(b.date).getFullYear() === billYear &&
          new Date(b.date)
            .toLocaleString('en-US', { month: 'short' })
            .toLowerCase() === billMonth)
      );
    });
    return match || null;
  };

  return (
    <div className="min-h-screen w-screen bg-gray-50 dark:bg-gray-900">
      <Header />
      <div className="flex">
        <EndUserSidebar />
        <main className="flex-1 p-6">
          <div className="max-w-5xl mx-auto">
            <h1 className="text-lg font-semibold text-gray-900 dark:text-white mb-4">
              My Bills
            </h1>

            <div className="overflow-x-auto">
              <table className="min-w-full border border-gray-300 dark:border-gray-700 text-sm text-left bg-white dark:bg-gray-800 rounded-md">
                <thead className="bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-200">
                  <tr>
                    <th className="px-4 py-2 border border-gray-300 dark:border-gray-700">
                      Month
                    </th>
                    <th className="px-4 py-2 border border-gray-300 dark:border-gray-700">
                      Amount
                    </th>
                    <th className="px-4 py-2 border border-gray-300 dark:border-gray-700">
                      Due Date
                    </th>
                    <th className="px-4 py-2 border border-gray-300 dark:border-gray-700">
                      Status
                    </th>
                    <th className="px-4 py-2 border border-gray-300 dark:border-gray-700">
                      Actions
                    </th>
                  </tr>
                </thead>
                <tbody>
                  {displayedBills.map((bill, index) => {
                    const fullBill = findFullBill(bill);
                    const receiptId = fullBill?.receiptId || 'pending';

                    return (
                      <tr
                        key={index}
                        className="border-t border-gray-300 dark:border-gray-700 hover:bg-gray-100 dark:hover:bg-gray-700"
                      >
                        <td className="px-4 py-2 text-gray-900 dark:text-gray-200">
                          {bill.month} {bill.year}
                        </td>
                        <td className="px-4 py-2 text-gray-900 dark:text-gray-200">
                          ₹{bill.amount}
                        </td>
                        <td className="px-4 py-2 text-gray-900 dark:text-gray-200">
                          {bill.dueDate}
                        </td>
                        <td className="px-4 py-2">
                          <span
                            className={`px-3 py-1 rounded-full text-xs font-semibold ${
                              bill.status === 'Pending'
                                ? 'bg-yellow-100 text-yellow-700'
                                : 'bg-green-100 text-green-700'
                            }`}
                          >
                            {bill.status}
                          </span>
                        </td>
                        <td className="px-4 py-2 text-blue-600 dark:text-blue-400 font-medium cursor-pointer hover:underline">
                          <Link
                            to={`/enduser/bill/${receiptId}`}
                            state={{ bill: fullBill }}
                            className="text-blue-600 hover:underline"
                          >
                            View / Pay
                          </Link>
                        </td>
                      </tr>
                    );
                  })}
                </tbody>
              </table>
            </div>

            <div className="flex justify-between items-center mt-4 text-sm text-gray-700 dark:text-gray-300">
              <button
                onClick={() => setCurrentPage((prev) => Math.max(prev - 1, 1))}
                disabled={currentPage === 1}
                className={`${
                  currentPage === 1
                    ? 'text-black bg-gray-200 cursor-not-allowed'
                    : 'text-black bg-gray-200'
                }`}
              >
                ← Previous
              </button>

              <div className="flex items-center space-x-2">
                {Array.from({ length: totalPages }).map((_, i) => (
                  <button
                    key={i}
                    onClick={() => setCurrentPage(i + 1)}
                    className={`px-2 rounded ${
                      i + 1 === currentPage
                        ? 'text-black bg-gray-200 cursor-not-allowed'
                        : 'text-black bg-gray-200'
                    }`}
                  >
                    {i + 1}
                  </button>
                ))}
              </div>

              <button
                onClick={() =>
                  setCurrentPage((prev) => Math.min(prev + 1, totalPages))
                }
                disabled={currentPage === totalPages}
                className={`${
                  currentPage === totalPages
                    ? 'text-black bg-gray-200 cursor-not-allowed'
                    : 'text-black bg-gray-200'
                }`}
              >
                Next →
              </button>
            </div>

            <div className="mt-6 text-sm text-gray-800 dark:text-gray-300">
              <strong>Note:</strong> All bills are generated on the 1st of each
              month
            </div>
          </div>
        </main>
      </div>
    </div>
  );
}
