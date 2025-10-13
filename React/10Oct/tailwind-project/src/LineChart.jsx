import {LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer} from 'recharts';

const data = [
{ month: "Jan", sales: 4000, expenses: 2400 },
{ month: "Feb", sales: 3000, expenses: 2700 },
{ month: "Mar", sales: 2500, expenses: 2200 },
{ month: "Apr", sales: 2700, expenses: 2500 },
{ month: "May", sales: 3300, expenses: 2900 },
];

export default function MyLineChart() {
    return (
        <div>
            <LineChart width={500} height={300} data={data}>
            <CartesianGrid strokeDasharray="3 3" />
            <XAxis dataKey="month" />
        <YAxis />
        <Tooltip />
        <Legend />
            <Line type="monotone" dataKey="sales" stroke="#2929c5" />
            <Line type="monotone" dataKey="expenses" stroke="#13ad4c" />
        </LineChart>
        </div>
    );
}