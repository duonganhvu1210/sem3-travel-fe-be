import api from './api';

export const dashboardService = {
  // F161-F166: Get dashboard summary
  getSummary: async () => {
    const response = await api.get('/admindashboard/summary');
    return response.data;
  },

  // F168: Get revenue chart data (12 months)
  getRevenueChart: async () => {
    const response = await api.get('/admindashboard/charts');
    return response.data;
  },

  // F167: Get recent bookings (5 latest)
  getRecentBookings: async () => {
    const response = await api.get('/admindashboard/recent-bookings');
    return response.data;
  },

  // F169: Get unread contacts
  getUnreadContacts: async () => {
    const response = await api.get('/admindashboard/unread-contacts');
    return response.data;
  },
};
