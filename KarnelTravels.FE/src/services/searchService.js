import api from './api';

// Search service - handles all search and filter operations
const searchService = {
  // Search all categories with filters
  async searchAll(params) {
    const response = await api.get('/search', { params });
    return response.data;
  },

  // Search hotels
  async searchHotels(params) {
    const response = await api.get('/hotels', { params });
    return response.data;
  },

  // Search tours
  async searchTours(params) {
    const response = await api.get('/tours', { params });
    return response.data;
  },

  // Search restaurants
  async searchRestaurants(params) {
    const response = await api.get('/restaurants', { params });
    return response.data;
  },

  // Search transports
  async searchTransports(params) {
    const response = await api.get('/transports', { params });
    return response.data;
  },

  // Search tourist spots
  async searchSpots(params) {
    const response = await api.get('/touristspots', { params });
    return response.data;
  },

  // Search resorts
  async searchResorts(params) {
    const response = await api.get('/resorts', { params });
    return response.data;
  },

  // Autocomplete suggestions
  async getSuggestions(query) {
    const response = await api.get('/search/suggestions', { 
      params: { q: query } 
    });
    return response.data;
  },

  // Get search history
  async getSearchHistory() {
    const response = await api.get('/search/history');
    return response.data;
  },

  // Save search to history
  async saveSearchHistory(searchData) {
    const response = await api.post('/search/history', searchData);
    return response.data;
  },

  // Advanced search - combined search (Tour + Hotel + Transport)
  async advancedSearch(params) {
    const response = await api.get('/search/advanced', { params });
    return response.data;
  },

  // Get popular searches
  async getPopularSearches() {
    const response = await api.get('/search/popular');
    return response.data;
  }
};

export default searchService;
