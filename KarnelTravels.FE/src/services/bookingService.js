import api from './api';

const bookingService = {
  // Get all bookings with pagination and filters (F191, F194, F195, F196)
  getAll: async ({ 
    page = 1, 
    pageSize = 10, 
    search = '', 
    status = null, 
    type = null,
    startDate = null, 
    endDate = null 
  } = {}) => {
    const params = new URLSearchParams();
    params.append('page', page);
    params.append('pageSize', pageSize);
    if (search) params.append('search', search);
    if (status !== null && status !== '') params.append('status', status);
    if (type !== null && type !== '') params.append('type', type);
    if (startDate) params.append('startDate', startDate);
    if (endDate) params.append('endDate', endDate);

    const response = await api.get(`/adminbookings?${params.toString()}`);
    return response.data;
  },

  // Get booking by ID (F192)
  getById: async (id) => {
    const response = await api.get(`/adminbookings/${id}`);
    return response.data;
  },

  // Update booking status (F193, F197)
  updateStatus: async (id, status, cancellationReason = null) => {
    const response = await api.patch(`/adminbookings/${id}/status`, {
      status,
      cancellationReason
    });
    return response.data;
  },

  // Quick confirm booking
  confirm: async (id) => {
    return bookingService.updateStatus(id, 1); // BookingStatus.Confirmed = 1
  },

  // Quick cancel booking
  cancel: async (id, reason = '') => {
    return bookingService.updateStatus(id, 3, reason); // BookingStatus.Cancelled = 3
  },

  // Quick complete booking
  complete: async (id) => {
    return bookingService.updateStatus(id, 2); // BookingStatus.Completed = 2
  },

  // Send confirmation email (F198)
  sendConfirmation: async (id) => {
    const response = await api.post(`/adminbookings/${id}/send-confirmation`);
    return response.data;
  },

  // Export bookings (F199)
  export: async (filters = {}) => {
    const params = new URLSearchParams();
    if (filters.search) params.append('search', filters.search);
    if (filters.status !== null && filters.status !== '') params.append('status', filters.status);
    if (filters.type !== null && filters.type !== '') params.append('type', filters.type);
    if (filters.startDate) params.append('startDate', filters.startDate);
    if (filters.endDate) params.append('endDate', filters.endDate);
    params.append('format', 'csv');

    const response = await api.get(`/adminbookings/export?${params.toString()}`, {
      responseType: 'blob'
    });
    
    // Create download link
    const url = window.URL.createObjectURL(new Blob([response.data]));
    const link = document.createElement('a');
    link.href = url;
    link.setAttribute('download', `bookings_export_${new Date().toISOString().split('T')[0]}.csv`);
    document.body.appendChild(link);
    link.click();
    link.remove();
    
    return { success: true };
  },

  // Get statistics (F200)
  getStatistics: async ({ startDate = null, endDate = null, type = null } = {}) => {
    const params = new URLSearchParams();
    if (startDate) params.append('startDate', startDate);
    if (endDate) params.append('endDate', endDate);
    if (type !== null && type !== '') params.append('type', type);

    const response = await api.get(`/adminbookings/statistics?${params.toString()}`);
    return response.data;
  }
};

export default bookingService;
