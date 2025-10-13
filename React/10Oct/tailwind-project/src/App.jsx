import { useState } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from '/vite.svg'
import './App.css'
import MyBarChart from './BarChart'
import MyLineChart from './LineChart'

function App() {
  const [count, setCount] = useState(0)

  return (
    <>
      <div>
        Bar Chart Example
      <MyBarChart />
       Line Chart Example
      <MyLineChart />
      </div>
    
    </>
  )
}

export default App
