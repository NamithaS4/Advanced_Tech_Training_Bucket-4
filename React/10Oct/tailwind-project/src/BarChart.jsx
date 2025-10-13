import {BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer} from 'recharts';

const data = [
{ month: "Jan", sales: 4000 },
{ month: "Feb", sales: 3000 },
{ month: "Mar", sales: 2500 },
{ month: "Apr", sales: 2700 },
{ month: "May", sales: 3300 },
];

export default function MyBarChart() {
    return (         
        <div>
            <BarChart width={500} height={300} data={data}>
            <CartesianGrid strokeDasharray="3 3" />
            <XAxis dataKey="month" />
        <YAxis />
        <Tooltip />
        <Legend />
            <Bar dataKey="sales" fill="#2929c5" />
        </BarChart>
        </div>
)
}

