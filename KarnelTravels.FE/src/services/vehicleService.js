import api from './api';

// ==================== Vehicle Type Service ====================
export const vehicleTypeService = {
  getAll: async () => {
    const response = await api.get('/adminvehicles/vehicle-types');
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/adminvehicles/vehicle-types/${id}`);
    return response.data;
  },

  create: async (data) => {
    const response = await api.post('/adminvehicles/vehicle-types', data);
    return response.data;
  },

  update: async (id, data) => {
    const response = await api.put(`/adminvehicles/vehicle-types/${id}`, data);
    return response.data;
  },

  delete: async (id) => {
    const response = await api.delete(`/adminvehicles/vehicle-types/${id}`);
    return response.data;
  },
};

// ==================== Transport Provider Service ====================
export const transportProviderService = {
  getAll: async () => {
    const response = await api.get('/adminvehicles/providers');
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/adminvehicles/providers/${id}`);
    return response.data;
  },

  create: async (data) => {
    const response = await api.post('/adminvehicles/providers', data);
    return response.data;
  },

  update: async (id, data) => {
    const response = await api.put(`/adminvehicles/providers/${id}`, data);
    return response.data;
  },

  delete: async (id) => {
    const response = await api.delete(`/adminvehicles/providers/${id}`);
    return response.data;
  },
};

// ==================== Route Service ====================
export const routeService = {
  getAll: async () => {
    const response = await api.get('/adminvehicles/routes');
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/adminvehicles/routes/${id}`);
    return response.data;
  },

  create: async (data) => {
    const response = await api.post('/adminvehicles/routes', data);
    return response.data;
  },

  update: async (id, data) => {
    const response = await api.put(`/adminvehicles/routes/${id}`, data);
    return response.data;
  },

  delete: async (id) => {
    const response = await api.delete(`/adminvehicles/routes/${id}`);
    return response.data;
  },
};

// ==================== Vehicle Service ====================
export const vehicleService = {
  getAll: async () => {
    const response = await api.get('/adminvehicles/vehicles');
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/adminvehicles/vehicles/${id}`);
    return response.data;
  },

  getByProvider: async (providerId) => {
    const response = await api.get(`/adminvehicles/vehicles/by-provider/${providerId}`);
    return response.data;
  },

  create: async (data) => {
    const response = await api.post('/adminvehicles/vehicles', data);
    return response.data;
  },

  update: async (id, data) => {
    const response = await api.put(`/adminvehicles/vehicles/${id}`, data);
    return response.data;
  },

  delete: async (id) => {
    const response = await api.delete(`/adminvehicles/vehicles/${id}`);
    return response.data;
  },

  updateStatus: async (id, status) => {
    const response = await api.patch(`/adminvehicles/vehicles/${id}/status`, { status });
    return response.data;
  },
};

// ==================== Schedule Service ====================
export const scheduleService = {
  getAll: async () => {
    const response = await api.get('/adminvehicles/schedules');
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/adminvehicles/schedules/${id}`);
    return response.data;
  },

  getByVehicle: async (vehicleId) => {
    const response = await api.get(`/adminvehicles/schedules/by-vehicle/${vehicleId}`);
    return response.data;
  },

  getByRoute: async (routeId) => {
    const response = await api.get(`/adminvehicles/schedules/by-route/${routeId}`);
    return response.data;
  },

  create: async (data) => {
    const response = await api.post('/adminvehicles/schedules', data);
    return response.data;
  },

  update: async (id, data) => {
    const response = await api.put(`/adminvehicles/schedules/${id}`, data);
    return response.data;
  },

  delete: async (id) => {
    const response = await api.delete(`/adminvehicles/schedules/${id}`);
    return response.data;
  },
};

export default {
  vehicleTypeService,
  transportProviderService,
  routeService,
  vehicleService,
  scheduleService,
};
