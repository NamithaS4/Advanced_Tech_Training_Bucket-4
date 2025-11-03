export const endUserDataService = {
  getUserData() {
    return {
      userId: 1,
      name: 'Namitha',
      zone: 'Bangalore South',
      consumption: [
        { date: '2025-10-01', kwh: 320 },
        { date: '2025-10-05', kwh: 410 },
        { date: '2025-10-10', kwh: 380 },
        { date: '2025-10-15', kwh: 450 },
        { date: '2025-10-20', kwh: 300 },
        { date: '2025-10-25', kwh: 350 },
        { date: '2025-10-30', kwh: 420 },
        { date: '2025-11-05', kwh: 460 },
        { date: '2025-11-10', kwh: 430 },
        { date: '2025-11-15', kwh: 390 },
        { date: '2025-11-20', kwh: 470 },
        { date: '2025-11-25', kwh: 410 },
        { date: '2025-11-28', kwh: 440 },
      ],
      billing: {
        totalUnits: 4780,
        ratePerKwh: 5.5,
        dueDate: '2025-11-30',
        lastPaymentDate: '2025-10-10',
        lastPaymentAmount: 1200,
        outstandingBalance: 120,
      },
      payments: [
        {
          date: '2025-11-28',
          billAmount: 2350,
          paidAmount: 0,
          status: 'Pending',
          receiptId: null,
          details: [
            {
              date: '2025-11-01',
              reading: '45 kWh',
              consumption: '45 kWh',
              cost: '₹220',
            },
            {
              date: '2025-11-10',
              reading: '55 kWh',
              consumption: '55 kWh',
              cost: '₹270',
            },
            {
              date: '2025-11-20',
              reading: '60 kWh',
              consumption: '60 kWh',
              cost: '₹300',
            },
          ],
        },
        {
          date: '2025-10-10',
          billAmount: 1200,
          paidAmount: 1200,
          status: 'Paid',
          receiptId: 'RCPT1023',
          details: [
            {
              date: '2025-10-01',
              reading: '28 kWh',
              consumption: '28 kWh',
              cost: '₹140',
            },
            {
              date: '2025-10-10',
              reading: '30 kWh',
              consumption: '30 kWh',
              cost: '₹150',
            },
            {
              date: '2025-10-20',
              reading: '35 kWh',
              consumption: '35 kWh',
              cost: '₹170',
            },
          ],
        },
        {
          date: '2025-10-20',
          billAmount: 1000,
          paidAmount: 1000,
          status: 'Paid',
          receiptId: 'RCPT1025',
          details: [
            {
              date: '2025-10-05',
              reading: '30 kWh',
              consumption: '30 kWh',
              cost: '₹150',
            },
            {
              date: '2025-10-15',
              reading: '32 kWh',
              consumption: '32 kWh',
              cost: '₹160',
            },
            {
              date: '2025-10-25',
              reading: '35 kWh',
              consumption: '35 kWh',
              cost: '₹175',
            },
          ],
        },
        {
          date: '2025-09-10',
          billAmount: 1150,
          paidAmount: 1150,
          status: 'Paid',
          receiptId: 'RCPT1019',
          details: [
            {
              date: '2025-09-01',
              reading: '25 kWh',
              consumption: '25 kWh',
              cost: '₹120',
            },
            {
              date: '2025-09-05',
              reading: '28 kWh',
              consumption: '28 kWh',
              cost: '₹140',
            },
            {
              date: '2025-09-10',
              reading: '30 kWh',
              consumption: '30 kWh',
              cost: '₹150',
            },
          ],
        },
        {
          date: '2025-09-20',
          billAmount: 1300,
          paidAmount: 1300,
          status: 'Paid',
          receiptId: 'RCPT1021',
          details: [
            {
              date: '2025-09-10',
              reading: '32 kWh',
              consumption: '32 kWh',
              cost: '₹160',
            },
            {
              date: '2025-09-15',
              reading: '35 kWh',
              consumption: '35 kWh',
              cost: '₹175',
            },
            {
              date: '2025-09-20',
              reading: '38 kWh',
              consumption: '38 kWh',
              cost: '₹190',
            },
          ],
        },
        {
          date: '2025-08-10',
          billAmount: 1230,
          paidAmount: 1230,
          status: 'Paid',
          receiptId: 'RCPT1015',
          details: [
            {
              date: '2025-08-01',
              reading: '22 kWh',
              consumption: '22 kWh',
              cost: '₹110',
            },
            {
              date: '2025-08-05',
              reading: '25 kWh',
              consumption: '25 kWh',
              cost: '₹125',
            },
            {
              date: '2025-08-10',
              reading: '28 kWh',
              consumption: '28 kWh',
              cost: '₹140',
            },
          ],
        },
      ],
      meterData: {
        day: {
          previous: [
            { time: '00:00', value: 50 },
            { time: '03:00', value: 40 },
            { time: '06:00', value: 30 },
            { time: '09:00', value: 50 },
            { time: '12:00', value: 60 },
            { time: '15:00', value: 45 },
            { time: '18:00', value: 20 },
            { time: '21:00', value: 80 },
            { time: '23:00', value: 60 },
          ],
          current: [
            { time: '00:00', value: 20 },
            { time: '03:00', value: 45 },
            { time: '06:00', value: 35 },
            { time: '09:00', value: 20 },
            { time: '12:00', value: 20 },
            { time: '15:00', value: 55 },
            { time: '18:00', value: 96 },
            { time: '21:00', value: 20 },
            { time: '23:00', value: 50 },
          ],
        },
        week: {
          previous: [
            { day: 'Mon', value: 200 },
            { day: 'Tue', value: 240 },
            { day: 'Wed', value: 180 },
            { day: 'Thu', value: 260 },
            { day: 'Fri', value: 300 },
            { day: 'Sat', value: 320 },
            { day: 'Sun', value: 280 },
          ],
          current: [
            { day: 'Mon', value: 220 },
            { day: 'Tue', value: 240 },
            { day: 'Wed', value: 295 },
            { day: 'Thu', value: 290 },
            { day: 'Fri', value: 240 },
            { day: 'Sat', value: 280 },
            { day: 'Sun', value: 320 },
          ],
        },
        month: {
          previous: [
            { week: 'Week 1', value: 2000 },
            { week: 'Week 2', value: 1900 },
            { week: 'Week 3', value: 1550 },
            { week: 'Week 4', value: 1895 },
          ],
          current: [
            { week: 'Week 1', value: 1900 },
            { week: 'Week 2', value: 2100 },
            { week: 'Week 3', value: 1770 },
            { week: 'Week 4', value: 1650 },
          ],
        },
      },
      notifications: [
        {
          id: 1,
          title: 'Meter Reading Update',
          description: 'Your latest meter reading has been recorded.',
          date: '24 October 2025',
          time: '06:15 PM',
          content:
            'Your meter reading for this month has been updated. ' +
            'Please check you meter data for updates. ',
        },
        {
          id: 2,
          title: 'Bill Payment Reminder',
          description: 'Your electricity bill is due tomorrow.',
          date: '24 October 2025',
          time: '09:00 AM',
          content:
            'Please make sure to complete your payment before the due date to avoid any late fees. ' +
            'You can pay directly through the Bills & Payments section.',
        },
        {
          id: 3,
          title: 'Low Consumption Alert',
          description: 'Your energy usage is lower than last month.',
          date: '03 May 2025',
          time: '02:30 PM',
          content:
            'Good news! Your electricity consumption this week has decreased by 10% compared to last month. ' +
            'Keep up the energy-saving habits to reduce your carbon footprint.',
        },
      ],
    };
  },

  getDashboardStats() {
    const data = this.getUserData();
    const totalKwh = data.consumption.reduce((sum, d) => sum + d.kwh, 0);
    const averageKwh = (totalKwh / data.consumption.length).toFixed(0);
    const due = new Date(data.billing.dueDate).toLocaleDateString('en-US', {
      day: 'numeric',
      month: 'short',
    });
    const paid = new Date(data.billing.lastPaymentDate).toLocaleDateString(
      'en-US',
      { day: 'numeric', month: 'short' }
    );

    const currentBill = `₹${(averageKwh * data.billing.ratePerKwh).toFixed(0)} Due on ${due}`;

    return {
      currentConsumption: `${averageKwh} kWh`,
      currentBill,
      outstanding: `₹${data.billing.outstandingBalance} Pending`,
      lastPayment: `Paid ₹${data.billing.lastPaymentAmount} on ${paid}`,
    };
  },

  getChartData(filter = 'month') {
    const data = this.getUserData().consumption;
    if (filter === 'day') {
      return [
        { name: '12 AM', kwh: 15 },
        { name: '4 AM', kwh: 25 },
        { name: '8 AM', kwh: 35 },
        { name: '12 PM', kwh: 70 },
        { name: '4 PM', kwh: 50 },
        { name: '8 PM', kwh: 80 },
      ];
    }
    if (filter === 'week') {
      return data.slice(-7).map((d) => ({
        name: new Date(d.date).toLocaleDateString('en-US', {
          month: 'short',
          day: 'numeric',
        }),
        kwh: d.kwh,
      }));
    }
    return data.map((d) => ({
      name: new Date(d.date).toLocaleDateString('en-US', {
        month: 'short',
        day: 'numeric',
      }),
      kwh: d.kwh,
    }));
  },

  getConsumptionData() {
    return this.getUserData().consumption;
  },

  getBills() {
    const data = this.getUserData();
    const bills = data.payments.map((p) => {
      const month = new Date(p.date).toLocaleString('en-US', {
        month: 'short',
      });
      const year = new Date(p.date).getFullYear();
      const dueDateObj = new Date(p.date);
      dueDateObj.setMonth(dueDateObj.getMonth() + 1, 0);
      return {
        month,
        year,
        amount: p.billAmount,
        dueDate: dueDateObj.toLocaleDateString('en-US', {
          day: '2-digit',
          month: 'short',
        }),
        status: p.status,
        receiptId: p.receiptId,
      };
    });

    const currentDue = new Date(data.billing.dueDate);
    const currentMonth = currentDue.toLocaleString('en-US', { month: 'short' });
    const currentYear = currentDue.getFullYear();
    if (
      !bills.some((b) => b.month === currentMonth && b.year === currentYear)
    ) {
      bills.push({
        month: currentMonth,
        year: currentYear,
        amount: (data.billing.totalUnits * data.billing.ratePerKwh).toFixed(2),
        dueDate: new Date(
          currentDue.getFullYear(),
          currentDue.getMonth() + 1,
          0
        ).toLocaleDateString('en-US', {
          day: '2-digit',
          month: 'short',
        }),
        status: 'Pending',
        receiptId: null,
      });
    }

    return bills;
  },
};
