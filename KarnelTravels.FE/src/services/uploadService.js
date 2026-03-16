import api from './api';

const uploadService = {
  // Upload single image
  uploadImage: async (file) => {
    const formData = new FormData();
    formData.append('file', file);

    // Do NOT set Content-Type header - axios will do it automatically with boundary
    const response = await api.post('/Upload/image', formData);
    return response.data;
  },

  // Upload multiple images
  uploadImages: async (files) => {
    const formData = new FormData();
    files.forEach((file) => {
      formData.append('files', file);
    });

    // Do NOT set Content-Type header - axios will do it automatically with boundary
    const response = await api.post('/Upload/images', formData);
    return response.data;
  },
};

export default uploadService;
