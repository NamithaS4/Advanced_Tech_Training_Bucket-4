import React, { useState } from 'react';
import Header from '../../components/layout/Header/Header';
import EndUserSidebar from '../../components/layout/Sidebar/EndUserSidebar';
import { endUserDataService } from '../../services/endUserDataService';
import {
  LineChart,
  Line,
  CartesianGrid,
  XAxis,
  YAxis,
  Tooltip,
  ResponsiveContainer,
} from 'recharts';

export default function MeterData() {
  const [range, setRange] = useState('day');
  const meterData = endUserDataService.getUserData().meterData[range];

  const formattedData = meterData.current.map((entry, index) => {
    const previousValue = meterData.previous[index]?.value || 0;
    const difference = entry.value - previousValue;

    let note = '';
    if (difference > 0) note = 'Usage increased compared to previous';
    else if (difference < 0) note = 'Usage decreased compared to previous';
    else note = 'No change in consumption';

    return {
      label:
        range === 'day'
          ? entry.time
          : range === 'week'
            ? entry.day
            : entry.week,
      previous: previousValue,
      current: entry.value,
      difference,
      note,
    };
  });

  return (
    <div className="min-h-screen w-screen bg-gray-50 dark:bg-gray-900 text-black">
      <Header />
      <div className="flex">
        <EndUserSidebar />
        <main className="flex-1 p-6">
          <div className="max-w-6xl mx-auto">
            <h1 className="text-xl font-semibold text-gray-900 dark:text-white">
              Select Date Range
            </h1>

            <div className="flex justify-end">
              <div className="bg-gray-200 dark:bg-white rounded-full flex p-1">
                {['day', 'week', 'month'].map((option) => (
                  <button
                    key={option}
                    onClick={() => setRange(option)}
                    className={`px-4 py-1 rounded-full font-medium transition ${
                      range === option
                        ? 'bg-purple-600 text-white'
                        : 'text-black bg-gray-200 dark:text-black hover:bg-gray-300 dark:bg-white'
                    }`}
                  >
                    {option.charAt(0).toUpperCase() + option.slice(1)}
                  </button>
                ))}
              </div>
            </div>

            <div className="p-6 mb-8 flex items-center">
              <div className="flex-1">
                <ResponsiveContainer width="100%" height={300}>
                  <LineChart data={formattedData}>
                    <CartesianGrid strokeDasharray="3 3" stroke="#ccc" />
                    <XAxis dataKey="label" />
                    <YAxis />
                    <Tooltip />
                    <Line
                      type="linear"
                      dataKey="previous"
                      stroke="#c47f47"
                      name="Previous"
                      strokeWidth={2}
                      dot={{ r: 3 }}
                    />
                    <Line
                      type="linear"
                      dataKey="current"
                      stroke="#b455e3"
                      name="Current"
                      strokeWidth={2}
                      dot={{ r: 3 }}
                    />
                  </LineChart>
                </ResponsiveContainer>
              </div>

              <div className="ml-8 flex flex-col space-y-3">
                <div className="flex items-center space-x-2">
                  <span className="w-4 h-4 rounded-full border-2 border-white bg-[#b455e3]" />
                  <span className="text-gray-900 dark:text-white font-medium">
                    Current
                  </span>
                </div>
                <div className="flex items-center space-x-2">
                  <span className="w-4 h-4 rounded-full border-2 border-white bg-[#c47f47]" />
                  <span className="text-gray-900 dark:text-white font-medium">
                    Previous
                  </span>
                </div>
              </div>
            </div>

            <div className="px-20 mb-8 flex items-center">
              <table className="border border-gray-300 dark:border-gray-700 text-sm text-left">
                <thead className="bg-gray-100 dark:bg-gray-700 text-gray-800 dark:text-white">
                  <tr>
                    <th className="border px-4 py-2">
                      {range === 'day'
                        ? 'Time'
                        : range === 'week'
                          ? 'Day'
                          : 'Week'}
                    </th>
                    <th className="border px-4 py-2">Current (kWh)</th>
                    <th className="border px-4 py-2">Previous (kWh)</th>
                    <th className="border px-4 py-2">Difference</th>
                    <th className="border px-4 py-2">Notes</th>
                  </tr>
                </thead>
                <tbody>
                  {formattedData.map((item, index) => (
                    <tr
                      key={index}
                      className="hover:bg-gray-50 dark:hover:bg-gray-700 dark:text-white"
                    >
                      <td className="border px-10 py-2">{item.label}</td>
                      <td className="border px-10 py-2">{item.current} kWh</td>
                      <td className="border px-10 py-2">{item.previous} kWh</td>
                      <td className="border px-10 py-2">
                        {item.difference} kWh
                      </td>
                      <td className="border px-20 py-2">{item.note}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        </main>
      </div>
    </div>
  );
}
