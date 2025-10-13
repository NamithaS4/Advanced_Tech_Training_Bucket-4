import React from "react";
import { PieChart as RePieChart, Pie, Cell, Tooltip, Legend } from "recharts";

const data = [
  { id: 1, name: "Product A", sales: 45 },
  { id: 2, name: "Product B", sales: 30 },
  { id: 3, name: "Product C", sales: 25 },
  { id: 4, name: "Product D", sales: 10 },
  { id: 5, name: "Product E", sales: 15 },
];

export default function MyPieChart() {
  return (
    <div className="flex flex-col items-center mt-10">
      <h2 className="text-2xl font-bold mb-4">Product Sales Distribution</h2>

      <RePieChart width={400} height={400}>
        <Pie
          data={data}
          dataKey="sales"
          nameKey="name"
          cx="50%"
          cy="50%"
          outerRadius={120}
          fill="#8884d8"
          label
        >
          {data.map((entry, index) => (
            <Cell
              key={`cell-${index}`}
              fill={["#0088FE", "#00C49F", "#FFBB28", "#FF8042", "#AA336A"][index % 5]}
            />
          ))}
        </Pie>
        <Tooltip />
        <Legend />
      </RePieChart>
    </div>
  );
}
