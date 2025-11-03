import React from 'react';
import { useParams, useLocation, useNavigate } from 'react-router-dom';
import Header from '../../components/layout/Header/Header';
import EndUserSidebar from '../../components/layout/Sidebar/EndUserSidebar';
import { endUserDataService } from '../../services/endUserDataService';
import jsPDF from 'jspdf';
import 'jspdf-autotable';

export default function BillDetails() {
  const { receiptId } = useParams();
  const location = useLocation();
  const navigate = useNavigate();

  let bill =
    location.state?.bill ||
    endUserDataService
      .getUserData()
      .payments.find((p) => p.receiptId === receiptId) ||
    null;

  if (!bill && receiptId === 'pending') {
    bill = endUserDataService
      .getUserData()
      .payments.find((p) => p.status === 'Pending');
  }
  const handlePayNow = () => {
    if (bill.status === 'Paid') {
      alert('This bill has already been paid.');
      return;
    }

    if (window.confirm(`Proceed to pay ₹${bill.billAmount}?`)) {
      bill.status = 'Paid';

      const data = endUserDataService.getUserData();
      const updatedPayments = data.payments.map((p) =>
        p.receiptId === bill.receiptId ? bill : p
      );
      data.payments = updatedPayments;
      localStorage.setItem('endUserData', JSON.stringify(data));

      alert('Payment successful!');
      navigate(0);
    }
  };

  const handlePrint = () => {
    const printContent = `
      <div style="font-family: Arial; padding: 20px;">
        <h2>Electricity Bill Receipt</h2>
        <p><strong>Receipt ID:</strong> ${bill.receiptId}</p>
        <p><strong>Month:</strong> ${new Date(bill.date).toLocaleString(
          'en-US',
          { month: 'long', year: 'numeric' }
        )}</p>
        <p><strong>Total Amount:</strong> ₹${bill.billAmount}</p>
        <p><strong>Due Date:</strong> ${new Date(bill.date).toLocaleDateString(
          'en-GB'
        )}</p>
        <p><strong>Status:</strong> ${bill.status}</p>
        <br/>
        <table border="1" cellpadding="6" cellspacing="0" width="100%">
          <thead>
            <tr style="background:#f0f0f0;">
              <th>Date</th>
              <th>Reading</th>
              <th>Consumption</th>
              <th>Cost</th>
            </tr>
          </thead>
          <tbody>
            ${bill.details
              ?.map(
                (d) =>
                  `<tr><td>${d.date}</td><td>${d.reading}</td><td>${d.consumption}</td><td>${d.cost}</td></tr>`
              )
              .join('')}
          </tbody>
        </table>
        <br/>
        <p>Thank you for your payment!</p>
      </div>
    `;

    const newWindow = window.open('', '', 'width=800,height=600');
    newWindow.document.write(printContent);
    newWindow.document.close();
    newWindow.print();
  };

  if (!bill) {
    return (
      <div>
        <Header />
        <div className="flex">
          <EndUserSidebar />
          <main className="flex-1 p-6">
            <h1 className="text-xl font-semibold mb-4">Bill Not Found</h1>
            <button
              onClick={() => navigate(-1)}
              className="text-blue-500 hover:underline"
            >
              Go Back
            </button>
          </main>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen w-screen bg-gray-50 dark:bg-gray-900">
      <Header />
      <div className="flex">
        <EndUserSidebar />
        <main className="flex-1 p-6">
          <button
            onClick={() => navigate(-1)}
            className="text-black bg-gray-200 mb-4"
          >
            ← Back
          </button>

          <h1 className="text-xl font-semibold mb-4 text-black dark:text-white">
            Bill Details –{' '}
            {new Date(bill.date).toLocaleString('en-US', {
              month: 'short',
              year: 'numeric',
            })}
          </h1>

          <table className="w-3/4 mb-4 border border-black text-black dark:text-white">
            <thead className="bg-gray-400">
              <tr>
                <th className="border px-20 py-2">Month</th>
                <th className="border px-20 py-2">Total Amount</th>
                <th className="border px-20 py-2">Due Date</th>
                <th className="border px-20 py-2">Status</th>
              </tr>
            </thead>
            <tbody>
              <tr>
                <td className="border px-4 py-2">
                  {new Date(bill.date).toLocaleString('en-US', {
                    month: 'long',
                    year: 'numeric',
                  })}
                </td>
                <td className="border px-4 py-2">
                  ₹{bill.billAmount?.toLocaleString()}
                </td>
                <td className="border px-4 py-2">
                  {new Date(bill.date).toLocaleDateString('en-GB')}
                </td>
                <td className="border px-4 py-2">{bill.status}</td>
              </tr>
            </tbody>
          </table>

          <table className="w-3/4 border border-black mb-6 text-black dark:text-white">
            <thead className="bg-gray-400">
              <tr>
                <th className="border px-20 py-2">Date</th>
                <th className="border px-20 py-2">Reading</th>
                <th className="border px-20 py-2">Consumption</th>
                <th className="border px-20 py-2">Cost</th>
              </tr>
            </thead>
            <tbody>
              {bill.details?.map((d, i) => (
                <tr key={i}>
                  <td className="border px-4 py-2">{d.date}</td>
                  <td className="border px-4 py-2">{d.reading}</td>
                  <td className="border px-4 py-2">{d.consumption}</td>
                  <td className="border px-4 py-2">{d.cost}</td>
                </tr>
              ))}
            </tbody>
          </table>

          <div className="flex gap-3">
            <button
              onClick={handlePrint}
              className="border rounded px-4 py-2 text-sm bg-gray-300 text-black"
            >
              Download PDF
            </button>
            <button
              onClick={handlePrint}
              className="border rounded px-4 py-2 text-sm bg-gray-300 text-black"
            >
              Print Bill
            </button>
            {bill.status === 'Pending' && (
              <button
                onClick={handlePayNow}
                className="border rounded px-4 py-2 text-sm bg-gray-300 text-black"
              >
                Pay Now
              </button>
            )}
          </div>
        </main>
      </div>
    </div>
  );
}
