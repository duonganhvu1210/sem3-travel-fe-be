import api from './api';

const orderService = {
  // Get user's orders with pagination and filters (F345, F346, F347)
  getOrders: async ({ 
    page = 1, 
    pageSize = 10, 
    searchQuery = '', 
    status = null, 
    serviceType = null,
    fromDate = null, 
    toDate = null,
    sortBy = 'BookingDate',
    sortDescending = true,
    userId = null
  } = {}) => {
    // Get userId from localStorage if not provided
    const storedUserId = localStorage.getItem('userId');
    const effectiveUserId = userId || storedUserId;

    const params = new URLSearchParams();
    params.append('pageNumber', page);
    params.append('pageSize', pageSize);
    if (searchQuery) params.append('SearchQuery', searchQuery);
    if (status !== null && status !== '') params.append('Status', status);
    if (serviceType !== null && serviceType !== '') params.append('ServiceType', serviceType);
    if (fromDate) params.append('FromDate', fromDate);
    if (toDate) params.append('ToDate', toDate);
    if (sortBy) params.append('SortBy', sortBy);
    if (sortDescending) params.append('SortDescending', sortDescending);
    if (effectiveUserId) params.append('UserId', effectiveUserId);

    const response = await api.get(`/orders?${params.toString()}`);
    return response.data;
  },

  // Get order statistics
  getStatistics: async (userId = null) => {
    // Get userId from localStorage if not provided
    const storedUserId = localStorage.getItem('userId');
    const effectiveUserId = userId || storedUserId;

    const params = new URLSearchParams();
    if (effectiveUserId) params.append('userId', effectiveUserId);
    
    const response = await api.get(`/orders/statistics?${params.toString()}`);
    return response.data;
  },

  // Get order detail by ID (F348, F349, F350)
  getOrderDetail: async (orderId) => {
    const response = await api.get(`/orders/${orderId}`);
    return response.data;
  },

  // Cancel order (F351)
  cancelOrder: async (orderId, reason = null) => {
    const response = await api.patch(`/orders/${orderId}/cancel`, {
      reason
    });
    return response.data;
  },

  // Request date change (F352)
  changeDate: async (orderId, newServiceDate, newEndDate = null, reason = null) => {
    const response = await api.patch(`/orders/${orderId}/change-date`, {
      newServiceDate,
      newEndDate,
      reason
    });
    return response.data;
  },

  // Download invoice PDF (F353)
  downloadInvoice: async (orderId) => {
    const response = await api.get(`/orders/${orderId}/invoice`, {
      responseType: 'blob'
    });
    
    // Create download link
    const url = window.URL.createObjectURL(new Blob([response.data]));
    const link = document.createElement('a');
    link.href = url;
    link.setAttribute('download', `Invoice_${orderId}.pdf`);
    document.body.appendChild(link);
    link.click();
    link.remove();
    window.URL.revokeObjectURL(url);
    
    return { success: true };
  },

  // Get invoice data (for preview)
  getInvoiceData: async (orderId) => {
    const response = await api.get(`/orders/${orderId}/invoice`);
    return response.data;
  }
};

export default orderService;
