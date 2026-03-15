import api from './api';

// ==================== Restaurant Service ====================
export const restaurantService = {
  // Restaurant CRUD
  getAll: async () => {
    const response = await api.get('/Restaurants');
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/Restaurants/${id}`);
    return response.data;
  },

  create: async (data) => {
    const response = await api.post('/Restaurants', data);
    return response.data;
  },

  update: async (id, data) => {
    const response = await api.put(`/Restaurants/${id}`, data);
    return response.data;
  },

  delete: async (id) => {
    const response = await api.delete(`/Restaurants/${id}`);
    return response.data;
  },

  // Search & Filter
  search: async (params) => {
    const response = await api.get('/Restaurants/search', { params });
    return response.data;
  },

  // Hours Management
  updateHours: async (id, data) => {
    const response = await api.put(`/Restaurants/${id}/hours`, data);
    return response.data;
  },

  // Menu Management
  getMenu: async (restaurantId) => {
    const response = await api.get(`/Restaurants/${restaurantId}/menu`);
    return response.data;
  },

  createMenuItem: async (restaurantId, data) => {
    const response = await api.post(`/Restaurants/${restaurantId}/menu`, data);
    return response.data;
  },

  updateMenuItem: async (restaurantId, menuItemId, data) => {
    const response = await api.put(`/Restaurants/${restaurantId}/menu/${menuItemId}`, data);
    return response.data;
  },

  deleteMenuItem: async (restaurantId, menuItemId) => {
    const response = await api.delete(`/Restaurants/${restaurantId}/menu/${menuItemId}`);
    return response.data;
  },

  // Reservations
  getReservations: async (restaurantId) => {
    const response = await api.get(`/Restaurants/${restaurantId}/reservations`);
    return response.data;
  },

  getReservation: async (restaurantId, reservationId) => {
    const response = await api.get(`/Restaurants/${restaurantId}/reservations/${reservationId}`);
    return response.data;
  },

  createReservation: async (data) => {
    const response = await api.post(`/Restaurants/${data.restaurantId}/reservations`, data);
    return response.data;
  },

  updateReservationStatus: async (restaurantId, reservationId, data) => {
    const response = await api.put(`/Restaurants/${restaurantId}/reservations/${reservationId}/status`, data);
    return response.data;
  },

  deleteReservation: async (restaurantId, reservationId) => {
    const response = await api.delete(`/Restaurants/${restaurantId}/reservations/${reservationId}`);
    return response.data;
  }
};

export default restaurantService;
