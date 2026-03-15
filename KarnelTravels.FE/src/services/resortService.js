import api from './api';

// ==================== Resort Service ====================
export const resortService = {
  // Resort CRUD
  getAll: async () => {
    const response = await api.get('/Resorts');
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/Resorts/${id}`);
    return response.data;
  },

  create: async (data) => {
    const response = await api.post('/Resorts', data);
    return response.data;
  },

  update: async (id, data) => {
    const response = await api.put(`/Resorts/${id}`, data);
    return response.data;
  },

  delete: async (id) => {
    const response = await api.delete(`/Resorts/${id}`);
    return response.data;
  },

  // Search & Filter
  search: async (params) => {
    const response = await api.get('/Resorts/search', { params });
    return response.data;
  },

  // Room Management
  getRooms: async (resortId) => {
    const response = await api.get(`/Resorts/${resortId}/rooms`);
    return response.data;
  },

  createRoom: async (resortId, data) => {
    const response = await api.post(`/Resorts/${resortId}/rooms`, data);
    return response.data;
  },

  updateRoom: async (resortId, roomId, data) => {
    const response = await api.put(`/Resorts/${resortId}/rooms/${roomId}`, data);
    return response.data;
  },

  deleteRoom: async (resortId, roomId) => {
    const response = await api.delete(`/Resorts/${resortId}/rooms/${roomId}`);
    return response.data;
  },

  // Room Availability & Pricing
  getRoomAvailability: async (resortId, roomId, params) => {
    const response = await api.get(`/Resorts/${resortId}/rooms/${roomId}/availability`, { params });
    return response.data;
  },

  updateRoomAvailability: async (resortId, roomId, data) => {
    const response = await api.put(`/Resorts/${resortId}/rooms/${roomId}/availability`, data);
    return response.data;
  },

  getRoomPricing: async (resortId, roomId, params) => {
    const response = await api.get(`/Resorts/${resortId}/rooms/${roomId}/pricing`, { params });
    return response.data;
  },

  bulkUpdatePricing: async (resortId, roomId, data) => {
    const response = await api.put(`/Resorts/${resortId}/rooms/${roomId}/pricing`, data);
    return response.data;
  },

  // Resort Services (Activities)
  getServices: async (resortId) => {
    const response = await api.get(`/Resorts/${resortId}/services`);
    return response.data;
  },

  createService: async (resortId, data) => {
    const response = await api.post(`/Resorts/${resortId}/services`, data);
    return response.data;
  },

  updateService: async (resortId, serviceId, data) => {
    const response = await api.put(`/Resorts/${resortId}/services/${serviceId}`, data);
    return response.data;
  },

  deleteService: async (resortId, serviceId) => {
    const response = await api.delete(`/Resorts/${resortId}/services/${serviceId}`);
    return response.data;
  },

  // Combo Packages
  getPackages: async (resortId) => {
    const response = await api.get(`/Resorts/${resortId}/packages`);
    return response.data;
  },

  createPackage: async (resortId, data) => {
    const response = await api.post(`/Resorts/${resortId}/packages`, data);
    return response.data;
  },

  updatePackage: async (resortId, packageId, data) => {
    const response = await api.put(`/Resorts/${resortId}/packages/${packageId}`, data);
    return response.data;
  },

  deletePackage: async (resortId, packageId) => {
    const response = await api.delete(`/Resorts/${resortId}/packages/${packageId}`);
    return response.data;
  }
};

export default resortService;
